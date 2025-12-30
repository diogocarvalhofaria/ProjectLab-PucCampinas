namespace ProjectLab.PucCampinas.Common.Models;

public class PaginatedResult<T>
{
    public List<T> Results { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool PreviousPage => CurrentPage > 1;
    public bool NextPage => CurrentPage < TotalPages;
}