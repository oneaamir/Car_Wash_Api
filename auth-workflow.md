# Phase 2 — Auth Workflow
# File Locations + Line-by-Line Code + Complete Flow
> Yeh file sirf Phase 2 ke liye hai — har cheez detail mein

---

## SARI FILES AUR UNKA KAAM

| File | Location | Kaam |
|------|----------|------|
| Models | `src/app/models/auth.models.ts` | Data ka blueprint (interfaces) |
| Auth Service | `src/app/core/services/auth.service.ts` | Login/Register/Logout API logic |
| Guard | `src/app/core/guards/auth.guard.ts` | Routes protect karna |
| Interceptor | `src/app/core/interceptors/auth.interceptor.ts` | Har request mein token auto-lagana |
| Login TS | `src/app/pages/auth/login/login.ts` | Login logic |
| Login HTML | `src/app/pages/auth/login/login.html` | Login form |
| Register TS | `src/app/pages/auth/register/register.ts` | Register logic |
| Register HTML | `src/app/pages/auth/register/register.html` | Register form |
| Profile TS | `src/app/pages/profile/profile.ts` | Profile load logic |
| Profile HTML | `src/app/pages/profile/profile.html` | Profile display |
| App Config | `src/app/app.config.ts` | Interceptor globally register |
| Routes | `src/app/app.routes.ts` | Guards routes pe lagaye |
| Header TS | `src/app/shared/components/header/header.ts` | AuthService inject |
| Header HTML | `src/app/shared/components/header/header.html` | Login/logout conditional nav |

---

## FILE 1 — `src/app/models/auth.models.ts`

**Kaam:** Backend ke saath data exchange karne ke liye TypeScript blueprints

```typescript
// LoginRequest — Login ke liye backend ko yeh bhejenge
export interface LoginRequest {
  email: string;      // string = text
  password: string;
}

// RegisterRequest — Register ke liye backend ko yeh bhejenge
export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
}

// AuthResponse — Backend login/register ke BAAD yeh wapas bhejta hai
export interface AuthResponse {
  userId: number;    // number = numeric
  fullName: string;
  email: string;
  role: string;      // "Customer", "Admin", ya "Washer"
  token: string;     // JWT Token — sabse important
  message: string;
}

// ProfileResponse — GET /api/auth/profile se milta hai
export interface ProfileResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  phone: string;
  message: string;
}
```

**Kyun interface banaya?**
- TypeScript ko pata hota hai ki backend se kya aayega
- Galat field likho → compile time par error (runtime se pehle)
- Backend ke DTOs se exactly match karta hai

---

## FILE 2 — `src/app/core/services/auth.service.ts`

**Kaam:** Poori auth logic — yeh Phase 2 ka BRAIN hai

