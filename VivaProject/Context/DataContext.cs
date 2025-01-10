using Microsoft.EntityFrameworkCore;
using VivaProject.Entities;
using System.Reflection;

namespace VivaProject.Context
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        public DbSet<Country> Countries { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(
                Assembly.GetExecutingAssembly()
            );
            base.OnModelCreating(modelBuilder);
        }
    }
}
