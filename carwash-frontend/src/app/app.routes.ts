import { Routes } from '@angular/router';
import { authGuard, guestGuard, adminGuard, washerGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Public routes - koi bhi dekh sakta hai
  {
    path: '',
    loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent)
  },

  // Public route - koi bhi dekh sakta hai (no auth needed)
  {
    path: 'services',
    loadComponent: () => import('./pages/services/services').then(m => m.ServicesComponent)
  },

  // Guest only routes - sirf logged-OUT users ke liye
  // Agar pehle se login ho to home pe redirect hoga
  {
    path: 'login',
    canActivate: [guestGuard],  // Guard laga diya - logged in user yahan nahi aa sakta
    loadComponent: () => import('./pages/auth/login/login').then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/auth/register/register').then(m => m.RegisterComponent)
  },

  // Protected routes - sirf logged-IN users ke liye
  // Agar login nahi hai to /login pe redirect hoga
  {
    path: 'profile',
    canActivate: [authGuard],
    loadComponent: () => import('./pages/profile/profile').then(m => m.ProfileComponent)
  },
  {
    path: 'cars',
    canActivate: [authGuard],   // Login zaroori — sirf Customer apni cars dekh sakta hai
    loadComponent: () => import('./pages/cars/cars').then(m => m.CarsComponent)
  },
  {
    path: 'bookings',
    canActivate: [authGuard],   // Login zaroori — sirf Customer apni bookings dekhe
    loadComponent: () => import('./pages/bookings/bookings').then(m => m.BookingsComponent)
  },
  {
    path: 'payments',
    canActivate: [authGuard],   // Login zaroori — sirf Customer apne payments dekhe
    loadComponent: () => import('./pages/payments/payments').then(m => m.PaymentsComponent)
  },

  // Public route - koi bhi reviews dekh sakta hai, lekin likhne ke liye login zaroori
  {
    path: 'reviews',
    loadComponent: () => import('./pages/reviews/reviews').then(m => m.ReviewsComponent)
  },

  // Admin only route
  {
    path: 'admin',
    canActivate: [adminGuard],
    loadComponent: () => import('./pages/admin/admin').then(m => m.AdminComponent)
  },

  // Washer only route
  {
    path: 'washer',
    canActivate: [washerGuard],
    loadComponent: () => import('./pages/washer/washer').then(m => m.WasherComponent)
  },

  // Wildcard - koi bhi unknown URL → home pe bhejo
  {
    path: '**',
    redirectTo: ''
  }
];
