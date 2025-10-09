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

	public int? IngredientId { get; set; }
	public Ingredient? Ingredient { get; set; }

	public int? IngredientAliasId { get; set; }
	public IngredientAlias? IngredientAlias { get; set; }

	public required string Quantity { get; set; }
	public bool IsPlural { get; set; } = false;

	public bool NotInList { get; set; } = false;

	public string? CustomIngredientName;

}