```typescript
import { Injectable, signal, PLATFORM_ID, inject } from '@angular/core';
// Injectable   → yeh class Angular mein inject ho sakti hai
// signal       → reactive state ke liye
// PLATFORM_ID  → browser hai ya server? check karne ke liye
// inject       → Angular 15+ ka modern injection tarika

import { HttpClient } from '@angular/common/http';
// HttpClient → backend se baat karne ke liye (GET, POST etc)

import { isPlatformBrowser } from '@angular/common';
// isPlatformBrowser → SSR mein localStorage nahi hota, isliye check zaroori

import { Observable, tap } from 'rxjs';
// Observable → async data stream (HTTP response)
// tap        → data dekho, change mat karo, side effect karo

import { environment } from '../../../environments/environment';
// '../../../' = teen folders upar: services → core → app → src
// phir environments/environment.ts file milti hai
// environment.apiUrl = 'http://localhost:5001/api'

import { AuthResponse, LoginRequest, ProfileResponse, RegisterRequest }
  from '../../models/auth.models';
// '../../' = do folders upar: services → core → app
// phir models/auth.models.ts milti hai

@Injectable({ providedIn: 'root' })
// @Injectable → yeh class inject ho sakti hai
// providedIn: 'root' → Angular sirf EK instance banayega (Singleton)
// Singleton → ek hi AuthService poori app share kare

export class AuthService {

  private apiUrl = environment.apiUrl;
  // private  → sirf is class ke andar use hoga
  // apiUrl   → 'http://localhost:5001/api'

  private platformId = inject(PLATFORM_ID);
  // inject() → Angular ka modern way to get a dependency
  // PLATFORM_ID → Angular batata hai: browser hai ya server

  private http = inject(HttpClient);
  // HttpClient → HTTP requests karne ke liye

  currentUser = signal<AuthResponse | null>(this.loadUserFromStorage());
  // signal<Type>(initialValue) → reactive variable
  // AuthResponse | null → ya to user object hai ya null (not logged in)
  // loadUserFromStorage() → page refresh ke baad bhi user rahe

  private loadUserFromStorage(): AuthResponse | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    // SSR check — server pe localStorage nahi hota → null return karo

    const saved = localStorage.getItem('carwash_user');
    // localStorage.getItem() → browser storage se value nikalo

    return saved ? JSON.parse(saved) : null;
    // saved hai → JSON string ko object mein badlo (JSON.parse)
    // saved nahi → null return karo
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    // data: LoginRequest → sirf LoginRequest shape ka object accept karo
    // Observable<AuthResponse> → async response return hoga

    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, data)
    // http.post<T>(url, body) → POST request
    // Template literal: backtick + ${} se URL banao
    // → POST http://localhost:5001/api/auth/login

    .pipe(
      // .pipe() → Observable pe operators lagao (chain karo)
      tap(response => {
        // tap() → response dekho, change mat karo, side effect karo
        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('carwash_token', response.token);
          // Token alag save karo — Interceptor yahan se uthayega

          localStorage.setItem('carwash_user', JSON.stringify(response));
          // Poora user save karo — page refresh ke baad bhi login rahe
          // JSON.stringify() → object ko JSON string mein badlo
        }
        this.currentUser.set(response);
        // Signal update karo → Header automatically update hoga
        // .set() → signal ki value change karo
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, data);
    // Simple POST — register ke baad hum /login pe redirect karte hain
    // Token save nahi karte kyunki user ko login karna padega manually
  }

  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiUrl}/auth/profile`);
    // GET request — Interceptor automatically Bearer token add karega
    // Hume manually kuch nahi karna
  }

  logout(): void {
    // void → kuch return nahi karta
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('carwash_token');  // Token delete
      localStorage.removeItem('carwash_user');   // User data delete
    }
    this.currentUser.set(null);
    // Signal null → Header "Login/Register" dikhayega
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem('carwash_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
    // !! → double NOT — string ko boolean mein badlo
    // !!null = false   (not logged in)
    // !!"abc" = true   (logged in)
  }

  getUserRole(): string | null {
    return this.currentUser()?.role ?? null;
    // this.currentUser() → signal read karo (parentheses zaroori)
    // ?.           → optional chaining — null ho to crash mat karo
    // ?? null      → null/undefined ho to null return karo
  }
}
```

---

## FILE 3 — `src/app/core/interceptors/auth.interceptor.ts`

**Kaam:** Har outgoing HTTP request mein JWT token automatically lagana

```typescript
import { HttpInterceptorFn } from '@angular/common/http';
// HttpInterceptorFn → Angular 15+ ka functional interceptor type

import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
// req  → current HTTP request (readonly — change nahi kar sakte directly)
// next → function jo request ko aage server tak bhejta hai

  const platformId = inject(PLATFORM_ID);

  if (!isPlatformBrowser(platformId)) {
    return next(req);
    // Server (SSR) pe localStorage nahi — original request bhejo
  }

  const token = localStorage.getItem('carwash_token');
  // Token lo localStorage se

  if (token) {
    const authReq = req.clone({
    // req.clone() → request ki copy banao
    // HTTP Request immutable hai — directly modify nahi kar sakte

      headers: req.headers.set('Authorization', `Bearer ${token}`)
      // headers.set() → naya header add karo existing headers mein
      // 'Authorization' → header ka naam (backend is header ko check karta hai)
      // 'Bearer xyz'    → JWT ka standard format (Bearer keyword + token)
    });
    return next(authReq);
    // Modified request (token ke saath) aage bhejo
  }

  return next(req);
  // Token nahi → original request bhejo as-is
};
```

**Yeh kahan register hua?**

`src/app/app.config.ts` mein:
```typescript
provideHttpClient(withFetch(), withInterceptors([authInterceptor]))
//                             ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
//                             withInterceptors([]) → interceptors ki list
//                             ab har HTTP request se pehle authInterceptor chalega
```

---

## FILE 4 — `src/app/core/guards/auth.guard.ts`

**Kaam:** Routes protect karna — unauthorized access rokna

```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
// CanActivateFn → guard function ka type

