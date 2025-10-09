export interface IngredientEntry {
  quantity: string;
  isPlural: boolean;
  ingredientId: number | null;
  ingredientAliasId: number | null;
  notInList: boolean;
  customIngredientName: string;
}

export interface RecipeDtoCreate {
  title: string;
  description: string;
  steps: string[];
  ingredientEntries: IngredientEntry[];
  timeToPrepareMinutes: number;
  timeToPrepareHours: number;
  timeToPrepareLongerThan1Day: boolean;
  difficulty: string;
}