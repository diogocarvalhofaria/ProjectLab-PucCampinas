namespace ProjectLab.PucCampinas.Features.Reservations.DTOs
{
    public class ReservationRequest
    {
        public Guid UserId { get; set; }
        public Guid LaboratoryId { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
