using System.ComponentModel.DataAnnotations;

// Model representing an alias or part of an ingredient
// e.g. "egg white" as a part of "egg"
// or "red bean" as an alias for "kidney bean"
public class IngredientAlias
{
	[Key]
	public int Id { get; set; }

	// Ingredient FK
	[Required]
	public int IngredientId { get; set; }
	public required Ingredient Ingredient { get; set; }

	[Required]
	public required string Name { get; set; } // Alias or part name, e.g., "egg white"

	public string? Plural { get; set; } // Plural form, e.g., "egg whites"
	
}