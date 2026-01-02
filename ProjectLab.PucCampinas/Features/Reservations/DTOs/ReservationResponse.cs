namespace ProjectLab.PucCampinas.Features.Reservations.DTOs
{
    public class ReservationResponse
    {
        public Guid Id { get; set; }
        public DateTime ReservationDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public Guid LaboratoryId { get; set; }
        public string LaboratoryName { get; set; } = string.Empty;
    }
}
