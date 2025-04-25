using inmobiliariaDEramo.Models;
using InmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliariaDEramo.Controllers
{
    [Authorize]
    public class ContratosController : Controller
    {
        private readonly IRepositorioContrato repositorio;
        private readonly IRepositorioPropietario repoPropietario;

        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioUsuario repositorioUsuario;

        public ContratosController(IRepositorioContrato repositorio, IRepositorioPropietario repoPropietrio, IRepositorioInquilino repositorioInquilino, IRepositorioInmueble repositorioInmueble, IRepositorioPago repositorioPago, IRepositorioUsuario repositorioUsuario)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietrio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
            this.repositorioPago = repositorioPago;
            this.repositorioUsuario = repositorioUsuario;
        }

        // GET: Contrato
        public ActionResult Index(DateTime? desde = null, DateTime? hasta = null, int pagina = 1, int cantidad = 5, int? idInmueble = null, int? plazoDias = null)
        {
            var lista = repositorio.ObtenerTodos();

            if (desde.HasValue && hasta.HasValue)
            {
                lista = lista
                    .Where(c => c.FechaDesde >= desde.Value && c.FechaHasta <= hasta.Value)
                    .ToList();

                if (hasta.Value < desde.Value)
                {
                    ViewBag.Error = TempData["Error"] = "La fecha hasta no puede ser menor a la fecha desde.";

                }


                ViewBag.FiltroDesde = desde.Value.ToString("yyyy-MM-dd");
                ViewBag.FiltroHasta = hasta.Value.ToString("yyyy-MM-dd");
            }
            else
            {
                ViewBag.FiltroDesde = "";
                ViewBag.FiltroHasta = "";
            }

            if (idInmueble.HasValue)
            {
                lista = lista.Where(c => c.IdInmueble == idInmueble.Value).ToList();
                ViewBag.InmuebleSeleccionado = idInmueble;
                var inmueble = repositorioInmueble.ObtenerPorId(idInmueble.Value);
                ViewBag.InmuebleNombre = inmueble?.Direccion;
            }
            else
            {
                ViewBag.InmuebleSeleccionado = null;
                ViewBag.InmuebleNombre = "";
            }

            if (plazoDias.HasValue)
            {
                var fechaObjetivo = DateTime.Today.AddDays(plazoDias.Value).Date;

                lista = lista.Where(c =>
                    c.FechaHasta.HasValue &&
                    c.FechaHasta.Value.Date == fechaObjetivo
                ).ToList();

                ViewBag.PlazoSeleccionado = plazoDias.Value.ToString();
            }
            else
            {
                ViewBag.PlazoSeleccionado = "";
            }



            if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];

            foreach (var contrato in lista)
            {
                var renovacion = repositorio.BuscarRenovacion(contrato);
                contrato.PuedeRenovarse = renovacion == null || !renovacion.Activo;
            }

            // Paginado
            int total = lista.Count;
            int totalPaginas = (int)Math.Ceiling(total / (double)cantidad);

            var inmueblesPaginados = lista
                .Skip((pagina - 1) * cantidad)
                .Take(cantidad)
                .ToList();

            ViewBag.PaginaActual = pagina;
            ViewBag.TotalPaginas = totalPaginas;
            ViewBag.Cantidad = cantidad;

            return View(inmueblesPaginados);
        }



        //     public ActionResult PorPropietario(int id)
        //     {
        //         var lista = repositorio.ObtenerTodos();//repositorio.ObtenerPorPropietario(id);
        //         if (TempData.ContainsKey("Id"))
        //             ViewBag.Id = TempData["Id"];
        //         if (TempData.ContainsKey("Mensaje"))
        //             ViewBag.Mensaje = TempData["Mensaje"];
        //         ViewBag.Id = id;
        //         //ViewBag.Propietario = repoPropietario.
        //         return View("Index", lista);
        //     }



        // GET: Contrato/Details/5
        public IActionResult Ver(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            return View(entidad);
        }



        // GET: Contrato/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            return View(entidad);

        }



        // GET: Contratos/Create

        public ActionResult Create()
        {
            Console.WriteLine(">> GET Create CONTRATO recibido");
            Console.WriteLine($"Usuario logueado: {User.Identity?.Name}");
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"Claim: {c.Type} => {c.Value}");
            }
            try
            {
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();

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



        // POST: Contratos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato entidad)
        {
            Console.WriteLine(">> POST Create CONTRATO recibido");

            try
            {
                var email = User?.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "No se pudo identificar el usuario actual.";
                    Console.WriteLine("ERROR: Usuario actual no identificado");
                    return RedirectToAction("Index");
                }

                var usuario = repositorioUsuario.ObtenerPorEmail(email);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    Console.WriteLine("ERROR: Usuario no encontrado con el email: " + email);
                    return RedirectToAction("Index");
                }

                entidad.UsuarioAltaId = usuario.Id;
                Console.WriteLine("Usuario actual: " + usuario.Id);

                if (entidad.FechaDesde < DateTime.Today || entidad.FechaHasta < entidad.FechaDesde)
                {
                    ModelState.AddModelError(nameof(Contrato.FechaDesde), "Las fechas deben ser válidas.");
                }

                var contratosExistentes = repositorio.ObtenerTodos();
                bool haySuperposicion = contratosExistentes.Any(c =>
                    c.IdInmueble == entidad.IdInmueble &&
                    c.FechaHasta >= entidad.FechaDesde &&
                    c.FechaDesde <= entidad.FechaHasta &&
                    c.Activo
                );

                if (haySuperposicion)
                {
                    ModelState.AddModelError(nameof(Contrato.IdInmueble), "El inmueble ya está ocupado en esas fechas.");
                }

                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    TempData["Mensaje"] = "Contrato creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                // Si hay errores, recargar combos y volver a mostrar la vista
                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                return View(entidad);
            }
            catch (Exception ex)
            {
                Console.WriteLine(">> ERROR al crear contrato: " + ex.Message);
                Console.WriteLine(">> STACK: " + ex.StackTrace);
                TempData["Error"] = "Hubo un error: " + ex.Message;

                ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
                return View(entidad);
            }
        }





        //     //Versión GUARDAR
        //     [HttpPost]
        //     public IActionResult Guardar(Inmueble inmueble)
        //     {
        //         try
        //         {
        //             if (ModelState.IsValid)
        //             {
        //                 if (inmueble.Id > 0) // Si el Id es mayor a 0, actualiza, si no, crea uno nuevo
        //                 {
        //                     repositorio.Modificacion(inmueble);
        //                 }
        //                 else
        //                 {
        //                     repositorio.Alta(inmueble);
        //                 }
        //                 return RedirectToAction("Index");
        //             }
        //             else
        //             {
        //                 ViewBag.Propietarios = repoPropietario.ObtenerTodos();
        //                 return View("Index", inmueble);
        //             }
        //         }
        //         catch (Exception ex)
        //         {
        //             ViewBag.Error = ex.Message;
        //             return View("Ver", inmueble);
        //         }
        //     }







        // GET: Inmueble/Edit/5
        public ActionResult Edit(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
            ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Contrato contrato)
        {
            if (ModelState.IsValid)
            {
                repositorio.Modificacion(contrato);
                return RedirectToAction("Index");
            }
            return View("Ver", contrato);
        }



        //     // GET: Inmueble/Eliminar/5
        [Authorize(Policy = "Administrador")]
        [HttpGet]
        public ActionResult Eliminar(int id)
        {
            var entidad = repositorio.ObtenerPorId(id);
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];
            if (TempData.ContainsKey("Error"))
                ViewBag.Error = TempData["Error"];
            return View(entidad);
        }

        //     // POST: Inmueble/Eliminar/5
        [HttpPost]
        [Authorize(Policy = "Administrador")]
        [ValidateAntiForgeryToken]
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




        [HttpGet]
        public IActionResult Finalizar(int id, [FromServices] IRepositorioPago repoPago)
        {

            Console.WriteLine($"Usuario logueado: {User.Identity?.Name}");
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"Claim: {c.Type} => {c.Value}");
            }

            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            var hoy = DateTime.Today;
            var pagos = repositorioPago.ObtenerPorContrato(id);

            // Cálculo de meses adeudados (entre hoy y FechaHasta)
            int mesesTotales = ((contrato.FechaHasta?.Year - contrato.FechaDesde?.Year) ?? 0) * 12 +
                                ((contrato.FechaHasta?.Month - contrato.FechaDesde?.Month) ?? 0);

            int mesesPagados = pagos.Count(p => p.Anulado == false);
            int mesesAdeudados = Math.Max(0, mesesTotales - mesesPagados);

            // Calcular si corresponde 1 o 2 meses de multa
            var mitad = contrato.FechaDesde?.AddMonths(mesesTotales / 2);
            decimal multa = 0;
            if (hoy < mitad)
                multa = (decimal)(contrato.MontoMensual * 2);
            else
                multa = (decimal)(contrato.MontoMensual);

            ViewBag.MesesAdeudados = mesesAdeudados;
            ViewBag.Multa = multa;
            ViewBag.MultaPagada = contrato.MultaPagada;

            return View(contrato);
        }


        [HttpPost]
        public IActionResult Finalizar(Contrato modelo, IFormCollection form, [FromServices] IRepositorioPago repoPago)
        {
            try
            {
                var contrato = repositorio.ObtenerPorId(modelo.Id);
                if (contrato == null)
                    return NotFound();

                //Controles de usuario
                var email = User?.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "No se pudo identificar al usuario actual.";
                    return RedirectToAction("Index");
                }

                var usuario = repositorioUsuario.ObtenerPorEmail(email);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("Index");
                }

                contrato.UsuarioBajaId = usuario.Id;




                contrato.FechaTerminacionAnticipada = modelo.FechaTerminacionAnticipada;

                if (contrato.FechaTerminacionAnticipada > contrato.FechaHasta)
                {
                    TempData["Mensaje"] = "La fecha de finalización anticipada no puede ser posterior a la fecha de finalización del contrato.";
                    return RedirectToAction("Finalizar", new { id = contrato.Id });
                }

                contrato.MontoMulta = modelo.MontoMulta;

                // Obtener el usuario actual para auditoría
                //var usuario = repositorioUsuario.ObtenerPorEmail(User.Identity.Name);
                contrato.UsuarioBajaId = usuario?.Id;

                bool cargarPago = form["CargarPago"] == "true";
                contrato.MultaPagada = cargarPago;

                repositorio.ModificarFinalizacionAnticipada(contrato);


                if (cargarPago && contrato.MontoMulta.HasValue)
                {
                    var pago = new Pago
                    {
                        ContratoId = contrato.Id,
                        NumeroPago = repoPago.ObtenerUltimoNumeroPago(contrato.Id) + 1,
                        FechaPago = DateTime.Today,
                        Importe = contrato.MontoMulta.Value,
                        Detalle = "Multa por finalización anticipada",
                        Anulado = true
                    };
                    repoPago.Alta(pago);
                    contrato.UsuarioBajaId = repositorioUsuario.ObtenerPorEmail(User.Identity.Name).Id;
                    pago.UsuarioBajaId = repositorioUsuario.ObtenerPorEmail(User.Identity.Name).Id;
                    repoPago.Anular(pago);

                }

                TempData["Mensaje"] = "Contrato finalizado anticipadamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al finalizar contrato: " + ex.Message;
                return RedirectToAction("Index");
            }
        }








        public IActionResult Estado(int id, [FromServices] IRepositorioPago repoPago)
        {
            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
                return NotFound();

            // Calcular deuda y total de pagos
            var pagos = repoPago.ObtenerPorContrato(id);
            var mesesPagados = repoPago.ObtenerUltimoNumeroPago(id);
            var mesesTotales = (int)Math.Ceiling(
                ((contrato.FechaTerminacionAnticipada ?? contrato.FechaHasta) - contrato.FechaDesde)?.TotalDays / 30.0 ?? 0
            );
            ViewBag.MesesAdeudados = Math.Max(mesesTotales - mesesPagados, 0);
            ViewBag.TotalPagos = mesesPagados;

            return View("Estado", contrato);
        }


        [HttpGet]
        public IActionResult Renovar(int id)
        {

            var contrato = repositorio.ObtenerPorId(id);
            if (contrato == null)
            {
                TempData["Error"] = "Contrato no encontrado.";
                return RedirectToAction("Index");
            }

            // Validar si el contrato ya caducó
            if (contrato.FechaHasta.HasValue && contrato.FechaHasta.Value > DateTime.Today)
            {
                TempData["Error"] = "Este contrato aún no ha finalizado. Solo se puede renovar después de la fecha de finalización.";
                return RedirectToAction("Index");
            }



            var renovado = new Contrato
            {
                IdInquilino = contrato.IdInquilino,
                IdInmueble = contrato.IdInmueble,
                Inquilino = contrato.Inquilino,
                Inmueble = contrato.Inmueble,
                FechaDesde = DateTime.Today,
                FechaHasta = DateTime.Today.AddYears(1),
                MontoMensual = contrato.MontoMensual,
            };

            return View(renovado);
        }



        [HttpPost]
        public IActionResult Renovar(Contrato nuevo, int id)
        {
            Console.WriteLine($"Buscando contrato con ID: {id}");
            var original = repositorio.ObtenerPorId(id);
            if (original == null)
            {
                Console.WriteLine("Contrato original no encontrado.");
            }


            if (nuevo.FechaDesde <= original.FechaHasta)
            {
                TempData["Error"] = $"La nueva fecha de inicio debe ser posterior a la fecha de finalización del contrato anterior ({original.FechaHasta?.ToShortDateString()}).";
                return View(original);
            }

            if (nuevo.FechaHasta <= nuevo.FechaDesde)
            {
                TempData["Error"] = "La nueva fecha de finalización debe ser posterior a la fecha de inicio.";
                return View(original);
            }

            if (ModelState.IsValid)
            {
                repositorio.Alta(nuevo);
                repositorio.Baja(id);
                TempData["Mensaje"] = "Contrato renovado correctamente.";
                return RedirectToAction("Index");
            }

            return View(original);
        }



    }


}