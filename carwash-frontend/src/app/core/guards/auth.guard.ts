import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

// CanActivateFn = Angular ka functional guard (Angular 15+)
//
// Guard kya hota hai?
// Socho bouncer (darwaan) ki tarah - kuch routes sirf logged-in users ke liye hain
// Guard pehle check karta hai: "Kya user login hai?"
// - Haan → andar jaane do (return true)
// - Nahi → login page pe bhejo (return redirect)

// authGuard = sirf logged-in users ke liye (profile, bookings, cars etc)
export const authGuard: CanActivateFn = () => {
  // inject() = Angular 15+ mein services inject karne ka functional tarika
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) {
    return true; // Access allow karo
  }

  // Login nahi hai - login page pe redirect karo
  return router.createUrlTree(['/login']);
};

// guestGuard = sirf logged-OUT users ke liye (login/register pages)
// Agar pehle se login hai aur /login pe jao to home pe bhejo
export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return true; // Login nahi hai - login/register page dikhaao
  }

  return router.createUrlTree(['/']); // Pehle se login hai - home pe bhejo
};

// adminGuard = sirf Admin role ke liye
export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.getUserRole() === 'Admin') {
    return true;
  }

  return router.createUrlTree(['/login']);
};

// washerGuard = sirf Washer role ke liye
export const washerGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.getUserRole() === 'Washer') {
    return true;
  }

  return router.createUrlTree(['/login']);
};
