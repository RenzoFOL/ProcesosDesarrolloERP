using System.ComponentModel.DataAnnotations;

namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class Producto
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio")]
        [StringLength(10, ErrorMessage = "El ID no puede exceder los 10 caracteres")]
        [Display(Name = "ID Producto")]
        public string ID_Producto { get; set; }

        [Required(ErrorMessage = "El nombre del producto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Descripción")]
        public string Descripcion { get; set; }

        [Display(Name = "Categoría")]
        public int? fkID_Categoria { get; set; }
        public decimal PrecioVenta { get; set; }
    }

    // Clase de apoyo para mostrar productos básicos en listas
    public class ProductoModel
    {
        public string ID_Producto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string NombreCategoria { get; set; }
        public int StockActual { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}