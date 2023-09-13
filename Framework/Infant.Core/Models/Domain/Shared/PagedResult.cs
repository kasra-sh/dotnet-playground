namespace Infant.Core.Models.Domain.Shared;

public class PagedResultDto<T>
{
    public int PageNum { get; set; }
    public int PageSize { get; set; }
    public int? TotalCount { get; set; }
    public ICollection<T> Items { get; set; }
}