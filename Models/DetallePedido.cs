public class DetallePedido
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public virtual Pedido Pedido { get; set; }
    public int ProductoId { get; set; }
    public virtual Producto Producto { get; set; }
    public int Cantidad { get; set; }
    public decimal Precio { get; set; }
}
