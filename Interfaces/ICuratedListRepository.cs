using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces;

public interface ICuratedListRepository
{
    Task<PaginatedListsDto> ReadUserListsPaginatedAsync(string userId, ListQueryObject listsQueryObject);
    Task<PaginatedListsPublicDto> ReadUserPublicListsAsync(string userId, ListQueryObject listsQueryObject);
    Task<CuratedList?> ReadListByIdAsync(int id);
    Task<CuratedList?> ReadListWithItemsByIdAsync(int id);
    Task<CuratedList> CreateListAsync(CuratedList curatedListModel);
    Task<CuratedList?> UpdateListAsync(int id, CuratedList curatedListModel);
    Task<CuratedList?> DeleteListAsync(int id);
    Task<PaginatedListsPublicDto> ReadFollowingsListsAsync(string userId, ListQueryObject listsQueryObject);
    Task<PaginatedListsPublicDto> SearchListsAsync(string query, ListQueryObject listsQueryObject);
}