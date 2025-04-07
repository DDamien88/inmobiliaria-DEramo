using inmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Mvc;

namespace inmobiliariaDEramo.Controllers
{
    public class PagosController : Controller
    {
        private readonly IRepositorioPago repoPagos;
        private readonly IRepositorioContrato repoContrato;
        private readonly IRepositorioPropietario repoPropietario;

        private readonly IRepositorioInquilino repositorioInquilino;
        private readonly IRepositorioInmueble repositorioInmueble;


        public PagosController(IRepositorioPago repoPagos, IRepositorioContrato repoContrato, IRepositorioPropietario repoPropietario, IRepositorioInquilino repositorioInquilino, IRepositorioInmueble repositorioInmueble)
        {
            this.repoPagos = repoPagos;
            this.repoContrato = repoContrato;
            this.repoPropietario = repoPropietario;
            this.repositorioInquilino = repositorioInquilino;
            this.repositorioInmueble = repositorioInmueble;
        }

        public IActionResult Pagos(int contratoId)
        {
            var pagos = repoPagos.ObtenerPorContrato(contratoId);
            ViewBag.ContratoId = contratoId;
            return View(pagos);
        }

        /* public IActionResult CrearPago(int contratoId)
         {
             var contrato = repoContrato.ObtenerPorId(contratoId);
             var numero = repoPagos.ObtenerPorContrato(contratoId).Count() + 1;
             var pago = new Pago
             {
                 ContratoId = contratoId,
                 NumeroPago = numero,
                 FechaPago = DateTime.Now,
                 Importe = contrato.MontoMensual
             };
             return View(pago);
         }

         [HttpPost]
         public IActionResult CrearPago(Pago pago)
         {
             if (ModelState.IsValid)
             {
                 repoPagos.Alta(pago);
                 return RedirectToAction("Pagos", new { contratoId = pago.ContratoId });
             }
             return View(pago);
         }

         public IActionResult EditarPago(int id)
         {
             var pago = repoPagos.ObtenerPorId(id);
             return View(pago);
         }

         [HttpPost]
         public IActionResult EditarPago(Pago pago)
         {
             if (ModelState.IsValid)
             {
                 repoPagos.Editar(pago); // Solo se edita Detalle
                 return RedirectToAction("Pagos", new { contratoId = pago.ContratoId });
             }
             return View(pago);
         }

         public IActionResult AnularPago(int id)
         {
             var pago = repoPagos.ObtenerPorId(id);
             repoPagos.Anular(id);
             return RedirectToAction("Pagos", new { contratoId = pago.ContratoId });
         }*/

    }
}
