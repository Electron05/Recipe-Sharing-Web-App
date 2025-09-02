using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBay.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public bool isDeleted { get; set; } = false;

        [Required]
        [MaxLength(200)]
        public required string Title { get; set; }

        [Required]
        public required string Description { get; set; }


        [Column(TypeName = "jsonb")]
        public List<string> Ingredients { get; set; } = new();

        [Column(TypeName = "jsonb")]
        public List<string> IgredientsAmounts { get; set; } = new();

        [Column(TypeName = "jsonb")]
        public List<string> Steps { get; set; } = new();


        public byte TimeToPrepareMinutes { get; set; }
        public byte TimeToPrepareHours { get; set; }
        public bool TimeToPrepareLongerThan1Day { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;



        public int AuthorId { get; set; }
        [Required]
        public required User Author { get; set; }


        public List<Comment> Comments { get; set; } = new();

        public int Likes { get; set; } = 0;
    }
}
