namespace inmobiliariaDEramo.Models
{
    public interface IRepositorioPago
    {
        IEnumerable<Pago> ObtenerPorContrato( int contratoId);       

    }
}