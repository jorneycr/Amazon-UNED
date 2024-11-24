public class EditarUsuarioViewModel
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public IList<string> Roles { get; set; }
    public IEnumerable<string> SelectedRoles { get; set; }
}