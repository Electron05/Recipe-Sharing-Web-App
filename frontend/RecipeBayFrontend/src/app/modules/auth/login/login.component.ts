import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { RouterLink } from '@angular/router';
import { LoginService } from '../../../services/login.service';
import { AuthService } from '../../../services/auth.service';
import { ChangeDetectionStrategy } from '@angular/core';


@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, 
            ReactiveFormsModule,
            MatButtonModule,
            MatFormFieldModule,
            MatInputModule,
            RouterLink],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css',],
  changeDetection: ChangeDetectionStrategy.OnPush
})

export class LoginComponent {
  loginForm: FormGroup;
  loginSuccess: boolean | null = null;
  loginMessage: string = '';


  constructor(private fb: FormBuilder, 
              private loginService: LoginService,
              private authService: AuthService) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  onSubmit() {
    if (this.loginForm.valid) {
      const { email, password } = this.loginForm.value;
      this.loginService.login({ usernameOrEmail: email, password }).subscribe({
        next: (res) => {
          this.loginSuccess = true;
          this.loginForm.reset();
          this.authService.login(res.token);
        },
        error: (err) => {
          this.loginSuccess = false;
          this.loginMessage = err.error?.message || 'Login failed. Please try again.';
        }
      });
    }
  }
}
