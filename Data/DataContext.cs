using Microsoft.EntityFrameworkCore;
using RestApiBase.Models;

namespace RestApiBase.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
        : base(options){}

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {}

        public DbSet<Usuario> Usuarios {get; set;}
        public DbSet<Value> Values {get; set;}
    }
}