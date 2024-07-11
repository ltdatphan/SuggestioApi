using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.CuratedList;
using SuggestioApi.Helpers;
using SuggestioApi.Models;

namespace SuggestioApi.Interfaces
{
    public interface ICuratedListRepository
    {
        Task<List<CuratedList>> GetAllAsync(QueryObject queryObject);
        Task<List<CuratedList>> GetAllByOwnerIdAsync(string ownerId);
        Task<CuratedList?> GetByIdAsync(int id, ListQueryObject listQueryObject); //First or default
        Task<CuratedList> CreateAsync(CuratedList curatedListModel);
        Task<CuratedList?> UpdateAsync(int id, CuratedList curatedListModel);
        Task<CuratedList?> DeleteAsync(int id);
        Task<bool> ListExist(int id);
        Task<bool> ListBelongsToOwner(int listId, string ownerId);
        Task<bool> CanUserViewList(int listId, string userId);
    }
}