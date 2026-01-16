namespace ProjectLab.PucCampinas.Features.Auth.DTOs
{
    public class LoginResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Ra { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
