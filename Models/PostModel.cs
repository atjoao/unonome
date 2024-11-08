using System.ComponentModel.DataAnnotations;

namespace csharp.Models;

public class PostModel{
    
    [Key]
    public int PostId { get; set; }

    [Required]
    public int UserId { get; set; }
    public UserModel? User { get; set; }

    [Required]
    public required string Content { get; set; }

    public DateTime PostedAt { get; set; } = DateTime.Now;

    public ICollection<CommentModel> Comments { get; set; } = [];

    public ICollection<LikeModel> Likes { get; set; } = [];

    public ICollection<FilesModel> Files { get; set; } = [];

}