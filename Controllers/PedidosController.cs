using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

[Authorize]
public class PedidosController : Controller
{
    private readonly AmazonContext _context;
    private readonly UserManager<Usuario> _userManager;


    public PedidosController(AmazonContext context, UserManager<Usuario> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Historial()
    {
        var usuarioId = User.Identity.Name; // Usa el email o username como identificador
        var pedidos = _context.Pedidos
            .Where(p => p.Usuario.UserName == usuarioId)
            .Include(p => p.Detalles)
            .ThenInclude(d => d.Producto)
            .ToList();

        return View(pedidos);
    }

    // Muestra el formulario de pago
    public IActionResult Pasarela()
    {
        var carrito = HttpContext.Session.GetObject<List<DetallePedido>>("Carrito");
        if (carrito == null || !carrito.Any())
        {
            TempData["Error"] = "Tu carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        ViewBag.Total = carrito.Sum(d => d.Cantidad * d.Precio);
        return View();
    }

    // Procesa los datos del pago
    [HttpPost]
    public IActionResult ProcesarPago(string numeroTarjeta, string fechaExpiracion, string cvv)
    {
        // Obtiene el carrito de la sesión
        var carrito = HttpContext.Session.GetObject<List<DetallePedido>>("Carrito");
        if (carrito == null || !carrito.Any())
        {
            TempData["Error"] = "Tu carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        // Simulación de validación de pago
        if (!ValidarPago(numeroTarjeta, fechaExpiracion, cvv))
        {
            TempData["Error"] = $"Los datos de la tarjeta son inválidos {numeroTarjeta} {fechaExpiracion} {cvv}";
            return RedirectToAction("Error");
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var usuario = _context.Usuarios.Find(userId);
                if (userId == null || usuario == null)
                {
                    TempData["Error"] = "Debe iniciar sesión para hacer una reserva.";
                    return RedirectToAction("Login", "Usuario");
                }
                // Generar el pedido
                var pedido = new Pedido
                {
                    UsuarioId = userId,
                    Detalles = new List<DetallePedido>(),
                    PrecioTotal = carrito.Sum(d => d.Cantidad * d.Precio),
                    Estado = "Pagado",
                    Fecha = DateTime.Now
                };

                // Reducir el stock de productos y crear detalles del pedido
                foreach (var detalle in carrito)
                {
                    var producto = _context.Productos.Find(detalle.ProductoId);
                    if (producto == null)
                    {
                        TempData["Error"] = $"El producto con ID {detalle.ProductoId} no existe.";
                        return RedirectToAction("Index", "Carrito");
                    }

                    if (producto.Stock < detalle.Cantidad)
                    {
                        TempData["Error"] = $"No hay suficiente stock para el producto '{producto.Nombre}'.";
                        return RedirectToAction("Index", "Carrito");
                    }

                    producto.Stock -= detalle.Cantidad;
                    pedido.Detalles.Add(new DetallePedido
                    {
                        ProductoId = producto.Id,
                        Cantidad = detalle.Cantidad,
                        Precio = producto.Precio
                    });
                }

                // Guardar el pedido y los cambios en productos
                _context.Pedidos.Add(pedido);
                _context.SaveChanges();

                // Confirmar la transacción
                transaction.Commit();

                // Limpia el carrito
                HttpContext.Session.Remove("Carrito");

                // Enviar datos al éxito
                // TempData["Recibo"] = Guid.NewGuid().ToString();
                // TempData["FechaPago"] = DateTime.Now.ToString("g");
                // TempData["UsuarioNombre"] = User.Identity.Name;
                // TempData["Productos"] = string.Join(", ", carrito.Select(d => d.Producto.Nombre));

                ViewBag.Recibo = Guid.NewGuid().ToString();
                ViewBag.FechaPago = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.Estado = "Pagado";
                ViewBag.UsuarioNombre = usuario.Nombre;
                ViewBag.Productos = string.Join(", ", carrito.Select(d => d.Producto.Nombre)); // O cualquier otro detalle relevante de la ruta

                return PartialView("_ConfirmacionPagoModal");
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                TempData["Error"] = $"Ocurrió un error al procesar el pago: {innerException}";
                transaction.Rollback();
                return RedirectToAction("Error");
            }
            catch (Exception ex)
            {
                // Revertir cambios en caso de error
                transaction.Rollback();

                TempData["Error"] = $"Ocurrió un error al procesar el pago: {ex.Message}";
                return RedirectToAction("Error");
            }
        }
    }

    // Método auxiliar para validar los datos de la tarjeta
    private bool ValidarPago(string numeroTarjeta, string fechaExpiracion, string cvv)
    {
        if (string.IsNullOrEmpty(numeroTarjeta) ||
            string.IsNullOrEmpty(fechaExpiracion) ||
            string.IsNullOrEmpty(cvv))
        {
            return false;
        }

        // Validar longitud y prefijo de la tarjeta (por ejemplo, tarjetas Visa inician con 4)
        if (numeroTarjeta.Length != 16)
        {
            return false;
        }

        // Validar formato de fecha de expiración
        if (!DateTime.TryParseExact(fechaExpiracion, "MM/yy", null, System.Globalization.DateTimeStyles.None, out var fechaExp))
        {
            return false;
        }

        // Verificar que la fecha no esté expirada
        if (fechaExp < DateTime.Now)
        {
            return false;
        }

        // Validar longitud del CVV
        if (cvv.Length != 3 || !cvv.All(char.IsDigit))
        {
            return false;
        }

        return true;
    }

    public IActionResult Confirmar()
    {
        var pedido = new PedidoViewModel
        {
            Detalles = ObtenerDetallesDelCarrito(), // Implementar esta función para obtener los detalles del carrito
            Total = CalcularTotal() // Implementar esta función para calcular el total del pedido
        };
        return View(pedido);
    }

    private List<DetallePedido> ObtenerDetallesDelCarrito()
    {
        // Supongamos que los detalles del carrito están almacenados en la sesión
        var carrito = HttpContext.Session.GetObjectFromJson<List<DetallePedido>>("Carrito");
        return carrito ?? new List<DetallePedido>(); // Devuelve una lista vacía si no hay carrito
    }

    private decimal CalcularTotal()
    {
        var carrito = ObtenerDetallesDelCarrito();
        return carrito.Sum(item => item.Cantidad * item.Precio);
    }

    public IActionResult Error()
    {
        var mensajeError = TempData["Error"] as string ?? "Ocurrió un error inesperado.";
        return View("Error", mensajeError);
    }

    [HttpPost]
    public async Task<IActionResult> CancelarPedido(int pedidoId)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Detalles)
            .FirstOrDefaultAsync(p => p.Id == pedidoId);

        if (pedido == null)
        {
            TempData["Error"] = "No se pudo encontrar el pedido.";
            return RedirectToAction("Historial", "Usuario");
        }

        var fechaActual = DateTime.Now;
        if ((fechaActual - pedido.Fecha).TotalDays > 2)
        {
            TempData["Error"] = "No se puede cancelar el pedido porque han pasado más de dos días desde su creación.";
            return RedirectToAction("Historial", "Usuario");
        }

        try
        {
            _context.DetallePedidos.RemoveRange(pedido.Detalles);

            _context.Pedidos.Remove(pedido);

            await _context.SaveChangesAsync();

            TempData["Success"] = "Pedido cancelado exitosamente.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error al intentar cancelar el pedido: {ex.Message}";
        }

        return RedirectToAction("Historial", "Usuario");
    }




}
