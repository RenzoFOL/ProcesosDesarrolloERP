using ProyectoProcesosExperienciaV1.Data.Modelos;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace ProyectoProcesosExperienciaV1.Data.Dao
{
    /// <summary>
    /// Clase de acceso a datos para la gestión de salidas de productos del inventario.
    /// Maneja operaciones CRUD para la tabla Salida y operaciones de venta con control de stock.
    /// </summary>
    public class SalidaDAO
    {
        #region Propiedades Privadas

        /// <summary>
        /// Cadena de conexión a la base de datos MySQL
        /// </summary>
        private readonly string _connectionString;

        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa una nueva instancia de la clase SalidaDAO
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a la base de datos</param>
        /// <exception cref="ArgumentNullException">Se lanza cuando connectionString es null o vacío</exception>
        public SalidaDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        #endregion

        #region Métodos de Consulta

        /// <summary>
        /// Obtiene todas las salidas registradas en el sistema
        /// </summary>
        /// <returns>Lista de objetos Salida ordenados por fecha descendente</returns>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la consulta SQL</exception>
        public async Task<List<Salida>> ObtenerTodasLasSalidasAsync()
        {
            var salidas = new List<Salida>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM Salida ORDER BY FechaSalida DESC";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                salidas.Add(MapearSalida(reader));
            }

            return salidas;
        }

        /// <summary>
        /// Obtiene el próximo ID disponible para una nueva salida
        /// </summary>
        /// <returns>Siguiente ID disponible</returns>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la consulta SQL</exception>
        public async Task<int> ObtenerProximoIDSalidaAsync()
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT IFNULL(MAX(ID_Salida), 0) + 1 FROM Salida";
            using var command = new MySqlCommand(query, connection);
            
            var result = await command.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        /// <summary>
        /// Versión síncrona de ObtenerProximoIDSalidaAsync para compatibilidad con código legacy
        /// </summary>
        /// <returns>Siguiente ID disponible</returns>
        [Obsolete("Use ObtenerProximoIDSalidaAsync() en su lugar")]
        public int ObtenerProximoIDSalida()
        {
            return ObtenerProximoIDSalidaAsync().GetAwaiter().GetResult();
        }

        #endregion

        #region Métodos de Inserción

        /// <summary>
        /// Agrega una nueva salida al sistema
        /// </summary>
        /// <param name="salida">Objeto Salida con los datos a insertar</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Se lanza cuando salida es null</exception>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la operación SQL</exception>
        public async Task AgregarSalidaAsync(Salida salida)
        {
            if (salida == null)
                throw new ArgumentNullException(nameof(salida), "La salida no puede ser null");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO Salida (fkID_Producto, Cantidad, FechaSalida)
                VALUES (@fkID_Producto, @Cantidad, @FechaSalida)";

            using var command = new MySqlCommand(query, connection);
            AgregarParametrosSalida(command, salida);

            await command.ExecuteNonQueryAsync();
        }

        #endregion

        #region Métodos de Eliminación

        /// <summary>
        /// Elimina una salida del sistema por su ID
        /// </summary>
        /// <param name="idSalida">ID de la salida a eliminar</param>
        /// <returns>True si se eliminó correctamente, False si no se encontró el registro</returns>
        /// <exception cref="ArgumentException">Se lanza cuando idSalida es menor o igual a 0</exception>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la operación SQL</exception>
        public async Task<bool> EliminarSalidaPorIDAsync(int idSalida)
        {
            if (idSalida <= 0)
                throw new ArgumentException("El ID de salida debe ser mayor a 0", nameof(idSalida));

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM Salida WHERE ID_Salida = @ID_Salida";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID_Salida", idSalida);

            int rowsAffected = await command.ExecuteNonQueryAsync();
            return rowsAffected > 0;
        }

        #endregion

        #region Métodos de Ventas

        /// <summary>
        /// Registra una venta completa con múltiples productos, verificando stock y usando transacciones
        /// </summary>
        /// <param name="productosVendidos">Lista de productos vendidos con cantidades</param>
        /// <returns>Lista de IDs de las salidas generadas</returns>
        /// <exception cref="ArgumentNullException">Se lanza cuando productosVendidos es null</exception>
        /// <exception cref="ArgumentException">Se lanza cuando la lista está vacía</exception>
        /// <exception cref="Exception">Se lanza cuando hay stock insuficiente o el producto no existe</exception>
        public async Task<List<int>> RegistrarVentaCompletaAsync(List<ProductoVenta> productosVendidos)
        {
            if (productosVendidos == null)
                throw new ArgumentNullException(nameof(productosVendidos));

            if (productosVendidos.Count == 0)
                throw new ArgumentException("La lista de productos no puede estar vacía", nameof(productosVendidos));

            var idsSalidas = new List<int>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                foreach (var producto in productosVendidos)
                {
                    await ValidarStockProducto(connection, transaction, producto);
                    int idSalida = await InsertarSalidaConTransaccion(connection, transaction, producto);
                    idsSalidas.Add(idSalida);
                }

                await transaction.CommitAsync();
                return idsSalidas;
            }
            catch (MySqlException ex) when (ex.Number == 1644) // Error lanzado por trigger
            {
                await transaction.RollbackAsync();
                throw new Exception("Stock insuficiente para completar la venta");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// Realiza una salida individual verificando y actualizando el stock
        /// </summary>
        /// <param name="idProducto">ID del producto</param>
        /// <param name="cantidad">Cantidad a dar de baja</param>
        /// <returns>True si la operación fue exitosa, False si no hay stock suficiente</returns>
        /// <exception cref="ArgumentException">Se lanza cuando los parámetros no son válidos</exception>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la operación SQL</exception>
        public async Task<bool> RealizarSalidaAsync(string idProducto, int cantidad)
        {
            if (string.IsNullOrWhiteSpace(idProducto))
                throw new ArgumentException("El ID del producto no puede ser vacío", nameof(idProducto));

            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a 0", nameof(cantidad));

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // Verificar stock disponible
                int stockActual = await ObtenerStockProducto(connection, transaction, idProducto);
                
                if (stockActual == -1) // Producto no existe
                    return false;

                if (stockActual < cantidad) // Stock insuficiente
                    return false;

                // Actualizar stock
                await ActualizarStockProducto(connection, transaction, idProducto, cantidad);

                // Registrar salida
                await RegistrarSalida(connection, transaction, idProducto, cantidad);

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        #endregion

        #region Métodos Privados de Utilidad

        /// <summary>
        /// Mapea un registro de MySqlDataReader a un objeto Salida
        /// </summary>
        /// <param name="reader">Reader con los datos del registro</param>
        /// <returns>Objeto Salida mapeado</returns>
        private static Salida MapearSalida(DbDataReader reader)
        {
            return new Salida
            {
                ID_Salida = Convert.ToInt32(reader["ID_Salida"]),
                fkID_Producto = reader["fkID_Producto"].ToString(),
                Cantidad = Convert.ToInt32(reader["Cantidad"]),
                FechaSalida = Convert.ToDateTime(reader["FechaSalida"])
            };
        }

        /// <summary>
        /// Agrega los parámetros de salida a un comando SQL
        /// </summary>
        /// <param name="command">Comando SQL</param>
        /// <param name="salida">Objeto Salida con los datos</param>
        private static void AgregarParametrosSalida(MySqlCommand command, Salida salida)
        {
            command.Parameters.AddWithValue("@fkID_Producto", salida.fkID_Producto);
            command.Parameters.AddWithValue("@Cantidad", salida.Cantidad);
            command.Parameters.AddWithValue("@FechaSalida", salida.FechaSalida);
        }

        /// <summary>
        /// Valida que existe stock suficiente para un producto
        /// </summary>
        private async Task ValidarStockProducto(MySqlConnection connection, MySqlTransaction transaction, ProductoVenta producto)
        {
            const string query = @"
                SELECT StockActual 
                FROM VistaStockPrecio 
                WHERE ID_Producto = @ID";

            using var command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ID", producto.ID_Producto);

            var stockDisponible = await command.ExecuteScalarAsync();

            if (stockDisponible == null)
                throw new Exception($"Producto no encontrado: {producto.ID_Producto}");

            var stockActual = Convert.ToInt32(stockDisponible);

            if (stockActual < producto.Cantidad)
                throw new Exception($"Stock insuficiente para {producto.NombreProducto}. Disponible: {stockActual}, solicitado: {producto.Cantidad}");
        }

        /// <summary>
        /// Inserta una salida dentro de una transacción
        /// </summary>
        private async Task<int> InsertarSalidaConTransaccion(MySqlConnection connection, MySqlTransaction transaction, ProductoVenta producto)
        {
            const string query = @"
                INSERT INTO Salida (fkID_Producto, Cantidad, FechaSalida)
                VALUES (@ID, @Cant, @Fecha);
                SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ID", producto.ID_Producto);
            command.Parameters.AddWithValue("@Cant", producto.Cantidad);
            command.Parameters.AddWithValue("@Fecha", DateTime.Now);

            var resultado = await command.ExecuteScalarAsync();
            return Convert.ToInt32(resultado);
        }

        /// <summary>
        /// Obtiene el stock actual de un producto
        /// </summary>
        /// <returns>Stock actual o -1 si el producto no existe</returns>
        private async Task<int> ObtenerStockProducto(MySqlConnection connection, MySqlTransaction transaction, string idProducto)
        {
            const string query = "SELECT Stock FROM Producto WHERE ID_Producto = @ID_Producto";
            
            using var command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ID_Producto", idProducto);
            
            var result = await command.ExecuteScalarAsync();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }

        /// <summary>
        /// Actualiza el stock de un producto restando la cantidad vendida
        /// </summary>
        private async Task ActualizarStockProducto(MySqlConnection connection, MySqlTransaction transaction, string idProducto, int cantidad)
        {
            const string query = "UPDATE Producto SET Stock = Stock - @Cantidad WHERE ID_Producto = @ID_Producto";
            
            using var command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@ID_Producto", idProducto);
            command.Parameters.AddWithValue("@Cantidad", cantidad);
            
            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Registra una salida en la base de datos
        /// </summary>
        private async Task RegistrarSalida(MySqlConnection connection, MySqlTransaction transaction, string idProducto, int cantidad)
        {
            const string query = @"
                INSERT INTO Salida (fkID_Producto, Cantidad, FechaSalida)
                VALUES (@fkID_Producto, @Cantidad, @FechaSalida)";

            using var command = new MySqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@fkID_Producto", idProducto);
            command.Parameters.AddWithValue("@Cantidad", cantidad);
            command.Parameters.AddWithValue("@FechaSalida", DateTime.Now);
            
            await command.ExecuteNonQueryAsync();
        }

        #endregion
    }
}