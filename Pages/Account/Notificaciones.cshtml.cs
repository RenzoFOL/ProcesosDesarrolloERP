using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class NotificacionesModel : PageModel
    {
        public class Notificacion
        {
            public string Titulo { get; set; }
            public string Mensaje { get; set; }
            public DateTime Fecha { get; set; }
        }

        public List<Notificacion> Notificaciones { get; set; }

        public void OnGet()
        {
            // Simulación de notificaciones, luego puede conectarse a base de datos
            Notificaciones = new List<Notificacion>
            {
                new Notificacion {
                    Titulo = "Actualización del sistema",
                    Mensaje = "Se han corregido errores en el módulo de ventas.",
                    Fecha = DateTime.Now.AddDays(-1)
                },
                new Notificacion {
                    Titulo = "Nueva función disponible",
                    Mensaje = "Ahora puedes exportar reportes en formato PDF.",
                    Fecha = DateTime.Now.AddDays(-3)
                }
            };
        }
    }
}
