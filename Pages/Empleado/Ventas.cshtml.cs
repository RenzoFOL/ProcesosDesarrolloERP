using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ProyectoProcesosExperienciaV1.Data.Dao;
using ProyectoProcesosExperienciaV1.Data.Modelos;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProyectoProcesosExperienciaV1.Pages.Empleado
{
    [Authorize(Roles = "Empleado")]
    public class VentasModel : PageModel
    {
        [TempData]
        public bool MostrarTicket { get; set; }

        [TempData]
        public string TicketDataJson { get; set; }

        // Usar solo una lista para manejar el carrito
        public List<ProductoVenta> Carrito { get; set; } = new();

        // Datos del ticket para mostrar después de la venta
        public List<ProductoTicket> ProductosTicket { get; set; } = new();
        public decimal TotalTicket { get; set; }
        public decimal SubTotal { get; set; }
        public decimal MontoDescuento { get; set; }

        public decimal PorcentajeDescuento { get; set; }

        public DateTime FechaVenta { get; set; }

        public string Error { get; set; }
        public int ProximoIDVenta { get; set; }

        public string Mensaje { get; set; }
        public decimal Total => Carrito.Sum(p => p.Subtotal);

        private readonly ProductoDAO _productoDAO;
        private readonly SalidaDAO _salidaDAO;

        [BindProperty]
        public string ID_Producto { get; set; }

        [BindProperty]
        public int Cantidad { get; set; }

        // Propiedades para compatibilidad con la vista
        public List<ProductoModel> ProductosAgregados => ConvertirCarritoAProductos();
        public decimal TotalVenta => MostrarTicket ? TotalTicket : Total;

        [TempData]
        public string MensajeExito { get; set; }

        [TempData]
        public string MensajeError { get; set; }

        public VentasModel(IConfiguration configuration)
        {
            var conn = configuration.GetConnectionString("InventarioConnection");
            _productoDAO = new ProductoDAO(conn);
            _salidaDAO = new SalidaDAO(conn);
        }


        public IActionResult OnPostAgregar()
        {
            CargarCarritoDesdeSesion();
            ProximoIDVenta = _salidaDAO.ObtenerProximoIDSalida();

            var producto = _productoDAO.ObtenerProductoPorID(ID_Producto);

            if (producto == null)
            {
                MensajeError = $"❌ El producto con ID '{ID_Producto}' no existe.";
                return Page();
            }

            if (Cantidad <= 0)
            {
                MensajeError = "❌ La cantidad debe ser mayor a cero.";
                return Page();
            }

            if (Cantidad > producto.StockActual)
            {
                MensajeError = $"⚠️ No hay suficiente stock. Stock actual: {producto.StockActual}.";
                return Page();
            }

            var existente = Carrito.FirstOrDefault(p => p.ID_Producto == ID_Producto);

            if (existente != null)
            {
                int nuevaCantidadTotal = existente.Cantidad + Cantidad;
                if (nuevaCantidadTotal > producto.StockActual)
                {
                    MensajeError = $"⚠️ Stock insuficiente. Solo hay {producto.StockActual} unidades disponibles.";
                    return Page();
                }

                existente.Cantidad = nuevaCantidadTotal;
            }
            else
            {
                Carrito.Add(new ProductoVenta
                {
                    ID_Producto = producto.ID_Producto,
                    NombreProducto = producto.Nombre,
                    PrecioVenta = producto.PrecioVenta,
                    Cantidad = Cantidad
                });
            }

            GuardarCarritoEnSesion();
            MensajeExito = "✅ Producto añadido a la venta.";

            ID_Producto = string.Empty;
            Cantidad = 0;

            return RedirectToPage();
        }

/// <summary>
/// Método para manejar productos agregados desde la página de inventario
/// Este método se ejecuta cuando se redirige desde inventario con un producto
/// </summary>
public IActionResult OnPostAgregarDesdeInventario()
{
    try
    {
        CargarCarritoDesdeSesion();
        ProximoIDVenta = _salidaDAO.ObtenerProximoIDSalida();
        
        // Validaciones básicas
        if (string.IsNullOrWhiteSpace(ID_Producto))
        {
            MensajeError = "❌ ID de producto no válido.";
            return Page();
        }
        
        if (Cantidad <= 0)
        {
            MensajeError = "❌ La cantidad debe ser mayor a cero.";
            return Page();
        }
        
        // Obtener información del producto
        var producto = _productoDAO.ObtenerProductoPorID(ID_Producto);
        if (producto == null)
        {
            MensajeError = $"❌ El producto con ID '{ID_Producto}' no existe.";
            return Page();
        }
        
        // Verificar stock disponible
        if (Cantidad > producto.StockActual)
        {
            MensajeError = $"⚠️ Stock insuficiente. Stock disponible: {producto.StockActual} unidades.";
            return Page();
        }
        
        // Verificar si el producto ya está en el carrito
        var productoExistente = Carrito.FirstOrDefault(p => p.ID_Producto == ID_Producto);
        
        if (productoExistente != null)
        {
            // Verificar que la suma total no exceda el stock
            int cantidadTotal = productoExistente.Cantidad + Cantidad;
            if (cantidadTotal > producto.StockActual)
            {
                MensajeError = $"⚠️ Stock insuficiente. Ya tiene {productoExistente.Cantidad} unidades en el carrito. " +
                              $"Stock disponible: {producto.StockActual} unidades.";
                return Page();
            }
            
            // Actualizar cantidad existente
            productoExistente.Cantidad = cantidadTotal;
            MensajeExito = $"✅ Se actualizó la cantidad de '{producto.Nombre}' a {cantidadTotal} unidades.";
        }
        else
        {
            // Agregar nuevo producto al carrito
            var nuevoProducto = new ProductoVenta
            {
                ID_Producto = producto.ID_Producto,
                NombreProducto = producto.Nombre,
                PrecioVenta = producto.PrecioVenta,
                Cantidad = Cantidad
            };
            
            Carrito.Add(nuevoProducto);
            MensajeExito = $"✅ Se agregaron {Cantidad} unidades de '{producto.Nombre}' al carrito.";
        }
        
        // Recalcular totales
        CalcularTotales();
        
        // Guardar carrito actualizado en sesión
        GuardarCarritoEnSesion();
        
        // Limpiar campos del formulario
        ID_Producto = string.Empty;
        Cantidad = 1;
        
        return Page();
    }
    catch (Exception ex)
    {
        MensajeError = $"❌ Error al agregar producto desde inventario: {ex.Message}";
        return Page();
    }
}
        // Método para limpiar los datos del ticket
        public async Task<IActionResult> OnPostLimpiarTicketAsync()
        {
            try
            {
                // Limpiar flags del ticket
                MostrarTicket = false;
                TicketDataJson = null;

                // Limpiar listas del ticket
                ProductosTicket.Clear();
                TotalTicket = 0;

                // También limpiar mensajes
                MensajeExito = null;
                MensajeError = null;

                return new JsonResult(new { success = true, message = "Ticket limpiado correctamente" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error al limpiar ticket: {ex.Message}" });
            }
        }

        // Método para vaciar carrito completamente después de una venta
        public async Task<IActionResult> OnPostVaciarCarritoCompletoAsync()
        {
            try
            {
                // Limpiar carrito
                Carrito.Clear();
                LimpiarSesion();
                
                // Limpiar datos del ticket
                MostrarTicket = false;
                TicketDataJson = null;
                ProductosTicket.Clear();
                TotalTicket = 0;
                
                // Limpiar mensajes
                MensajeExito = null;
                MensajeError = null;
                
                // Resetear campos del formulario
                ID_Producto = string.Empty;
                Cantidad = 0;
                
                return new JsonResult(new { success = true, message = "Carrito y ticket limpiados completamente" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error al limpiar: {ex.Message}" });
            }
        }

        // Método mejorado para confirmar venta con mejor manejo del estado
        public async Task<IActionResult> OnPostConfirmarAsync()
        {
            CargarCarritoDesdeSesion();

            if (Carrito == null || !Carrito.Any())
            {
                MensajeError = "❌ No hay productos para vender.";
                return Page();
            }

            try
            {
                var idsVenta = await _salidaDAO.RegistrarVentaCompletaAsync(Carrito);

                if (idsVenta != null && idsVenta.Any())
                {
                    // Crear datos del ticket ANTES de limpiar el carrito
                    var productosTicket = new List<ProductoTicket>();
                    var totalVenta = Total; // Capturar el total antes de limpiar
                    
                    for (int i = 0; i < Carrito.Count; i++)
                    {
                        var producto = Carrito[i];
                        var idSalida = idsVenta[i];
                        
                        productosTicket.Add(new ProductoTicket
                        {
                            ID_Salida = idSalida,
                            ID_Producto = producto.ID_Producto,
                            NombreProducto = producto.NombreProducto,
                            Cantidad = producto.Cantidad,
                            PrecioVenta = producto.PrecioVenta,
                            Subtotal = producto.Subtotal
                        });
                    }

                    // Guardar datos del ticket en TempData para persistir entre requests
                    var ticketData = new TicketData
                    {
                        Productos = productosTicket,
                        Total = totalVenta,
                        Fecha = DateTime.Now
                    };
                    
                    TicketDataJson = JsonSerializer.Serialize(ticketData);
                    MostrarTicket = true;
                    
                    // Guardar también en las propiedades para uso inmediato
                    ProductosTicket = productosTicket;
                    TotalTicket = totalVenta;
                    FechaVenta = DateTime.Now;

                    // NO limpiar el carrito aquí - se limpiará después del modal de éxito
                    // Esto permite que el ticket se muestre correctamente

                    MensajeExito = "🎉 ¡Venta realizada exitosamente!";
                    return RedirectToPage();
                }
                else
                {
                    MensajeError = "❌ No se pudo registrar la venta.";
                    return Page();
                }
            }
            catch (Exception ex)
            {
                MensajeError = $"❌ Error al confirmar venta: {ex.Message}";
                return Page();
            }
        }

        // Método auxiliar para limpiar completamente el estado después de una venta exitosa
        private void LimpiarEstadoCompleto()
        {
            // Limpiar carrito
            Carrito.Clear();
            LimpiarSesion();
            
            // Limpiar datos del ticket
            MostrarTicket = false;
            TicketDataJson = null;
            ProductosTicket.Clear();
            TotalTicket = 0;
            
            // Resetear campos del formulario
            ID_Producto = string.Empty;
            Cantidad = 0;
            ID_Actualizar = string.Empty;
            NuevaCantidad = 0;
            
            // Limpiar mensajes
            MensajeExito = null;
            MensajeError = null;
            Error = null;
            Mensaje = null;
        }

        // Método para verificar el estado y limpiar si es necesario
        public async Task<IActionResult> OnPostVerificarEstadoAsync()
        {
            try
            {
                // Si hay ticket pero no hay carrito, algo salió mal
                if (MostrarTicket && (Carrito == null || !Carrito.Any()))
                {
                    LimpiarEstadoCompleto();
                    return new JsonResult(new { success = true, message = "Estado limpiado", reload = true });
                }
                
                return new JsonResult(new { success = true, message = "Estado correcto" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // Sobrescribir el método OnGet para mejor manejo del estado
        public void OnGet()
        {
            CargarCarritoDesdeSesion();
            ProximoIDVenta = _salidaDAO.ObtenerProximoIDSalida();

            // Si hay datos del ticket, cargarlos
            if (MostrarTicket && !string.IsNullOrEmpty(TicketDataJson))
            {
                try
                {
                    var ticketData = JsonSerializer.Deserialize<TicketData>(TicketDataJson);
                    ProductosTicket = ticketData.Productos;
                    TotalTicket = ticketData.Total;
                    FechaVenta = ticketData.Fecha;
                }
                catch
                {
                    // Si falla la deserialización, limpiar todo
                    MostrarTicket = false;
                    TicketDataJson = null;
                    ProductosTicket.Clear();
                    TotalTicket = 0;
                }
            }
            
            // Verificar consistencia del estado
            if (MostrarTicket && (ProductosTicket == null || !ProductosTicket.Any()))
            {
                // Estado inconsistente, limpiar
                MostrarTicket = false;
                TicketDataJson = null;
            }
        }
        private void CargarCarritoDesdeSesion()
        {
            var data = HttpContext.Session.GetString("CarritoVenta");
            if (!string.IsNullOrEmpty(data))
            {
                try
                {
                    Carrito = JsonSerializer.Deserialize<List<ProductoVenta>>(data) ?? new List<ProductoVenta>();
                }
                catch
                {
                    Carrito = new List<ProductoVenta>();
                }
            }
        }

        private void GuardarCarritoEnSesion()
        {
            try
            {
                var json = JsonSerializer.Serialize(Carrito);
                HttpContext.Session.SetString("CarritoVenta", json);
            }
            catch
            {
                // Manejar error de serialización si es necesario
            }
        }
        /// <summary>
/// Método auxiliar para recalcular los totales del carrito
/// </summary>
private void CalcularTotales()
{
    SubTotal = Carrito.Sum(p => p.Subtotal);

    if (PorcentajeDescuento > 0)
    {
        MontoDescuento = SubTotal * (PorcentajeDescuento / 100);
    }
    else
    {
        MontoDescuento = 0;
    }

    // No necesitas asignar Total, ya se calcula automáticamente con su expresión:
    // public decimal Total => Carrito.Sum(p => p.Subtotal);
}


        private void LimpiarSesion()
        {
            HttpContext.Session.Remove("CarritoVenta");
        }

        // Método para convertir el carrito a ProductoModel para compatibilidad con la vista
        private List<ProductoModel> ConvertirCarritoAProductos()
        {
            // Si estamos mostrando el ticket, usar los datos del ticket
            if (MostrarTicket && ProductosTicket.Any())
            {
                return ProductosTicket.Select(item => new ProductoModel
                {
                    ID_Producto = item.ID_Producto,
                    Nombre = item.NombreProducto,
                    PrecioVenta = item.PrecioVenta,
                    StockActual = item.Cantidad
                }).ToList();
            }

            // Sino, usar el carrito normal
            return Carrito.Select(item => new ProductoModel
            {
                ID_Producto = item.ID_Producto,
                Nombre = item.NombreProducto,
                PrecioVenta = item.PrecioVenta,
                StockActual = item.Cantidad
            }).ToList();
        }

        public IActionResult OnPostEliminar(string id)
        {
            CargarCarritoDesdeSesion();

            var producto = Carrito.FirstOrDefault(p => p.ID_Producto == id);

            if (producto != null)
            {
                Carrito.Remove(producto);
                GuardarCarritoEnSesion();
                MensajeExito = $"✅ Producto '{producto.NombreProducto}' eliminado del carrito.";
            }
            else
            {
                MensajeError = "❌ El producto no fue encontrado en el carrito.";
            }

            return RedirectToPage();
        }

        public JsonResult OnGetObtenerProducto(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new JsonResult(null);

            var producto = _productoDAO.ObtenerProductoPorID(id);
            if (producto == null)
                return new JsonResult(null);

            return new JsonResult(new
            {
                producto.ID_Producto,
                producto.Nombre,
                producto.NombreCategoria,
                producto.Descripcion,
                producto.StockActual,
                PrecioVenta = producto.PrecioVenta.ToString("C")
            });
        }

        public IActionResult OnPostVaciar()
        {
            LimpiarSesion();
            MensajeExito = "🗑 Carrito vaciado exitosamente.";
            return RedirectToPage();
        }

        [BindProperty]
        public string ID_Actualizar { get; set; }

        [BindProperty]
        public int NuevaCantidad { get; set; }

        public IActionResult OnPostActualizarCantidad()
        {
            CargarCarritoDesdeSesion();
            var productoEnCarrito = Carrito.FirstOrDefault(p => p.ID_Producto == ID_Actualizar);
            var productoReal = _productoDAO.ObtenerProductoPorID(ID_Actualizar);

            if (productoEnCarrito == null || productoReal == null)
            {
                MensajeError = "❌ Producto no encontrado para actualizar.";
                return RedirectToPage();
            }

            if (NuevaCantidad <= 0)
            {
                MensajeError = "❌ La cantidad debe ser mayor a cero.";
                return RedirectToPage();
            }

            if (NuevaCantidad > productoReal.StockActual)
            {
                MensajeError = $"⚠️ Stock insuficiente. Solo hay {productoReal.StockActual} unidades disponibles.";
                return RedirectToPage();
            }

            productoEnCarrito.Cantidad = NuevaCantidad;
            GuardarCarritoEnSesion();

            MensajeExito = "✅ Cantidad actualizada correctamente.";
            return RedirectToPage();
        }
    }

    // Clases auxiliares para el ticket
    public class ProductoTicket
    {
        public int ID_Salida { get; set; }
        public string ID_Producto { get; set; }
        public string NombreProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioVenta { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class TicketData
    {
        public List<ProductoTicket> Productos { get; set; } = new();
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
    }
    
}