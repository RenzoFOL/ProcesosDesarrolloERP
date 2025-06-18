    namespace ProyectoProcesosExperienciaV1.Data.Modelos
    {
        public class Salida
        {
            public int ID_Salida { get; set; }
            public string fkID_Producto { get; set; }
            public int Cantidad { get; set; }
            public DateTime FechaSalida { get; set; }
        }
    }
