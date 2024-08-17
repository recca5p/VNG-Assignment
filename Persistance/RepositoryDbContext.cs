using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Domain.RepositoriyInterfaces;

namespace Persistance
{
    public sealed class RepositoryDbContext : DbContext, IRepositoryDbContext
    {
        public RepositoryDbContext()
        {
        }

        public RepositoryDbContext(DbContextOptions<RepositoryDbContext> options)
            : base(options)
        {
        }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // var connectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            // if (!string.IsNullOrEmpty(connectionString))
            // {
            //     optionsBuilder.UseSqlServer("Server=103.166.182.81,1433;Database=vng;User ID=sa;Password=12345Abc%;Pooling=true;Min Pool Size=0;Max Pool Size=200;Connect Timeout=60;Application Name=AppName;Encrypt=False;");
            // }
            optionsBuilder.UseSqlServer("Server=103.166.182.81,1433;Database=vng;User ID=sa;Password=12345Abc%;Pooling=true;Min Pool Size=0;Max Pool Size=200;Connect Timeout=60;Application Name=AppName;Encrypt=False;");

        }

        public DbSet<User> Users { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDbContext).Assembly);
    }
}
