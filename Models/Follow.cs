namespace SuggestioApi.Models;

public class Follow
{
    public int Id { get; set; }
    public string CurrentUserId { get; set; } = string.Empty;
    public string TargetUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Property
    public User CurrentUser { get; set; } = null!; //User who is the follower
    public User TargetUser { get; set; } = null!; //User who is being followed
}