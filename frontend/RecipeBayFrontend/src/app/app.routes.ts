import { Routes } from '@angular/router';
import { LoginComponent } from './modules/auth/login/login.component';
import { RegisterComponent } from './modules/auth/register/register.component';
import { CreateRecipeComponent } from './modules/recipes/create/createRecipe.component';
import { AuthGuard } from './guards/auth.guard';
import { DisplayRecipeComponent } from './modules/recipes/display/displayRecipe.component';
import { UserPageComponent } from './modules/user/user-page.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: 'create-recipe', component: CreateRecipeComponent, canActivate: [AuthGuard] },
  { path: 'recipe/:id', component: DisplayRecipeComponent },
  { path: 'users/:id', component: UserPageComponent }
];
