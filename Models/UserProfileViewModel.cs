using csharp.Models;

public class UserProfileViewModel {
    public required UserModel User { get; set; }
    public required List<PostModel> Posts { get; set; }
}
