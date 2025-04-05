using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaDEramo.Models
{
	public interface IRepositorioContrato : IRepositorio<Contrato>
	{
		IList<Contrato> BuscarPorInquilino(int IdInquilino);
	}
}