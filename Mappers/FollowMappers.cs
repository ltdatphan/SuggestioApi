using SuggestioApi.Dtos.Follow;
using SuggestioApi.Models;

namespace SuggestioApi.Mappers;

public static class FollowMappers
{
    public static FollowDto ToFollowDto(this Follow followModel)
    {
        return new FollowDto
        {
            Id = followModel.Id,
            CurrentUserId = followModel.CurrentUserId,
            TargetUserId = followModel.TargetUserId,
            CreatedAt = followModel.CreatedAt
        };
    }

    public static Follow ToFollowFromCreateDto(this FollowRequestDto requestDto)
    {
        return new Follow
        {
            CurrentUserId = requestDto.CurrentUserId,
            TargetUserId = requestDto.TargetUserId
        };
    }
}