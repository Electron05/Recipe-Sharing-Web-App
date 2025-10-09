using Humanizer;
using RecipeBay.Data;
using RecipeBay.Models;
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

                if (parentIngredient == null) { throw new Exception($"Parent ingredient '{currentParent}' not found for ingredient part '{trimmedLine}'"); }

                context.Ingredients.Add(new Ingredient
                {
                    Name = trimmedLine,
                    Plural = trimmedLine.Pluralize(),
                    ParentIngredientId = parentIngredient.Id,
                    IsPartOf = parentIngredient,
                });
            }
            else if (line.StartsWith("-"))
            {
                var trimmedLine = line[1..];
                var parentIngredient = context.Ingredients
                    .FirstOrDefault(i => i.Name.Equals(currentParent));
                if (parentIngredient == null) { throw new Exception($"Parent ingredient '{currentParent}' not found for alias '{trimmedLine}'"); }
                context.IngredientAliases.Add(new IngredientAlias
                {
                    Name = trimmedLine,
                    Plural = trimmedLine.Pluralize(),
                    Ingredient = parentIngredient,
                    IngredientId = parentIngredient.Id
                });
            }
            else
            {
                currentParent = line;
            }
        }
        context.SaveChanges();
    }

    // Seed recipes basing on existing ingredients
    // For each english letter make a list of igredints starting with it
    // Then make recipes including 1 A-letter ingredint, 1 B-letter ingredient ...
    // Then make recipes including 2 A-letter ingredients ...
    // 3,4,5,6,7 ... (max len of )
    public static void SeedRecipes(RecipeBayContext context)
    {

        User? firstAuthor = context.Users.FirstOrDefault(u => u.Id == 1);
        if (firstAuthor == null)
            throw new Exception("User with id 1 not found.");

        string letters = "abcdefghijklmnopqrstuvwxyz";
        var ingredientsByLetter = new Dictionary<char, List<Ingredient>>();
        foreach (var letter in letters)
        {
            var ingredients = context.Ingredients
                .Where(i => i.Name.ToLower().StartsWith(letter.ToString()))
                .ToList();
            ingredientsByLetter[letter] = ingredients;
        }
        int maxIngredientsGroupLength = ingredientsByLetter.Values.Max(l => l.Count);
        for (int groupLength = 1; groupLength <= maxIngredientsGroupLength; groupLength++)
        {
            foreach (var ingredientGroup in ingredientsByLetter)
            {
                if (ingredientGroup.Value.Count < groupLength) continue;
                // take groupLength ingredients from ingredientGroup

                var recipe = new Recipe
                {
                    Title = ingredientGroup.Key + " - " + groupLength,
                    Description = "Great Description!",
                    Steps = new List<string> { "Do some magic", "Add Love" },
                    TimeToPrepareMinutes = 20,
                    TimeToPrepareHours = 0,
                    TimeToPrepareLongerThan1Day = false,
                    DifficultyFrom1To3 = 1,
                    AuthorId = 1,
                    Author = firstAuthor
                };

                var ingredeintsInCurrentRecipe = ingredientGroup.Value[..groupLength];
                recipe.IngredientEntries = ingredeintsInCurrentRecipe.Select((i, idx) => new IngredientEntry
                {
                    Quantity = "Some",
                    Recipe = recipe,
                    Ingredient = i,
                    IngredientId = i.Id,
                    IsPlural = false,
                    NotInList = false,
                }).ToList();

                context.Recipes.Add(recipe);
            }
        }
        context.SaveChanges();
    }
}