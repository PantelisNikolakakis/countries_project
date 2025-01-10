using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using VivaProject.Context;
using VivaProject.Entities;
using VivaProject.Models;

namespace VivaProject.Services
{
    public class CountryService : ICountryService
    {
        private readonly IDataContext _dataContext;

        public CountryService(IDataContext dataContext) {
            _dataContext = dataContext;   
        }

        public async Task<int> InsertCountriesAsync(IEnumerable<InsertCountry> countries)
        {
            foreach (var country in countries)
            {
                var dbCountries = await GetCountriesAsync()
                    .ConfigureAwait(false);
                var existsInDb = dbCountries.Any(c => c.Name.Equals(country.Name));

                if (!existsInDb)
                {
                    Country newCountry = new Country
                    {
                        Name = country.Name,
                        Capital = country.Capital,
                        Borders = country.Borders
                    };

                    _dataContext.Countries.Add(newCountry);
                }
            }

            return await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            return await _dataContext.Countries.ToListAsync();
        }
    }
}
