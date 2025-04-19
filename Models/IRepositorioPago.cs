namespace inmobiliariaDEramo.Models
{
    public interface IRepositorioPago
    {
        int Alta(Pago pago);
        IEnumerable<Pago> ObtenerPorContrato(int contratoId);
        Pago ObtenerPorId(int id);
        int Anular(Pago pago);
        void EditarDetalle(Pago pago);
        int ObtenerUltimoNumeroPago(int id);
        int CalcularMesesAdeudados(int id);
    }
}
