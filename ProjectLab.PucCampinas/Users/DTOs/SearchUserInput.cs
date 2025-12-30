using ProjectLab.PucCampinas.shared.DTOs;

namespace ProjectLab.PucCampinas.Users.DTOs
{
    public class SearchUserInput : BasePaginationInput
    {
        public string? Keyword { get; init; }
        public string Order { get; init; } = "DESC";
    }
}
