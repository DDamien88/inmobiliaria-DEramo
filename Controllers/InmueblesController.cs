using inmobiliariaDEramo.Models;
using InmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace inmobiliariaDEramo.Controllers
{
    [Authorize]
    public class InmueblesController : Controller
    {
        private readonly IRepositorioInmueble repositorio;
        private readonly IRepositorioPropietario repoPropietario;

        private readonly IRepositorioContrato repoContratos;

        public InmueblesController(IRepositorioInmueble repositorio, IRepositorioPropietario repoPropietrio, IRepositorioContrato repositorioContratos)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietrio;
            this.repoContratos = repositorioContratos;
        }

        // GET: Inmuebles
        [HttpGet]
        public IActionResult Index(int? estado, int pagina = 1, int cantidad = 10, int? propietarioId = null, DateTime? fechaDesde = null, DateTime? fechaHasta = null)
        {
            var inmuebles = repositorio.ObtenerTodos();

            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                var contratos = repoContratos.ObtenerTodos();

                if (fechaHasta.Value < fechaDesde.Value)
                {
                    TempData["Error"] = "La fecha de finalización no puede ser menor a la fecha desde.";
                }

                var ocupados = contratos
                    .Where(c =>
                        c.FechaDesde <= fechaHasta && c.FechaHasta >= fechaDesde && estado == 0)
                    .Select(c => c.IdInmueble)
                    .Distinct();

                inmuebles = inmuebles
                    .Where(i => !ocupados.Contains(i.Id))
                    .ToList();

                ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
                ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");
            }



            // Filtrar por estado si se indicó
            if (estado.HasValue)
            {
                inmuebles = inmuebles.Where(i => i.Activo == (estado == 1)).ToList();
                ViewBag.EstadoSeleccionado = estado.Value.ToString();
            }
            else
            {
                ViewBag.EstadoSeleccionado = "";
            }

            if (propietarioId.HasValue)
            {
                var propietario = repoPropietario.ObtenerPorId(propietarioId.Value);
                if (propietario != null)
                {
                    inmuebles = inmuebles.Where(i => i.PropietarioId == propietarioId.Value).ToList();
                    ViewBag.PropietarioNombre = propietario.Nombre + " " + propietario.Apellido;
                }
                else
                {
                    ViewBag.PropietarioNombre = "";
                }
            }
            else
            {
                ViewBag.PropietarioNombre = "";
            }

            // Paginado
            int total = inmuebles.Count;
            int totalPaginas = (int)Math.Ceiling(total / (double)cantidad);

            var inmueblesPaginados = inmuebles
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Cantidad = cantidad;

            return View(inmueblesPaginados);
        }






        // GET: Inmueble/Imagenes/5
        public ActionResult Imagenes(int id, [FromServices] IRepositorioImagen repoImagen)
        {
            var entidad = repositorio.ObtenerPorId(id);
            entidad.Imagenes = repoImagen.BuscarPorInmueble(id);
            return View(entidad);
        }



        // POST: Inmueble/Portada
        [HttpPost]
        public ActionResult Portada(Imagen entidad, IFormFile Archivo, int InmuebleId, [FromServices] IWebHostEnvironment environment)
        {


            if (Archivo == null)
            {
                TempData["Error"] = "Archivo no recibido.";
                return RedirectToAction("Imagenes", new { id = InmuebleId });
            }

            var fileName = "portada_" + InmuebleId + Path.GetExtension(Archivo.FileName);
            var path = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                Archivo.CopyTo(stream);
            }

            var url = Path.Combine("/Uploads/Inmuebles", fileName);
            repositorio.ModificarPortada(InmuebleId, url);

            TempData["Mensaje"] = "Portada actualizada correctamente";
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPortada(int inmuebleId, [FromServices] IWebHostEnvironment environment)
        {
            try
            {
                var inmueble = repositorio.ObtenerPorId(inmuebleId);
                if (inmueble != null && !string.IsNullOrEmpty(inmueble.Portada))
                {
                    var rutaCompleta = Path.Combine(environment.WebRootPath, "Uploads", "Inmuebles", Path.GetFileName(inmueble.Portada));
                    if (System.IO.File.Exists(rutaCompleta))
                    {
                        System.IO.File.Delete(rutaCompleta);
                    }

                    repositorio.ModificarPortada(inmuebleId, null); // o string.Empty
                    TempData["Mensaje"] = "Portada eliminada correctamente.";
                }
                return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(Imagenes), new { id = inmuebleId });
            }
        }






        public ActionResult PorPropietario(int id)
        {
            var lista = repositorio.ObtenerTodos();//repositorio.ObtenerPorPropietario(id);
            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            ViewBag.Id = id;
            //ViewBag.Propietario = repoPropietario.
            return View("Index", lista);
        }

        // GET: Inmueble/Details/5
        public IActionResult Ver(int id)
        {
            Inmueble inmueble = null;
            if (id > 0)
            {
                inmueble = repositorio.ObtenerPorId(id);
            }
            if (inmueble == null)
            {
                inmueble = new Inmueble();
            }

            ViewData["Id"] = id; // Asegura que el ViewData tenga el Id correcto
            return View(inmueble);
        }



        // GET: Inmueble/Details/5
        public ActionResult Details(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            return View(entidad);
        }

        // GET: Inmueble/Create
        public ActionResult Create()
        {
            try
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                //ViewData["Propietarios"] = repoPropietario.ObtenerTodos();
                //ViewData[nameof(Propietario)] = repoPropietario.ObtenerTodos();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                throw;
            }
        }



        // POST: Inmueble/Create
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(Inmueble entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Id"] = entidad.Id;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    return View(entidad);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }



        //Versión GUARDAR
        [HttpPost]
        public IActionResult Guardar(Inmueble inmueble)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (inmueble.Id > 0)
                    {
                        repositorio.Modificacion(inmueble);
                    }
                    else
                    {
                        repositorio.Alta(inmueble);
                    }
                    TempData["Mensaje"] = "Inmueble guardado correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                    return View("Ver", inmueble);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Propietarios = repoPropietario.ObtenerTodos();
                ViewBag.Error = ex.Message;
                return View("Ver", inmueble);
            }
        }




        //Buscar
        [HttpGet]
        [Route("Inmuebles/Buscar/{q}")]
        public IActionResult Buscar(string q)
        {
            try
            {
                var res = repositorio.BuscarPorDireccion(q);
                var datos = res.Select(i => new
                {
                    idInmueble = i.Id,
                    direccion = i.Direccion,
                    uso = i.Uso,
                    tipo = i.Tipo,
                    propietario = new
                    {
                        nombre = i.Duenio?.Nombre,
                        apellido = i.Duenio?.Apellido,
                        dni = i.Duenio?.Dni
                    }
                });
                return Json(new { datos });

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }







        // GET: Inmueble/Edit/5
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Propietarios = repoPropietario.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Edit(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(inmueble);
                return RedirectToAction("Index");
            }
            return View("Ver", inmueble); // Vuelve a la misma vista si hay errores
        }

        // GET: Inmueble/Eliminar/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmueble/Eliminar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Inmueble entidad)
        {
            try
            {
                repositorio.Baja(id);
                TempData["Mensaje"] = "Eliminación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.StackTrate = ex.StackTrace;
                return View(entidad);
            }
        }

        

        public ActionResult Activar(int id)
        {
            try
            {
                repositorio.Activar(id);
                TempData["Mensaje"] = "Activación realizada correctamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                //poner breakpoints para detectar errores
                ModelState.AddModelError("", "Ocurrio un error al activar el propietario.");
                throw;
            }
        }
    }
}