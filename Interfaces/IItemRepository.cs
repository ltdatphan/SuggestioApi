using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.Item;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces
{
    public interface IItemRepository
    {
        Task<List<Item>> GetAllAsync();
        Task<List<Item>> GetAllItemByListIdAsync(int listId);
        Task<Item?> GetByIdAsync(int id); //First or default
        Task<Item> CreateAsync(Item itemModel); //Add item to list
        Task<Item?> UpdateAsync(int id, Item itemModel);
        Task<Item?> DeleteAsync(int id);
        Task<bool> IsItemOwnedByUser(int itemId, string ownerId);
    }
}