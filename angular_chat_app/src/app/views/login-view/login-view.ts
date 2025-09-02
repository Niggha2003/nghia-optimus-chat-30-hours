import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { ReactiveFormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { inject } from '@angular/core';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-login-view',
  standalone: true,
  imports: [
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    ReactiveFormsModule,
    RouterModule
  ],
  templateUrl: './login-view.html',
  styleUrl: './login-view.scss'
})
export class LoginView {
  _authService: AuthService;
  loginForm: FormGroup;
  errorMessage: string = '';

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
    this._authService = authService;
  }

  onSubmit() {
    if (this.loginForm.valid) {
      this._authService.login(this.loginForm.value).subscribe({
        next: (response) => {
          console.log('Login successful', response);
          console.log(typeof response);
          localStorage.setItem('auth-token', response.AccessToken);
          localStorage.setItem('refresh-token', response.RefreshToken);
          
          if (this.router.getCurrentNavigation()?.previousNavigation) {
            this.router.navigateByUrl(this.router.getCurrentNavigation()!.previousNavigation!.finalUrl!);
          } else {
            this.router.navigate(['/']);
          }
        },
        error: (error) => {
          console.error('Login failed', error);
          this.errorMessage = error.error?.message || 'An error occurred during login.';
        }
      });
    }
  }
}
