using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class PedidosController : Controller
{
    private readonly AmazonContext _context;

    public PedidosController(AmazonContext context)
    {
        _context = context;
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

    public IActionResult ConfirmarPedido()
    {
        var usuarioId = User.Identity.Name;
        var usuario = _context.Usuarios.FirstOrDefault(u => u.UserName == usuarioId);

        if (usuario == null)
        {
            TempData["Error"] = "Usuario no encontrado.";
            return RedirectToAction("Index", "Carrito");
        }

        var carrito = TempData["Carrito"] as List<DetallePedido>;
        if (carrito == null || !carrito.Any())
        {
            TempData["Error"] = "El carrito está vacío.";
            return RedirectToAction("Index", "Carrito");
        }

        var pedido = new Pedido
        {
            Usuario = usuario,
            Detalles = carrito,
            PrecioTotal = carrito.Sum(d => d.Precio),
            Estado = "Confirmado",
            Fecha = DateTime.Now
        };

        _context.Pedidos.Add(pedido);
        _context.SaveChanges();

        TempData["Mensaje"] = "Pedido confirmado exitosamente.";
        return RedirectToAction("Historial");
    }
}
