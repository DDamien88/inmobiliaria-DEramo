using inmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace inmobiliariaDEramo.Controllers
{
    // [Authorize]
    public class ContratosController : Controller
    {
        private readonly IRepositorioContrato repositorio;
        private readonly IRepositorioPropietario repoPropietario;

        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;


        public ContratosController(IRepositorioContrato repositorio, IRepositorioPropietario repoPropietrio, IRepositorioInquilino repositorioInquilino, IRepositorioInmueble repositorioInmueble)
        {
            this.repositorio = repositorio;
            this.repoPropietario = repoPropietrio;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
        }

        // GET: Inmueble
        public ActionResult Index()
        {
            var lista = repositorio.ObtenerTodos();
            /*if (TempData.ContainsKey("Id"))
                ViewBag.Id = TempData["Id"];
            if (TempData.ContainsKey("Mensaje"))
                ViewBag.Mensaje = TempData["Mensaje"];*/
            return View(lista);
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

        //     // GET: Inmueble/Details/5
        //     public IActionResult Ver(int id)
        //     {
        //         Inmueble inmueble = null;
        //         if (id > 0)
        //         {
        //             inmueble = repositorio.ObtenerPorId(id);
        //         }
        //         if (inmueble == null)
        //         {
        //             inmueble = new Inmueble();
        //         }

        //         ViewData["Id"] = id; // Asegura que el ViewData tenga el Id correcto
        //         return View(inmueble);
        //     }



        //     // GET: Inmueble/Details/5
        //     public ActionResult Details(int id)
        //     {
        //         var entidad = repositorio.ObtenerPorId(id);
        //         return View(entidad);
        //     }

        // GET: Inmueble/Create
        public ActionResult Create()
        {
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



        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Contrato entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    repositorio.Alta(entidad);
                    Console.WriteLine($"Inquilino: {entidad.IdInquilino}, Inmueble: {entidad.IdInmueble}");

                    //TempData["Id"] = entidad.Id;
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                    foreach (var error in errores)
                    {
                        Console.WriteLine("❌ Error de validación: " + error);
                    }

                    ViewBag.Inquilinos = repositorioInquilino.ObtenerTodos();
                    ViewBag.Inmuebles = repositorioInmueble.ObtenerTodos();
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
        //[ValidateAntiForgeryToken]
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
    }
}