using AutoMapper;
using CityInfo.API.Data;
using CityInfo.API.Models.Cities;
using CityInfo.API.Services.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]             // helps returning problems details
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDtoWithoutPointsOfInterest>>>
            GetCities()
        {
            var cityEntities = await _cityInfoRepository.GetCitiesAsync();
            return Ok(_mapper.Map<IEnumerable<CityDtoWithoutPointsOfInterest>>(cityEntities));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult>
            GetCity(int id, [FromQuery] bool includePointOfInterest)
        {
            var city = await _cityInfoRepository.GetCityAsync(id, includePointOfInterest);
            if (city == null) return NotFound();

            if (includePointOfInterest)
                return Ok(_mapper.Map<CityDto>(city));
            else
                return Ok(_mapper.Map<CityDtoWithoutPointsOfInterest>(city));
        }
    }
}
