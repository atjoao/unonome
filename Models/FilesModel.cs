using System.ComponentModel.DataAnnotations;

namespace csharp.Models
{
    public class FilesModel (string FilePath)
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string FilePath { get; set; } = FilePath;

        public int PostId { get; set; }
        public PostModel? Post { get; set; }

    }
}