// Ejemplo para OlvidoModel.cs
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class OlvidoModel : PageModel
    {
        public void OnGet()
        {   
            ViewData["Step"] = "1";
        }
    }
}
