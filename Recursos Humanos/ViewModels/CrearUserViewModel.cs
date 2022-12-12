using Microsoft.AspNetCore.Mvc;
using Recursos_Humanos.Models;
namespace Recursos_Humanos.ViewModels
{
    public class CreateUserViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Rol { get; set; }
        public string Password { get; set; }
    }
}
