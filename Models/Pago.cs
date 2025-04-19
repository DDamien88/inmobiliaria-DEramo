using inmobiliariaDEramo.Models;
using InmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace inmobiliariaDEramo.Models
{
    [Table("Pagos")]

    public class Pago
    {
        public int Id { get; set; }

        [Required]
        public int ContratoId { get; set; }

        public Contrato Contrato { get; set; }

        [Required]
        public int NumeroPago { get; set; }

        [Required]
        public DateTime FechaPago { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public decimal Importe { get; set; }

        [Required]
        public string Detalle { get; set; }

        public bool Anulado { get; set; } = true;

        public int? UsuarioAltaId { get; set; }
        public int? UsuarioBajaId { get; set; }

        [ForeignKey(nameof(UsuarioAltaId))]
        public Usuario? UsuarioAlta { get; set; }

        [ForeignKey(nameof(UsuarioBajaId))]
        public Usuario? UsuarioBaja { get; set; }

    }
}