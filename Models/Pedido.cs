public class Pedido
{
    public int Id { get; set; }
    public string UsuarioId { get; set; }
    public virtual Usuario Usuario { get; set; }
    public virtual ICollection<DetallePedido> Detalles { get; set; }
    public decimal PrecioTotal { get; set; }
    public string Estado { get; set; }
    public DateTime Fecha { get; set; }
}
