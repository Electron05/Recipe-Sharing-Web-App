import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { environment } from '../../../environments/environment';
export interface IngredientSuggestion {
  id: number;
  name: string;
  plural: string
}

@Injectable({ providedIn: 'root' })
export class IngredientSuggestionsService {
  constructor(private http: HttpClient) {}

  search(name: string) : Observable<IngredientSuggestion[]> {
    if(name.length < 3) return of([]); //new Observable<IngredientSuggestion[]>()
    return this.http.get<IngredientSuggestion[]>(`${environment.apiUrl}/Ingredients/search?name=${name}`);
  }

  IngredientBasicIdOrAliasIds(name : string) : Observable<number[]>{
    return this.http.get<number[]>(`${environment.apiUrl}/Ingredients/exists?name=${name}`);
  }
}