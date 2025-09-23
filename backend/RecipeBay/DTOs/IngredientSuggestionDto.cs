using System.ComponentModel.DataAnnotations;

namespace RecipeBay.DTOs
{
	public class IngredientSuggestionDto
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public string? Plural { get; set; }
		public bool isAlias { get; set; }
		public int? BaseIngredientId { get; set; } 
	}
}