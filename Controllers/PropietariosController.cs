﻿using inmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Cryptography;

namespace inmobiliariaDEramo.Controllers
{
	[Authorize]
	public class PropietariosController : Controller
	{
		// Sin inyección de dependencias (crear dentro del ctor)
		//private readonly RepositorioPropietario repositorio;

		// Con inyección de dependencias (pedir en el ctor como parámetro)
		private readonly IRepositorioPropietario repositorio;
		private readonly IConfiguration config;

		private readonly IRepositorioInmueble repositorioInmueble;

		public PropietariosController(IRepositorioPropietario repo, IConfiguration config, IRepositorioInmueble repositorioInmueble)
		{
			// Sin inyección de dependencias y sin usar el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioPropietario();
			// Sin inyección de dependencias y pasando el config (quitar el parámetro repo del ctor)
			//this.repositorio = new RepositorioPropietario(config);
			// Con inyección de dependencias
			this.repositorio = repo;
			this.config = config;
			this.repositorioInmueble = repositorioInmueble;
		}

		// GET: Propietario
		[HttpGet]
		public ActionResult Index(int pagina = 1, int cantidad = 5)
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
				// Paginado
				int total = lista.Count;
				int totalPaginas = (int)Math.Ceiling(total / (double)cantidad);

				var proPaginados = lista
					.Skip((pagina - 1) * cantidad)
					.Take(cantidad)
					.ToList();

				ViewBag.PaginaActual = pagina;
				ViewBag.TotalPaginas = totalPaginas;
				ViewBag.Cantidad = cantidad;

				return View(proPaginados);
			}
			catch (Exception ex)
			{
				throw;


			}
		}



		// GET: Propietario/Details/5
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

		// GET: Propietario/Busqueda
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

		// GET: Propietario/Buscar/5
		/*[Route("[controller]/Buscar/{q}", Name = "BuscarPropietario")]
		public IActionResult Buscar(string q)
		{
			try
			{
				var res = repositorio.BuscarPorNombre(q);
				return Json(new { Datos = res });
			}
			catch (Exception ex)
			{
				return Json(new { Error = ex.Message });
			}
		}*/

		// GET: Propietario/Create
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


		// POST: Propietario/Create
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create(Propietario propietario)
		{
			try
			{
				if (ModelState.IsValid)// Pregunta si el modelo es válido
				{
					if (string.IsNullOrEmpty(propietario.Clave)) // 🔍 Verificar si la clave es NULL o vacía
					{
						ModelState.AddModelError("Clave", "La contraseña es obligatoria.");
						return View(propietario);
					}

					// Reemplazo de clave plana por clave con hash
					// 
					propietario.Clave = HashPassword(propietario.Clave, out byte[] salt);

					repositorio.Alta(propietario);
					TempData["Id"] = propietario.IdPropietario;
					return RedirectToAction(nameof(Index));
				}
				else
					return View(propietario);
			}
			catch (Exception ex)
			{ // Loggear el error si es necesario
				ModelState.AddModelError("", "Ocurrió un error al crear el propietario.");
				return View(propietario);
				throw;
			}
		}

		// GET: Propietario/Edit/5
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

		// POST: Propietario/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		//public ActionResult Edit(int id, IFormCollection collection)
		public ActionResult Edit(int id, Propietario entidad)
		{
			// Si en lugar de IFormCollection ponemos Propietario, el enlace de datos lo hace el sistema
			Propietario p = null;
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
			var propietario = repositorio.ObtenerPorId(id);
			if (propietario == null)
			{
				return NotFound(); // Devuelve un error 404 si el propietario no existe
			}

			var model = new CambioClaveView { Id = propietario.IdPropietario };
			return View(model); // Muestra la vista con el formulario
		}*/

		// POST: Propietario/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CambiarPass(int id, CambioClaveView cambio)
		{
			Propietario propietario = null;
			try
			{
				// recuperar propietario original
				propietario = repositorio.ObtenerPorId(id);
				// verificar clave antigüa
				var pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
								password: cambio.ClaveVieja ?? "",
								salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
								prf: KeyDerivationPrf.HMACSHA1,
								iterationCount: 1000,
								numBytesRequested: 256 / 8));
				if (propietario.Clave != pass)
				{
					TempData["Error"] = "Clave incorrecta";
					// se rederige porque no hay vista de cambio de pass, está compartida con Edit
					return RedirectToAction("Edit", new { id = id });
				}
				if (ModelState.IsValid)
				{
					propietario.Clave = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: cambio.ClaveNueva,
							salt: System.Text.Encoding.ASCII.GetBytes(config["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					repositorio.Modificacion(propietario);
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
		}



		// GET: Propietario/Delete/5
		[HttpGet]
		[Authorize(Policy = "Administrador")]
		public ActionResult Eliminar(int id, Inmueble entidad)
		{
			try
			{
				entidad = repositorioInmueble.ObtenerPorId(id);

				var propietario = repositorio.ObtenerPorId(id);
				if (propietario == null)
					return NotFound();

				return View(propietario);
			}
			catch (Exception ex)
			{
				TempData["Error"] = "Error al buscar el propietario: " + ex.Message;
				return RedirectToAction(nameof(Index));
			}
		}




		// POST: Propietario/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrador")]
		public ActionResult Eliminar(int id, Propietario propietario)
		{
			try
			{
				// // Obtener todos los inmuebles asociados al propietario
				// var inmuebles = repositorioInmueble.BuscarPorPropietario(id);

				// foreach (var inmueble in inmuebles)
				// {
				// 	Console.WriteLine($"Dando de baja inmueble: {inmueble.Id} - {inmueble.Direccion}");
				// 	repositorioInmueble.Baja(inmueble.Id);
				// }

				repositorio.Baja(id);
				TempData["Mensaje"] = "Propietario e inmuebles dados de baja correctamente.";
				return RedirectToAction(nameof(Index));
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", "Ocurrió un error al eliminar el propietario.");
				return View();
			}
		}









		[HttpGet]
		public JsonResult Buscar(string id)
		{
			var propietarios = repositorio.ObtenerTodos()
				.Where(p => (p.Nombre + " " + p.Apellido + " " + p.Dni).ToLower().Contains(id.ToLower()))
				.Select(p => new { idPropietario = p.IdPropietario, nombre = p.Nombre, apellido = p.Apellido, dni = p.Dni })
				.ToList();

			return Json(new { datos = propietarios });
		}



		public ActionResult Activar(int id)
		{
			try
			{
				repositorio.Activar(idPropietario: id);
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
		public IActionResult Detalles(int id)
		{
			var propietario = repositorio.ObtenerPorId(id);
			if (propietario == null) return NotFound();

			var inmuebles = repositorioInmueble.BuscarPorPropietario(id);
			ViewBag.Inmuebles = inmuebles;

			return View(propietario);
		}



	}
}