import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule],
  template: `
    <main class="home">
      <div class="card">
        <h1>🚗 CarWash</h1>
        <p>Welcome, <strong>{{ auth.getFullName() }}</strong>!</p>
        <p class="role">Role: {{ auth.getRole() }}</p>
        <button (click)="logout()">Logout</button>
      </div>
    </main>
  `,
  styles: [`
    .home { min-height:100vh; display:grid; place-items:center; background:#f5f7fb; }
    .card { background:white; padding:32px 40px; border-radius:8px; box-shadow:0 10px 30px rgba(0,0,0,.08); text-align:center; }
    h1 { margin:0 0 12px; }
    p { margin:0 0 8px; color:#333; }
    .role { font-size:13px; color:#888; margin-bottom:24px; }
    button { padding:10px 24px; border:0; border-radius:6px; background:#dc2626; color:white; cursor:pointer; font-size:14px; }
  `]
})
export class HomeComponent {
  auth = inject(AuthService);
  logout(): void { this.auth.logout(); }
}
