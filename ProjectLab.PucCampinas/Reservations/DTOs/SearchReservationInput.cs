using ProjectLab.PucCampinas.shared.DTOs;

namespace ProjectLab.PucCampinas.Reservations.DTOs
{
    public class SearchReservationInput : BasePaginationInput
    {
        public string? Keyword { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public string Order { get; init; } = "DESC";
    }
}
