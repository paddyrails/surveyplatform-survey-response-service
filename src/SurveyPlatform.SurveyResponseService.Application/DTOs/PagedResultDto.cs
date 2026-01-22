namespace SurveyPlatform.SurveyResponseService.Application.DTOs;

public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    
    public PagedResultDto() { }
    
    public PagedResultDto(IReadOnlyList<T> items, int totalCount, int page, int pageSize, int totalPages)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
        TotalPages = totalPages;
    }
}
