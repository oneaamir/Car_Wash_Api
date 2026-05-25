import { Component, inject, computed } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { HeaderComponent }          from './shared/components/header/header';
import { FooterComponent }          from './shared/components/footer/footer';
import { CustomerSidebarComponent } from './shared/components/customer-sidebar/customer-sidebar';
import { AdminSidebarComponent }    from './shared/components/admin-sidebar/admin-sidebar';
import { AuthService }              from './core/services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, HeaderComponent, FooterComponent, CustomerSidebarComponent, AdminSidebarComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  private auth = inject(AuthService);
  isCustomerLayout = computed(() => this.auth.currentUser()?.role === 'Customer');
  isAdminLayout    = computed(() => this.auth.currentUser()?.role === 'Admin');
}
