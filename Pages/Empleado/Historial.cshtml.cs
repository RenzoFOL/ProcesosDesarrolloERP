using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoProcesosExperienciaV1.Pages.Empleado
{
    [Authorize(Roles = "Empleado")]
    public class HistorialModel : PageModel
    {
        private readonly SalidaDAO _salidaDAO;

        public List<Salida> Salidas { get; set; } = new List<Salida>();

        // 🔍 Búsqueda por ID o fecha
        [BindProperty(SupportsGet = true)]
        public string Busqueda { get; set; }

        // 🔽 Ordenamiento
        [BindProperty(SupportsGet = true)]
        public string Orden { get; set; }

        [BindProperty]
        public int ID_Salida { get; set; }

        [BindProperty]
        public string CodigoAutorizacion { get; set; }

        [TempData]
        public string MensajeExito { get; set; }

        [TempData]
        public string MensajeError { get; set; }

        public HistorialModel(IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("InventarioConnection");
            _salidaDAO = new SalidaDAO(connectionString);
        }

        public async Task OnGet()
        {
            try
            {
                // Obtener todas las salidas
                Salidas = await _salidaDAO.ObtenerTodasLasSalidasAsync();

                // Filtrado por búsqueda
                if (!string.IsNullOrEmpty(Busqueda))
                {
                    Busqueda = Busqueda.ToLower();

                    Salidas = Salidas.Where(s =>
                        (!string.IsNullOrEmpty(s.fkID_Producto) && s.fkID_Producto.ToLower().Contains(Busqueda)) ||
                        s.FechaSalida.ToString("dd/MM/yyyy").Contains(Busqueda)
                    ).ToList();
                }

                // Ordenamiento
                switch (Orden)
                {
                    case "FechaAsc":
                        Salidas = Salidas.OrderBy(s => s.FechaSalida).ToList();
                        break;
                    case "FechaDesc":
                        Salidas = Salidas.OrderByDescending(s => s.FechaSalida).ToList();
                        break;
                    case "Cantidad":
                        Salidas = Salidas.OrderByDescending(s => s.Cantidad).ToList();
                        break;
                    default:
                        // Orden por defecto (fecha descendente)
                        Salidas = Salidas.OrderByDescending(s => s.FechaSalida).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el historial: {ex.Message}";
                Salidas = new List<Salida>();
            }
        }

        public async Task<IActionResult> OnPostProcesarReclamo()
{
    if (CodigoAutorizacion != "1234")
    {
        MensajeError = "❌ Código de autorización inválido. Intente nuevamente.";
        return RedirectToPage();
    }

    try
    {
        bool resultado = await _salidaDAO.EliminarSalidaPorIDAsync(ID_Salida);

        if (resultado)
        {
            MensajeExito = $"✅ Venta #{ID_Salida} devuelta correctamente.";
        }
        else
        {
            MensajeError = $"❌ No se pudo procesar la devolución de la venta #{ID_Salida}.";
        }
    }
    catch (Exception ex)
    {
        MensajeError = $"❌ Error al procesar reclamo: {ex.Message}";
    }

    return RedirectToPage();
}

    }
}
