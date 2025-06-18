using ProyectoProcesosExperienciaV1.Data.Modelos;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProyectoProcesosExperienciaV1.Data.Dao
{
    public class VistaStockDAO
    {
        private readonly string connectionString;

        public VistaStockDAO(string connectionString)
        {
            this.connectionString = connectionString;
        }

        // Método para obtener todos los productos con su stock actual
        public async Task<List<VistaStock>> ObtenerStockAsync()
        {
            var listaStock = new List<VistaStock>();

            using (var connection = new MySqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM VistaStock";

                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        listaStock.Add(new VistaStock
                        {
                            ID_Producto = reader["ID_Producto"].ToString(),
                            Nombre = reader["Nombre"].ToString(),
                            fkID_Categoria = Convert.ToInt32(reader["fkID_Categoria"]),
                            StockActual = Convert.ToInt32(reader["StockActual"])
                        });
                    }
                }
            }

            return listaStock;
        }
    }
}
