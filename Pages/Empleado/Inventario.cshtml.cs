using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Text.Json;

namespace ProyectoProcesosExperienciaV1.Pages.Empleado
{
    [Authorize(Roles = "Empleado")]
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

        // Propiedades para manejar el agregado al carrito desde inventario
        [BindProperty]
        public string ID_Producto { get; set; }

        [BindProperty]
        public int Cantidad { get; set; }

        [TempData]
        public string MensajeExito { get; set; }

        [TempData]
        public string MensajeError { get; set; }

        public InventarioModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("InventarioConnection");
            _productoDAO = new ProductoDAO(_connectionString);
        }


        public void OnGet()
        {
            try
            {
                // Usar el método actualizado que incluye el precio correctamente
                Productos = _productoDAO.ObtenerProductosConStockYPrecio();

                // Filtrar por categoría si se especifica
                if (!string.IsNullOrEmpty(Categoria) && Categoria != "Todas")
                {
                    Productos = Productos.Where(p =>
                        p.NombreCategoria != null &&
                        p.NombreCategoria.Equals(Categoria, StringComparison.OrdinalIgnoreCase)
                    ).ToList();
                }

                // Filtrar por búsqueda
                if (!string.IsNullOrEmpty(Busqueda))
                {
                    Busqueda = Busqueda.ToLower();
                    Productos = Productos.Where(p =>
                        p.ID_Producto != null && p.ID_Producto.ToLower().Contains(Busqueda) ||
                        p.Nombre != null && p.Nombre.ToLower().Contains(Busqueda) ||
                        p.Descripcion != null && p.Descripcion.ToLower().Contains(Busqueda)
                    ).ToList();
                }

                // Ordenar
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
                        // Por defecto, ordenar por nombre ascendente
                        Productos = Productos.OrderBy(p => p.Nombre).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {
                // Manejar la excepción según sea necesario
                // Por ejemplo, almacenar en un log, mostrar un mensaje amigable, etc.
                TempData["ErrorMessage"] = $"Error al cargar el inventario: {ex.Message}";
                Productos = new List<ProductoModel>();
            }
        }

        /// <summary>
        /// Método para agregar un producto al carrito desde el inventario y redirigir a ventas
        /// </summary>
        public IActionResult OnPostAgregarDesdeInventario()
        {
            try
            {
                // Validar que se hayan proporcionado los datos necesarios
                if (string.IsNullOrWhiteSpace(ID_Producto))
                {
                    TempData["MensajeError"] = "❌ Debe especificar un ID de producto válido.";
                    return RedirectToPage();
                }

                if (Cantidad <= 0)
                {
                    TempData["MensajeError"] = "❌ La cantidad debe ser mayor a cero.";
                    return RedirectToPage();
                }

                // Verificar que el producto existe y tiene stock suficiente
                var producto = _productoDAO.ObtenerProductoPorID(ID_Producto);
                if (producto == null)
                {
                    TempData["MensajeError"] = $"❌ El producto con ID '{ID_Producto}' no existe.";
                    return RedirectToPage();
                }

                if (Cantidad > producto.StockActual)
                {
                    TempData["MensajeError"] = $"⚠️ No hay suficiente stock. Stock disponible: {producto.StockActual} unidades.";
                    return RedirectToPage();
                }

                // Obtener el carrito actual de la sesión (si existe)
                var carritoExistente = ObtenerCarritoDeSesion();

                // Verificar si el producto ya está en el carrito
                var productoEnCarrito = carritoExistente.FirstOrDefault(p => p.ID_Producto == ID_Producto);

                if (productoEnCarrito != null)
                {
                    // Si ya existe, verificar que la nueva cantidad total no exceda el stock
                    int nuevaCantidadTotal = productoEnCarrito.Cantidad + Cantidad;
                    if (nuevaCantidadTotal > producto.StockActual)
                    {
                        TempData["MensajeError"] = $"⚠️ Stock insuficiente. Ya tiene {productoEnCarrito.Cantidad} unidades en el carrito. Stock disponible: {producto.StockActual} unidades.";
                        return RedirectToPage();
                    }

                    // Actualizar la cantidad
                    productoEnCarrito.Cantidad = nuevaCantidadTotal;
                }
                else
                {
                    // Agregar nuevo producto al carrito
                    carritoExistente.Add(new ProductoVenta
                    {
                        ID_Producto = producto.ID_Producto,
                        NombreProducto = producto.Nombre,
                        PrecioVenta = producto.PrecioVenta,
                        Cantidad = Cantidad
                    });
                }

                // Guardar el carrito actualizado en la sesión
                GuardarCarritoEnSesion(carritoExistente);

                // Configurar mensaje de éxito
                TempData["MensajeExito"] = $"✅ Se agregaron {Cantidad} unidades de '{producto.Nombre}' al carrito.";

                // Redirigir a la página de ventas
                return RedirectToPage("/Empleado/Ventas");
            }
            catch (Exception ex)
            {
                TempData["MensajeError"] = $"❌ Error al agregar producto al carrito: {ex.Message}";
                return RedirectToPage();
            }
        }

        /// <summary>
        /// Obtiene el carrito actual de la sesión
        /// </summary>
        private List<ProductoVenta> ObtenerCarritoDeSesion()
        {
            var carritoJson = HttpContext.Session.GetString("CarritoVenta");
            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<ProductoVenta>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<ProductoVenta>>(carritoJson) ?? new List<ProductoVenta>();
            }
            catch
            {
                // Si hay error en la deserialización, retornar carrito vacío
                return new List<ProductoVenta>();
            }
        }

        /// <summary>
        /// Guarda el carrito en la sesión
        /// </summary>
        private void GuardarCarritoEnSesion(List<ProductoVenta> carrito)
        {
            try
            {
                var carritoJson = JsonSerializer.Serialize(carrito);
                HttpContext.Session.SetString("CarritoVenta", carritoJson);
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                Console.WriteLine($"Error al guardar carrito en sesión: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Clase para representar un producto en el carrito de ventas
    /// (Esta clase debería estar en el mismo namespace o importada desde donde esté definida)
    /// </summary>

}