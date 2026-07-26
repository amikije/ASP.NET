namespace TmsApi.Application.Dtos;

public class PagedRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public string? OrderBy { get; set; }

    public bool Descending { get; set; }
}