using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuggestioApi.Dtos.Item;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;

namespace SuggestioApi.Controllers
{
    [Route("api")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ICuratedListRepository _listRepo;
        private readonly IItemRepository _itemRepo;

        public ItemController(IItemRepository itemRepo, ICuratedListRepository listRepo)
        {
            _itemRepo = itemRepo;
            _listRepo = listRepo;
        }

        //Note sure about this?
        [HttpGet("items")]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var itemModels = await _itemRepo.GetAllAsync();

            return Ok(itemModels.Select(i => i.ToItemDto()));
        }

        //Not sure about this?
        [HttpGet("items/{id:int}")]
        public async Task<IActionResult> GetItemById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to view this item.");

            //TODO: CHECK IF ITEM BELONGS TO OWNER

            var item = await _itemRepo.GetByIdAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return Ok(item.ToItemDto());
        }

        [Authorize]
        [HttpGet("lists/{listId:int}/items")]
        public async Task<IActionResult> GetAllItemsInList([FromRoute] int listId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to view this list.");

            // Check if current list is owned by user
            var isListBelongToOwner = await _listRepo.ListBelongsToOwner(listId, userId);

            if (!isListBelongToOwner)
                return Unauthorized("You do not have permission to view this list.");

            //Perform action
            var itemModels = await _itemRepo.GetAllItemByListIdAsync(listId);

            if (itemModels == null)
            {
                return NotFound();
            }

            return Ok(itemModels.Select(i => i.ToItemDto()));
        }

        [Authorize]
        [HttpPost("lists/{listId:int}/items")]
        public async Task<IActionResult> AddItemToList([FromRoute] int listId, [FromBody] CreateItemRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to add this item to the list provided.");

            // Check if current list is owned by user
            var isListBelongToOwner = await _listRepo.ListBelongsToOwner(listId, userId);

            if (!isListBelongToOwner)
                return Unauthorized("You do not have permission to add this item to the list provided.");

            //Perform action
            var itemModel = createDto.ToItemFromCreateDto(listId);
            await _itemRepo.CreateAsync(itemModel);

            return CreatedAtAction(nameof(GetItemById), new { id = itemModel.Id }, itemModel.ToItemDto());
        }

        [Authorize]
        [HttpPut("items/{id:int}")]
        public async Task<IActionResult> UpdateItem([FromRoute] int id, [FromBody] UpdateItemRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to update this item.");

            //Check if current list is owned by user
            var isItemOwnedByUser = await _itemRepo.IsItemOwnedByUser(id, userId);

            if (!isItemOwnedByUser)
                return Unauthorized("You do not have permission to update this item.");

            // Perform action
            var itemModel = await _itemRepo.UpdateAsync(id, updateDto.ToItemFromUpdateDto());

            if (itemModel == null)
            {
                return NotFound();
            }

            return Ok(itemModel.ToItemDto());
        }

        [Authorize]
        [HttpDelete("items/{id:int}")]
        public async Task<IActionResult> DeleteItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to delete this item.");

            //Check if current list is owned by user
            var isItemOwnedByUser = await _itemRepo.IsItemOwnedByUser(id, userId);

            if (!isItemOwnedByUser)
                return Unauthorized("You do not have permission to delete this item.");

            //Perform action
            var listModel = await _itemRepo.DeleteAsync(id);

            if (listModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}