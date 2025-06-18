namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class EntradaExtendida
    {
        public int ID_Entrada { get; set; }
        public string ID_Producto { get; set; } = "";
        public string NombreProducto { get; set; } = "";
        public int Cantidad { get; set; }
        public decimal Precio_Entrada { get; set; }
        public decimal Precio_Venta { get; set; }
        public DateTime FechaEntrada { get; set; }
    }
}
