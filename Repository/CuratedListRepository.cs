using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;
using SuggestioApi.Models;

namespace SuggestioApi.Repository;

public class CuratedListRepository : ICuratedListRepository
{
    private readonly ApplicationDBContext _context;

    public CuratedListRepository(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<CuratedList> CreateListAsync(CuratedList curatedListModel)
    {
        await _context.CuratedLists.AddAsync(curatedListModel);
        await _context.SaveChangesAsync();
        return curatedListModel;
    }

    public async Task<PaginatedListsDto> ReadUserListsPaginatedAsync(string userId, ListQueryObject listsQueryObject)
    {
        var listModels = _context.CuratedLists.Include(cl => cl.Items).AsQueryable();
        listModels = listModels.Where(cl => cl.OwnerId == userId);

        //Public private filtering
        if (!string.IsNullOrWhiteSpace(listsQueryObject.Access))
        {
            if (listsQueryObject.Access.Equals("Public", StringComparison.OrdinalIgnoreCase))
                listModels = listModels.Where(l => l.IsPublic);

            if (listsQueryObject.Access.Equals("Private", StringComparison.OrdinalIgnoreCase))
                listModels = listModels.Where(l => !l.IsPublic);
        }

        //Sorting
        if (!string.IsNullOrWhiteSpace(listsQueryObject.SortBy))
        {
            //By Title
            if (listsQueryObject.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Title)
                    : listModels.OrderBy(l => l.Title);
            //By CreatedAt
            if (listsQueryObject.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.CreatedAt)
                    : listModels.OrderBy(l => l.CreatedAt);
            //By UpdatedAt
            if (listsQueryObject.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.UpdatedAt)
                    : listModels.OrderBy(l => l.UpdatedAt);
            //By ItemCount
            if (listsQueryObject.SortBy.Equals("ItemCount", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Items.Count)
                    : listModels.OrderBy(l => l.Items.Count);
        }

        var skipNumber = (listsQueryObject.PageNumber - 1) * listsQueryObject.PageSize;
        var totalItems = await listModels.CountAsync();
        var paginatedLists = await listModels.Skip(skipNumber).Take(listsQueryObject.PageSize).ToListAsync();

        return new PaginatedListsDto
        {
            Lists = paginatedLists.Select(l => l.ToListDto()).ToList(),
            PageNumber = listsQueryObject.PageNumber,
            PageSize = listsQueryObject.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<PaginatedListsPublicDto> ReadUserPublicListsAsync(string userId, ListQueryObject listsQueryObject)
    {
        var listModels = _context.CuratedLists.Include(cl => cl.User).Include(cl => cl.Items).AsQueryable();
        //Lists have to be public
        listModels = listModels.Where(cl => cl.OwnerId == userId && cl.IsPublic);

        if (!string.IsNullOrWhiteSpace(listsQueryObject.SortBy))
        {
            //By Title
            if (listsQueryObject.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Title)
                    : listModels.OrderBy(l => l.Title);
            //By CreatedAt
            if (listsQueryObject.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.CreatedAt)
                    : listModels.OrderBy(l => l.CreatedAt);
            //By UpdatedAt
            if (listsQueryObject.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.UpdatedAt)
                    : listModels.OrderBy(l => l.UpdatedAt);
            //By ItemCount
            if (listsQueryObject.SortBy.Equals("ItemCount", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Items.Count)
                    : listModels.OrderBy(l => l.Items.Count);
        }

        var skipNumber = (listsQueryObject.PageNumber - 1) * listsQueryObject.PageSize;
        var totalItems = await listModels.CountAsync();
        var paginatedLists = await listModels.Skip(skipNumber).Take(listsQueryObject.PageSize).ToListAsync();

        return new PaginatedListsPublicDto
        {
            Lists = paginatedLists.Select(l => l.ToListPublicDto()).ToList(),
            PageNumber = listsQueryObject.PageNumber,
            PageSize = listsQueryObject.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<CuratedList?> ReadListByIdAsync(int id)
    {
        var listDetails = await _context.CuratedLists
            .Include(cl => cl.User)
            .Include(cl => cl.Items)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
        return listDetails;
    }

    public async Task<CuratedList?> ReadListWithItemsByIdAsync(int id)
    {
        var listDetails = await _context.CuratedLists
            .Include(cl => cl.Items)
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();

        return listDetails;
    }

    public async Task<PaginatedListsPublicDto> ReadFollowingsListsAsync(string userId, ListQueryObject listsQueryObject)
    {
        var targetUserIds = await _context.Follows
            .Where(fl => fl.CurrentUserId == userId)
            .Select(fl => fl.TargetUserId)
            .ToListAsync();

        if (targetUserIds.Count == 0)
            return new PaginatedListsPublicDto
            {
                Lists = [],
                PageNumber = listsQueryObject.PageNumber,
                PageSize = listsQueryObject.PageSize,
                TotalItems = 0
            };

        var listModels = _context.CuratedLists.Include(curList => curList.User)
            .Include(curList => curList.Items).AsQueryable();
        // IMPORTANT: the lists have to be public
        listModels = listModels.Where(curList => targetUserIds.Contains(curList.OwnerId) && curList.IsPublic);

        //Sorting
        if (!string.IsNullOrWhiteSpace(listsQueryObject.SortBy))
        {
            //By Title
            if (listsQueryObject.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Title)
                    : listModels.OrderBy(l => l.Title);
            //By CreatedAt
            if (listsQueryObject.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.CreatedAt)
                    : listModels.OrderBy(l => l.CreatedAt);
            //By UpdatedAt
            if (listsQueryObject.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.UpdatedAt)
                    : listModels.OrderBy(l => l.UpdatedAt);
            //By ItemCount
            if (listsQueryObject.SortBy.Equals("ItemCount", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Items.Count)
                    : listModels.OrderBy(l => l.Items.Count);
        }

        var skipNumber = (listsQueryObject.PageNumber - 1) * listsQueryObject.PageSize;
        var totalItems = await listModels.CountAsync();
        var paginatedLists = await listModels.Skip(skipNumber).Take(listsQueryObject.PageSize).ToListAsync();

        return new PaginatedListsPublicDto
        {
            Lists = paginatedLists.Select(l => l.ToListPublicDto()).ToList(),
            PageNumber = listsQueryObject.PageNumber,
            PageSize = listsQueryObject.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<CuratedList?> UpdateListAsync(int id, CuratedList curatedList)
    {
        var curatedListModel = await _context.CuratedLists
            .Include(cl => cl.Items)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (curatedListModel == null) return null;

        curatedListModel.Title = curatedList.Title;
        curatedListModel.Subtitle = curatedList.Subtitle;
        curatedListModel.IsPublic = curatedList.IsPublic;
        curatedListModel.ListType = curatedList.ListType;
        curatedListModel.CoverImgUrl = curatedList.CoverImgUrl;

        await _context.SaveChangesAsync();

        return curatedListModel;
    }

    public async Task<CuratedList?> DeleteListAsync(int id)
    {
        var curatedListModel = await _context.CuratedLists.FirstOrDefaultAsync(x => x.Id == id);

        if (curatedListModel == null) return null;

        _context.CuratedLists.Remove(curatedListModel);
        await _context.SaveChangesAsync();
        return curatedListModel;
    }

    public async Task<PaginatedListsPublicDto> SearchListsAsync(string query, ListQueryObject listsQueryObject)
    {
        var listModels = _context.CuratedLists.Include(cl => cl.User).Include(cl => cl.Items).AsQueryable();
        //Lists have to be public
        listModels = listModels.Where(cl => cl.IsPublic);

        listModels = listModels.Where(cl => cl.Title.ToLower().Contains(query.ToLower()));

        if (!string.IsNullOrWhiteSpace(listsQueryObject.SortBy))
        {
            //By Title
            if (listsQueryObject.SortBy.Equals("Title", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Title)
                    : listModels.OrderBy(l => l.Title);
            //By CreatedAt
            if (listsQueryObject.SortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.CreatedAt)
                    : listModels.OrderBy(l => l.CreatedAt);
            //By UpdatedAt
            if (listsQueryObject.SortBy.Equals("UpdatedAt", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.UpdatedAt)
                    : listModels.OrderBy(l => l.UpdatedAt);
            //By ItemCount
            if (listsQueryObject.SortBy.Equals("ItemCount", StringComparison.OrdinalIgnoreCase))
                listModels = listsQueryObject.IsDescending
                    ? listModels.OrderByDescending(l => l.Items.Count)
                    : listModels.OrderBy(l => l.Items.Count);
        }

        var skipNumber = (listsQueryObject.PageNumber - 1) * listsQueryObject.PageSize;
        var totalItems = await listModels.CountAsync();
        var paginatedLists = await listModels.Skip(skipNumber).Take(listsQueryObject.PageSize).ToListAsync();

        return new PaginatedListsPublicDto
        {
            Lists = paginatedLists.Select(l => l.ToListPublicDto()).ToList(),
            PageNumber = listsQueryObject.PageNumber,
            PageSize = listsQueryObject.PageSize,
            TotalItems = totalItems
        };
    }


    // public async Task<List<CuratedList>> GetAllAsync(QueryObject queryObject)
    // {
    //     var lists = _context.CuratedLists.Include(l => l.Items).AsQueryable();

    //     if (!string.IsNullOrWhiteSpace(queryObject.CompanyName))
    //     {
    //         lists = lists.Where(l => l.Title == queryObject.CompanyName);
    //     }

    //     if (!string.IsNullOrWhiteSpace(queryObject.Symbol))
    //     {
    //         lists = lists.Where(l => l.Title == queryObject.Symbol);
    //     }

    //     if (!string.IsNullOrWhiteSpace(queryObject.SortBy))
    //     {
    //         if (queryObject.SortBy.Equals("Symbol", StringComparison.OrdinalIgnoreCase))
    //         {
    //             lists = queryObject.IsDescending ? lists.OrderByDescending(s => s.Title) : lists.OrderBy(s => s.Title);
    //         }
    //     }

    //     var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;


    //     return await lists.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();
    // }
}