using Microsoft.AspNetCore.Identity;

namespace ProyectoProcesosExperienciaV1.Data.ApplicationDbContext
{
    /// <summary>
    /// Clase de usuario extendida con campos personalizados.
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        public bool IsEnabled { get; set; } = false;
        public string RoleAssigned { get; set; } = "Pendiente";
    }
}
