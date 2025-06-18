using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ProyectoProcesosExperienciaV1.Pages.Account
{
    public class CodigoVerificacionModel : PageModel
    {
public void OnGet()
{
    ViewData["Step"] = "2";
}

    }
    
}
