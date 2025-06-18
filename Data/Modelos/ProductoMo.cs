using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class ProductoMo
    {
        public string ID_Producto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int StockActual { get; set; }
        public string NombreCategoria { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}