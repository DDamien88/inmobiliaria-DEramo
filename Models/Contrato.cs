using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using InmobiliariaDEramo.Models;

namespace inmobiliariaDEramo.Models
{
    [Table("Contratos")]
    public class Contrato
    {
        //[Display(Name = "Nº")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Inquilino")]
        public int IdInquilino { get; set; }
        [Required]
        [ForeignKey(nameof(IdInquilino))]


        [Display(Name = "Inmueble")]
        public int IdInmueble { get; set; }
        [Required]
        [ForeignKey(nameof(IdInmueble))]

        public double MontoMensual { get; set; }
        [Required]
        public DateTime? FechaDesde { get; set; }
        [Required]
        public DateTime? FechaHasta { get; set; }
        public Inquilino? Inquilino { get; set; }
        public Inmueble? Inmueble { get; set; }

        public Boolean Activo { get; set; }
        public DateTime? FechaTerminacionAnticipada { get; set; }
        public decimal? MontoMulta { get; set; }
        public bool MultaPagada { get; set; } = false;

        [NotMapped]
        public bool PuedeRenovarse { get; set; }

        public int? UsuarioAltaId { get; set; }
        public int? UsuarioBajaId { get; set; }

        [ForeignKey(nameof(UsuarioAltaId))]
        public Usuario? UsuarioAlta { get; set; }

        [ForeignKey(nameof(UsuarioBajaId))]
        public Usuario? UsuarioBaja { get; set; }



    }
}