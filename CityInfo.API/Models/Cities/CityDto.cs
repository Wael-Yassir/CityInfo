using CityInfo.API.Models.PointsOfInterest;

namespace CityInfo.API.Models.Cities
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int NumberOfPointsOfInterest
        {
            get
            {
                return PointsOfInterest.Count;
            }
        }
        public List<PointOfInterestDto> PointsOfInterest { get; set; } =
            new List<PointOfInterestDto>();
    }
}
