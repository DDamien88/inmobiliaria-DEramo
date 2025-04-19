using Microsoft.AspNetCore.Mvc;
using inmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Mvc;
using InmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaDEramo.Controllers
{
    [Authorize]
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repositorioPago;
        private readonly IRepositorioContrato repositorioContrato;
        private readonly IRepositorioUsuario repositorioUsuario;

        public PagosController(IRepositorioPago repositorioPago, IRepositorioContrato repositorioContrato, IRepositorioUsuario repositorioUsuario)
        {
            this.repositorioPago = repositorioPago;
            this.repositorioContrato = repositorioContrato;
            this.repositorioUsuario = repositorioUsuario;
        }

        // GET: Pagos
        public IActionResult Index(int contratoId)
        {
            var pagos = repositorioPago.ObtenerPorContrato(contratoId);
            ViewBag.ContratoId = contratoId;
            return View(pagos);
        }



        // GET: Pagos/Create
        public IActionResult Create(int contratoId)
        {
            Console.WriteLine(">> GET Create pago recibido");
            Console.WriteLine($"Usuario logueado: {User.Identity?.Name}");
            foreach (var c in User.Claims)
            {
                Console.WriteLine($"Claim: {c.Type} => {c.Value}");
            }
            var contrato = repositorioContrato.ObtenerPorId(contratoId);
            var pagosExistentes = repositorioPago.ObtenerPorContrato(contratoId).Count(p => p.Anulado);
            int nuevoNumeroPago = pagosExistentes + 1;

            var pago = new Pago
            {
                ContratoId = contratoId,
                NumeroPago = nuevoNumeroPago,
                FechaPago = DateTime.Now,
                Importe = (decimal)contrato.MontoMensual
            };

            return View(pago);
        }




        // POST: Pagos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {

            Console.WriteLine(">> POST Create Pago recibido");

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

                pago.UsuarioAltaId = usuario.Id;
                Console.WriteLine("Usuario actual: " + usuario.Id);

                // Evita que intente validar la propiedad de navegación "Contrato"
                ModelState.Remove(nameof(Pago.Contrato));

                if (ModelState.IsValid)
                {
                    repositorioPago.Alta(pago);
                    return RedirectToAction("Index", new { contratoId = pago.ContratoId });
                }

                return View(pago);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pago);
            }
        }







        // GET: Pagos/Editar/5
        public IActionResult Editar(int id)
        {
            var pago = repositorioPago.ObtenerPorId(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }


        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult Editar(Pago pago)
        {
            ModelState.Remove(nameof(Pago.Contrato)); // 👈 elimina validación automática
            try
            {
                if (ModelState.IsValid)
                {
                    repositorioPago.EditarDetalle(pago);
                    TempData["Mensaje"] = "Detalle actualizado correctamente";
                    return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
                }

                return View(pago);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(pago);
            }
        }






        //  Pagos/Anular/5

        public IActionResult Anular(int id)
        {
            try
            {
                // Obtener usuario logueado
                var email = User?.Identity?.Name;
                if (string.IsNullOrEmpty(email))
                {
                    TempData["Error"] = "No se pudo identificar el usuario actual.";
                    return RedirectToAction("Index");
                }

                var usuario = repositorioUsuario.ObtenerPorEmail(email);
                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado.";
                    return RedirectToAction("Index");
                }

                var pago = repositorioPago.ObtenerPorId(id);
                if (pago == null)
                {
                    TempData["Error"] = "Pago no encontrado.";
                    return RedirectToAction("Index");
                }

                // Anular y registrar usuario que anuló
                pago.UsuarioBajaId = usuario.Id;
                repositorioPago.Anular(pago);

                TempData["Mensaje"] = "Pago anulado correctamente.";
                return RedirectToAction(nameof(Index), new { contratoId = pago.ContratoId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al anular el pago: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // GET: Pago/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var entidad = repositorioPago.ObtenerPorId(id);
            return View(entidad);

        }


    }
}
