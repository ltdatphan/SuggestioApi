using SuggestioApi.Dtos.User;

namespace SuggestioApi.Dtos.Paginated;

public class PaginatedUserDto
{
    public List<UserDto> Users { get; set; } = [];
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}