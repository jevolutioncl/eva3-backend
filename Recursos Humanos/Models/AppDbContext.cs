using Microsoft.EntityFrameworkCore;

namespace Recursos_Humanos.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Departamento> Departamentos{ get; set; }
        public DbSet<Colaborador> Colaboradores{ get; set; }
        public DbSet<Usuario> Usuarios { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=RRHH;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Colaborador>().Property(x => x.FechaContratacion).HasColumnType("date");
        }
    }
}
