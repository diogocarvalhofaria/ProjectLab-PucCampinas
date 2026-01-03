using ProjectLab.PucCampinas.shared.DTOs;

namespace ProjectLab.PucCampinas.Features.Users.DTOs
{
    public class SearchUserInput : BasePaginationInput
    {
        public string? Keyword { get; init; }
        public string Order { get; init; } = "DESC";
    }
}
