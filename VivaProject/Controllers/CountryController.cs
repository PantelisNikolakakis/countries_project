using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using VivaProject.Models;
using VivaProject.Services;
using Microsoft.Extensions.Caching.Memory;
using VivaProject.Entities;


namespace VivaProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ICountryService _countryService;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "CountriesCache";

        public CountryController(IHttpClientFactory httpClientFactory, ICountryService countryService, IMemoryCache memoryCache)
        {   
            _httpClient = httpClientFactory.CreateClient("RestCountriesClient"); 
            _countryService = countryService;
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        // GET: api/country
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryResponse>>> GetCountries()
        {
            // Step 1: Check if data exists in cache
            if (_memoryCache.TryGetValue(CacheKey, out List<CountryResponse> cachedCountries))
            {
                return Ok(cachedCountries);
            }

            // Step 2: Check Database
            var dbCountries = await _countryService.GetCountriesAsync()
                .ConfigureAwait(false);

            if (dbCountries.Any())
            {
                var countryResponses = dbCountries.Select(c => new CountryResponse
                {
                    CommonName = c.Name,
                    Capital = c.Capital,
                    Borders = c.Borders.Split(',')
                }).ToList();

                // Update Cache with data from Database
                _memoryCache.Set(CacheKey, countryResponses, TimeSpan.FromHours(1));

                return Ok(countryResponses);
            }

            // Step 3: Make HTTP Call
            var apiUrl = "https://restcountries.com/v3.1/all?fields=name,capital,borders";

            using (var client = new HttpClient())
            {
                try
                {
                    // Call the 3rd-party API
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "BEARER TOKEN");
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
                    ServicePointManager.Expect100Continue = true;
                    var response = await _httpClient.GetAsync(client.BaseAddress);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var countries = JsonSerializer.Deserialize<List<CountryApiResponse>>(responseBody);

                    // Map to required response format
                    var result = countries.Select(c => new CountryResponse
                    {
                        CommonName = c.name.common,
                        Capital = c.capital != null && c.capital.Any() ? c.capital[0] : "N/A",
                        Borders = c.borders ?? new List<string>()
                    }).ToList();

                    // Map to model format
                    var mappedCountries = result.Select(c => new InsertCountry
                    {
                        Name = c.CommonName,
                        Capital = c.Capital,
                        Borders = string.Join(",", c.Borders)
                    }).ToList();

                    // Insert to DB
                    _ = await _countryService.InsertCountriesAsync(mappedCountries)
                        .ConfigureAwait(false);

                    // Save to Cache
                    _memoryCache.Set(CacheKey, result, TimeSpan.FromHours(1));

                    return Ok(result);
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"HttpRequestException: {ex.Message} - {ex.InnerException?.Message}");
                }
                catch (TimeoutException ex)
                {
                    return StatusCode(408, $"Timeout Error: {ex.Message}");
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Error fetching data: {ex.Message}");
                }
            }
        }

    }
}
