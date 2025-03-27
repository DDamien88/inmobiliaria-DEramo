using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaDEramo.Models
{
    public class Inquilino
    {
        [Key]
        [Display(Name = "Código Int.")]
        public int IdInquilino { get; set; }
        [Required]
        public string Nombre { get; set; }
        [Required]
        public string Apellido { get; set; }

        public string Dni { get; set; }

        public string Telefono { get; set; }
        public string Email { get; set; }


        public override string ToString()
        {
            //return $"{Apellido}, {Nombre}";
            //return $"{Nombre} {Apellido}";
            var res = $"{Nombre} {Apellido}";
            if (!String.IsNullOrEmpty(Dni))
            {
                res += $" ({Dni})";
            }
            return res;
        }
    }
}