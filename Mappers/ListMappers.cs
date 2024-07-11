using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuggestioApi.Dtos.CuratedList;
using SuggestioApi.Models;

namespace SuggestioApi.Mappers
{
    public static class ListMappers
    {
        public static CuratedListDto ToListDto(this CuratedList listModel)
        {
            return new CuratedListDto
            {
                Id = listModel.Id,
                Title = listModel.Title,
                Subtitle = listModel.Subtitle,
                OwnerId = listModel.OwnerId,
                IsPublic = listModel.IsPublic,
                ListType = listModel.ListType,
                CoverImgUrl = listModel.CoverImgUrl,
                CreatedAt = listModel.CreatedAt,
                UpdatedAt = listModel.UpdatedAt,
            };
        }

        public static BasicCuratedListDto ToBasicListDto(this CuratedList listModel)
        {
            return new BasicCuratedListDto
            {
                Id = listModel.Id,
                Title = listModel.Title,
                // Subtitle = listModel.Subtitle,
                // OwnerId = listModel.OwnerId,
                IsPublic = listModel.IsPublic,
                // ListType = listModel.ListType,
                CoverImgUrl = listModel.CoverImgUrl,
                // CreatedAt = listModel.CreatedAt,
                // UpdatedAt = listModel.UpdatedAt,
            };
        }

        public static CuratedListWithItemsDto ToCuratedListWithItemsDto(this CuratedList listModel)
        {
            return new CuratedListWithItemsDto
            {
                Id = listModel.Id,
                Title = listModel.Title,
                Subtitle = listModel.Subtitle,
                OwnerId = listModel.OwnerId,
                IsPublic = listModel.IsPublic,
                ListType = listModel.ListType,
                CoverImgUrl = listModel.CoverImgUrl,
                CreatedAt = listModel.CreatedAt,
                UpdatedAt = listModel.UpdatedAt,
                ListItems = listModel.Items.Select(l => l.ToItemDto()).ToList()
            };
        }

        public static CuratedListWithUserDto ToCuratedListWithUserDto(this CuratedList listModel)
        {
            return new CuratedListWithUserDto
            {
                Id = listModel.Id,
                Title = listModel.Title,
                Subtitle = listModel.Subtitle,
                OwnerId = listModel.OwnerId,
                // IsPublic = listModel.IsPublic,
                ListType = listModel.ListType,
                CoverImgUrl = listModel.CoverImgUrl,
                CreatedAt = listModel.CreatedAt,
                UpdatedAt = listModel.UpdatedAt,
                // ListItems = listModel.Items.Select(l => l.ToItemDto()).ToList()
                OwnerUsername = listModel.User.UserName,
                OwnerFullName = listModel.User.FirstName + " " + listModel.User.LastName,
                OwnerProfileImgUrl = listModel.User.ProfileImgUrl
            };
        }



        public static CuratedList ToListFromCreateDto(this CreateCuratedListRequestDto listDto, string ownerId)
        {
            return new CuratedList
            {
                Title = listDto.Title,
                Subtitle = listDto.Subtitle,
                OwnerId = ownerId,
                IsPublic = listDto.IsPublic,
                ListType = listDto.ListType,
                CoverImgUrl = listDto.CoverImgUrl,
            };
        }

        public static CuratedList ToListFromUpdateDto(this UpdateCuratedListRequestDto listDto)
        {
            return new CuratedList
            {
                Title = listDto.Title,
                Subtitle = listDto.Subtitle,
                IsPublic = listDto.IsPublic,
                ListType = listDto.ListType,
                CoverImgUrl = listDto.CoverImgUrl,
            };
        }
    }
}