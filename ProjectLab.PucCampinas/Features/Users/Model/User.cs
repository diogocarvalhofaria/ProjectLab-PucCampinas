using ProjectLab.PucCampinas.Features.Reservations.Model;
using System.ComponentModel.DataAnnotations;

namespace ProjectLab.PucCampinas.Features.Users.Model
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Role { get; set; } = "Professor";
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
