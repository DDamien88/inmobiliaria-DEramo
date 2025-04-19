using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InmobiliariaDEramo.Models;

namespace inmobiliariaDEramo.Models
{
	[Table("Inmuebles")]
	public class Inmueble
	{
		//[Display(Name = "Nº")]
		public int Id { get; set; }
		//[Required]
		[Display(Name = "Dirección")]
		[Required]
		[RegularExpression(@"^[A-Za-z\s]{3,100}$", ErrorMessage = "El campo solo debe contener letras y espacios.")]
		public string? Direccion { get; set; }
		[Required]
		[RegularExpression(@"^[A-Za-z\s]{3,50}$", ErrorMessage = "El uso debe contener solo letras.")]
		public string? Uso { get; set; }
		[Required]
		[RegularExpression(@"^[A-Za-z\s]{3,50}$", ErrorMessage = "El tipo debe contener solo letras.")]
		public string? Tipo { get; set; }
		[Required]
		[Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
		public double? Precio { get; set; }
		[Required]
		[Range(1, 100, ErrorMessage = "Debe haber al menos 1 ambiente. (del 1 al 100)")]
		public int Ambientes { get; set; }
		[Required]
		[Range(1, 10000, ErrorMessage = "La superficie debe ser mayor a 0.")]
		public int Superficie { get; set; }
		public decimal Latitud { get; set; }
		public decimal Longitud { get; set; }
		[Display(Name = "Dueño")]
		[Required]
		public int PropietarioId { get; set; }
		[ForeignKey(nameof(PropietarioId))]
		public Propietario? Duenio { get; set; }
		public string? Portada { get; set; }
		[NotMapped]
		public IFormFile? PortadaFile { get; set; }
		[ForeignKey(nameof(Imagen.InmuebleId))]
		public IList<Imagen> Imagenes { get; set; } = new List<Imagen>();
		public Boolean Activo { get; set; }

		public override string ToString()
		{
			return $"Dirección: {Direccion} - Uso: {Uso} - Tipo: {Tipo} - Precio: {Precio} - Ambientes: {Ambientes} - Superficie mts2: {Superficie} - Dueño: {Duenio.Nombre} {Duenio.Apellido} {Duenio.Dni}";
		}

	}
}