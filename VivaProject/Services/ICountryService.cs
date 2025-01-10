using VivaProject.Entities;
using VivaProject.Models;

namespace VivaProject.Services
{
    public interface ICountryService
    {
        Task<int> InsertCountriesAsync(IEnumerable<InsertCountry> countries);
        Task<IEnumerable<Country>> GetCountriesAsync();
    }
}
