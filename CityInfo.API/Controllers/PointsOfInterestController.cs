using CityInfo.API.Data;
using CityInfo.API.Models.PointsOfInterest;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CityInfo.API.Controllers
{
    [ApiController]
    [Route("api/Cities/{cityId}/[controller]")]
    public class PointsOfInterestController : ControllerBase
    {

        public PointsOfInterestController()
        {
            
        }

        [HttpGet]
        public ActionResult<IEnumerable<PointOfInterestDto>> GetPointsOfInterest([FromRoute] int cityId)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
            if (city == null)
                return NotFound();

            return Ok(city.PointsOfInterest);
        }

        [HttpGet("{pointOfInterestId:int}", Name = nameof(GetPointOfInterest))]
        public ActionResult<PointOfInterestDto> GetPointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
            if (city == null)
                return NotFound();

            var pointOfInterest = city.PointsOfInterest.Find(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null)
                return NotFound();

            return Ok(pointOfInterest);
        }

        // there is no need to use [FromBody] as .NET Core will assume it if the passed object is
        // a complex object. The same for [FromRoute] before cityId
        [HttpPost]
        public ActionResult<PointOfInterestDto>
            CreatePointOfInterest(int cityId, PointOfInterestCreation pointOfInterestCreation)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
            if (city == null)
                return NotFound();

            int maxId = CitiesDataStore.Current.Cities.SelectMany(c => c.PointsOfInterest).Max(p => p.Id);
            var pointOfInterestDto = new PointOfInterestDto()
            {
                Id = ++maxId,
                Name = pointOfInterestCreation.Name,
                Description = pointOfInterestCreation.Description,
            };

            city.PointsOfInterest.Add(pointOfInterestDto);

            return CreatedAtRoute(nameof(GetPointOfInterest),
                new
                {
                    cityId = cityId,
                    pointOfInterestId = pointOfInterestDto.Id
                },
                pointOfInterestCreation);
        }

        // In update, no need to return any thing (just 204 status) as all the data is already known to the client
        // in PUT the api consumer should provide value for all the fields, and if a field value is not provided,
        // it has to be set to its default value
        [HttpPut("{pointOfInterestId:int}")]
        public ActionResult UpdatePointOfInterest(int cityId, int pointOfInterestId,
            PointOfInterestUpdate pointOfInterestUpdate)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
            if (city == null) return NotFound();

            var pointOfInterest = city.PointsOfInterest.Find(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null) return NotFound();

            pointOfInterest.Name = pointOfInterestUpdate.Name;
            pointOfInterest.Description = pointOfInterestUpdate.Description;

            return NoContent();
        }

        // The body of patch request should contains the fields to be patched, the new values of them
        // and the operation need to be done (ADD, REPLACE, REMOVE or COPY) according to JSON Patch (RFC 6902)
        // This can be done using Microsoft.AspNetCore.JsonPatch & Microsoft.AspNetCore.Mvc.NewtonsoftJson
        [HttpPatch("{pointOfInterestId:int}")]
        public ActionResult PartiallyUpdatePointOfInterest(int cityId, int pointOfInterestId,
            JsonPatchDocument<PointOfInterestUpdate> patchDocument)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
            if (city == null) return NotFound();

            var pointOfInterestStore = city.PointsOfInterest.Find(p => p.Id == pointOfInterestId);
            if (pointOfInterestStore == null) return NotFound();

            var pointOfInterestToPatch = new PointOfInterestUpdate()
            {
                Name = pointOfInterestStore.Name,
                Description = pointOfInterestStore.Description
            };

            // Send ModelState to be notified for any error like trying to alter a property that does not exist
            // the ModelState will contains only the error happens when pathcing in jsonDocument, so we need also 
            // to validate the model after patching.
            patchDocument.ApplyTo(pointOfInterestToPatch, ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);

            if (!TryValidateModel(pointOfInterestToPatch)) return BadRequest(ModelState);

            pointOfInterestStore.Name = pointOfInterestToPatch.Name;
            pointOfInterestStore.Description = pointOfInterestToPatch.Description;
            return NoContent();
        }

        [HttpDelete("{pointOfInterestId:int}")]
        public ActionResult DeletePointOfInterest(int cityId, int pointOfInterestId)
        {
            var city = CitiesDataStore.Current.Cities.Find(c => c.Id == cityId);
            if (city == null) return NotFound();

            var pointOfInterest = city.PointsOfInterest.Find(p => p.Id == pointOfInterestId);
            if (pointOfInterest == null) return NotFound();

            city.PointsOfInterest.Remove(pointOfInterest);
            return NoContent();
        }
    }
}
