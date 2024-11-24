using System.ComponentModel.DataAnnotations;

public class CambiarContrasenaViewModel
{
    [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
    [DataType(DataType.Password)]
    public string ContrasenaActual { get; set; }

    [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres.")]
    public string NuevaContrasena { get; set; }

    [DataType(DataType.Password)]
    [Compare("NuevaContrasena", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmarContrasena { get; set; }
}
