using Microsoft.AspNetCore.Mvc;
using Recursos_Humanos.Models;
using Recursos_Humanos.ViewModels;
using System.Security.Cryptography;

namespace Recursos_Humanos.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GestionUsuario()
        {
            return View(_context.Usuarios.ToList());
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel Uvm)
        {
            if (ModelState.IsValid)
            {
                Usuario U = new Usuario();
                U.Name = Uvm.Name;
                U.Email = Uvm.Email;
                U.Username = Uvm.Username;
                U.Rol = Uvm.Rol;

                CreatePasswordHash(Uvm.Password, out byte[] passwordHash, out byte[] passwordSalt);
                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;

                _context.Usuarios.Add(U);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(GestionUsuario));
            }
            else
            {
                return View(Uvm);
            }
        }

        [HttpGet]

        public async Task<IActionResult> EditUser(Guid UsuarioId)
        {
            var U = _context.Usuarios.FirstOrDefault(u => u.UsuarioId.Equals(UsuarioId));
            if (U == null) return NotFound();

            EditarUserViewModel Evm = new EditarUserViewModel()
            {
                Email = U.Email,
                Username = U.Username,
                UsuarioId = UsuarioId,
                Name = U.Name,
                Rol = U.Rol
            };

            return View(Evm);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditarUserViewModel Evm)
        {
            var U = _context.Usuarios.FirstOrDefault(u => u.UsuarioId.Equals(Evm.UsuarioId));
            if (U == null) return NotFound();

            U.Name = Evm.Name;
            U.Email = Evm.Email;
            U.Username = Evm.Username;
            U.Rol = Evm.Rol;

            CreatePasswordHash(Evm.Password, out byte[] passwordHash, out byte[] passwordSalt);

            U.PasswordSalt = passwordSalt;
            U.PasswordHash = passwordHash;

            _context.Update(U);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(GestionUsuario));
        }
        public IActionResult EliminarUsuario(Guid UsuarioId)
        {
            var Usuario = _context.Usuarios.Find(UsuarioId);
            if (Usuario != null)
            {
                _context.Usuarios.Remove(Usuario);
                _context.SaveChanges();
                return RedirectToAction(nameof(GestionUsuario));
            }
            else
            {
                return NotFound();
            }
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
    }
}
