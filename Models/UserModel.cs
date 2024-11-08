using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace csharp.Models;

public class UserModel(string username, string email, string passwordHash, string ProfileDescription)
{
    [Key]
    public int Id { get; set; }

    [MaxLength(20)]
    public string Username { get; set; } = username;

    [Required]
    [JsonIgnore]
    public string Email { get; set; } = email;

    [Required]
    [JsonIgnore]
    public string PasswordHash { get; set; } = passwordHash;

    public string ProfileDescription { get; set; } = ProfileDescription ?? "";

    // user id -> followed id
    public ICollection<FollowModel> Following { get; set; } = [];
    
    // follower id -> user id 
    public ICollection<FollowModel> Followers { get; set; } = [];

}