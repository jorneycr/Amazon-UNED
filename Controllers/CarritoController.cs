using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class CarritoController : Controller
{
    private readonly AmazonContext _context;

    public CarritoController(AmazonContext context)
    {
        _context = context;
    }

    private List<DetallePedido> ObtenerCarrito()
    {
        var carrito = HttpContext.Session.GetObject<List<DetallePedido>>("Carrito");
        return carrito ?? new List<DetallePedido>();
    }

    private void GuardarCarrito(List<DetallePedido> carrito)
    {
        HttpContext.Session.SetObject("Carrito", carrito);
    }

    public IActionResult Index()
    {
        var carrito = ObtenerCarrito();
        return View(carrito);
    }

    public IActionResult Agregar(int productoId, int cantidad)
    {
        var producto = _context.Productos.Find(productoId);
        if (producto == null || producto.Stock < cantidad)
        {
            TempData["Error"] = "Producto no disponible o cantidad excede el stock.";
            return RedirectToAction("Index", "Productos");
        }

        var carrito = ObtenerCarrito();
        var detalle = carrito.FirstOrDefault(d => d.ProductoId == productoId);
        if (detalle != null)
        {
            detalle.Cantidad += cantidad;
        }
        else
        {
            carrito.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = cantidad,
                Precio = producto.Precio
            });
        }

        GuardarCarrito(carrito);
        TempData["Mensaje"] = "Producto agregado al carrito.";
        return RedirectToAction("Index", "Productos");
    }

    public IActionResult Eliminar(int productoId)
    {
        var carrito = ObtenerCarrito();
        var detalle = carrito.FirstOrDefault(d => d.ProductoId == productoId);
        if (detalle != null)
        {
            carrito.Remove(detalle);
        }

        GuardarCarrito(carrito);
        return RedirectToAction("Index");
    }


    public IActionResult Actualizar(int productoId, int nuevaCantidad)
    {
        var carrito = ObtenerCarrito();
        var detalle = carrito.FirstOrDefault(d => d.ProductoId == productoId);
        if (detalle != null)
        {
            detalle.Cantidad = nuevaCantidad;
        }

        GuardarCarrito(carrito);
        return RedirectToAction("Index");
    }
}
