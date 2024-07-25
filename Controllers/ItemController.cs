using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using SuggestioApi.Dtos.Item;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;

namespace SuggestioApi.Controllers;

[EnableRateLimiting("fixed")]
[Route("api/items")]
[ApiController]
public class ItemController : ControllerBase
{
    private readonly IItemRepository _itemRepo;

    public ItemController(IItemRepository itemRepo)
    {
        _itemRepo = itemRepo;
    }

    //? For now, only the owner can view the item details
    [Authorize(Policy = "ItemOwner")]
    [HttpGet("{itemId:int}")]
    public async Task<IActionResult> GetItemById([FromRoute] int itemId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var item = await _itemRepo.ReadItemByIdAsync(itemId);
        if (item == null) return NotFound("Item not found or you do not have permission to view it.");
        return Ok(item.ToItemDto());
    }

    [Authorize(Policy = "ItemOwner")]
    //[ValidateAntiForgeryToken]
    [HttpPut("{itemId:int}")]
    public async Task<IActionResult> UpdateItem([FromRoute] int itemId, [FromBody] UpdateItemRequestDto updateDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var itemModel = await _itemRepo.UpdateItemByIdAsync(itemId, updateDto.ToItemFromUpdateDto());
        if (itemModel == null) return NotFound("Item not found or you do not have permission to update it.");

        return Ok(itemModel.ToItemDto());
    }

    [Authorize(Policy = "ItemOwner")]
    //[ValidateAntiForgeryToken]
    [HttpDelete("{itemId:int}")]
    public async Task<IActionResult> DeleteItem([FromRoute] int itemId)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var listModel = await _itemRepo.DeleteItemByIdAsync(itemId);
        if (listModel == null) return NotFound("Item not found or you do not have permission to delete it.");

        return NoContent();
    }
}