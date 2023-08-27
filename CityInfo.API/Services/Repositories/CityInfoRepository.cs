using CityInfo.API.Entities;
using CityInfo.API.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.Services.Repositories
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync()
        {
            return await _context.Citites.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<City?>
            GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Citites.Include(c => c.PointsOfInterest)
                    .FirstOrDefaultAsync(c => c.Id == cityId);
            }

            return await _context.Citites.FirstOrDefaultAsync(c => c.Id == cityId);
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Citites.AnyAsync(c => c.Id == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>>
            GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                .Where(p => p.CityId == cityId).ToListAsync();
        }

        public async Task<PointOfInterest?>
            GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                .FirstOrDefaultAsync(p => p.CityId == cityId && p.Id == pointOfInterestId);
        }

        public async Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
                city.PointsOfInterest.Add(pointOfInterest);
        }

        public void DeletePointOfInterestForCityAsync(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }
    }
}
