namespace SuggestioApi.Dtos.Follow;

public class FollowRequestDto
{
    public string CurrentUserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
}