export interface RecipeDtoFeed {
  id: number;
  title: string;
  description: string;
  timeToPrepareMinutes: number;
  timeToPrepareHours: number;
  timeToPrepareLongerThan1Day: boolean;
  difficulty: string;
  likes: number;
}

export interface RecipeDtoDisplay {
  id: number;
  isDeleted: boolean;
  title: string;
  description: string;
  ingredients: string[];
  ingredientsAmounts: string[];
  steps: string[];
  timeToPrepareMinutes: number;
  timeToPrepareHours: number;
  timeToPrepareLongerThan1Day: boolean;
  createdAt: string;
  authorId?: number | null;
  likes: number;
}