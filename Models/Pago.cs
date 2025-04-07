using inmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaDEramo.Models
{
    [Table("Pagos")]

    public class Pago
    {
        public int Id { get; set; }
        public int NumeroPago { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Importe { get; set; }
        public string Detalle { get; set; }

        public int ContratoId { get; set; }
        public Contrato Contrato { get; set; }

        public bool Anulado { get; set; } = false;
    }
}