import { AuthService } from '../services/auth.service';
// '../services/auth.service' → ek folder upar, phir services folder

// GUARD 1 — authGuard: sirf logged-IN users ke liye
export const authGuard: CanActivateFn = () => {
// Angular route activate hone se pehle yeh function call karta hai

  const authService = inject(AuthService);
  // inject() → AuthService ka existing singleton instance lo

  const router = inject(Router);
  // Router → navigate() ke liye

  if (authService.isLoggedIn()) {
    return true;
    // ✅ Login hai → access allow karo
  }

  return router.createUrlTree(['/login']);
  // ❌ Login nahi → /login pe redirect karo
  // createUrlTree → Angular isko URL redirect ke taur pe treat karta hai
};

// GUARD 2 — guestGuard: sirf logged-OUT users ke liye
export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return true;
    // ✅ Logout hai → login/register page dikhaao
  }

  return router.createUrlTree(['/']);
  // ❌ Pehle se login hai → home pe bhejo
  // Agar login hoke /login URL type karo → home pe redirect
};
```

**Routes mein kahan use hua?**

`src/app/app.routes.ts` mein:
```typescript
{ path: 'login',    canActivate: [guestGuard] }
// canActivate → route activate se PEHLE guard check karo

{ path: 'register', canActivate: [guestGuard] }

{ path: 'profile',  canActivate: [authGuard]  }
```

---

## FILE 5 — `src/app/pages/auth/login/login.ts`

**Kaam:** Login page ka TypeScript — form data, API call, redirect

```typescript
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
// FormsModule → [(ngModel)] aur form validation ke liye — import zaroori

import { Router, RouterLink } from '@angular/router';
// Router    → programmatically navigate karne ke liye
// RouterLink → HTML mein routerLink directive ke liye

