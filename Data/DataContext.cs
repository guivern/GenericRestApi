using Microsoft.EntityFrameworkCore;
using RestApiBase.Models;

namespace RestApiBase.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // datos semilla
            modelBuilder.Entity<Rol>().HasData(
                new Rol{
                    Id = 1,
                    Nombre = "Administrador",
                    Descripcion = "Posee todos los permisos",
                },
                new Rol{
                    Id = 2,
                    Nombre = "Operador",
                    Descripcion = "Posee permisos de solo lectura",
                }
            );
        }

        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Value> Values {get; set;}
        public DbSet<Rol> Roles {get; set;}
    }
}