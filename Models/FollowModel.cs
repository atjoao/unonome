namespace csharp.Models;

public class FollowModel {
    public int Id { get; set; }

    public int FollowerId { get; set; }
    public UserModel? Follower { get; set; }

    public int FollowedId { get; set; }
    public UserModel? Followed { get; set; }
}