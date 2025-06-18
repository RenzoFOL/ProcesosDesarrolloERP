// Ejemplo para OlvidoModel.cs
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class NuevaContrasenaModel : PageModel
    {
        public void OnGet()
        {   
            ViewData["Step"] = "3";
         }
    }
}
