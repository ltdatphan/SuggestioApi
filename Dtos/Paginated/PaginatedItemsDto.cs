using SuggestioApi.Dtos.Item;

namespace SuggestioApi.Dtos.Paginated;

public class PaginatedItemsDto
{
    public List<ItemDto> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}