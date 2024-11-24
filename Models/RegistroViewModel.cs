using System.ComponentModel.DataAnnotations;

public class RegistroViewModel
{
    [Required(ErrorMessage = "El campo Email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "El campo Confirmación de Contraseña es obligatorio.")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; }

    [Required(ErrorMessage = "El campo Nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El campo Apellido es obligatorio.")]
    public string Apellido { get; set; }
}
