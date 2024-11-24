using Microsoft.AspNetCore.Mvc;

public class ProductosController : Controller
{
    private readonly AmazonContext _context;

    public ProductosController(AmazonContext context)
    {
        _context = context;
    }

    public ActionResult Home()
    {
        // Filtrar productos por la categoría "Tecnología"
        // var productosTecnologia = _context.Productos
        //     .Where(p => p.Categoria == "Tecnología")
        //     .ToList() ?? new List<Producto>(); // Asegurarte de que no sea null

        return View();
    }

    public ActionResult Detalles(int id)
    {
        var producto = _context.Productos.Find(id);
        if (producto == null) return HttpNotFound();
        return View(producto);
    }

    private ActionResult HttpNotFound()
    {
        throw new NotImplementedException();
    }
}
