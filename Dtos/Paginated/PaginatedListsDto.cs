using SuggestioApi.Dtos.CuratedList;

namespace SuggestioApi.Dtos.Paginated;

public class PaginatedListsDto
{
    public List<CuratedListDto> Lists { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}