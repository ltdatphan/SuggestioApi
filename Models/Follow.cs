namespace SuggestioApi.Models;

public class Follow
{
    public int Id { get; set; }
    public string CurrentUserId { get; set; }
    public string TargetUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    //Navigation Property
    public User CurrentUser { get; set; } //User who is the follower
    public User TargetUser { get; set; } //User who is being followed
}
