using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.Item;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Repository
{
    public class ItemRepository : IItemRepository
    {
        private readonly ApplicationDBContext _context;
        public ItemRepository(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Item> CreateAsync(Item itemModel)
        {
            await _context.Items.AddAsync(itemModel);
            await _context.SaveChangesAsync();
            return itemModel;
        }

        public async Task<Item?> DeleteAsync(int id)
        {
            var itemModel = await _context.Items.FindAsync(id);
            if (itemModel == null)
            {
                return null;
            }

            _context.Items.Remove(itemModel);
            await _context.SaveChangesAsync();
            return itemModel;
        }

        public async Task<List<Item>> GetAllAsync()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<List<Item>> GetAllItemByListIdAsync(int listId)
        {
            return await _context.Items.Where(x => x.ListId == listId).ToListAsync();
        }

        public async Task<Item?> GetByIdAsync(int id)
        {
            return await _context.Items.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> IsItemOwnedByUser(int itemId, string ownerId)
        {
            var itemModel = await _context.Items
                .Include(i => i.CuratedList) //Include any related lists
                .ThenInclude(list => list.User) //Include any related user to the lists
                .FirstOrDefaultAsync(i => i.Id == itemId); //Filter out only the items with matching id

            //If query is null, item does not exist
            if (itemModel == null)
                return false;

            //If item exist, check if its owned by the user
            return itemModel.CuratedList.OwnerId == ownerId;
        }

        public async Task<Item?> UpdateAsync(int id, Item item)
        {
            var itemModel = await _context.Items.FirstOrDefaultAsync(x => x.Id == id);

            if (itemModel == null)
            {
                return null;
            }

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
    }
}