import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReactiveFormsModule, FormBuilder, FormGroup, Validators, FormArray, AsyncValidatorFn, AbstractControl } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDividerModule } from '@angular/material/divider';
import { MatSelectModule } from '@angular/material/select';


import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { Observable, of, combineLatest, startWith, concatWith, switchAll } from 'rxjs';
import { debounceTime, switchMap, filter, map, tap } from 'rxjs/operators';
import { IngredientSuggestionsService, IngredientSuggestion } from '../../../services/ingredientSuggestions.service';

import { RecipeDtoCreate, IngredientEntry } from '../../../types/creare-recipe';
import { RecipeService } from '../../../services/recipe.service';

@Component({
  selector: 'app-create-recipe',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCheckboxModule,
    MatDividerModule,
    MatSelectModule,
    MatAutocompleteModule
  ],
  templateUrl: './createRecipe.component.html',
  styleUrls: ['./createRecipe.component.css']
})

export class CreateRecipeComponent {
  ingredientsForm: FormGroup;
  detailsForm: FormGroup;
  filteredIngredients$: Observable<IngredientSuggestion[]>[] = [];

  autocompleteDisabled: number[] = []; 
  // when selceting ingredient from autocomplete gets set to 2
  // then validator sets to 1, then after adding for example 's' validator sets to 0 after getting new data
  // autocomplete gets shown with new loaded data, this removes autocomplete "flash"

  createRecipeSuccess: boolean | null = null;
  createRecipeMessage: string = '';



  constructor(private fb: FormBuilder, 
              private ingredientService: IngredientSuggestionsService,
              private createRecipeService: RecipeService) {
    this.ingredientsForm = this.fb.group({
      ingredients: this.fb.array([])
    });

    this.detailsForm = this.fb.group({
      title: ['', [Validators.required]],
      description: [''],
      steps: this.fb.array([]),
      minutes: [0],
      hours: [0],
      longerThanOneDay: [false],
      difficulty: ['easy']
    });
  }

  createIngredientRow(): FormGroup {
    return this.fb.group({
      quantity: ['', [Validators.required, Validators.minLength(1)]],
      ingredient: ['', [Validators.required, Validators.minLength(3)], this.ingredientValidator],
      isPlural: [false],
      notInList: [true],
      aliasId: [-1],
      ingredientId: [-1],
      sortOrder: [-1]
    });
  }

  get ingredients(): FormArray {
    return this.ingredientsForm.get('ingredients') as FormArray;
  }

  addIngredient() {
    this.ingredients.push(this.createIngredientRow());
    const index = this.ingredients.length - 1;
    this.autocompleteDisabled.push(0);

    this.ingredients.at(index).get('sortOrder')?.setValue(index)

    const ingredientControl = this.ingredients.at(index).get('ingredient');
    ingredientControl?.valueChanges.subscribe(ingredient => {
      this.onIngredientChange(index);
    });

    this.filteredIngredients$[index] = ingredientControl!.valueChanges.pipe(
      startWith(''),
      debounceTime(300),
      switchMap((name: string) => {
        if (!name || name.length < 3) return of([]);

        return this.ingredientService.search(name).pipe(
          tap(results => {
            if(this.autocompleteDisabled[index]>0) 
              this.autocompleteDisabled[index]--;
          })
        );
      })
    );
  }

  removeIngredient(index: number) {
    this.ingredients.removeAt(index);
    this.autocompleteDisabled.splice(index, 1);
  }

  get steps(): FormArray {
  return this.detailsForm.get('steps') as FormArray;
  }

  addStep() {
    this.steps.push(this.fb.group({ text: ['', [Validators.required, Validators.minLength(1)]] }));
  }

  removeStep(index: number) {
    this.steps.removeAt(index);
  }


  onIngredientChange(index : number){
    this.ingredients.at(index).get('aliasId')?.setValue(-1);
    this.ingredients.at(index).get('ingredientId')?.setValue(-1);
  }

  ingredientValidator: AsyncValidatorFn = (control: AbstractControl) => {
    // Async validator for providing ingredient ids

    const parent = control.parent;
    if (!parent) return of(null);

    const name = control.value;

    if (!name || name.length < 3) return of(null);

    // Provide ingredient id and  optionally alias id if user picked alias, if both ids are -1 set error
    return this.ingredientService.IngredientBasicIdOrAliasIds(name).pipe(
      map(res => {
        if (res[0] === -1 && res[1] === -1) {
          parent.get('notInList')?.setValue(true);
          parent.get('isPlural')?.setValue(false);
          return { notExists: true };
        } else {
          parent.get('ingredientId')?.setValue(res[0]);
          parent.get('aliasId')?.setValue(res[1]);
          parent.get('isPlural')?.setValue(res[2]==-1) // yeah sorry for magic number, see IngredientsController
          parent.get('notInList')?.setValue(false);
          return null;
        }
      })
    );
  };

  onIngredientSelected(event: any, index: number) {
    this.autocompleteDisabled[index] = 2;
    console.debug("DEBUG: Autocomplete disabled");
  }

  isFormValid(): boolean {
    if (this.ingredients.length === 0 || this.ingredientsForm.invalid) return false;
    if (this.steps.length === 0 || this.detailsForm.invalid) return false;

    return true;
  }

  submitAll() {

    const ingredientEntries : IngredientEntry[] = this.ingredients.controls.map(group => ({
      quantity: group.get('quantity')?.value,
      isPlural: group.get('isPlural')?.value,
      ingredientId: group.get('ingredientId')?.value === -1 ? null : group.get('ingredientId')?.value,
      ingredientAliasId: group.get('aliasId')?.value === -1 ? null : group.get('aliasId')?.value,
      notInList: group.get('notInList')?.value,
      customIngredientName: group.get('ingredient')?.value
    }));

    const dto : RecipeDtoCreate = {
      title: this.detailsForm.get('title')?.value,
      description: this.detailsForm.get('description')?.value,
      steps: this.steps.controls.map(step => step.get('text')?.value),
      ingredientEntries,
      timeToPrepareMinutes: this.detailsForm.get('minutes')?.value,
      timeToPrepareHours: this.detailsForm.get('hours')?.value,
      timeToPrepareLongerThan1Day: this.detailsForm.get('longerThanOneDay')?.value,
      difficulty: this.detailsForm.get('difficulty')?.value
    }

    this.createRecipeService.createRecipe(dto).subscribe({
      next: (res) => {
        this.createRecipeSuccess = true;
      },
      error: (err) => {
        this.createRecipeSuccess = false;
        this.createRecipeMessage = err?.error?.message || 'Something went wrong. Please try again later.';
      }
    })
  }
}
