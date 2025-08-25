using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipeBay.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }

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

        [Required]
        public required byte TimeToPrepareMinutes { get; set; }

        [Required]
        public required byte TimeToPrepareHours { get; set; }

        [Required]
        public bool TimeToPrepareLongerThan1Day = false;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


        public int ?AuthorId { get; set; }
        public User ?Author { get; set; }

        // Likes count (optional for future)
        public int Likes { get; set; } = 0;
    }
}
