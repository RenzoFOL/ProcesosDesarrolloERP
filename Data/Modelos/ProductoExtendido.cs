using System;
using System.ComponentModel.DataAnnotations;

namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class ProductoExtendido
    {
        [Display(Name = "ID")]
        public string ID_Producto { get; set; }
        
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }
        
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }
        
        [Display(Name = "Categoría ID")]
        public int? fkID_Categoria { get; set; }
        
        [Display(Name = "Categoría")]
        public string NombreCategoria { get; set; }
        
        [Display(Name = "Stock")]
        public int StockActual { get; set; }
        
        [Display(Name = "Precio de Venta")]
        [DataType(DataType.Currency)]
        public decimal? PrecioVenta { get; set; }
        
        [Display(Name = "Precio de Entrada")]
        [DataType(DataType.Currency)]
        public decimal? PrecioEntrada { get; set; }
        
        // Propiedades calculadas para la vista
        [Display(Name = "Estado")]
        public string EstadoStock => StockActual > 0 ? "Disponible" : "Agotado";
        
        [Display(Name = "Clase CSS")]
        public string ClaseEstado => StockActual > 0 ? "badge bg-success" : "badge bg-danger";

        
    }
}