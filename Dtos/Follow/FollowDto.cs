namespace SuggestioApi.Dtos.Follow;

public class FollowDto
{
    public int Id { get; set; }
    public string CurrentUserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}