import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <main class="auth-page">
      <section class="auth-card">
        <h1>Register</h1>

        <form [formGroup]="registerForm" (ngSubmit)="onSubmit()">
          <label>
            Full Name
            <input type="text" formControlName="fullName" />
          </label>

          <label>
            Email
            <input type="email" formControlName="email" />
          </label>

          <label>
            Password
            <input type="password" formControlName="password" />
          </label>

          <button type="submit" [disabled]="registerForm.invalid">
            Register
          </button>
        </form>
      </section>
    </main>
  `,
  styles: [`
    .auth-page {
      min-height: 100vh;
      display: grid;
      place-items: center;
      background: #f5f7fb;
      padding: 24px;
    }

    .auth-card {
      width: min(100%, 420px);
      background: white;
      padding: 24px;
      border-radius: 8px;
      box-shadow: 0 10px 30px rgba(0,0,0,0.08);
    }

    form {
      display: grid;
      gap: 16px;
      margin-top: 16px;
    }

    label {
      display: grid;
      gap: 8px;
      font-size: 14px;
    }

    input {
      padding: 10px 12px;
      border: 1px solid #d0d7e2;
      border-radius: 6px;
    }

    button {
      padding: 10px 14px;
      border: 0;
      border-radius: 6px;
      background: #16a34a;
      color: white;
      cursor: pointer;
    }

    button:disabled {
      opacity: 0.5;
      cursor: not-allowed;
    }
  `]
})
export class RegisterComponent {
  private fb = inject(FormBuilder);

  registerForm = this.fb.group({
    fullName: ['', [Validators.required]],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]]
  });

  onSubmit(): void {
    if (this.registerForm.invalid) return;
    console.log('Register form data:', this.registerForm.value);
  }
}
