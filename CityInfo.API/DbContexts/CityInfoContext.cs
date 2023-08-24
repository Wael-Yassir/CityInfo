using CityInfo.API.Entities;
using CityInfo.API.Models.Cities;
using CityInfo.API.Models.PointsOfInterest;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Citites { get; set; } = null!;
        public DbSet<PointOfInterest> PointsOfInterest { get; set; } = null!;

        // we can provide options on program.cs when registering the service.
        public CityInfoContext(DbContextOptions<CityInfoContext> options) 
            : base(options)
        {
            
        }

        // To configure the db context, it can be done in program.cs or by override OnConfiguring
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlite("conntectionstring");
        //    base.OnConfiguring(optionsBuilder);
        //}

        // To seed the database with some data, OnModelCreating can be override
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                    new City("New York City")
                    {
                        Id = 1,
                        Description = "The one with that big park."
                    },
                              
                    new City("Antwerp")
                    {
                        Id = 2,
                        Description = "The one with the cathedral that was never really finished."
                    },

                    new City("Paris")
                    {
                        Id = 3,
                        Description = "The one with that big tower."
                    }
                );

            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                    new PointOfInterest("Central Park")
                    {
                        Id = 1,
                        CityId = 1,
                        Description = "The most visited urban park in the united states."
                    },

                    new PointOfInterest("Empire State Building")
                    {
                        Id = 2,
                        CityId = 1,
                        Description = "A 102-story skyscraper located in Midtown Manhattan"
                    },

                    new PointOfInterest("Cathedral of Our Lady")
                    {
                        Id = 3,
                        CityId = 2,
                        Description = "A Gothic style cathedral, concieved by architects Jan and Piete."
                    },

                    new PointOfInterest("Antwerp Central Station")
                    {
                        Id = 4,
                        CityId = 2,
                        Description = "The finest example of railway architecture in Belgium."
                    },

                    new PointOfInterest("Eiffel Tower")
                    {
                        Id = 5,
                        CityId = 3,
                        Description = "A wrought iron lattice tower on the Champ de Mars, named after."
                    },

                    new PointOfInterest("The Louvre")
                    {
                        Id = 6,
                        CityId = 3,
                        Description = "The world's biggest museum."
                    }
                );

            base.OnModelCreating(modelBuilder);
        }
    }
}
