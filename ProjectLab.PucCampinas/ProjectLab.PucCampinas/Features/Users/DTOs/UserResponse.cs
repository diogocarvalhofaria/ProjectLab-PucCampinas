namespace ProjectLab.PucCampinas.Features.Users.DTOs
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "Professor";
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Cep { get; set; }
        public string? Logradouro { get; set; }
        public string? Bairro { get; set; }
        public string? Cidade { get; set; }
        public string? Estado { get; set; }
    }
}
