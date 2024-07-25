using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces;

public interface IItemRepository
{
    Task<Item?> CreateItemAsync(Item itemModel, int listId); //Add item to list
    Task<Item?> ReadItemByIdAsync(int id);

    Task<(CuratedList?, PaginatedRawItems)> ReadListItemsByListIdAsync(int listId, string userId,
        ItemQueryObject itemQueryObject);

    // Task<PaginatedItemsDto> ReadUserListItemsByListIdAsync(int listId, string userId);
    // Task<PaginatedItemsPublicDto> ReadPublicListItemsByListIdAsync(int listId, string userId);
    Task<Item?> UpdateItemByIdAsync(int id, Item itemModel);
    Task<Item?> DeleteItemByIdAsync(int id);
}