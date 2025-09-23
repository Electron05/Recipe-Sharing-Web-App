using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RecipeBay.Models;
public class IngredientEntry
{
	[Key]
	public int Id { get; set; }

	[Required]
	public int RecipeId { get; set; }
	public required Recipe Recipe { get; set; }

	[Required]
	public int IngredientId { get; set; }
	public required Ingredient Ingredient { get; set; }

	public required string Quantity { get; set; }
	public bool IsPlural { get; set; } = false;

	public short SortOrder { get; set; } // Order of appearance in the recipe
}
