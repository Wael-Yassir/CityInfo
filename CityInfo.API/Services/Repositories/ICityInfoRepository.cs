using CityInfo.API.Entities;
using CityInfo.API.Services.Pagination;

namespace CityInfo.API.Services.Repositories
{
    public interface ICityInfoRepository
    {
        // Can return IEnumerable, or IQueryable and the difference is IQueryable when can be built 
        // on top of it (i.e. OrderBy()) to be used before teh query is executed in the database.
        // When using Async this enhance the scalability as normally, one thread handle one request.

        // Deleting is just like adding, it is an in-memory operation,
        // not I/O operation so no need for it to be async.

        Task<IEnumerable<City>> GetCitiesAsync();
        
        Task<(IEnumerable<City>, PaginationMetadata)> 
            GetCitiesAsync(string? name, string? searchQuery, int pageNumber, int pageSize);    // for filtering by city name

        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        
        Task<bool> CityExistsAsync(int cityId);
        
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);

        // AddPointOfInterestForCityAsync is async because in the call we need to call GetCityAsync
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        
        void DeletePointOfInterestForCityAsync(PointOfInterest pointOfInterest);
        
        Task<bool> SaveChangesAsync();
    }
}
