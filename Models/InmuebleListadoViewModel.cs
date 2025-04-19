using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using InmobiliariaDEramo.Models;

namespace inmobiliariaDEramo.Models;

public class InmuebleListadoViewModel
{
    public IEnumerable<Inmueble> Inmuebles { get; set; }
    public int PaginaActual { get; set; }
    public int TotalPaginas { get; set; }
    public string Busqueda { get; set; }
    public int? Estado { get; set; }
}
