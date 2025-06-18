namespace ProyectoProcesosExperienciaV1.Data.Modelos
{
    public class VistaStock
    {
        public string ID_Producto { get; set; }
        public string Nombre { get; set; }
        public int fkID_Categoria { get; set; }
        public int StockActual { get; set; }
    }
}
