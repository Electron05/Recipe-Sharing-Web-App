using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RecipeBay.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<Recipe> Recipes { get; set; } = new();
    }
}
