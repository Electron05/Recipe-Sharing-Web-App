using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RecipeBay.Models;
public class IngredientEntryDtoCreate
{
	public required string Quantity { get; set; }
	public bool IsPlural { get; set; } = false;

	public int? IngredientId { get; set; }
	public int? IngredientAliasId { get; set; }

	public bool NotInList { get; set; } = false;
	public string? CustomIngredientName;

	public short SortOrder { get; set; } // Order of appearance in the recipe
}
