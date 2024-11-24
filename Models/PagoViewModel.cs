using System.ComponentModel.DataAnnotations;

public class PagoViewModel
{
    [Required]
    [CreditCard(ErrorMessage = "El número de tarjeta no es válido.")]
    public string CardNumber { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "El nombre no puede exceder de 100 caracteres.")]
    public string CardHolder { get; set; }

    [Required]
    [RegularExpression(@"^\d{2}/\d{2}$", ErrorMessage = "Formato inválido. Use MM/YY.")]
    public string ExpiryDate { get; set; }

    [Required]
    [RegularExpression(@"^\d{3}$", ErrorMessage = "El CVV debe ser un número de 3 dígitos.")]
    public string CVV { get; set; }
}
