using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityInfo.API.Entities
{
    public class PointOfInterest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        // By convention, this is a navigation property resulting a foreign key of CityId.
        [ForeignKey("CityId")]
        public City? City { get; set; }
        public int CityId { get; set; }     // not required to be added.

        public PointOfInterest(string name)
        {
            Name = name;
        }
    }
}
