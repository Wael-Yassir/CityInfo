using CityInfo.API.Data;
using CityInfo.API.Models.Cities;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]             // helps returning problems details
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly CitiesDataStore _citiesDataStore;

        public CitiesController(CitiesDataStore citiesDataStore)
        {
            _citiesDataStore = citiesDataStore ?? throw new ArgumentNullException(nameof(citiesDataStore));
        }

        [HttpGet]
        public ActionResult<IEnumerable<CityDto>> GetCities()
        {
            return Ok(_citiesDataStore.Cities);
        }

        [HttpGet("{id:int}")]
        public ActionResult<CityDto> GetCity(int id)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == id);
            if (city == null)
            {
                return NotFound();
            }

            return Ok(city);
        }
    }
}
