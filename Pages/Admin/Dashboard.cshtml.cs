using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoProcesosExperienciaV1.Pages.Admin
{
    [Authorize(Roles = "Administrador")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            // Aquí podrías cargar estadísticas o notificaciones importantes del sistema
        }
    }
}
