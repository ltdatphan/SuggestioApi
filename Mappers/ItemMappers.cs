using SuggestioApi.Dtos.Item;
using SuggestioApi.Models;

namespace SuggestioApi.Mappers;

public static class ItemMappers
{
    public static ItemDto ToItemDto(this Item itemModel)
    {
        return new ItemDto
        {
            Id = itemModel.Id,
            ItemName = itemModel.ItemName,
            ListId = itemModel.ListId,
            Subtitle = itemModel.Subtitle,
            Category = itemModel.Category,
            ItemImgUrl = itemModel.ItemImgUrl,
            ItemUrl = itemModel.ItemUrl,
            Rating = itemModel.Rating,
            Notes = itemModel.Notes,
            CreatedAt = itemModel.CreatedAt,
            UpdatedAt = itemModel.UpdatedAt
        };
    }

    public static ItemPublicDto ToItemPublicDto(this Item itemModel)
    {
        return new ItemPublicDto
        {
            Id = itemModel.Id,
            ItemName = itemModel.ItemName,
            Subtitle = itemModel.Subtitle,
            Category = itemModel.Category,
            ItemImgUrl = itemModel.ItemImgUrl,
            ItemUrl = itemModel.ItemUrl,
            Rating = itemModel.Rating,
            Notes = itemModel.Notes,
            CreatedAt = itemModel.CreatedAt,
            UpdatedAt = itemModel.UpdatedAt
        };
    }

    public static Item ToItemFromCreateDto(this CreateItemRequestDto itemDto, int listId)
    {
        return new Item
        {
            ItemName = itemDto.ItemName,
            ListId = listId,
            Subtitle = itemDto.Subtitle,
            Category = itemDto.Category,
            ItemImgUrl = itemDto.ItemImgUrl,
            ItemUrl = itemDto.ItemUrl,
            Rating = itemDto.Rating,
            Notes = itemDto.Notes
        };
    }

    public static Item ToItemFromUpdateDto(this UpdateItemRequestDto itemDto)
    {
        return new Item
        {
            ItemName = itemDto.ItemName,
            Subtitle = itemDto.Subtitle,
            Category = itemDto.Category,
            ItemImgUrl = itemDto.ItemImgUrl,
            ItemUrl = itemDto.ItemUrl,
            Rating = itemDto.Rating,
            Notes = itemDto.Notes
        };
    }
}