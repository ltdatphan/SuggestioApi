using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Repository;

public class ItemRepository : IItemRepository
{
    private readonly ApplicationDBContext _context;

    public ItemRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<Item?> CreateItemAsync(Item itemModel, int listId)
    {
        var listModel = await _context.CuratedLists
            .Where(cl => cl.Id == listId)
            .FirstOrDefaultAsync();

        if (listModel == null)
            return null;

        await _context.Items.AddAsync(itemModel);
        await _context.SaveChangesAsync();
        return itemModel;
    }

    // Assume only the user can read a single item details (for editing purposes)
    public async Task<Item?> ReadItemByIdAsync(int id)
    {
        return await _context.Items
            .Include(i => i.CuratedList)
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();
    }

    // Return list items if:
    // - List is public
    // - Current user is the owner (regardless of list visibility)
    public async Task<(CuratedList?, PaginatedRawItems)> ReadListItemsByListIdAsync(int listId, string userId,
        ItemQueryObject itemQueryObject)
    {
        var listModel = await _context.CuratedLists
            .Where(cl => cl.Id == listId)
            .FirstOrDefaultAsync();


        // Return no items if
        // - No list with the provided id
        // - There is a list but the user is not the owner and list is not public
        if (listModel == null || (listModel.OwnerId != userId && !listModel.IsPublic))
            return (listModel, new PaginatedRawItems
            {
                Items = [],
                PageNumber = itemQueryObject.PageNumber,
                PageSize = itemQueryObject.PageSize,
                TotalItems = 0
            });

        // Get items
        var itemModels = _context.Items.Where(i => i.ListId == listId).AsQueryable();

        if (!string.IsNullOrWhiteSpace(itemQueryObject.SortBy))
        {
            //By ItemName
            if (itemQueryObject.SortBy.Equals("ItemName", StringComparison.OrdinalIgnoreCase))
                itemModels = itemQueryObject.IsDescending
                    ? itemModels.OrderByDescending(i => i.ItemName)
                    : itemModels.OrderBy(i => i.ItemName);
            //By CreatedAt
            if (itemQueryObject.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                itemModels = itemQueryObject.IsDescending
                    ? itemModels.OrderByDescending(i => i.CreatedAt)
                    : itemModels.OrderBy(i => i.CreatedAt);
            //By UpdatedAt
            if (itemQueryObject.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                itemModels = itemQueryObject.IsDescending
                    ? itemModels.OrderByDescending(i => i.UpdatedAt)
                    : itemModels.OrderBy(i => i.UpdatedAt);
            //By rating
            if (itemQueryObject.SortBy.Equals("Rating", StringComparison.OrdinalIgnoreCase))
                itemModels = itemQueryObject.IsDescending
                    ? itemModels.OrderByDescending(i => i.Rating)
                    : itemModels.OrderBy(i => i.Rating);
        }

        var skipNumber = (itemQueryObject.PageNumber - 1) * itemQueryObject.PageSize;
        var totalItems = await itemModels.CountAsync();
        var paginatedItems = await itemModels.Skip(skipNumber).Take(itemQueryObject.PageSize).ToListAsync();

        return (listModel, new PaginatedRawItems
        {
            Items = paginatedItems.ToList(),
            PageNumber = itemQueryObject.PageNumber,
            PageSize = itemQueryObject.PageSize,
            TotalItems = totalItems
        });
    }

    public async Task<Item?> UpdateItemByIdAsync(int id, Item item)
    {
        var itemModel = await _context.Items
            .FirstOrDefaultAsync(x => x.Id == id);

        if (itemModel == null) return null;

        itemModel.ItemName = item.ItemName;
        itemModel.Subtitle = item.Subtitle;
        itemModel.Category = item.Category;
        itemModel.ItemImgUrl = item.ItemImgUrl;
        itemModel.ItemUrl = item.ItemUrl;
        itemModel.Rating = item.Rating;
        itemModel.Notes = item.Notes;

        await _context.SaveChangesAsync();

        return itemModel;
    }

    public async Task<Item?> DeleteItemByIdAsync(int id)
    {
        var itemModel = await _context.Items
            .Where(i => i.Id == id)
            .FirstOrDefaultAsync();

        if (itemModel == null) return null;

        _context.Items.Remove(itemModel);
        await _context.SaveChangesAsync();
        return itemModel;
    }
}