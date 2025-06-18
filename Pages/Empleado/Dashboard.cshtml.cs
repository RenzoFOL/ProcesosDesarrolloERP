using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoProcesosExperienciaV1.Pages.Empleado
{
    [Authorize(Roles = "Empleado")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            // Aquí el empleado podría ver su nombre, tareas asignadas o acciones rápidas
        }
    }
}
