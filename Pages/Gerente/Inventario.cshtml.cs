using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;

namespace ProyectoProcesosExperienciaV1.Pages.Gerente
{
    [Authorize(Roles = "Gerente")]
    public class InventarioModel : PageModel
    {
        private readonly ProductoDAO _productoDAO;
        private readonly string _connectionString;

        public List<ProductoModel> Productos { get; set; } = new List<ProductoModel>();

        [BindProperty(SupportsGet = true)]
        public string Busqueda { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Orden { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Categoria { get; set; }

        public InventarioModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventarioConnection");
            _productoDAO = new ProductoDAO(_connectionString);
        }

        public void OnGet()
        {
            try
            {
                Productos = _productoDAO.ObtenerProductosConStockYPrecio();

                if (!string.IsNullOrEmpty(Categoria) && Categoria != "Todas")
                {
                    Productos = Productos.Where(p =>
                        p.NombreCategoria != null &&
                        p.NombreCategoria.Equals(Categoria, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                if (!string.IsNullOrEmpty(Busqueda))
                {
                    Busqueda = Busqueda.ToLower();
                    Productos = Productos.Where(p =>
                        p.ID_Producto?.ToLower().Contains(Busqueda) == true ||
                        p.Nombre?.ToLower().Contains(Busqueda) == true ||
                        p.Descripcion?.ToLower().Contains(Busqueda) == true
                    ).ToList();
                }

                switch (Orden)
                {
                    case "AZ":
                        Productos = Productos.OrderBy(p => p.Nombre).ToList();
                        break;
                    case "ZA":
                        Productos = Productos.OrderByDescending(p => p.Nombre).ToList();
                        break;
                    case "Precio":
                        Productos = Productos.OrderBy(p => p.PrecioVenta).ToList();
                        break;
                    case "PrecioDesc":
                        Productos = Productos.OrderByDescending(p => p.PrecioVenta).ToList();
                        break;
                    case "Stock":
                        Productos = Productos.OrderBy(p => p.StockActual).ToList();
                        break;
                    case "StockDesc":
                        Productos = Productos.OrderByDescending(p => p.StockActual).ToList();
                        break;
                    default:
                        Productos = Productos.OrderBy(p => p.Nombre).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar el inventario: {ex.Message}";
                Productos = new List<ProductoModel>();
            }
        }
    }
}
