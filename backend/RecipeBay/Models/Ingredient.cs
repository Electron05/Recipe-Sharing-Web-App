using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Ingredient
{
    [Key]
    public int Id { get; set; }

    [Required]
    public required string Name { get; set; }          // Canonical singular, e.g. "egg"

    public required string Plural { get; set; }        // Canonical plural, e.g. "eggs"

    // List of aliases 
    public List<IngredientAlias> Aliases { get; set; } = new();

    public List<IngredientEntry> RecipeEntries { get; set; } = new();

    // If this is a part, e.g. "egg white", reference to "egg"
    // Important so recipes with whites will be shown when searching for recipes with eggs
	public int? ParentIngredientId { get; set; }
	public Ingredient? IsPartOf { get; set; }
}