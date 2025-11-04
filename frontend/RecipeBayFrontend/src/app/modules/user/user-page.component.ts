import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { UserService } from '../../services/user.service';
import { AuthService } from '../../services/auth.service';
import { UserProfileDtoDisplay } from '../../types/user';
import { Subscription } from 'rxjs';


@Component({
  standalone: true,
  imports: [CommonModule, RouterModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  selector: 'app-user-page',
  templateUrl: './user-page.component.html',
  styleUrls: ['./user-page.component.scss']
})
export class UserPageComponent implements OnInit, OnDestroy {
  profile?: UserProfileDtoDisplay;
  isOwnProfile = false;
  editing = false;
  editBio = '';
  private routeSub?: Subscription;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.routeSub = this.route.params.subscribe(params => {
      const idParam = params['id'];
      let id = idParam ? parseInt(idParam, 10) : null;

      if (id == null) {
        const decoded = this.authService.getDecodedToken();
        const sub = decoded?.sub ?? decoded?.Sub ?? decoded?.userId ?? null;
        if (!sub) {
          return;
        }
        id = typeof sub === 'string' ? parseInt(sub, 10) : sub;
      }

      if (id != null) {
        this.loadProfile(id);
      }
    });
  }

  ngOnDestroy(): void {
    this.routeSub?.unsubscribe();
  }

  private loadProfile(id: number) {
    this.userService.getProfile(id).subscribe({
      next: (p) => {
        this.profile = p;
        this.checkOwnership();
        this.editing = false;
      },
      error: (err) => console.error('Failed to load profile', err)
    });
  }

  private checkOwnership() {
    const decoded = this.authService.getDecodedToken();
    const sub = decoded?.sub ?? decoded?.Sub ?? decoded?.userId ?? null;
    if (!sub || !this.profile) return;
    const tokenId = typeof sub === 'string' ? parseInt(sub, 10) : sub;
    this.isOwnProfile = tokenId === this.profile.id;
  }

  startEdit() {
    this.editing = true;
    this.editBio = this.profile?.bio ?? '';
  }

  cancelEdit() {
    this.editing = false;
    this.editBio = '';
  }

  saveBio() {
    const trimmed = (this.editBio ?? '').trim();
    if (trimmed.length > 500) {
      alert('Bio too long (max 500 characters)');
      return;
    }
    this.userService.setBio(trimmed).subscribe({
      next: (p) => {
        this.profile = p;
        this.editing = false;
      },
      error: (err) => {
        console.error('Failed to save bio', err);
        alert('Failed to save bio');
      }
    });
  }

  follow() {
    if (!this.profile) return;
    this.userService.followUser(this.profile.id).subscribe({
      next: () => this.refreshProfile(),
      error: (err) => {
        console.error('Failed to follow', err);
        alert('Failed to follow user');
      }
    });
  }

  unfollow() {
    if (!this.profile) return;
    this.userService.unfollowUser(this.profile.id).subscribe({
      next: () => this.refreshProfile(),
      error: (err) => {
        console.error('Failed to unfollow', err);
        alert('Failed to unfollow user');
      }
    });
  }

  private refreshProfile() {
    if (!this.profile) return;
    this.userService.getProfile(this.profile.id).subscribe({
      next: (p) => {
        this.profile = p;
        this.checkOwnership();
      },
      error: (err) => console.error('Failed to refresh profile', err)
    });
  }
}
