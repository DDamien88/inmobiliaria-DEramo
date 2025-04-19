using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace inmobiliariaDEramo.Models
{
	public interface IRepositorioContrato : IRepositorio<Contrato>
	{
		IList<Contrato> Activar(int id);
		IList<Contrato> BuscarPorInquilino(int IdInquilino);
        Contrato BuscarRenovacion(Contrato contrato);
        bool EstaOcupado(int inmuebleId, DateTime desde, DateTime hasta);
        int ModificarFinalizacionAnticipada(Contrato contrato);
        IList<Contrato> ObtenerContratosVigentesPorInmueble(int idInmueble);
    }
}