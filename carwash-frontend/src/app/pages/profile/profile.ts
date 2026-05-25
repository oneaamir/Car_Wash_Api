import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { ProfileResponse } from '../../models/auth.models';

@Component({
  selector: 'app-profile',
  imports: [RouterLink],
  templateUrl: './profile.html',
  styleUrl: './profile.scss'
})
export class ProfileComponent implements OnInit {
  // OnInit = Angular lifecycle hook - component banne ke turant baad call hota hai
  // Yahan API call karte hain

  profile: ProfileResponse | null = null; // Profile data store karega
  isLoading = true;
  errorMessage = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  // ngOnInit() = component create hone ke baad Angular khud call karta hai
  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.authService.getProfile().subscribe({
      next: (data) => {
        this.profile = data;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Could not load profile. Please try again.';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  // Role ka badge color return karo
  getRoleColor(): string {
    const role = this.profile?.role;
    if (role === 'Admin') return 'badge-admin';
    if (role === 'Washer') return 'badge-washer';
    return 'badge-customer';
  }
}
