using Microsoft.AspNetCore.Mvc;

public class ProductosController : Controller
{
    private readonly AmazonContext _context;

    public ProductosController(AmazonContext context)
    {
        _context = context;
    }

    public ActionResult Index(string categoria, string nombre)
    {
        var categorias = _context.Productos
            .Select(p => p.Categoria)
            .Distinct()
            .ToList();

        var productos = _context.Productos.AsQueryable();

        if (!string.IsNullOrEmpty(categoria))
        {
            productos = productos.Where(p => p.Categoria == categoria);
        }

        if (!string.IsNullOrEmpty(nombre))
        {
            nombre = nombre.ToLower(); // Convertimos el texto ingresado a minúsculas
            productos = productos.Where(p => p.Nombre.ToLower().Contains(nombre));
        }

        var model = new ProductosViewModel
        {
            Categorias = categorias,
            Productos = productos.ToList()
        };

        return View(model);
    }

    public ActionResult Detalles(int id)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.Id == id);

        if (producto == null)
        {
            return NotFound(); // Manejar el caso en el que el producto no exista
        }

        return View(producto);
    }

    public ActionResult ListarProductos()
    {
        var productos = _context.Productos.ToList();
        return View(productos); // Devuelve la vista ListarProductos.cshtml
    }

    public IActionResult Crear()
    {
        var producto = new Producto(); 
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Crear(Producto producto)
    {
        if (ModelState.IsValid)
        {
            _context.Productos.Add(producto);
            _context.SaveChanges();
            return RedirectToAction("ListarProductos");
        }
        return View(producto);
    }

    public IActionResult Editar(int id)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
        {
            return NotFound();
        }
        return View(producto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Editar(int id, Producto producto)
    {
        if (id != producto.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            _context.Update(producto);
            _context.SaveChanges();
            return RedirectToAction("ListarProductos");
        }
        return View(producto);
    }

    public IActionResult Eliminar(int id)
    {
        var producto = _context.Productos.FirstOrDefault(p => p.Id == id);
        if (producto == null)
        {
            return NotFound();
        }

        _context.Productos.Remove(producto);
        _context.SaveChanges();
        return RedirectToAction("ListarProductos");
    }


}
