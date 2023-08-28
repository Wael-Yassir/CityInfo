namespace CityInfo.API.Models.Authentication
{
    public class AuthenticationDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? City { get; set; }
        public string? Token { get; set; }
    }
}
