using AutoMapper;
using CityInfo.API.Entities;
using CityInfo.API.Models.Cities;

namespace CityInfo.API.Profiles
{
    public class CityProfile : Profile
    {
        // To add configuration, it can be added through the constructor
        public CityProfile()
        {
            CreateMap<City, CityDtoWithoutPointsOfInterest>();
            CreateMap<City, CityDto>();
        }
    } 
}
