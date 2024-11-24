public class UsuarioConRolesViewModel
{
    public string Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
}