using ProjectLab.PucCampinas.Features.Laboratories.Model;
using ProjectLab.PucCampinas.Features.Users.Model;
using System.ComponentModel.DataAnnotations;

namespace ProjectLab.PucCampinas.Features.Reservations.Model
{
    public class Reservation
    {
        [Required]
        public Guid Id { get; set; } = Guid.NewGuid();
        [Required]
        public Guid LaboratoryId { get; set; }
        [Required]
        public Laboratory? Laboratory { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        [Required]
        public DateTime ReservationDate { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

    }
}
