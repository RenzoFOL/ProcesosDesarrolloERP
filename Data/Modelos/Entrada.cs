namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class Entrada
    {
        public int ID_Entrada { get; set; }
        public string fkID_Producto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio_Entrada { get; set; }
        public decimal Precio_Venta { get; set; }
        public DateTime FechaEntrada { get; set; }
    }
}
