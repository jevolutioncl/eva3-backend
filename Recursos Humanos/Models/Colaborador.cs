using System.ComponentModel.DataAnnotations;

namespace Recursos_Humanos.Models
{
    public class Colaborador
    {
        [Key]
        public string Rut { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Direccion { get; set; }
        public string Comuna { get; set; }
        public string Telefono { get; set; }
        public string Correo { get; set; }
        public DateTime FechaContratacion { get; set; }
        public bool ContratoIndefinido { get; set; }
        public int DepartamentoId { get; set; }
        public Departamento? Departamento{ get; set; }
    }
}
