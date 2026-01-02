namespace ProjectLab.PucCampinas.Features.Users.DTOs
{
    public class UserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Professor";
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Cep { get; set; }
    }
}
