using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.CuratedList;
using SuggestioApi.Helpers;
using SuggestioApi.Interfaces;
using SuggestioApi.Models;

namespace SuggestioApi.Repository
{
    public class CuratedListRepository : ICuratedListRepository
    {
        private readonly ApplicationDBContext _context;
        public CuratedListRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<CuratedList>> GetAllAsync(QueryObject queryObject)
        {
            var lists = _context.CuratedLists.Include(l => l.Items).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.CompanyName))
            {
                lists = lists.Where(l => l.Title == queryObject.CompanyName);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
            {
                lists = lists.Where(l => l.Title == queryObject.Symbol);
            }

            if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
            {
                if (queryObject.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
                {
                    lists = queryObject.IsDescending ? lists.OrderByDescending(s => s.Title) : lists.OrderBy(s => s.Title);
                }
            }

            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;


            return await lists.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
        }

        public async Task<List<CuratedList>> GetAllByOwnerIdAsync(string ownerId)
        {
            return await _context.CuratedLists
            .Where(l => l.OwnerId == ownerId)
            .OrderByDescending(l => l.UpdatedAt)
            .ToListAsync();
        }

        public async Task<CuratedList?> GetByIdAsync(int id, ListQueryObject listQueryObject)
        {
            var listDetails = _context.CuratedLists.AsQueryable();

            if (listQueryObject.WithItems)
            {
                listDetails = listDetails.Include(cl => cl.Items);
            }

            listDetails = listDetails.Where(x => x.Id == id);

            return await listDetails.FirstOrDefaultAsync();

            // return await listDetails.FirstOrDefaultAsync();
        }

        public async Task<CuratedList> CreateAsync(CuratedList curatedListModel)
        {
            await _context.CuratedLists.AddAsync(curatedListModel);
            await _context.SaveChangesAsync();
            return curatedListModel;
        }

        public async Task<CuratedList?> UpdateAsync(int id, CuratedList curatedList)
        {
            var curatedListModel = await _context.CuratedLists.FirstOrDefaultAsync(x => x.Id == id);

            if (curatedListModel == null)
            {
                return null;
            }

            curatedListModel.Title = curatedList.Title;
            curatedListModel.Subtitle = curatedList.Subtitle;
            // curatedListModel.OwnerId = curatedList.OwnerId;
            curatedListModel.IsPublic = curatedList.IsPublic;
            curatedListModel.ListType = curatedList.ListType;
            curatedListModel.CoverImgUrl = curatedList.CoverImgUrl;

            await _context.SaveChangesAsync();

            return curatedListModel;
        }

        public async Task<CuratedList?> DeleteAsync(int id)
        {
            var curatedListModel = await _context.CuratedLists.FirstOrDefaultAsync(x => x.Id == id);

            if (curatedListModel == null)
            {
                return null;
            }

            _context.CuratedLists.Remove(curatedListModel);
            await _context.SaveChangesAsync();
            return curatedListModel;
        }

        public async Task<bool> ListExist(int id)
        {
            return await _context.CuratedLists.AnyAsync(l => l.Id == id);
        }

        public async Task<bool> ListBelongsToOwner(int listId, string ownerId)
        {
            return await _context.CuratedLists.AnyAsync(l => l.Id == listId && l.OwnerId == ownerId);
        }

        public async Task<bool> CanUserViewList(int listId, string userId)
        {
            var listModel = await _context.CuratedLists.FirstOrDefaultAsync(l => l.Id == listId && l.IsPublic == true);

            if (listModel == null)
                return false;

            return await _context.Follows.AnyAsync(fl => fl.CurrentUserId == userId && fl.TargetUserId == listModel.OwnerId);

            // return await _context.CuratedLists.AnyAsync(l => l.Id == listId && l.OwnerId == userId);
        }
    }
}