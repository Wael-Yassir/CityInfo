namespace CityInfo.API.Models.Cities
{
    public class CityDtoWithoutPointsOfInterest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
