import { Component, inject, signal } from '@angular/core';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-customer-sidebar',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  templateUrl: './customer-sidebar.html',
  styleUrl: './customer-sidebar.scss'
})
export class CustomerSidebarComponent {
  authService = inject(AuthService);
  private router = inject(Router);

  isMobileOpen = signal(false);

  get currentUser() {
    return this.authService.currentUser();
  }

  get initials(): string {
    const name = this.currentUser?.fullName ?? '';
    return name
      .split(' ')
      .filter(w => w.length > 0)
      .map(w => w[0].toUpperCase())
      .slice(0, 2)
      .join('');
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  closeMobile(): void {
    this.isMobileOpen.set(false);
  }
}
