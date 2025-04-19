using inmobiliariaDEramo.Models;

namespace InmobiliariaDEramo.Models
{
    public interface IRepositorioImagen : IRepositorio<Imagen>
    {
        IList<Imagen> BuscarPorInmueble(int inmuebleId);
    }
}