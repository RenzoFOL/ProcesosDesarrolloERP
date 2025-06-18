using ProyectoProcesosExperienciaV1.Data.Modelos;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;


namespace ProyectoProcesosExperienciaV1.Data.Dao
{
    /// <summary>
    /// Clase de acceso a datos para la gestión de entradas de productos en el inventario.
    /// Proporciona operaciones CRUD para la tabla Entrada y consultas extendidas con información de productos.
    /// </summary>
    public class EntradaDAO
    {
        #region Propiedades Privadas
        
        /// <summary>
        /// Cadena de conexión a la base de datos MySQL
        /// </summary>
        private readonly string _connectionString;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Inicializa una nueva instancia de la clase EntradaDAO
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a la base de datos</param>
        /// <exception cref="ArgumentNullException">Se lanza cuando connectionString es null o vacío</exception>
        public EntradaDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        #endregion

        #region Métodos de Consulta

        /// <summary>
        /// Obtiene todas las entradas registradas en el sistema de forma asíncrona
        /// </summary>
        /// <returns>Lista de objetos Entrada con toda la información de las entradas</returns>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la consulta SQL</exception>
        public async Task<List<Entrada>> ObtenerEntradasAsync()
        {
            var entradas = new List<Entrada>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT * FROM Entrada ORDER BY FechaEntrada DESC";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                entradas.Add(MapearEntrada(reader));
            }

            return entradas;
        }

        /// <summary>
        /// Obtiene información extendida de las entradas incluyendo datos del producto relacionado
        /// </summary>
        /// <returns>Lista de objetos EntradaExtendida con información de entrada y producto</returns>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la consulta SQL</exception>
        public async Task<List<EntradaExtendida>> ObtenerEntradasExtendidasAsync()
        {
            var lista = new List<EntradaExtendida>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                SELECT e.ID_Entrada, e.fkID_Producto, p.Nombre, e.Cantidad, 
                       e.Precio_Entrada, e.Precio_Venta, e.FechaEntrada
                FROM Entrada e
                INNER JOIN Producto p ON e.fkID_Producto = p.ID_Producto
                ORDER BY e.FechaEntrada DESC";

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new EntradaExtendida
                {
                    ID_Entrada = reader.GetInt32(reader.GetOrdinal("ID_Entrada")),
                    ID_Producto = reader.GetString(reader.GetOrdinal("fkID_Producto")),
                    NombreProducto = reader.GetString(reader.GetOrdinal("Nombre")),
                    Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                    Precio_Entrada = reader.GetDecimal(reader.GetOrdinal("Precio_Entrada")),
                    Precio_Venta = reader.GetDecimal(reader.GetOrdinal("Precio_Venta")),
                    FechaEntrada = reader.GetDateTime(reader.GetOrdinal("FechaEntrada"))
                });
            }

            return lista;
        }

        /// <summary>
        /// Versión síncrona de ObtenerEntradasExtendidasAsync para compatibilidad con código legacy
        /// </summary>
        /// <returns>Lista de objetos EntradaExtendida</returns>
        [Obsolete("Use ObtenerEntradasExtendidasAsync() en su lugar")]
        public List<EntradaExtendida> ObtenerEntradasExtendidas()
        {
            return ObtenerEntradasExtendidasAsync().GetAwaiter().GetResult();
        }

        #endregion

        #region Métodos de Inserción

        /// <summary>
        /// Agrega una nueva entrada al sistema de forma asíncrona
        /// </summary>
        /// <param name="entrada">Objeto Entrada con los datos a insertar</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Se lanza cuando entrada es null</exception>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la operación SQL</exception>
        public async Task AgregarEntradaAsync(Entrada entrada)
        {
            if (entrada == null)
                throw new ArgumentNullException(nameof(entrada), "La entrada no puede ser null");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                INSERT INTO Entrada (fkID_Producto, Cantidad, Precio_Entrada, Precio_Venta, FechaEntrada)
                VALUES (@fkID_Producto, @Cantidad, @Precio_Entrada, @Precio_Venta, @FechaEntrada)";

            using var command = new MySqlCommand(query, connection);
            AgregarParametrosEntrada(command, entrada);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Versión síncrona de AgregarEntradaAsync para compatibilidad con código legacy
        /// </summary>
        /// <param name="nuevaEntrada">Objeto Entrada con los datos a insertar</param>
        /// <exception cref="ArgumentNullException">Se lanza cuando nuevaEntrada es null</exception>
        [Obsolete("Use AgregarEntradaAsync() en su lugar")]
        public void InsertarEntrada(Entrada nuevaEntrada)
        {
            if (nuevaEntrada == null)
                throw new ArgumentNullException(nameof(nuevaEntrada));

            AgregarEntradaAsync(nuevaEntrada).GetAwaiter().GetResult();
        }

        #endregion

        #region Métodos de Actualización

        /// <summary>
        /// Actualiza una entrada existente en el sistema
        /// </summary>
        /// <param name="entrada">Objeto Entrada con los datos actualizados</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentNullException">Se lanza cuando entrada es null</exception>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la operación SQL</exception>
        public async Task ActualizarEntradaAsync(Entrada entrada)
        {
            if (entrada == null)
                throw new ArgumentNullException(nameof(entrada), "La entrada no puede ser null");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"
                UPDATE Entrada 
                SET fkID_Producto = @fkID_Producto, 
                    Cantidad = @Cantidad,
                    Precio_Entrada = @Precio_Entrada, 
                    Precio_Venta = @Precio_Venta,
                    FechaEntrada = @FechaEntrada
                WHERE ID_Entrada = @ID_Entrada";

            using var command = new MySqlCommand(query, connection);
            AgregarParametrosEntrada(command, entrada);
            command.Parameters.AddWithValue("@ID_Entrada", entrada.ID_Entrada);

            await command.ExecuteNonQueryAsync();
        }

        #endregion

        #region Métodos de Eliminación

        /// <summary>
        /// Elimina una entrada del sistema por su ID de forma asíncrona
        /// </summary>
        /// <param name="idEntrada">ID de la entrada a eliminar</param>
        /// <returns>Task que representa la operación asíncrona</returns>
        /// <exception cref="ArgumentException">Se lanza cuando idEntrada es menor o igual a 0</exception>
        /// <exception cref="MySqlException">Se lanza cuando hay un error en la operación SQL</exception>
        public async Task EliminarEntradaAsync(int idEntrada)
        {
            if (idEntrada <= 0)
                throw new ArgumentException("El ID de entrada debe ser mayor a 0", nameof(idEntrada));

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM Entrada WHERE ID_Entrada = @ID_Entrada";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID_Entrada", idEntrada);

            await command.ExecuteNonQueryAsync();
        }

        /// <summary>
        /// Versión síncrona de EliminarEntradaAsync para compatibilidad con código legacy
        /// </summary>
        /// <param name="id">ID de la entrada a eliminar</param>
        /// <exception cref="ArgumentException">Se lanza cuando id es menor o igual a 0</exception>
        [Obsolete("Use EliminarEntradaAsync() en su lugar")]
        public void EliminarEntrada(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a 0", nameof(id));

            EliminarEntradaAsync(id).GetAwaiter().GetResult();
        }

        #endregion

        #region Métodos Privados de Utilidad

        /// <summary>
        /// Mapea un registro de MySqlDataReader a un objeto Entrada
        /// </summary>
        /// <param name="reader">Reader con los datos del registro</param>
        /// <returns>Objeto Entrada mapeado</returns>
