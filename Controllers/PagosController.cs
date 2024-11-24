using Microsoft.AspNetCore.Mvc;

public class PagosController : Controller
{
    // Muestra el formulario de pago
    public IActionResult Pasarela()
    {
        return View();
    }

    // Procesa los datos del pago
    [HttpPost]
    public IActionResult ProcesarPago(PagoModelo modelo)
    {
        if (modelo.NumeroTarjeta.StartsWith("4"))
        {
            return RedirectToAction("Exito");
        }
        else
        {
            return RedirectToAction("Error");
        }
    }

    // Página de confirmación de pago exitoso
    public IActionResult Exito()
    {
        return View();
    }

    // Página de error de pago
    public IActionResult Error()
    {
        return View();
    }
}
