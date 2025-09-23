using Humanizer;
using RecipeBay.Data;

public static class IngredientSeeder
{
    public static void SeedIngredients(RecipeBayContext context, string filePath)
    {
        var lines = File.ReadAllLines(filePath)
                        .Where(l => !string.IsNullOrWhiteSpace(l))
                        .ToList();
        // First pass: Add all main ingredients
        foreach (var line in lines)
        {
            if (line.StartsWith("\t")) continue; // Skip aliases for now
            var plural = line.Pluralize();
            context.Ingredients.Add(new Ingredient
            {
                Name = line,
                Plural = plural
            });

        }
        context.SaveChanges();
        // Second pass: Add all aliases with correct parent references
        string currentParent = null;
        foreach (var line in lines)
        {
            if (line.StartsWith("\t"))
            {
                var trimmedLine = line.Trim();
                var parentIngredient = context.Ingredients
                    .FirstOrDefault(i => i.Name.Equals(currentParent));

                if (parentIngredient == null) { throw new Exception($"Parent ingredient '{currentParent}' not found for alias '{trimmedLine}'"); }

                context.IngredientAliases.Add(new IngredientAlias
                {
                    Name = trimmedLine,
                    Plural = trimmedLine.Pluralize(),
                    Ingredient = parentIngredient
                });
            }
            else
            {
                currentParent = line;
            }
        }
        context.SaveChanges();
    }
}