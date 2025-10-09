import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { RecipeDtoCreate } from '../types/creare-recipe';
import { RecipeDtoFeed, RecipeDtoDisplay } from '../types/get-recipe';


@Injectable({ providedIn: 'root' })
export class RecipeService {
  constructor(private http: HttpClient) {}

  createRecipe(data: RecipeDtoCreate): Observable<any> {
    return this.http.post(`${environment.apiUrl}/Recipes`, data);
  }

  getRecentRecipes(howMany: number, bottomOfFeedRecipeId: number | null): Observable<RecipeDtoFeed[]> {
    let params = `howMany=${howMany}`;
    if (bottomOfFeedRecipeId !== undefined && bottomOfFeedRecipeId !== null) {
      params += `&bottomOfFeedRecipeId=${bottomOfFeedRecipeId}`;
    }
    return this.http.get<RecipeDtoFeed[]>(`${environment.apiUrl}/Recipes/requestForFeed?${params}`);
  }

  getRecipeById(id: number) : Observable<RecipeDtoDisplay> {
    return this.http.get<RecipeDtoDisplay>(`${environment.apiUrl}/Recipes/${id}`);
  }

}