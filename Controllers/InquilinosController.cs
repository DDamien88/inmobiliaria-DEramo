using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using inmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;

namespace inmobiliariaDEramo.Controllers
{
	[Authorize]
	public class InquilinosController : Controller
	{
		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioInquilino repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly IRepositorioInquilino repositorio;
		private readonly IConfiguration config;

		public InquilinosController(IRepositorioInquilino repo, IConfiguration config)
		{
			// Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioInquilino();
			// Sin inyección de dependencias y pasando el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioInquilino(config);
			// Con inyección de dependencias
			this.repositorio = repo;
			this.config = config;
		}

		// GET: Inquilino
		[Route("[controller]/Index/{pagina:int?}")]
		public ActionResult Index(int pagina = 1)
		{
			try
			{
				var lista = repositorio.ObtenerTodos();
				// var lista = repositorio.ObtenerLista(Math.Max(pagina, 1), 5);
				ViewBag.Id = TempData["Id"];
				// TempData es para pasar datos entre acciones
				// ViewBag/Data es para pasar datos del controlador a la vista
				// Si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
				return View(lista);
			}
			catch (Exception ex)
			{// Poner breakpoints para detectar errores
				throw;
			}
		}

		// GET: Inquilino/Details/5
		public ActionResult Details(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View();//¿qué falta?
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		// GET: Inquilino/Busqueda
		public IActionResult Busqueda()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		// GET: Inquilino/Buscar/5
		[HttpGet]
		[Route("Inquilinos/Buscar/{q}")]
		public IActionResult Buscar(string q)
		{
			try
			{
				var res = repositorio.BuscarPorNombre(q);
				return Json(new { datos = res });
			}
			catch (Exception ex)
			{
				return Json(new { error = ex.Message });
			}
		}



		// GET: Inquilino/Create
		public ActionResult Create()
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		public static string HashPassword(string password, out byte[] salt)
		{
			// Generar un "salt" aleatorio
			salt = RandomNumberGenerator.GetBytes(16);

			// Hashear la contraseña usando PBKDF2
			return Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 100000,
				numBytesRequested: 32));
		}


		// POST: Inquilino/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Inquilino Inquilino)
		{
			try
			{
				if (ModelState.IsValid)// Pregunta si el modelo es válido
				{
					// if (string.IsNullOrEmpty(Inquilino.Clave)) // 🔍 Verificar si la clave es NULL o vacía
					// {
					// 	ModelState.AddModelError("Clave", "La contraseña es obligatoria.");
					// 	return View(Inquilino);
					// }

					// Reemplazo de clave plana por clave con hash
					// 
					//Inquilino.Clave = HashPassword(Inquilino.Clave, out byte[] salt);

					repositorio.Alta(Inquilino);
					TempData["Id"] = Inquilino.IdInquilino;
					return RedirectToAction(nameof(Index));
				}
				else
					return View(Inquilino);
			}
			catch (Exception ex)
			{ // Loggear el error si es necesario
				ModelState.AddModelError("", "Ocurrió un error al crear el Inquilino.");
				return View(Inquilino);
				throw;
			}
		}

		// GET: Inquilino/Edit/5
		public ActionResult Edit(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);//pasa el modelo a la vista
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		// POST: Inquilino/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Edit(int id, IFormCollection collection)
		public ActionResult Edit(int id, Inquilino entidad)
		{
			// Si en lugar de IFormCollection ponemos Inquilino, el enlace de datos lo hace el sistema
			Inquilino p = null;
			try
			{
				p = repositorio.ObtenerPorId(id);
				// En caso de ser necesario usar: 
				//
				//Convert.ToInt32(collection["CAMPO"]);
				//Convert.ToDecimal(collection["CAMPO"]);
				//Convert.ToDateTime(collection["CAMPO"]);
				//int.Parse(collection["CAMPO"]);
				//decimal.Parse(collection["CAMPO"]);
				//DateTime.Parse(collection["CAMPO"]);
				////////////////////////////////////////
				p.Nombre = entidad.Nombre;
				p.Apellido = entidad.Apellido;
				p.Dni = entidad.Dni;
				p.Email = entidad.Email;
				p.Telefono = entidad.Telefono;
				repositorio.Modificacion(p);
				TempData["Mensaje"] = "Datos guardados correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		//GET para cambio de clave
		/*[HttpGet]
		public IActionResult CambiarPass(int id)
		{
			var Inquilino = repositorio.ObtenerPorId(id);
			if (Inquilino == null)
			{
				return NotFound(); // Devuelve un error 404 si el Inquilino no existe
			}

			var model = new CambioClaveView { Id = Inquilino.IdInquilino };
			return View(model); // Muestra la vista con el formulario
		}*/

		// POST: Inquilino/Edit/5
		/*[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CambiarPass(int id, CambioClaveView cambio)
		{
			Inquilino Inquilino = null;
			try
			{
				// recuperar Inquilino original
				Inquilino = repositorio.ObtenerPorId(id);
				// verificar clave antigüa
				var pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
								password: cambio.ClaveVieja ?? "",
								salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
								prf: KeyDerivationPrf.HMACSHA1,
								iterationCount: 1000,
								numBytesRequested: 256 / 8));
				if (Inquilino.Clave != pass)
				{
					TempData["Error"] = "Clave incorrecta";
					// se rederige porque no hay vista de cambio de pass, está compartida con Edit
					return RedirectToAction("Edit", new { id = id });
				}
				if (ModelState.IsValid)
				{
					Inquilino.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: cambio.ClaveNueva,
							salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					repositorio.Modificacion(Inquilino);
					TempData["Mensaje"] = "Contraseña actualizada correctamente";
					return RedirectToAction(nameof(Index));
				}
				else//estado inválido
				{//pasaje de los errores del modelstate a un string en tempData
					foreach (ModelStateEntry modelState in ViewData.ModelState.Values)
					{
						foreach (ModelError error in modelState.Errors)
						{
							TempData["Error"] += error.ErrorMessage + "\n";
						}
					}
					return RedirectToAction("Edit", new { id = id });
				}
			}
			catch (Exception ex)
			{
				TempData["Error"] = ex.Message;
				TempData["StackTrace"] = ex.StackTrace;
				return RedirectToAction("Edit", new { id = id });
			}
		}*/

		// GET: Inquilino/Delete/5
		public ActionResult Eliminar(int id)
		{
			try
			{
				var entidad = repositorio.ObtenerPorId(id);
				return View(entidad);
			}
			catch (Exception ex)
			{//poner breakpoints para detectar errores
				throw;
			}
		}

		// POST: Inquilino/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Eliminar(int id, Inquilino entidad)
		{
			try
			{
				repositorio.Baja(id);
				TempData["Mensaje"] = "Eliminación realizada correctamente";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				//poner breakpoints para detectar errores
				ModelState.AddModelError("", "Ocurrio un error al eliminar el Inquilino.");
				throw;
			}
		}

		public ActionResult Activar(int id)
		{
			try
			{
				repositorio.Activar(idInquilino: id);
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