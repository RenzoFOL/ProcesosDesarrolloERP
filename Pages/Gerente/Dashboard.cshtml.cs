using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace ProyectoProcesosExperienciaV1.Pages.Gerente
{
    [Authorize(Roles = "Gerente")]
    public class DashboardModel : PageModel
    {
        public void OnGet()
        {
            // No hay lógica por ahora porque los datos son simulados
        }
    }
}
