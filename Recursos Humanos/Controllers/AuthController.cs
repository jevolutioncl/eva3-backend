using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Recursos_Humanos.Models;
using Recursos_Humanos.ViewModels;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Recursos_Humanos.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public IActionResult LoginIn()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginIn(LoginViewModel Lvm)
        {
            var usuarios = _context.Usuarios.ToList();
            if(usuarios.Count == 0)
            {
                Usuario U = new Usuario();
                U.Name = "Administrador";
                U.Email = "admin@recursos.cl";
                U.Username = "admin";
                U.Rol = "SuperAdministrador";

                CreatePasswordHash("admin", out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.Usuarios.Add(U);
                _context.SaveChanges();
            }

            var us = _context.Usuarios.Where(u => u.Username.Equals(Lvm.Username)).FirstOrDefault();
            if (us != null)
            {
                //Usuario encontrado
                if(VerificarPass(Lvm.Password, us.PasswordHash, us.PasswordSalt))
                {
                    //Usuario y contraseña correcta
                    var Claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, us.Name),
                        new Claim(ClaimTypes.NameIdentifier, Lvm.Username),
                        new Claim(ClaimTypes.Role, us.Rol)
                    };

                    var identity = new ClaimsIdentity(Claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                        new AuthenticationProperties {  IsPersistent = true}
                        );

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    //Usuario correcto, pero contraseña incorrecta
                    ModelState.AddModelError("", "Contraseña incorrecta");
                    return View(Lvm);
                }
            }
            else
            {
                //Usuario no existe
                ModelState.AddModelError("", "Usuario no encontrado");
                return View(Lvm);
            }
            
            
            return View();
        }
        [HttpGet]

        public async Task<IActionResult> EditUser2(Guid UsuarioId)
        {
            var U = _context.Usuarios.FirstOrDefault(u => u.UsuarioId.Equals(UsuarioId));
            if (U == null) return NotFound();

            EditarProfileViewModel Evm = new EditarProfileViewModel()
            {
                Email = U.Email,
                Username = U.Username,
                UsuarioId = UsuarioId,
                Name = U.Name,
            };

            return View(Evm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser2(EditarProfileViewModel Evm)
        {
            var U = _context.Usuarios.FirstOrDefault(u => u.UsuarioId.Equals(Evm.UsuarioId));
            if (U == null) return NotFound();

            U.Name = Evm.Name;
            U.Email = Evm.Email;
            U.Username = Evm.Username;

            CreatePasswordHash(Evm.Password, out byte[] passwordHash, out byte[] passwordSalt);

            U.PasswordSalt = passwordSalt;
            U.PasswordHash = passwordHash;

            _context.Update(U);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Profile));
        }

        //ver perfil
        public IActionResult Profile()
        {
            if (User.Identity.IsAuthenticated)
            {
                var usuarioPerfil = _context.Usuarios.FirstOrDefault(u => u.Name == User.Identity.Name);
                ProfileViewModel pvm = new ProfileViewModel()
                {
                    Usuario = usuarioPerfil
                };

                return View(pvm);
            }
            return RedirectToAction(nameof(LoginIn));
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(LoginIn));
        }
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            //administrador
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }
        private bool VerificarPass(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return pass.SequenceEqual(passwordHash);
            }
        }
    }
}
