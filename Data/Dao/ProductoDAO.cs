using ProyectoProcesosExperienciaV1.Data.Modelos;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoProcesosExperienciaV1.Data.Dao
{
    public class ProductoDAO
    {
        private readonly string _connectionString;

        public ProductoDAO(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Método para obtener todos los productos
        public List<Producto> ObtenerTodosLosProductos()
        {
            var productos = new List<Producto>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT * FROM Producto";
            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new Producto
                {
                    ID_Producto = reader["ID_Producto"].ToString(),
                    Nombre = reader["Nombre"].ToString(),
                    Descripcion = reader["Descripcion"]?.ToString(),
                    fkID_Categoria = reader["fkID_Categoria"] != DBNull.Value ? Convert.ToInt32(reader["fkID_Categoria"]) : null
                });
            }

            return productos;
        }

        // Método para obtener un producto por ID
public ProductoModel ObtenerProductoPorID(string idProducto)
{
    using var connection = new MySqlConnection(_connectionString);
    connection.Open();

    string query = "SELECT * FROM VistaStockPrecio WHERE ID_Producto = @idProducto";


    using var command = new MySqlCommand(query, connection);
    command.Parameters.AddWithValue("@idProducto", idProducto);
    using var reader = command.ExecuteReader();

    if (reader.Read())
    {
        return new ProductoModel
        {
            ID_Producto = reader["ID_Producto"].ToString(),
            Nombre = reader["Nombre"].ToString(),
            Descripcion = reader["Descripcion"]?.ToString(),
            PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
            StockActual = Convert.ToInt32(reader["StockActual"]),
            NombreCategoria = reader["NombreCategoria"]?.ToString()
        };
    }

    return null;
}



        // Método para agregar un nuevo producto
        public void AgregarProducto(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO Producto (ID_Producto, Nombre, Descripcion, fkID_Categoria) 
                            VALUES (@ID_Producto, @Nombre, @Descripcion, @fkID_Categoria)";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID_Producto", producto.ID_Producto);
            command.Parameters.AddWithValue("@Nombre", producto.Nombre);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@fkID_Categoria", producto.fkID_Categoria ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para actualizar un producto existente
        public void ActualizarProducto(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"UPDATE Producto 
                            SET Nombre = @Nombre, Descripcion = @Descripcion, fkID_Categoria = @fkID_Categoria 
                            WHERE ID_Producto = @ID_Producto";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID_Producto", producto.ID_Producto);
            command.Parameters.AddWithValue("@Nombre", producto.Nombre);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@fkID_Categoria", producto.fkID_Categoria ?? (object)DBNull.Value);

            command.ExecuteNonQuery();
        }

        // Método para eliminar un producto
        public void EliminarProducto(string idProducto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = "DELETE FROM Producto WHERE ID_Producto = @idProducto";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idProducto", idProducto);

            command.ExecuteNonQuery();
        }

        // Método para obtener productos con stock
        public List<ProductoModel> ObtenerProductosConStock()
        {
            var productos = new List<ProductoModel>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            // Consulta básica para obtener productos con stock
            string query = @"
                SELECT 
                    p.ID_Producto, 
                    p.Nombre, 
                    p.Descripcion, 
                    c.Nombre AS NombreCategoria, 
                    IFNULL(vs.StockActual, 0) AS StockActual,
                    IFNULL(e.Precio_Venta, 0) AS PrecioVenta
                FROM 
                    Producto p
                LEFT JOIN 
                    Categoria c ON p.fkID_Categoria = c.ID_Categoria
                LEFT JOIN 
                    VistaStock vs ON p.ID_Producto = vs.ID_Producto
                LEFT JOIN 
                    (SELECT 
                        fkID_Producto, 
                        Precio_Venta,
                        MAX(FechaEntrada) AS UltimaFecha
                     FROM Entrada
                     GROUP BY fkID_Producto) AS e ON p.ID_Producto = e.fkID_Producto";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new ProductoModel
                {
                    ID_Producto = reader["ID_Producto"].ToString(),
                    Nombre = reader["Nombre"].ToString(),
                    Descripcion = reader["Descripcion"]?.ToString() ?? "",
                    NombreCategoria = reader["NombreCategoria"]?.ToString() ?? "Sin categoría",
                    StockActual = Convert.ToInt32(reader["StockActual"]),
                    PrecioVenta = reader.IsDBNull(reader.GetOrdinal("PrecioVenta")) 
                        ? 0 
                        : Convert.ToDecimal(reader["PrecioVenta"])
                });
            }

            return productos;
        }

        // Método optimizado para obtener productos con stock y precio actual
        public List<ProductoModel> ObtenerProductosConStockYPrecio()
        {
            var productos = new List<ProductoModel>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            // Esta consulta obtiene el precio de venta más reciente para cada producto
            string query = @"
                SELECT 
                    p.ID_Producto, 
                    p.Nombre, 
                    p.Descripcion, 
                    c.Nombre AS NombreCategoria, 
                    IFNULL(vs.StockActual, 0) AS StockActual,
                    IFNULL(
                        (SELECT e.Precio_Venta 
                         FROM Entrada e 
                         WHERE e.fkID_Producto = p.ID_Producto 
                         ORDER BY e.FechaEntrada DESC 
                         LIMIT 1), 
                        0
                    ) AS PrecioVenta
                FROM 
                    Producto p
                LEFT JOIN 
                    Categoria c ON p.fkID_Categoria = c.ID_Categoria
                LEFT JOIN 
                    VistaStock vs ON p.ID_Producto = vs.ID_Producto";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new ProductoModel
                {
                    ID_Producto = reader["ID_Producto"].ToString(),
                    Nombre = reader["Nombre"].ToString(),
                    Descripcion = reader["Descripcion"]?.ToString() ?? "",
                    NombreCategoria = reader["NombreCategoria"]?.ToString() ?? "Sin categoría",
                    StockActual = Convert.ToInt32(reader["StockActual"]),
                    PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"])
                });
            }

            return productos;
        }

        // Método para obtener productos extendidos con detalles completos
        public List<ProductoExtendido> ObtenerProductosExtendidos()
        {
            var productos = new List<ProductoExtendido>();

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"
                SELECT 
                    p.ID_Producto, 
                    p.Nombre, 
                    p.Descripcion, 
                    p.fkID_Categoria,
                    c.Nombre AS NombreCategoria, 
                    IFNULL(vs.StockActual, 0) AS StockActual,
                    IFNULL(
                        (SELECT e.Precio_Venta 
                         FROM Entrada e 
                         WHERE e.fkID_Producto = p.ID_Producto 
                         ORDER BY e.FechaEntrada DESC 
                         LIMIT 1), 
                        0
                    ) AS PrecioVenta,
                    IFNULL(
                        (SELECT e.Precio_Entrada 
                         FROM Entrada e 
                         WHERE e.fkID_Producto = p.ID_Producto 
                         ORDER BY e.FechaEntrada DESC 
                         LIMIT 1), 
                        0
                    ) AS PrecioEntrada
                FROM 
                    Producto p
                LEFT JOIN 
                    Categoria c ON p.fkID_Categoria = c.ID_Categoria
                LEFT JOIN 
                    VistaStock vs ON p.ID_Producto = vs.ID_Producto";

            using var command = new MySqlCommand(query, connection);
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                productos.Add(new ProductoExtendido
                {
                    ID_Producto = reader["ID_Producto"].ToString(),
                    Nombre = reader["Nombre"].ToString(),
                    Descripcion = reader["Descripcion"]?.ToString() ?? "",
                    fkID_Categoria = reader.IsDBNull(reader.GetOrdinal("fkID_Categoria")) 
                        ? null 
                        : Convert.ToInt32(reader["fkID_Categoria"]),
                    NombreCategoria = reader["NombreCategoria"]?.ToString() ?? "Sin categoría",
                    StockActual = Convert.ToInt32(reader["StockActual"]),
                    PrecioVenta = Convert.ToDecimal(reader["PrecioVenta"]),
                    PrecioEntrada = Convert.ToDecimal(reader["PrecioEntrada"])
                });
            }

            return productos;
        }

        // Método para verificar si existe un producto
        public bool ExisteProducto(string idProducto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = "SELECT COUNT(*) FROM Producto WHERE ID_Producto = @idProducto";
            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@idProducto", idProducto);

            return Convert.ToInt32(command.ExecuteScalar()) > 0;
        }

        // Método asíncrono para obtener todos los productos
        public async Task<List<Producto>> ObtenerTodosLosProductosAsync()
        {
            var productos = new List<Producto>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT * FROM Producto";
            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productos.Add(new Producto
                {
                    ID_Producto = reader["ID_Producto"].ToString(),
                    Nombre = reader["Nombre"].ToString(),
                    Descripcion = reader["Descripcion"]?.ToString(),
                    fkID_Categoria = reader["fkID_Categoria"] != DBNull.Value ? Convert.ToInt32(reader["fkID_Categoria"]) : null
                });
            }

            return productos;
        }
    
        public Producto BuscarProductoPorID(string id)
        {
            Producto producto = null;

            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            var query = "SELECT * FROM Producto WHERE ID_Producto = @id";
            using var cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                producto = new Producto
                {
                    ID_Producto = reader.GetString("ID_Producto"),
                    Nombre = reader.GetString("Nombre"),
                    Descripcion = reader.IsDBNull(reader.GetOrdinal("Descripcion")) ? null : reader.GetString("Descripcion"),
                    fkID_Categoria = reader.IsDBNull(reader.GetOrdinal("fkID_Categoria")) ? null : (int?)reader.GetInt32("fkID_Categoria")
                };
            }

            return producto;
        }

        // ✅ MÉTODO FALTANTE: Insertar un nuevo producto
        public void InsertarProducto(Producto producto)
        {
            using var connection = new MySqlConnection(_connectionString);
            connection.Open();

            string query = @"INSERT INTO Producto (ID_Producto, Nombre, Descripcion, fkID_Categoria) 
                           VALUES (@ID_Producto, @Nombre, @Descripcion, @fkID_Categoria)";

            using var command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID_Producto", producto.ID_Producto);
            command.Parameters.AddWithValue("@Nombre", producto.Nombre);
            command.Parameters.AddWithValue("@Descripcion", producto.Descripcion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@fkID_Categoria", producto.fkID_Categoria.HasValue ? 
                (object)producto.fkID_Categoria.Value : DBNull.Value);

            command.ExecuteNonQuery();
        }
}}
