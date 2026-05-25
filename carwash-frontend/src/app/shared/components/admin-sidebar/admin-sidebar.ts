import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { AdminTabService } from '../../../core/services/admin-tab.service';

@Component({
  selector: 'app-admin-sidebar',
  standalone: true,
  imports: [],
  templateUrl: './admin-sidebar.html',
  styleUrl: './admin-sidebar.scss'
})
export class AdminSidebarComponent {
  authService  = inject(AuthService);
  tabService   = inject(AdminTabService);
  private router = inject(Router);

  navItems = [
    { tab: 'overview',  label: 'Overview',    icon: '🏠' },
    { tab: 'bookings',  label: 'Bookings',    icon: '📅' },
    { tab: 'washers',   label: 'Washers',     icon: '🧹' },
    { tab: 'customers', label: 'Customers',   icon: '🧑' },
    { tab: 'services',  label: 'Services',    icon: '⚙️' },
    { tab: 'payments',  label: 'Payments',    icon: '💳' },
    { tab: 'promos',    label: 'Promo Codes', icon: '🏷️' },
    { tab: 'reports',   label: 'Reports',     icon: '📊' },
  ];

  get currentUser() { return this.authService.currentUser(); }

  get initials(): string {
    return (this.currentUser?.fullName ?? '')
      .split(' ').filter((w: string) => w).map((w: string) => w[0].toUpperCase()).slice(0, 2).join('');
  }

  selectTab(tab: string): void {
    this.tabService.activeTab.set(tab);
    this.router.navigate(['/admin']);
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
