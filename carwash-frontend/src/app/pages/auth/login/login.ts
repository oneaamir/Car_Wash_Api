import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  imports: [
    FormsModule,  // Template-driven forms ke liye - [(ngModel)] yahan se aata hai
    RouterLink    // routerLink directive ke liye
  ],
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {

  // Form data - [(ngModel)] in fields se bind hoga
  formData = {
    email: '',
    password: ''
  };

  showPassword = false;
  isLoading = false;     // Submit button disable karne ke liye loading mein
  errorMessage = '';     // Backend se error aaye to dikhao
  successMessage = '';   // Success message

  // Constructor mein services inject karo
  // Angular khud AuthService aur Router ka instance dega
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  // Form submit hone par yeh function call hoga
  onSubmit(): void {
    this.isLoading = true;
    this.errorMessage = '';

    // authService.login() ek Observable return karta hai
    // Observable ko "activate" karne ke liye subscribe() zaroori hai
    this.authService.login(this.formData).subscribe({
      // next: success hone par chalega
      next: (response) => {
        // Role ke hisaab se redirect karo
        if (response.role === 'Admin') {
          this.router.navigate(['/admin']);
        } else if (response.role === 'Washer') {
          this.router.navigate(['/washer']);
        } else {
          this.router.navigate(['/dashboard']); // Customer → Dashboard
        }
      },
      // error: kuch galat hone par chalega
      error: (err) => {
        // err.error = backend ka response body
        this.errorMessage = err.error?.message || 'Login failed. Please try again.';
        this.isLoading = false;
      }
    });
  }
}
