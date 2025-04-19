using System;
using System.Collections.Generic;
using System.Linq;
using inmobiliariaDEramo.Models;

namespace InmobiliariaDEramo.Models
{
    public interface IRepositorioUsuario : IRepositorio<Usuario>
    {
        Usuario ObtenerPorEmail(string email);
        int ObtenerPorIdDos(int id);
    }
}