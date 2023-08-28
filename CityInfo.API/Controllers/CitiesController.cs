using AutoMapper;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using CityInfo.API.Models.Cities;
using CityInfo.API.Services.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers
{
    [ApiController]             // helps returning problems details
    [Authorize]
    [Route("api/[controller]")]
    public class CitiesController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ICityInfoRepository _cityInfoRepository;

        private readonly int _maxCitiesPageSize = 20;

        public CitiesController(ICityInfoRepository cityInfoRepository, IMapper mapper)
        {
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));

            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        // FromQuery is not needed, as name is not complex object, or form file, 
        // or route parameter so the framework will detect it as query string.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityDtoWithoutPointsOfInterest>>>
            GetCities(string? name, string? searchQuery, int pageNumber = 1, int pageSize = 10)
        {
            // Information from token can be accesed through: User.Claims;

            if (pageSize > _maxCitiesPageSize) pageSize = _maxCitiesPageSize;

            var (cityEntities, paginationMetadata) = await _cityInfoRepository
                .GetCitiesAsync(name, searchQuery, pageNumber, pageSize);

            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));

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
