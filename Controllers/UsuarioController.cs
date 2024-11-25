using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

public class UsuarioController : Controller
{
    private readonly UserManager<Usuario> _userManager;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager; // Agrega RoleManager para gestionar roles
    private readonly AmazonContext _context;

    public UsuarioController(UserManager<Usuario> userManager, SignInManager<Usuario> signInManager, RoleManager<IdentityRole> roleManager, AmazonContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager; // Asigna RoleManager
        _context = context;
    }

    [HttpGet]
    public IActionResult Registro() => View();

    [HttpPost]
public async Task<IActionResult> Registro(RegistroViewModel model)
{
    if (ModelState.IsValid)
    {
        var user = new Usuario
        {
            UserName = model.Email,
            Email = model.Email,
            Nombre = model.Nombre,
            Apellido = model.Apellido
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            // Se asigna el rol, pero no se inicia sesión automáticamente
            await _userManager.AddToRoleAsync(user, "Usuario");

            // Redirigir al inicio de sesión en lugar de al home
            return RedirectToAction("Login", "Usuario");
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
    }
    return View(model);
}


    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Busca al usuario por correo electrónico
            var user = await _userManager.FindByEmailAsync(model.Email);

            // Verifica si el usuario existe y si está bloqueado
            if (user != null && user.LockoutEnabled)
            {
                ModelState.AddModelError("", "Tu cuenta está inactiva. Contacta al soporte para más información.");
                return View(model);
            }

            // Intenta iniciar sesión si el usuario no está bloqueado
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Inicio de sesión no válido");
        }
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Perfil()
    {
        var userId = _userManager.GetUserId(User);
        var usuario = await _userManager.Users
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.Email,
                u.Nombre,
                u.Apellido,
                u.PhoneNumber
            })
            .FirstOrDefaultAsync();

        return View(usuario);
    }


    [HttpGet]
    public IActionResult CambiarContrasena()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CambiarContrasena(CambiarContrasenaViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _userManager.ChangePasswordAsync(user, model.ContrasenaActual, model.NuevaContrasena);

            if (result.Succeeded)
            {
                TempData["Mensaje"] = "Contraseña cambiada exitosamente.";
                return RedirectToAction("Perfil");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Historial()
    {
        var userId = _userManager.GetUserId(User); // Obtén el ID del usuario autenticado
        var pedidos = await _context.Pedidos
            .Where(r => r.Usuario.Id == userId)
            .Include(r => r.Detalles)
            .ToListAsync();

        return View(pedidos);
    }

    // Métodos exclusivos para Admin

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> ListarUsuarios()
    {
        var usuarios = await _userManager.Users.ToListAsync();
        var usuariosConRoles = new List<UsuarioConRolesViewModel>();

        foreach (var usuario in usuarios)
        {
            var roles = await _userManager.GetRolesAsync(usuario);
            usuariosConRoles.Add(new UsuarioConRolesViewModel
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Roles = roles
            });
        }

        return View(usuariosConRoles);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> EditarUsuario(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        var model = new EditarUsuarioViewModel
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            Email = usuario.Email,
            PhoneNumber = usuario.PhoneNumber,
            Roles = await _userManager.GetRolesAsync(usuario)
        };

        ViewBag.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(); // Obtén todos los roles disponibles

        return View(model);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> EditarUsuario(EditarUsuarioViewModel model)
    {
        var usuario = await _userManager.FindByIdAsync(model.Id);
        if (usuario == null) return NotFound();

        usuario.Nombre = model.Nombre;
        usuario.Apellido = model.Apellido;
        usuario.Email = model.Email;
        usuario.PhoneNumber = model.PhoneNumber;

        var result = await _userManager.UpdateAsync(usuario);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);
            return View(model);
        }

        // Actualiza roles
        var userRoles = await _userManager.GetRolesAsync(usuario);
        await _userManager.RemoveFromRolesAsync(usuario, userRoles); // Elimina roles actuales
        await _userManager.AddToRolesAsync(usuario, model.SelectedRoles); // Asigna los nuevos roles seleccionados

        return RedirectToAction("ListarUsuarios");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> InactivarUsuario(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        usuario.LockoutEnabled = true;

        await _userManager.UpdateAsync(usuario);
        return RedirectToAction("ListarUsuarios");
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ActivarUsuario(string id)
    {
        var usuario = await _userManager.FindByIdAsync(id);
        if (usuario == null) return NotFound();

        usuario.LockoutEnabled = false;

        await _userManager.UpdateAsync(usuario);
        return RedirectToAction("ListarUsuarios");
    }
}