import { AuthService } from '../../../core/services/auth.service';
// '../../../' → teen folders upar: login → auth → pages → app
// phir core/services/auth.service

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  // FormsModule import → template mein [(ngModel)] kaam karega
  // RouterLink import → template mein routerLink kaam karega
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {

  formData = {
    email: '',
    password: ''
  };
  // formData → template mein [(ngModel)] is object se bind hoga
  // User email field mein type kare → formData.email update hoga (two-way)

  isLoading = false;
  // true → button disabled, "Logging in..." dikhega

  errorMessage = '';
  // Backend se error aaye → yahan store karo → template mein dikhao

  constructor(
    private authService: AuthService,
    // private → sirf is class mein use hoga
    // Angular automatically AuthService ka instance dega (DI)
    private router: Router
  ) {}

  onSubmit(): void {
  // login.html mein: <form (ngSubmit)="onSubmit()">
  // Form submit hone par Angular yeh function call karta hai

    this.isLoading = true;     // Button disable karo
    this.errorMessage = '';    // Pehli error clear karo

    this.authService.login(this.formData).subscribe({
    // authService.login() → Observable return karta hai
    // .subscribe() → Observable activate karo → HTTP request jaati hai

      next: (response) => {
      // next: callback → success par chalta hai
        if (response.role === 'Admin') {
          this.router.navigate(['/admin']);
        } else if (response.role === 'Washer') {
          this.router.navigate(['/washer']);
        } else {
          this.router.navigate(['/']);
          // Customer → home pe bhejo
        }
      },

      error: (err) => {
      // error: callback → failure par chalta hai
        this.errorMessage = err.error?.message || 'Login failed.';
        // err.error        → backend ka response body
        // err.error?.message → agar message hai to use karo
        // ||               → agar nahi hai to default text
        this.isLoading = false;
        // Error par loading false karo → button dobara enable ho
      }
    });
  }
}
```

---

## FILE 6 — `src/app/pages/auth/login/login.html`

**Kaam:** Login form ka HTML — Angular bindings explain karein

```html
<form (ngSubmit)="onSubmit()" #loginForm="ngForm">
<!-- (ngSubmit) → form submit event (Enter ya button click) → onSubmit() call -->
<!-- #loginForm="ngForm" → form ka reference variable -->
<!--   loginForm.invalid → koi bhi required field invalid ho to true -->

  @if (errorMessage) {
  <!-- @if → Angular 17+ conditional -->
  <!-- errorMessage truthy (empty nahi) ho to dikhao -->
    <div class="error-message">{{ errorMessage }}</div>
    <!-- {{ errorMessage }} → interpolation — TypeScript variable HTML mein -->
  }

  <div class="form-group">
    <input
      type="email"
      name="email"
      <!-- name="email" → Angular form ko field identify karne ke liye zaroori -->

      [(ngModel)]="formData.email"
      <!-- [(ngModel)] → two-way binding -->
      <!--   User type kare → formData.email update hoga -->
      <!--   formData.email change ho → input update hoga -->

      required
      <!-- required → field khali nahi ho sakti (HTML5 + Angular validation) -->
      email
      <!-- email → valid email format chahiye -->

      #emailField="ngModel"
      <!-- #emailField → is input ka reference variable -->
      <!--   emailField.invalid → validation fail hai? -->
      <!--   emailField.touched → user ne field touch karke chora? -->
    />

    @if (emailField.invalid && emailField.touched) {
    <!-- sirf tab dikhao jab: invalid AND touched (dono conditions) -->
      <span class="field-error">Valid email is required</span>
    }
  </div>

  <button
    type="submit"
    [disabled]="isLoading || loginForm.invalid"
    <!-- [disabled] → property binding (square brackets) -->
    <!-- isLoading true YA form invalid → button disable -->
  >
    @if (isLoading) { Logging in... } @else { Login }
    <!-- Loading state mein text change karo -->
  </button>

</form>
```

---

## FILE 7 — `src/app/pages/auth/register/register.ts`

**Kaam:** Register logic — extra field `confirmPassword` sirf frontend ke liye

```typescript
formData = {
  fullName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: ''
  // confirmPassword → sirf frontend validation ke liye
  // backend ko nahi bhejenge (RegisterRequest mein nahi hai)
};

onSubmit(): void {
  // Frontend check — password match
  if (this.formData.password !== this.formData.confirmPassword) {
    this.errorMessage = 'Passwords do not match!';
    return;
    // return → function yahan rok do, backend call mat karo
  }

  // Backend ke liye sirf zaroori fields — confirmPassword choro
  const registerData = {
    fullName: this.formData.fullName,
    email: this.formData.email,
    phone: this.formData.phone,
    password: this.formData.password
    // confirmPassword yahan nahi — backend ki RegisterRequest mein nahi
  };

  this.authService.register(registerData).subscribe({
    next: () => {
      this.successMessage = 'Registration successful! Redirecting...';
      setTimeout(() => this.router.navigate(['/login']), 2000);
      // setTimeout → 2000ms (2 seconds) ke baad /login pe redirect
      // Pehle success message dikhaao, phir redirect
    },
    error: (err) => {
      this.errorMessage = err.error?.message || 'Registration failed.';
      this.isLoading = false;
    }
  });
}
```

---

## FILE 8 — `src/app/pages/profile/profile.ts`

**Kaam:** Protected page — backend se profile data fetch aur display

```typescript
export class ProfileComponent implements OnInit {
// implements OnInit → ngOnInit() method use karna chahte hain

  profile: ProfileResponse | null = null;
  // profile → shuru mein null, API se data aane ke baad set hoga

  isLoading = true;
  // shuru mein true → "Loading..." dikhega

  ngOnInit(): void {
  // Angular khud call karta hai — component ready hone ke baad
  // Yahan API calls karo, constructor mein nahi
    this.loadProfile();
  }

