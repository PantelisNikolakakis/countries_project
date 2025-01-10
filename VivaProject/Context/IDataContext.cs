using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using VivaProject.Entities;

namespace VivaProject.Context
{
    public interface IDataContext
    {
        public DbSet<Country> Countries { get; set; }
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
