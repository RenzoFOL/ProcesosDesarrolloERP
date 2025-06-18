using MySql.Data.MySqlClient;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace ProyectoProcesosExperienciaV1.Data.Dao
{
    /// <summary>
    /// Clase de acceso a datos para la entidad Categoria.
    /// Proporciona métodos CRUD para la gestión de categorías en la base de datos MySQL.
    /// </summary>
    public class CategoriaDAO
    {
        #region Campos privados
        
        /// <summary>
        /// Cadena de conexión a la base de datos MySQL.
        /// </summary>
        private readonly string _connectionString;
        
        #endregion

        #region Constructor
        
        /// <summary>
        /// Inicializa una nueva instancia de la clase CategoriaDAO.
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a la base de datos MySQL.</param>
        /// <exception cref="ArgumentNullException">Se lanza cuando connectionString es null o vacío.</exception>
        public CategoriaDAO(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
        
        #endregion

        #region Métodos de Consulta

        /// <summary>
        /// Obtiene todas las categorías de la base de datos de forma asíncrona.
        /// </summary>
        /// <returns>Lista de todas las categorías disponibles.</returns>
        /// <exception cref="MySqlException">Se lanza cuando ocurre un error en la base de datos.</exception>
        public async Task<List<Categoria>> ObtenerTodasAsync()
        {
            var categorias = new List<Categoria>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT ID_Categoria, Nombre FROM Categoria ORDER BY Nombre";
            
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                categorias.Add(MapearCategoria(reader));
            }

            return categorias;
        }

        /// <summary>
        /// Obtiene todas las categorías de la base de datos de forma síncrona.
        /// </summary>
        /// <returns>Lista de todas las categorías disponibles.</returns>
        /// <exception cref="MySqlException">Se lanza cuando ocurre un error en la base de datos.</exception>
        public List<Categoria> ObtenerTodas()
        {
            return ObtenerTodasAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID.
        /// </summary>
        /// <param name="id">ID de la categoría a buscar.</param>
        /// <returns>La categoría encontrada o null si no existe.</returns>
        /// <exception cref="ArgumentException">Se lanza cuando el ID es menor o igual a cero.</exception>
        /// <exception cref="MySqlException">Se lanza cuando ocurre un error en la base de datos.</exception>
        public async Task<Categoria> ObtenerPorIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.", nameof(id));

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT ID_Categoria, Nombre FROM Categoria WHERE ID_Categoria = @id";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            
            return await reader.ReadAsync() ? MapearCategoria(reader) : null;
        }

        /// <summary>
        /// Obtiene una categoría específica por su ID de forma síncrona.
        /// </summary>
        /// <param name="id">ID de la categoría a buscar.</param>
        /// <returns>La categoría encontrada o null si no existe.</returns>
        public Categoria ObtenerPorId(int id)
        {
            return ObtenerPorIdAsync(id).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Verifica si existe una categoría con el ID especificado.
        /// </summary>
        /// <param name="id">ID de la categoría a verificar.</param>
        /// <returns>True si la categoría existe, false en caso contrario.</returns>
        public async Task<bool> ExisteAsync(int id)
        {
            if (id <= 0) return false;

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Categoria WHERE ID_Categoria = @id";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        #endregion

        #region Métodos de Modificación

        /// <summary>
        /// Inserta una nueva categoría en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="categoria">Objeto Categoria con los datos a insertar.</param>
        /// <returns>ID de la categoría insertada.</returns>
        /// <exception cref="ArgumentNullException">Se lanza cuando categoria es null.</exception>
        /// <exception cref="ArgumentException">Se lanza cuando el nombre de la categoría es null o vacío.</exception>
        /// <exception cref="MySqlException">Se lanza cuando ocurre un error en la base de datos.</exception>
        public async Task<int> InsertarAsync(Categoria categoria)
        {
            ValidarCategoria(categoria);

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"INSERT INTO Categoria (Nombre) 
                                 VALUES (@nombre);
                                 SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@nombre", categoria.Nombre.Trim());

            var nuevoId = Convert.ToInt32(await command.ExecuteScalarAsync());
            categoria.ID_Categoria = nuevoId;
            
            return nuevoId;
        }

        /// <summary>
        /// Inserta una nueva categoría en la base de datos de forma síncrona.
        /// </summary>
        /// <param name="categoria">Objeto Categoria con los datos a insertar.</param>
        /// <returns>ID de la categoría insertada.</returns>
        public int Insertar(Categoria categoria)
        {
            return InsertarAsync(categoria).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Actualiza una categoría existente en la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="categoria">Objeto Categoria con los datos actualizados.</param>
        /// <returns>True si se actualizó correctamente, false si no se encontró la categoría.</returns>
        /// <exception cref="ArgumentNullException">Se lanza cuando categoria es null.</exception>
        /// <exception cref="ArgumentException">Se lanza cuando los datos de la categoría son inválidos.</exception>
        /// <exception cref="MySqlException">Se lanza cuando ocurre un error en la base de datos.</exception>
        public async Task<bool> ActualizarAsync(Categoria categoria)
        {
            ValidarCategoria(categoria);
            
            if (categoria.ID_Categoria <= 0)
                throw new ArgumentException("El ID de la categoría debe ser mayor a cero.", nameof(categoria));

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = @"UPDATE Categoria 
                                 SET Nombre = @nombre 
                                 WHERE ID_Categoria = @id";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", categoria.ID_Categoria);
            command.Parameters.AddWithValue("@nombre", categoria.Nombre.Trim());

            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        /// <summary>
        /// Actualiza una categoría existente en la base de datos de forma síncrona.
        /// </summary>
        /// <param name="categoria">Objeto Categoria con los datos actualizados.</param>
        /// <returns>True si se actualizó correctamente, false si no se encontró la categoría.</returns>
        public bool Actualizar(Categoria categoria)
        {
            return ActualizarAsync(categoria).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Elimina una categoría de la base de datos de forma asíncrona.
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar.</param>
        /// <returns>True si se eliminó correctamente, false si no se encontró la categoría.</returns>
        /// <exception cref="ArgumentException">Se lanza cuando el ID es menor o igual a cero.</exception>
        /// <exception cref="InvalidOperationException">Se lanza cuando la categoría tiene productos asociados.</exception>
        /// <exception cref="MySqlException">Se lanza cuando ocurre un error en la base de datos.</exception>
        public async Task<bool> EliminarAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor a cero.", nameof(id));

            // Verificar si la categoría tiene productos asociados
            if (await TieneProductosAsociados(id))
                throw new InvalidOperationException("No se puede eliminar la categoría porque tiene productos asociados.");

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "DELETE FROM Categoria WHERE ID_Categoria = @id";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            var filasAfectadas = await command.ExecuteNonQueryAsync();
            return filasAfectadas > 0;
        }

        /// <summary>
        /// Elimina una categoría de la base de datos de forma síncrona.
        /// </summary>
        /// <param name="id">ID de la categoría a eliminar.</param>
        /// <returns>True si se eliminó correctamente, false si no se encontró la categoría.</returns>
        public bool Eliminar(int id)
        {
            return EliminarAsync(id).GetAwaiter().GetResult();
        }

        #endregion

        #region Métodos Privados de Apoyo

        /// <summary>
        /// Mapea un MySqlDataReader a un objeto Categoria.
        /// </summary>
        /// <param name="reader">Reader con los datos de la categoría.</param>
        /// <returns>Objeto Categoria mapeado.</returns>
        private static Categoria MapearCategoria(DbDataReader reader)
        {
            return new Categoria
            {
                ID_Categoria = Convert.ToInt32(reader["ID_Categoria"]),
                Nombre = reader["Nombre"].ToString()
            };
        }

        /// <summary>
        /// Valida que los datos de una categoría sean correctos.
        /// </summary>
        /// <param name="categoria">Categoría a validar.</param>
        /// <exception cref="ArgumentNullException">Se lanza cuando categoria es null.</exception>
        /// <exception cref="ArgumentException">Se lanza cuando el nombre es inválido.</exception>
        private static void ValidarCategoria(Categoria categoria)
        {
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria));

            if (string.IsNullOrWhiteSpace(categoria.Nombre))
                throw new ArgumentException("El nombre de la categoría no puede estar vacío.", nameof(categoria));

            if (categoria.Nombre.Trim().Length > 100)
                throw new ArgumentException("El nombre de la categoría no puede exceder los 100 caracteres.", nameof(categoria));
        }

        /// <summary>
        /// Verifica si una categoría tiene productos asociados.
        /// </summary>
        /// <param name="categoriaId">ID de la categoría a verificar.</param>
        /// <returns>True si tiene productos asociados, false en caso contrario.</returns>
        private async Task<bool> TieneProductosAsociados(int categoriaId)
        {
            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            const string query = "SELECT COUNT(*) FROM Producto WHERE fkID_Categoria = @id";
            
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", categoriaId);

            var count = Convert.ToInt32(await command.ExecuteScalarAsync());
            return count > 0;
        }

        #endregion

        #region Métodos de Compatibilidad (Deprecated)

        /// <summary>
        /// Obtiene todas las categorías. (Método mantenido por compatibilidad)
        /// </summary>
        /// <returns>Lista de categorías.</returns>
        [Obsolete("Use ObtenerTodasAsync() en su lugar para mejor rendimiento.")]
        public List<Categoria> ObtenerCategorias() => ObtenerTodas();

        /// <summary>
        /// Inserta una categoría. (Método mantenido por compatibilidad)
        /// </summary>
        /// <param name="categoria">Categoría a insertar.</param>
        [Obsolete("Use InsertarAsync() en su lugar para mejor rendimiento.")]
        public void InsertarCategoria(Categoria categoria) => Insertar(categoria);

        /// <summary>
        /// Elimina una categoría. (Método mantenido por compatibilidad)
        /// </summary>
        /// <param name="id">ID de la categoría.</param>
        [Obsolete("Use EliminarAsync() en su lugar para mejor rendimiento.")]
        public void EliminarCategoria(int id) => Eliminar(id);

        #endregion
    }
}