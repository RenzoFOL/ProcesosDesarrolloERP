using System.ComponentModel.DataAnnotations;

namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    /// <summary>
    /// Representa una línea de venta simplificada.
    /// </summary>
    public class LineaVenta
    {
        [Required(ErrorMessage = "El ID del producto es obligatorio")]
        public string ID_Producto { get; set; } = string.Empty;

        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a cero")]
        public int Cantidad { get; set; }

        [Display(Name = "Producto")]
        public string NombreProducto { get; set; } = string.Empty;
    }
}
