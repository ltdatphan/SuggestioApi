using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SuggestioApi.Data;
using SuggestioApi.Dtos.CuratedList;
using SuggestioApi.Helpers;
using SuggestioApi.Interfaces;
using SuggestioApi.Mappers;

namespace SuggestioApi.Controllers
{
    [Route("api/lists")]
    [ApiController]
    public class ListController : ControllerBase
    {
        // private readonly ApplicationDBContext _context;
        private readonly ICuratedListRepository _curatedListRepo;
        public ListController(ICuratedListRepository listRepo)
        {
            // _context = context;
            _curatedListRepo = listRepo;
        }

        // TODO: REMOVE THIS
        // [Authorize]
        // [HttpGet("lists")]
        // public async Task<IActionResult> GetAll([FromQuery] QueryObject queryObject)
        // {
        //     if (!ModelState.IsValid)
        //         return BadRequest(ModelState);

        //     var listModels = await _curatedListRepo.GetAllAsync(queryObject);

        //     return Ok(listModels.Select(l => l.ToListDto()));
        // }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetListById([FromRoute] int id, [FromQuery] ListQueryObject listQueryObject)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to view this list.");

            // Check if current list is owned by user or if user has permission to view list
            var isListBelongToOwner = await _curatedListRepo.ListBelongsToOwner(id, userId);
            var canUserViewList = await _curatedListRepo.CanUserViewList(id, userId);

            if (!isListBelongToOwner && !canUserViewList)
                return Unauthorized("You do not have permission to view this list.");

            var list = await _curatedListRepo.GetByIdAsync(id, listQueryObject);
            if (list == null)
            {
                return NotFound();
            }

            if (listQueryObject.WithItems)
                return Ok(list.ToCuratedListWithItemsDto());

            else
                return Ok(list.ToListDto());
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCurrentUserLists()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to view this list.");

            var list = await _curatedListRepo.GetAllByOwnerIdAsync(userId);
            return Ok(list.Select(l => l.ToBasicListDto()));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateList([FromBody] CreateCuratedListRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to create a list.");

            var listModel = createDto.ToListFromCreateDto(userId);
            await _curatedListRepo.CreateAsync(listModel);
            return CreatedAtAction(nameof(GetListById), new { id = listModel.Id }, listModel.ToListDto());
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateList([FromRoute] int id, [FromBody] UpdateCuratedListRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to edit this list.");

            // Check if current list belongs to owner
            var isListBelongToOwner = await _curatedListRepo.ListBelongsToOwner(id, userId);

            if (!isListBelongToOwner)
                return Unauthorized("You do not have permission to edit this list.");

            // Perform the update
            var listModel = await _curatedListRepo.UpdateAsync(id, updateDto.ToListFromUpdateDto());

            if (listModel == null)
            {
                return NotFound();
            }

            return Ok(listModel.ToListDto());
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteList([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //Check for current user in claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
                return Unauthorized("You do not have permission to delete this list.");

            // Check if current list belongs to owner
            var isListBelongToOwner = await _curatedListRepo.ListBelongsToOwner(id, userId);

            if (!isListBelongToOwner)
                return Unauthorized("You do not have permission to delete this list.");

            // Perform the delete
            var listModel = await _curatedListRepo.DeleteAsync(id);

            if (listModel == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}