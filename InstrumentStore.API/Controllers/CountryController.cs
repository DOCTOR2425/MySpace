using InstrumentStore.Domain.Abstractions;
using InstrumentStore.Domain.DataBase.Models;
using Microsoft.AspNetCore.Mvc;

namespace InstrumentStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryService _countryService;

        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Country>>> GetAllCountrys()
        {
            return Ok(await _countryService.GetAll());
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateCountry([FromBody] string countryName)
        {
            Country brand = new Country
            {
                CountryId = Guid.NewGuid(),
                Name = countryName
            };

            return Ok(await _countryService.Create(brand));
        }
    }
}
