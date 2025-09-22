import { Component } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-logout-confirm-dialog',
  template: `
    <div style="padding: 2rem; text-align: center;">
      <h2>Confirm Logout</h2>
      <p>Are you sure you want to log out?</p>
      <button mat-raised-button color="warn" (click)="confirm()">Logout</button>
      <button mat-button (click)="close()">Cancel</button>
    </div>
  `,
  standalone: true,
  imports: [],
})
export class LogoutConfirmDialogComponent {
  constructor(private dialogRef: MatDialogRef<LogoutConfirmDialogComponent>) {}

  confirm() {
    this.dialogRef.close(true);
  }
  close() {
    this.dialogRef.close(false);
  }
}