using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace csharp.Models;

public class CommentModel {
    [Key]
    public int Id { get; set; }
    
    public int PostId { get; set; }

    public PostModel? Post { get; set; }
    
    [Required]
    public int UserId { get; set; }
    public UserModel? User { get; set; }

    [Required]
    public required string Content { get; set; }

    public DateTime CommentedAt { get; set; } = DateTime.Now;
}