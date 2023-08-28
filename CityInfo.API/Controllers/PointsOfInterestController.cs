using AutoMapper;
using CityInfo.API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using CityInfo.API.Services.MailService;
using CityInfo.API.Services.Repositories;
using CityInfo.API.Models.PointsOfInterest;
using Microsoft.AspNetCore.Authorization;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/Cities/{cityId}/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {
        private readonly ILogger<PointsOfInterestController> _logger;
        private readonly IMailService _mailService;
        private readonly ICityInfoRepository _cityInfoRepository;
        private readonly IMapper _mapper;

        public PointsOfInterestController(
            ILogger<PointsOfInterestController> logger,
            IMailService mailService,
            ICityInfoRepository cityInfoRepository,
            IMapper mapper)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _mailService = mailService ??
                throw new ArgumentNullException(nameof(mailService));
            _cityInfoRepository = cityInfoRepository ??
                throw new ArgumentNullException(nameof(cityInfoRepository));
            _mapper = mapper ??
                throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PointOfInterestDto>>>
            GetPointsOfInterest([FromRoute] int cityId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
            {
                _logger.LogInformation("City with id {0} wasn't found when accessing point of interest", cityId);
                return NotFound();
            }

            var pointsOfInterests = await _cityInfoRepository.GetPointsOfInterestForCityAsync(cityId);

            return Ok(_mapper.Map<IEnumerable<PointOfInterestDto>>(pointsOfInterests));
        }

        [HttpGet("{pointOfInterestId:int}", Name = nameof(GetPointOfInterest))]
        public async Task<ActionResult<PointOfInterestDto>>
            GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
                return NotFound();

            var pointOfInterest = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterest == null)
                return NotFound();

            return Ok(_mapper.Map<PointOfInterestDto>(pointOfInterest));
        }

        // there is no need to use [FromBody] as .NET Core will assume it if the passed object is
        // a complex object. The same for [FromRoute] before cityId
        [HttpPost]
        public async Task<ActionResult<PointOfInterestDto>>
            CreatePointOfInterest(int cityId, PointOfInterestCreationDto pointOfInterestCreation)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
                return NotFound();


            var pointOfInterest = _mapper.Map<PointOfInterest>(pointOfInterestCreation);
            await _cityInfoRepository.AddPointOfInterestForCityAsync(cityId, pointOfInterest);
            await _cityInfoRepository.SaveChangesAsync();

            // once the data is saved in the db, PointOfIntered will have a generated Id
            // so we can map it back to PointOfInterestDto and return it back.
            var createdPointOfInterestToReturn = _mapper.Map<PointOfInterestDto>(pointOfInterest);

            return CreatedAtRoute(nameof(GetPointOfInterest),
                new
                {
                    cityId = cityId,
                    pointOfInterestId = createdPointOfInterestToReturn.Id
                },
                createdPointOfInterestToReturn);
        }

        // In update, no need to return any thing (just 204 status) as all the data is already known to the client
        // in PUT the api consumer should provide value for all the fields, and if a field value is not provided,
        // it has to be set to its default value
        [HttpPut("{pointOfInterestId:int}")]
        public async Task<ActionResult> UpdatePointOfInterest
            (int cityId, int pointOfInterestId, PointOfInterestUpdateDto pointOfInterest)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) 
                return NotFound();

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            
            if (pointOfInterestEntity == null) return NotFound();

            // Automapper will override the values at the destination object.
            _mapper.Map(pointOfInterest, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        // The body of patch request should contains the fields to be patched, the new values of them
        // and the operation need to be done (ADD, REPLACE, REMOVE or COPY) according to JSON Patch (RFC 6902)
        // This can be done using Microsoft.AspNetCore.JsonPatch & Microsoft.AspNetCore.Mvc.NewtonsoftJson
        [HttpPatch("{pointOfInterestId:int}")]
        public async Task<ActionResult> 
            PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId, 
                JsonPatchDocument<PointOfInterestUpdateDto> patchDocument)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId)) 
                return NotFound();

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);
            if (pointOfInterestEntity == null) 
                return NotFound();

            var pointOfInterestToPatch = _mapper.Map<PointOfInterestUpdateDto>(pointOfInterestEntity);

            // Send ModelState to be notified for any error like trying to alter a property that does not exist
            // the ModelState will contains only the error happens when pathcing in jsonDocument, so we need also 
            // to validate the model after patching.
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!TryValidateModel(pointOfInterestToPatch)) 
                return BadRequest(ModelState);

            _mapper.Map(pointOfInterestToPatch, pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{pointOfInterestId:int}")]
        public async Task<ActionResult> DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            if (!await _cityInfoRepository.CityExistsAsync(cityId))
                return NotFound();

            var pointOfInterestEntity = await _cityInfoRepository
                .GetPointOfInterestForCityAsync(cityId, pointOfInterestId);

            if (pointOfInterestEntity == null) return NotFound();

            _cityInfoRepository.DeletePointOfInterestForCityAsync(pointOfInterestEntity);
            await _cityInfoRepository.SaveChangesAsync();

            _mailService.Send(
                "Point of interest deleted",
                $"Point of interest {pointOfInterestEntity.Name} with id {pointOfInterestEntity.Id} was deleted."
            );

            return NoContent();
        }
    }
}
