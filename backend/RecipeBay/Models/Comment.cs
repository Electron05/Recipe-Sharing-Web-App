using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBay.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public bool isDeleted { get; set; } = false;

        [Required]
        [MaxLength(4096)]
        public required string Body { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public int AuthorId { get; set; }
        public required User Author { get; set; }


        public int RecipeId { get; set; }
        public required Recipe Recipe { get; set; }

        public int Likes { get; set; } = 0;
    }
}
