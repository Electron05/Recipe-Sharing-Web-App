import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RecipeDtoDisplay } from '../../../types/get-recipe';
import { RecipeService } from '../../../services/recipe.service';

import { CommonModule } from '@angular/common';

@Component({
	selector: 'app-display-recipe',
	templateUrl: './displayRecipe.component.html',
	styleUrls: ['./displayRecipe.component.css'],
  imports: [CommonModule]
	})

	export class DisplayRecipeComponent implements OnInit {
	recipeId!: number;
	recipe: RecipeDtoDisplay | null = null;
	constructor(private route: ActivatedRoute,
              private recipeService: RecipeService){}

	ngOnInit() {
		this.recipeId = Number(this.route.snapshot.paramMap.get('id'));
    this.recipeService.getRecipeById(this.recipeId).subscribe(r => {
      this.recipe = r;
    });

	}
}