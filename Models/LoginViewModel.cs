using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required(ErrorMessage = "El campo Email es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingrese una dirección de correo válida.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "El campo Contraseña es obligatorio.")]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}
