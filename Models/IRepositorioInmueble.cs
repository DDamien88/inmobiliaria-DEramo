using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaDEramo.Models
{
	public interface IRepositorioInmueble : IRepositorio<Inmueble>
	{
		IList<Inmueble> BuscarPorPropietario(int idPropietario);
		IList<Inmueble> BuscarPorDireccion(string direccion);
	}
}