  loadProfile(): void {
    this.authService.getProfile().subscribe({
    // GET /api/auth/profile
    // Interceptor automatically Bearer token add karega
    // Backend token verify karega → profile data return karega
      next: (data) => {
        this.profile = data;      // Data store karo
        this.isLoading = false;   // Loading khatam
      },
      error: () => {
        this.errorMessage = 'Could not load profile.';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();         // Token delete, signal null
    this.router.navigate(['/login']);   // Login pe bhejo
  }
}
```

---

## FILE 9 — `src/app/app.config.ts` (Updated)

```typescript
// PHASE 1 mein tha:
provideHttpClient(withFetch())

// PHASE 2 mein update kiya:
provideHttpClient(withFetch(), withInterceptors([authInterceptor]))
//                             ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
//                             Interceptor globally register kiya
//                             Ab har HTTP request se pehle authInterceptor chalega
```

---

## FILE 10 — `src/app/app.routes.ts` (Updated)

```typescript
import { authGuard, guestGuard } from './core/guards/auth.guard';
// Guards import kiye

export const routes: Routes = [

  // PHASE 1 — public route
  { path: '', loadComponent: () => import('./pages/home/home')... },

  // PHASE 2 NEW — guestGuard: sirf logged-OUT users
  {
    path: 'login',
    canActivate: [guestGuard],
    // canActivate → route activate se pehle guard run karo
    // guestGuard: login nahi → allow, login hai → home redirect
    loadComponent: () => import('./pages/auth/login/login')
                         .then(m => m.LoginComponent)
    // lazy loading → sirf tab load ho jab user /login pe jaye
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/auth/register/register')
                         .then(m => m.RegisterComponent)
  },

  // PHASE 2 NEW — authGuard: sirf logged-IN users
  {
    path: 'profile',
    canActivate: [authGuard],
    // authGuard: login hai → allow, login nahi → /login redirect
    loadComponent: () => import('./pages/profile/profile')
                         .then(m => m.ProfileComponent)
  },

  { path: '**', redirectTo: '' }
];
```

---

## FILE 11 — `src/app/shared/components/header/header.ts` (Updated)

```typescript
export class HeaderComponent {

  authService = inject(AuthService);
  // inject() → modern Angular DI
  // authService → public (private nahi) kyunki template mein bhi use hoga
  //   template: authService.isLoggedIn(), authService.currentUser()

  private router = inject(Router);
  // private → template mein nahi, sirf logout() mein use

  logout(): void {
    this.authService.logout();          // Token delete, signal null karo
    this.router.navigate(['/login']);   // Login pe redirect
    this.closeMenu();                   // Mobile menu band karo
  }
}
```

---

## FILE 12 — `src/app/shared/components/header/header.html` (Updated)

```html
@if (authService.isLoggedIn()) {
<!-- authService.isLoggedIn() → localStorage mein token hai? -->
<!-- Logged IN state -->

  <a routerLink="/profile" class="nav-link">
    👤 {{ authService.currentUser()?.fullName }}
    <!--   authService.currentUser() → Signal read karo (parentheses zaroori) -->
    <!--   ?.fullName → null safety (agar null ho to crash nahi) -->
    <!--   Signal reactive hai → login hone par naam auto update hoga -->
  </a>

  <button (click)="logout()">Logout</button>
  <!-- (click) → event binding → logout() function call karo -->

} @else {
<!-- Logged OUT state -->

  <a routerLink="/login" class="btn btn-secondary">Login</a>
  <a routerLink="/register" class="btn btn-primary">Register</a>

}
```

---

## COMPLETE FLOW 1 — Register karna

```
User browser mein: localhost:4200/register
        ↓
FILE: src/app/app.routes.ts
  { path: 'register', canActivate: [guestGuard] }
  Angular route match karta hai
        ↓
FILE: src/app/core/guards/auth.guard.ts → guestGuard()
  authService.isLoggedIn() → false (login nahi)
  → return true → route allow
        ↓
FILE: src/app/pages/auth/register/register.ts
  RegisterComponent lazy load hota hai
        ↓
FILE: src/app/pages/auth/register/register.html
  Form render hota hai — sab fields empty
        ↓
User form fill karta hai
  [(ngModel)]="formData.fullName" → formData.fullName update hoti rehti hai
  [(ngModel)]="formData.email"   → formData.email update hoti rehti hai
  ... (har keystroke pe)
        ↓
User "Create Account" click karta hai
  <form (ngSubmit)="onSubmit()"> → onSubmit() call
        ↓
FILE: src/app/pages/auth/register/register.ts → onSubmit()
  1. Password match check: formData.password === formData.confirmPassword?
     No match → errorMessage set → STOP (return)
  2. registerData banao (confirmPassword exclude)
  3. authService.register(registerData).subscribe({...})
        ↓
FILE: src/app/core/services/auth.service.ts → register()
  this.http.post('/api/auth/register', data)
  Observable create hoti hai
  .subscribe() ne activate kiya → HTTP request jaati hai
        ↓
FILE: src/app/core/interceptors/auth.interceptor.ts
  token = localStorage.getItem('carwash_token') → null
  if(token) → false → original request jaati hai (no token yet)
        ↓
Backend: POST /api/auth/register → 201 Created
  Response: { userId, fullName, email, role, token, message }
        ↓
FILE: src/app/pages/auth/register/register.ts → next() callback
  successMessage = 'Registration successful!'
  setTimeout(2000) → router.navigate(['/login'])
```

---

## COMPLETE FLOW 2 — Login karna

```
User: /login pe jaata hai
        ↓
FILE: src/app/app.routes.ts
  { path: 'login', canActivate: [guestGuard] }
        ↓
FILE: src/app/core/guards/auth.guard.ts → guestGuard()
  isLoggedIn() → false → allow
        ↓
FILE: src/app/pages/auth/login/login.html
  Form render hota hai
        ↓
User email + password type karta hai
  [(ngModel)]="formData.email" → formData.email update
  [(ngModel)]="formData.password" → formData.password update
        ↓
Login button click
  <form (ngSubmit)="onSubmit()"> → onSubmit() call
        ↓
FILE: src/app/pages/auth/login/login.ts → onSubmit()
  isLoading = true → button disable
  authService.login(this.formData).subscribe({...})
        ↓
FILE: src/app/core/services/auth.service.ts → login()
  http.post('/api/auth/login', data).pipe(tap(...))
  Observable activate → request jaati hai
        ↓
FILE: src/app/core/interceptors/auth.interceptor.ts
  token = null (abhi login nahi hua)
  → original request (no Authorization header)
        ↓
Backend: POST /api/auth/login
  email + password validate karta hai
  JWT token generate karta hai
  Response: { userId, fullName, email, role:"Customer", token:"eyJ..." }
        ↓
FILE: src/app/core/services/auth.service.ts → tap() runs
  localStorage.setItem('carwash_token', response.token)
    → Token save kiya — ab Interceptor use karega
  localStorage.setItem('carwash_user', JSON.stringify(response))
    → User save kiya — page refresh ke baad bhi login rahe
  this.currentUser.set(response)
    → Signal update kiya!
        ↓
FILE: src/app/shared/components/header/header.html
  @if (authService.isLoggedIn()) → ab true!
  {{ authService.currentUser()?.fullName }} → naam dikhega
  Signal reactive hai → Angular automatically re-render karta hai
        ↓
FILE: src/app/pages/auth/login/login.ts → next() callback
  response.role === 'Customer' → router.navigate(['/'])
  Home page pe redirect!
```

---

## COMPLETE FLOW 3 — Protected Route (/profile)

```
User: /profile URL type karta hai
        ↓
FILE: src/app/app.routes.ts
  { path: 'profile', canActivate: [authGuard] }
        ↓
FILE: src/app/core/guards/auth.guard.ts → authGuard()
  authService.isLoggedIn()
    ↓
  FILE: src/app/core/services/auth.service.ts → isLoggedIn()
    !!localStorage.getItem('carwash_token')
    Token hai → true → !! → true
    Token nahi → null → !! → false
        ↓
  [Token hai] → return true → route allow
  [Token nahi] → router.createUrlTree(['/login']) → redirect
        ↓
FILE: src/app/pages/profile/profile.ts
  ProfileComponent load hota hai
  ngOnInit() → loadProfile() call
        ↓
FILE: src/app/core/services/auth.service.ts → getProfile()
  http.get('/api/auth/profile')
  Observable activate
        ↓
FILE: src/app/core/interceptors/auth.interceptor.ts
  token = 'eyJ...' (localStorage mein hai)
  if(token) → true
  authReq = req.clone({ Authorization: 'Bearer eyJ...' })
  → Modified request with token aage jaati hai
        ↓
Backend: GET /api/auth/profile
  Authorization header check karta hai
  Token valid → profile data return karta hai
  Response: { userId, fullName, email, role, phone }
        ↓
FILE: src/app/pages/profile/profile.ts → next() callback
  this.profile = data      → data store
  this.isLoading = false   → loading khatam
        ↓
FILE: src/app/pages/profile/profile.html
  @if (profile && !isLoading) { ... }
  → profile null nahi, loading false → content dikhao
  {{ profile.fullName }}, {{ profile.email }} etc display hota hai
```

---

## COMPLETE FLOW 4 — Logout karna

```
User: Header mein "Logout" button click karta hai
        ↓
FILE: src/app/shared/components/header/header.html
  <button (click)="logout()">Logout</button>
        ↓
FILE: src/app/shared/components/header/header.ts → logout()
  this.authService.logout()      → AuthService logout call
  this.router.navigate(['/login']) → Login page pe bhejo
        ↓
FILE: src/app/core/services/auth.service.ts → logout()
  localStorage.removeItem('carwash_token')  → Token delete
  localStorage.removeItem('carwash_user')   → User data delete
  this.currentUser.set(null)                → Signal null karo
        ↓
FILE: src/app/shared/components/header/header.html
  @if (authService.isLoggedIn()) → ab false!
  → @else block → Login + Register buttons dikhte hain
  Signal reactive — Angular automatically re-renders
        ↓
router.navigate(['/login']) → Login page dikhta hai
```

---

## SARI FILES KA CONNECTION MAP

```
src/app/
│
├── app.config.ts
│   └── withInterceptors([authInterceptor])
│             ↑ registered here
│
├── app.routes.ts
│   ├── canActivate: [guestGuard]  → login, register
│   └── canActivate: [authGuard]   → profile
│
├── models/
│   └── auth.models.ts
│       ├── LoginRequest    → auth.service.ts mein use
│       ├── RegisterRequest → auth.service.ts mein use
│       ├── AuthResponse    → auth.service.ts return type + signal type
│       └── ProfileResponse → auth.service.ts return type
│
├── core/
│   ├── services/
│   │   └── auth.service.ts  ← CENTRAL HUB
│   │       ├── login()      ← login.ts subscribe karta hai
│   │       ├── register()   ← register.ts subscribe karta hai
│   │       ├── getProfile() ← profile.ts subscribe karta hai
│   │       ├── logout()     ← header.ts call karta hai
│   │       ├── isLoggedIn() ← guard + header use karta hai
│   │       └── currentUser  ← signal — header template read karta hai
│   │
│   ├── guards/
│   │   └── auth.guard.ts
│   │       ├── authGuard  → profile route pe canActivate
│   │       └── guestGuard → login/register route pe canActivate
│   │
│   └── interceptors/
│       └── auth.interceptor.ts
│           └── har HTTP request se pehle run hota hai
│               token → Authorization header add karo
│
├── shared/components/header/
│   ├── header.ts    → authService inject, logout()
│   └── header.html  → @if(isLoggedIn) naam | @else Login/Register
│
└── pages/
    ├── auth/login/
    │   ├── login.ts    → authService.login().subscribe()
    │   └── login.html  → [(ngModel)] form, (ngSubmit)
    ├── auth/register/
    │   ├── register.ts    → authService.register().subscribe()
    │   └── register.html  → [(ngModel)] form, confirmPassword
    └── profile/
        ├── profile.ts    → ngOnInit → authService.getProfile().subscribe()
        └── profile.html  → @if(profile) data display
```

---

*Phase 2 Auth Workflow — Complete*
*Next: Phase 3 — Services & Add-Ons*
