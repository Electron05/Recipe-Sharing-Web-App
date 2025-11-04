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
        public bool isDeleted { get; set; } = false;

        [Required]
        [MaxLength(50)]
        public required string Username { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Email { get; set; }

        [Required]
        public required string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // New profile fields
        [MaxLength(500)]
        public string? Bio { get; set; }

        [MaxLength(1000)]
        public string? ProfilePictureUrl { get; set; }

        public List<Recipe> Recipes { get; set; } = new();
        public List<Comment> Comments { get; set; } = new();

        public List<User> Followers { get; set; } = new();
        public List<User> Following { get; set; } = new();

        public List<Recipe> BookmarkedRecipes { get; set; } = new();
        public List<Recipe> MadeRecipes { get; set; } = new();
    }
}
