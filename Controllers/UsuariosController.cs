using System.Security.Claims;
using inmobiliariaDEramo.Models;
using InmobiliariaDEramo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;



namespace InmobiliariaDEramo.Controllers
{
    [Authorize]
    public class UsuariosController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment environment;
        private readonly IRepositorioUsuario repositorio;

        public UsuariosController(IConfiguration configuration, IWebHostEnvironment environment, IRepositorioUsuario repositorio)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.repositorio = repositorio;
        }
        // GET: Usuarios
        [Authorize(Policy = "Administrador")]
        public ActionResult Index()
        {
            var usuarios = repositorio.ObtenerTodos();
            return View(usuarios);
        }

        // GET: Usuarios/Details/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Details(int id)
        {
            var e = repositorio.ObtenerPorId(id);
            return View(e);
        }

        // GET: Usuarios/Create
        [Authorize(Policy = "Administrador")]
        public ActionResult Create()
        {
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View();
        }

        // POST: Usuarios/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Create(Usuario u)
        {
            if (!ModelState.IsValid)
                return View();
            try
            {
                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                password: u.Clave,
                                salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                                prf: KeyDerivationPrf.HMACSHA1,
                                iterationCount: 1000,
                                numBytesRequested: 256 / 8));
                u.Clave = hashed;
                u.Rol = User.IsInRole("Administrador") ? u.Rol : (int)enRoles.Empleado;
                var nbreRnd = Guid.NewGuid();//posible nombre aleatorio
                int res = repositorio.Alta(u);
                var salt = configuration["Salt"];
                if (string.IsNullOrEmpty(salt))
                    throw new Exception("Salt no configurado.");

                if (u.AvatarFile != null && u.Id > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    //Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);
                    // Esta operación guarda la foto en memoria en la ruta que necesitamos
                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                    repositorio.Modificacion(u);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                return View();
            }
        }



        // GET: Usuarios/Perfil/5
        [Authorize]
        public ActionResult Perfil()
        {
            ViewData["Title"] = "Mi perfil";
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View("Edit", u);
        }



        // GET: Usuarios/Edit/5
        [Authorize]
        public IActionResult Edit(int id)
        {
            ViewData["Title"] = "Editar usuario";
            var u = repositorio.ObtenerPorId(id);
            if (u == null)
            {
                TempData["Error"] = "Usuario no encontrado.";
                return RedirectToAction("Index");
            }
            ViewBag.Roles = Usuario.ObtenerRoles();
            return View(u);
        }



        // POST: Usuarios/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(int id, Usuario u, string ClaveNueva, string ReClaveNueva)
        {
            var vista = nameof(Edit);

            try
            {
                if (!User.IsInRole("Administrador"))
                {
                    vista = nameof(Perfil);
                    var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
                    if (usuarioActual.Id != id)
                        return RedirectToAction(nameof(Index), "Home");
                }

                var usuarioDb = repositorio.ObtenerPorId(id);

                if (!string.IsNullOrEmpty(ClaveNueva) || !string.IsNullOrEmpty(ReClaveNueva))
                {
                    if (ClaveNueva != ReClaveNueva)
                    {
                        ModelState.AddModelError("ClaveNueva", "Las contraseñas no coinciden.");
                        ViewBag.Roles = Usuario.ObtenerRoles();
                        return View(vista, u);
                    }

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: ClaveNueva,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    u.Clave = hashed;
                }
                else
                {
                    // Si no cambia la clave, conservar la actual
                    u.Clave = usuarioDb.Clave;
                }

                if (u.AvatarFile != null && u.AvatarFile.Length > 0)
                {
                    string wwwPath = environment.WebRootPath;
                    string path = Path.Combine(wwwPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    string fileName = "avatar_" + u.Id + Path.GetExtension(u.AvatarFile.FileName);
                    string pathCompleto = Path.Combine(path, fileName);
                    u.Avatar = Path.Combine("/Uploads", fileName);

                    using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
                    {
                        u.AvatarFile.CopyTo(stream);
                    }
                }
                else
                {
                    // Si no cambia el avatar, conservar el actual
                    u.Avatar = usuarioDb.Avatar;
                }

                // Si no es admin, mantener su rol
                if (!User.IsInRole("Administrador"))
                {
                    u.Rol = usuarioDb.Rol;
                }

                repositorio.Modificacion(u);
                TempData["Mensaje"] = "Perfil actualizado correctamente.";
                return RedirectToAction(vista, new { id = u.Id });
            }
            catch (Exception ex)
            {
                ViewBag.Roles = Usuario.ObtenerRoles();
                TempData["Error"] = "Error al guardar: " + ex.Message;
                return View(vista, u);
            }
        }







        // GET: Usuarios/Delete/5
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id)
        {
            var usuario = repositorio.ObtenerPorId(id);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        // POST: Usuarios/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Administrador")]
        public ActionResult Eliminar(int id, Usuario usuario)
        {
            try
            {

                repositorio.Baja(id);
                TempData["Mensaje"] = "Usuario dade de baja correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar usuario: " + ex.Message;
                return View(usuario);
            }
        }

        [Authorize(Policy = "Administrador")]
        public IActionResult Activar(int id)
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





        [Authorize]
        public IActionResult Avatar()
        {
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);

            //leer el archivo
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            //devolverlo
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [Authorize]
        public string AvatarBase64()
        {
            var u = repositorio.ObtenerPorEmail(User.Identity.Name);
            string fileName = "avatar_" + u.Id + Path.GetExtension(u.Avatar);
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);

            //leer el archivo
            byte[] fileBytes = System.IO.File.ReadAllBytes(pathCompleto);
            //devolverlo
            return Convert.ToBase64String(fileBytes);
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{fileName}")]
        public IActionResult FromBase64([FromBody] string imagen, [FromRoute] string fileName)
        {
            //arma el path
            string wwwPath = environment.WebRootPath;
            string path = Path.Combine(wwwPath, "Uploads");
            string pathCompleto = Path.Combine(path, fileName);
            //convierto a arreglo de bytes
            var bytes = Convert.FromBase64String(imagen);
            //lo escribe
            System.IO.File.WriteAllBytes(pathCompleto, bytes);
            return Ok();
        }

        [Authorize]
        public ActionResult Foto()
        {
            try
            {
                var u = repositorio.ObtenerPorEmail(User.Identity.Name);
                var stream = System.IO.File.Open(
                        Path.Combine(environment.WebRootPath, u.Avatar.Substring(1)),
                        FileMode.Open,
                        FileAccess.Read);
                var ext = Path.GetExtension(u.Avatar);
                return new FileStreamResult(stream, $"image/{ext.Substring(1)}");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Authorize]
        public ActionResult Datos()
        {
            try
            {
                var u = repositorio.ObtenerPorEmail(User.Identity.Name);
                string buffer = "Nombre;Apellido;Email" + Environment.NewLine +
                        $"{u.Nombre};{u.Apellido};{u.Email}";
                var stream = new MemoryStream(System.Text.Encoding.Unicode.GetBytes(buffer));
                var res = new FileStreamResult(stream, "text/plain");
                res.FileDownloadName = "Datos.csv";
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult LoginModal()
        {
            return PartialView("_LoginModal", new LoginView());
        }

        [AllowAnonymous]
        // GET: Usuarios/Login/
        public ActionResult Login(string returnUrl)
        {
            TempData["returnUrl"] = returnUrl;
            return View();
        }

        // POST: Usuarios/Login/
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginView login)
        {
            try
            {
                var returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
                if (ModelState.IsValid)
                {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: login.Clave,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));

                    var e = repositorio.ObtenerPorEmail(login.Usuario);
                    if (e == null || e.Clave != hashed)
                    {
                        ModelState.AddModelError("", "El email o la clave no son correctos");
                        TempData["returnUrl"] = returnUrl;
                        return View();
                    }

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, e.Email),
                        new Claim("FullName", e.Nombre + " " + e.Apellido),
                        new Claim(ClaimTypes.Role, e.RolNombre),
                    };

                    var claimsIdentity = new ClaimsIdentity(
                            claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(
                            CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity));
                    TempData.Remove("returnUrl");
                    return Redirect(returnUrl);
                }
                TempData["returnUrl"] = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
        }

        // GET: /salir
        [Route("salir", Name = "logout")]
        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }



    }
}