private static Entrada MapearEntrada(DbDataReader reader)
{
    return new Entrada
    {
        ID_Entrada = Convert.ToInt32(reader["ID_Entrada"]),
        fkID_Producto = reader["fkID_Producto"].ToString(),
        Cantidad = Convert.ToInt32(reader["Cantidad"]),
        Precio_Entrada = Convert.ToDecimal(reader["Precio_Entrada"]),
        Precio_Venta = Convert.ToDecimal(reader["Precio_Venta"]),
        FechaEntrada = Convert.ToDateTime(reader["FechaEntrada"])
    };
}


        /// <summary>
        /// Agrega los parámetros comunes de entrada a un comando SQL
        /// </summary>
        /// <param name="command">Comando SQL al que agregar los parámetros</param>
        /// <param name="entrada">Objeto Entrada con los datos</param>
        private static void AgregarParametrosEntrada(MySqlCommand command, Entrada entrada)
        {
            command.Parameters.AddWithValue("@fkID_Producto", entrada.fkID_Producto);
            command.Parameters.AddWithValue("@Cantidad", entrada.Cantidad);
            command.Parameters.AddWithValue("@Precio_Entrada", entrada.Precio_Entrada);
            command.Parameters.AddWithValue("@Precio_Venta", entrada.Precio_Venta);
            command.Parameters.AddWithValue("@FechaEntrada", entrada.FechaEntrada);
        }

        #endregion
    }
}