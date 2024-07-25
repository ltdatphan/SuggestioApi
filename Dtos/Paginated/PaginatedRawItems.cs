namespace SuggestioApi.Dtos.Paginated;

public class PaginatedRawItems
{
    //NOTE: THIS contains raw DATA from item
    //Requires transformation to DTO before sending back
    public List<Models.Item> Items { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}