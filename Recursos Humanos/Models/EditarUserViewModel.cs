﻿using Microsoft.AspNetCore.Mvc;

namespace Recursos_Humanos.Models
{
    public class EditarUserViewModel
    {
        public Guid UsuarioId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Rol { get; set; }
        public string Password { get; set; }

    }
}
