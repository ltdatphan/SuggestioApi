using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SuggestioApi.Dtos.CuratedList;
using SuggestioApi.Dtos.Item;
using SuggestioApi.Dtos.Paginated;
using SuggestioApi.Helpers;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;

namespace SuggestioApi.Controllers;

[EnableRateLimiting("fixed")]
[Route("api/lists")]
[ApiController]
public class ListController : ControllerBase
{
    private readonly ICuratedListRepository _curatedListRepo;
    private readonly IItemRepository _itemRepo;

    public ListController(ICuratedListRepository listRepo, IItemRepository itemRepo)
    {
        _curatedListRepo = listRepo;
        _itemRepo = itemRepo;
    }

    // Only the list info
    [Authorize]
    [HttpGet("{listId:int}")]
    public async Task<IActionResult> GetListById([FromRoute] int listId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("You do not have permission to view this list.");

        var listModel = await _curatedListRepo.ReadListByIdAsync(listId);

        if (listModel == null) return NotFound("List not found or you do not have permission to view it.");

        var isOwner = listModel.OwnerId == userId;

        //If another user tries to view private list
        if (!isOwner && !listModel.IsPublic)
            return NotFound("List not found or you do not have permission to view it.");

        // Return data for owner
        if (isOwner)
            return Ok(listModel.ToListDto());
        return Ok(listModel.ToListPublicDto());
    }

    [Authorize]
    [HttpGet("search")]
    public async Task<IActionResult> SearchList([FromQuery] ListsSearchQueryObject listsSearchQueryObject,
        [FromQuery] ListQueryObject listQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var paginatedListsPublicDto =
            await _curatedListRepo.SearchListsAsync(listsSearchQueryObject.Query, listQueryObject);

        return Ok(paginatedListsPublicDto);
    }


    [Authorize]
    [HttpGet("{listId:int}/items")]
    public async Task<IActionResult> GetListItemsByListId([FromRoute] int listId,
        [FromQuery] ItemQueryObject itemQueryObject)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        //Check for current user in claims
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("You do not have permission to view this list.");

        var (listModel, paginatedRawItem) = await _itemRepo.ReadListItemsByListIdAsync(listId, userId, itemQueryObject);

        if (listModel == null) return NotFound("List not found or you do not have permission to view it.");

        var isOwner = listModel.OwnerId == userId;

        if (!isOwner && !listModel.IsPublic)
            return NotFound("List not found or you do not have permission to view it.");

        if (isOwner)
        {
            var paginatedItems = new PaginatedItemsDto
            {
                Items = paginatedRawItem.Items.Select(i => i.ToItemDto()).ToList(),
                PageNumber = paginatedRawItem.PageNumber,
                PageSize = paginatedRawItem.PageSize,
                TotalItems = paginatedRawItem.TotalItems
            };

            return Ok(paginatedItems);
        }

        var paginatedPublicItems = new PaginatedItemsPublicDto
        {
            Items = paginatedRawItem.Items.Select(i => i.ToItemPublicDto()).ToList(),
            PageNumber = paginatedRawItem.PageNumber,
            PageSize = paginatedRawItem.PageSize,
            TotalItems = paginatedRawItem.TotalItems
        };
        return Ok(paginatedPublicItems);
    }

    [Authorize(Policy = "ListOwner")]
    //[ValidateAntiForgeryToken]
    [HttpPut("{listId:int}")]
    public async Task<IActionResult> UpdateList([FromRoute] int listId,
        [FromBody] UpdateCuratedListRequestDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Perform the update if the user is the owner
        var listModel = await _curatedListRepo.UpdateListAsync(listId, updateDto.ToListFromUpdateDto());

        if (listModel == null) return NotFound("List not found or you do not have permission to edit it.");

        return Ok(listModel.ToListDto());
    }

    [Authorize(Policy = "ListOwner")]
    //[ValidateAntiForgeryToken]
    [HttpDelete("{listId:int}")]
    public async Task<IActionResult> DeleteList([FromRoute] int listId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Perform the delete
        var listModel = await _curatedListRepo.DeleteListAsync(listId);

        if (listModel == null) return NotFound("List not found.");

        return NoContent();
    }

    [Authorize(Policy = "ListOwner")]
    //[ValidateAntiForgeryToken]
    [HttpPost("{listId:int}/items")]
    public async Task<IActionResult> AddItemToList([FromRoute] int listId, [FromBody] CreateItemRequestDto createDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        //Perform action
        var itemModel = createDto.ToItemFromCreateDto(listId);
        var resultItemModel = await _itemRepo.CreateItemAsync(itemModel, listId);

        if (resultItemModel == null)
            return NotFound("List not found or you do not have permission to add items to it.");

        return Ok(resultItemModel.ToItemDto());
    }
}