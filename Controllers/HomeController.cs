using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using reservaDeViajes.Models;


public class HomeController : Controller
{
    private readonly AmazonContext _context;

    public HomeController(AmazonContext context)
    {
        _context = context;
    }

    public IActionResult Index()
{
    // Obtener productos de tecnología
    var productosTecnologia = _context.Productos
        .Where(p => p.Categoria == "Tecnología")
        .ToList();

    // Obtener todas las categorías
    var categorias = _context.Productos
        .Select(p => p.Categoria)
        .Distinct()
        .ToList();

    // Obtener productos destacados (por ejemplo, con precio mayor a 100)
    var promocionesDestacadas = _context.Productos
        .Where(p => p.Precio > 100)
        .ToList();

    // Crear un ViewModel para enviar múltiples datos
    var viewModel = new HomeViewModel
    {
        ProductosTecnologia = productosTecnologia,
        Categorias = categorias,
        PromocionesDestacadas = promocionesDestacadas
    };

    return View(viewModel);
}


    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
