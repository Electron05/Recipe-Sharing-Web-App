import { Component, OnInit } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AuthService } from './services/auth.service';
import { Router, RouterOutlet, RouterLink, NavigationEnd } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { LogoutConfirmDialogComponent } from './modules/auth/logout/logout-confirmation.component';
import { RecipeService } from './services/recipe.service';
import { RecipeDtoFeed } from './types/get-recipe';

@Component({
  selector: 'app-root',
  imports: [
    RouterOutlet,
    RouterLink,
    MatSidenavModule,
    CommonModule,
    MatIconModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  isAuthenticated = false;
  bottomOfTheFeedRecipeId: number | null = null;
  feed: RecipeDtoFeed[] = [];
  isLoadingFeed = false;
  initialFeedLoaded = false;

  constructor(
    private authService: AuthService,
    private dialog: MatDialog,
    private router: Router,
    private recipeService: RecipeService
  ) {
    this.authService.isAuthenticated$.subscribe(val => this.isAuthenticated = val);

    this.router.events.subscribe(event => {
      if (event instanceof NavigationEnd) {
        if (this.isHome) {
          this.loadInitialFeed();
        } else {
          this.feed = [];
          this.initialFeedLoaded = false;
          this.isLoadingFeed = false;
          this.bottomOfTheFeedRecipeId = null;
        }
      }
    });
  }

  get currentUserId(): number | null {
    const decoded = this.authService.getDecodedToken();
    const sub = decoded?.sub ?? decoded?.Sub ?? decoded?.userId ?? null;
    if (!sub) return null;
    return typeof sub === 'string' ? parseInt(sub, 10) : sub;
  }

  ngOnInit() {
    if (this.isHome) {
      this.loadInitialFeed();
    }
  }

  loadInitialFeed() {
    this.isLoadingFeed = true;
    this.recipeService.getRecentRecipes(20, null).subscribe(recipes => {
      this.feed = recipes;
      if (recipes.length > 0) {
        this.bottomOfTheFeedRecipeId = recipes[recipes.length - 1].id;
      }
      this.isLoadingFeed = false;
      this.initialFeedLoaded = true;
    }, () => {
      this.isLoadingFeed = false;
      this.initialFeedLoaded = true;
    });
  }

  requestRecipesForFeed() {
    if (this.isLoadingFeed || !this.isHome) return;
    this.isLoadingFeed = true;
    this.recipeService.getRecentRecipes(20, this.bottomOfTheFeedRecipeId).subscribe(recipes => {
      this.feed.push(...recipes);
      if (recipes.length > 0) {
        this.bottomOfTheFeedRecipeId = recipes[recipes.length - 1].id;
      }
      this.isLoadingFeed = false;
    }, () => {
      this.isLoadingFeed = false;
    });
  }

  clickLogout() {
    const dialogRef = this.dialog.open(LogoutConfirmDialogComponent, {
      disableClose: true,
      width: '320px'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.authService.logout();
      }
    });
  }

  onScroll(event: Event) {
    if (!this.isHome) return;
    const target = event.target as HTMLElement;
    const threshold = 200;
    if (target.scrollHeight - target.scrollTop - target.clientHeight < threshold) {
      this.requestRecipesForFeed();
    }
  }

  get isHome(): boolean {
    return this.router.url === '/';
  }
}