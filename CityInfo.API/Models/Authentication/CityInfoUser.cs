namespace CityInfo.API.Models.Authentication
{
    public class CityInfoUser
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string City { get; set; }

        public CityInfoUser(
            int id, 
            string userName, 
            string firstname, 
            string lastname, 
            string city)
        {
            Id = id;
            UserName = userName;
            Firstname = firstname;
            Lastname = lastname;
            City = city;
        }
    }
}
