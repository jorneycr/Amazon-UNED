using Microsoft.AspNetCore.Mvc;
using System.Linq;

public class CarritoController : Controller
{
    private readonly AmazonContext _context;
    private readonly List<DetallePedido> _carrito;

    public CarritoController(AmazonContext context)
    {
        _context = context;
        _carrito = new List<DetallePedido>();
    }

    public IActionResult Index()
    {
        return View(_carrito);
    }

    public IActionResult Agregar(int productoId, int cantidad)
    {
        var producto = _context.Productos.Find(productoId);
        if (producto == null || producto.Stock < cantidad)
        {
            TempData["Error"] = "Producto no disponible o cantidad excede el stock.";
            return RedirectToAction("Index", "Productos");
        }

        var detalle = _carrito.FirstOrDefault(d => d.ProductoId == productoId);
        if (detalle != null)
        {
            detalle.Cantidad += cantidad;
            detalle.Precio = detalle.Cantidad * producto.Precio;
        }
        else
        {
            _carrito.Add(new DetallePedido
            {
                ProductoId = producto.Id,
                Producto = producto,
                Cantidad = cantidad,
                Precio = cantidad * producto.Precio
            });
        }

        TempData["Mensaje"] = "Producto agregado al carrito.";
        return RedirectToAction("Index", "Productos");
    }

    public IActionResult Eliminar(int productoId)
    {
        var detalle = _carrito.FirstOrDefault(d => d.ProductoId == productoId);
        if (detalle != null)
        {
            _carrito.Remove(detalle);
        }

        return RedirectToAction("Index");
    }

    public IActionResult Actualizar(int productoId, int nuevaCantidad)
    {
        var detalle = _carrito.FirstOrDefault(d => d.ProductoId == productoId);
        if (detalle != null)
        {
            detalle.Cantidad = nuevaCantidad;
            detalle.Precio = nuevaCantidad * detalle.Producto.Precio;
        }

        return RedirectToAction("Index");
    }
}
