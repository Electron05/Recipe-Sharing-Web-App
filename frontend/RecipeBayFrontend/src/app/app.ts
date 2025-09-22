import { Component, signal } from '@angular/core';
import { MatSidenavModule } from '@angular/material/sidenav';
import { AuthService } from './services/auth.service';
import { RouterOutlet, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog } from '@angular/material/dialog';
import { LogoutConfirmDialogComponent } from './modules/auth/logout/logout-confirmation.component';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, 
            RouterLink, 
            MatSidenavModule,
            CommonModule,
            MatIconModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})

export class App {
  isAuthenticated = false;

  constructor(private authService: AuthService, private dialog: MatDialog) {
    this.authService.isAuthenticated$.subscribe(val => this.isAuthenticated = val);
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
}
