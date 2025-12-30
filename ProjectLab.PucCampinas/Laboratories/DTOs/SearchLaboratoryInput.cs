using ProjectLab.PucCampinas.shared.DTOs;

namespace ProjectLab.PucCampinas.Laboratories.DTOs
{
    public class SearchLaboratoryInput : BasePaginationInput
    {
        public string? Keyword { get; init; }
        public string? Building { get; init; }
        public string Order { get; init; } = "DESC";
    }
}
