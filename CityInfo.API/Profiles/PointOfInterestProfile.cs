using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models.PointsOfInterest;

namespace CityInfo.API.Profiles
{
    public class PointOfInterestProfile : Profile
    {
        public PointOfInterestProfile()
        {
            CreateMap<PointOfInterest, PointOfInterestDto>();
            CreateMap<PointOfInterest, PointOfInterestUpdateDto>().ReverseMap();
            CreateMap<PointOfInterestCreationDto, PointOfInterest>();
        }
    }
}
