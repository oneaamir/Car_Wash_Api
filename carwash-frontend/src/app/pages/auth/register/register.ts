import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-register',
  imports: [FormsModule, RouterLink],
  templateUrl: './register.html',
  styleUrl: './register.scss'
})
export class RegisterComponent {

  formData = {
    fullName: '',
    email: '',
    phone: '',
    password: '',
    confirmPassword: ''  // Sirf frontend validation ke liye - backend ko nahi bhejenge
  };

  isLoading = false;
  errorMessage = '';
  successMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    // Password match check - frontend validation
    if (this.formData.password !== this.formData.confirmPassword) {
      this.errorMessage = 'Passwords do not match!';
      return; // Aage mat jao
    }

    this.isLoading = true;
    this.errorMessage = '';

    // Backend ke liye sirf required fields - confirmPassword exclude karo
    const registerData = {
      fullName: this.formData.fullName,
      email: this.formData.email,
      phone: this.formData.phone,
      password: this.formData.password
    };

    this.authService.register(registerData).subscribe({
      next: () => {
        this.successMessage = 'Registration successful! Redirecting to login...';
        // 2 seconds baad login page pe jao
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Registration failed. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
