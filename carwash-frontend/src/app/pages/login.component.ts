import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <main class="auth-page">
      <section class="auth-card">
        <h1>Login</h1>

        @if (error()) {
          <div class="error">{{ error() }}</div>
        }

        <form [formGroup]="loginForm" (ngSubmit)="onSubmit()">
          <label>
            Email
            <input type="email" formControlName="email" placeholder="you@example.com" />
          </label>
          <label>
            Password
            <input type="password" formControlName="password" placeholder="••••••" />
          </label>
          <button type="submit" [disabled]="loginForm.invalid || loading()">
            {{ loading() ? 'Signing in…' : 'Login' }}
          </button>
        </form>

        <p class="switch">No account? <a routerLink="/register">Register</a></p>
      </section>
    </main>
  `,
  styles: [`
    .auth-page { min-height:100vh; display:grid; place-items:center; background:#f5f7fb; padding:24px; }
    .auth-card { width:min(100%,420px); background:white; padding:24px; border-radius:8px; box-shadow:0 10px 30px rgba(0,0,0,.08); }
    h1 { margin:0 0 20px; }
    .error { background:#fee2e2; border:1px solid #fca5a5; color:#dc2626; padding:10px 12px; border-radius:6px; font-size:14px; margin-bottom:16px; }
    form { display:grid; gap:16px; }
    label { display:grid; gap:8px; font-size:14px; }
    input { padding:10px 12px; border:1px solid #d0d7e2; border-radius:6px; font-size:14px; }
    button { padding:10px 14px; border:0; border-radius:6px; background:#1f6feb; color:white; cursor:pointer; font-size:14px; }
    button:disabled { opacity:.5; cursor:not-allowed; }
    .switch { margin-top:16px; text-align:center; font-size:14px; }
    .switch a { color:#1f6feb; }
  `]
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  loading = signal(false);
  error = signal('');

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', Validators.required]
  });

  onSubmit(): void {
    if (this.loginForm.invalid) return;

    this.loading.set(true);
    this.error.set('');

    const { email, password } = this.loginForm.value;

    this.auth.login({ email: email!, password: password! }).subscribe({
      next: () => {
        this.loading.set(false);
        this.router.navigate(['/home']);
      },
      error: err => {
        this.loading.set(false);
        this.error.set(err.error ?? 'Login failed. Check your credentials.');
      }
    });
  }
}
