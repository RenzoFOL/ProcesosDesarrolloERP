namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class ProductoVenta
    {
        public int ID_Salida { get; set; }
        public string ID_Producto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }

        public decimal Subtotal => Cantidad * PrecioVenta;

        public int ProximoIDVenta { get; set; }

    }
}
