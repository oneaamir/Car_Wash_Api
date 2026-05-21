# CarWash Frontend - Learning Roadmap & Notes
> Angular 21 | CSS | Beginner to Intermediate | Hinglish Notes

---

## TABLE OF CONTENTS
1. [Backend Analysis Summary](#backend-analysis)
2. [Project Structure](#project-structure)
3. [Complete Roadmap](#roadmap)
4. [Phase 1 - Setup & Layout](#phase-1) ✅ COMPLETED
5. [Phase 2 - Authentication](#phase-2) ✅ COMPLETED
6. [Phase 3 - Services & Add-Ons](#phase-3) ✅ COMPLETED
7. [Phase 4 - Cars Management](#phase-4) ✅ COMPLETED
8. [Phase 5 - Bookings](#phase-5) ✅ COMPLETED
9. [Phase 6 - Payments & Receipts](#phase-6) ✅ COMPLETED
10. [Phase 7 - Reviews](#phase-7) ⏳ PENDING
11. [Phase 8 - Admin Panel](#phase-8) ⏳ PENDING
12. [Phase 9 - Washer Panel](#phase-9) ⏳ PENDING
13. [Angular Concepts Dictionary](#concepts)

---

## BACKEND ANALYSIS SUMMARY {#backend-analysis}

Backend: ASP.NET Core 8.0 Web API
Base URL: http://localhost:5001/api

### API Endpoints Overview:
| Module | Endpoint | Auth Required |
|--------|----------|---------------|
| Auth | /api/auth/register, /api/auth/login, /api/auth/profile | No/Yes |
| Cars | /api/cars/my, /api/cars/{id} | Customer |
| Services | /api/serviceplans | No (GET), Admin (POST/PUT/DELETE) |
| Add-Ons | /api/addons | No (GET), Admin (POST/PUT/DELETE) |
| Bookings | /api/bookings/my | Customer |
| Payments | /api/payments | Customer |
| Receipts | /api/receipts | Customer |
| Reviews | /api/reviews | No (GET), Customer (POST) |
| Admin | /api/admin/* | Admin |
| Washers | /api/washers/* | Washer |
| Reports | /api/reports/* | Admin |

### User Roles:
- **Customer** - Apni cars manage karo, bookings karo, payments karo
- **Admin** - Sab kuch manage karo
- **Washer** - Assigned bookings dekho, status update karo

### JWT Token:
- Login karo → Token milta hai (60 min valid)
- Token ko LocalStorage mein save karenge
- Har API call mein Header mein bhejenge: `Authorization: Bearer <token>`

---

## PROJECT STRUCTURE {#project-structure}

```
carwash-frontend/
├── src/
│   ├── app/
│   │   ├── core/                    ← Business logic (services, guards)
│   │   │   ├── services/            ← API call karne wali services
│   │   │   │   ├── auth.service.ts      (Phase 2)
│   │   │   │   ├── car.service.ts       (Phase 4)
│   │   │   │   ├── booking.service.ts   (Phase 5)
│   │   │   │   └── ...
│   │   │   ├── guards/              ← Route protection
│   │   │   │   └── auth.guard.ts        (Phase 2)
│   │   │   └── interceptors/        ← HTTP ke beech mein kaam karne wale
│   │   │       └── auth.interceptor.ts  (Phase 2)
│   │   ├── shared/                  ← Reusable components
│   │   │   └── components/
│   │   │       ├── header/          ← Navigation bar ✅
│   │   │       └── footer/          ← Footer ✅
│   │   ├── pages/                   ← Page components (routes)
│   │   │   ├── home/                ← Home page ✅
│   │   │   ├── auth/                ← Login/Register (Phase 2)
│   │   │   ├── services/            ← Service plans list (Phase 3)
│   │   │   ├── cars/                ← My cars (Phase 4)
│   │   │   ├── bookings/            ← My bookings (Phase 5)
│   │   │   ├── payments/            ← Payments (Phase 6)
│   │   │   ├── admin/               ← Admin panel (Phase 8)
│   │   │   └── washer/              ← Washer panel (Phase 9)
│   │   ├── app.ts                   ← Root component ✅
│   │   ├── app.html                 ← Root template ✅
│   │   ├── app.routes.ts            ← All routes ✅
│   │   └── app.config.ts            ← App configuration ✅
│   ├── environments/
│   │   ├── environment.ts           ← Dev config (API URL) ✅
│   │   └── environment.prod.ts      ← Prod config ✅
│   ├── styles.scss                  ← Global CSS ✅
│   └── index.html                   ← Main HTML file ✅
```

---

## COMPLETE ROADMAP {#roadmap}

### Phase 1: Project Setup & Layout ✅
- Folder structure setup
- Header & Footer components
- Home page
- Routing setup
- Global CSS variables
- Environment configuration

### Phase 2: Authentication ✅
- Register page (form validation)
- Login page
- Auth Service (API calls)
- JWT token store in localStorage
- Auth Guard (routes protect karna)
- HTTP Interceptor (har request mein token attach karna)
- Profile page

### Phase 3: Services & Add-Ons ✅
- Service Plans listing page (public — no login needed)
- Add-Ons listing page (public)
- Loading skeleton animations
- Empty state handling
- Error state handling

### Phase 4: Cars Management ⏳
- My Cars page
- Add Car form
- Edit Car
- Delete Car

### Phase 5: Bookings ✅
- Create Booking (multi-step: select service → select car → confirm)
- My Bookings list
- Booking details
- Cancel booking

### Phase 6: Payments & Receipts ✅
- Make Payment for a booking
- View Receipt
- Payment history

### Phase 7: Reviews ⏳
- Write a review after booking
- View all reviews

### Phase 8: Admin Panel ⏳
- Admin dashboard
- Manage users
- Manage all bookings
- Manage service plans (CRUD)
- Manage add-ons (CRUD)
- Manage promo codes (CRUD)
- Booking reports
- Revenue reports

### Phase 9: Washer Panel ⏳
- View assigned bookings
- Update booking status

---

## PHASE 1 - PROJECT SETUP & LAYOUT {#phase-1}
**Status: ✅ COMPLETED**

---

### Phase 1 Mein Kya Banaya?

1. **Folder Structure** - Project organized kiya
2. **Environment Files** - API URL configure kiya
3. **Global Styles** - CSS variables and common styles
4. **Header Component** - Navigation bar with routing
5. **Footer Component** - App footer
6. **Home Page** - Landing page with hero, stats, features
7. **App Layout** - Header + Router Outlet + Footer
8. **Routing Setup** - Home page route

---

### FILES EXPLAINED IN DETAIL

---

#### 1. `src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5001/api'
};
```

**Kya hai yeh?**
Environment files woh jagah hain jahan hum apni app ki settings store karte hain jo 
different environments (development vs production) mein alag hoti hain.

**Kyun use kiya?**
- `apiUrl` yahan store kiya taaki ek jagah se badal sakein
- Development mein: localhost:5001
- Production mein: actual server URL

**Kaise use hoga?**
Services mein import karenge:
```typescript
import { environment } from '../../environments/environment';
const url = environment.apiUrl + '/auth/login';
```

---

#### 2. `src/app/app.config.ts`
```typescript
export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideRouter(routes),
    provideHttpClient(withFetch()),
    provideClientHydration(withEventReplay())
  ]
};
```

**Kya hai yeh?**
Yeh Angular ka main configuration file hai. `providers` array mein hum Angular ko
batate hain ki kaunsi services/features globally available honi chahiye.

**Keyword Explanation:**
- `ApplicationConfig` = Interface jo config ka shape define karta hai
- `providers` = Array of Angular providers (services jo inject ho sakti hain)
- `provideBrowserGlobalErrorListeners()` = Browser errors ko catch karta hai
- `provideRouter(routes)` = Routing system enable karta hai
- `provideHttpClient(withFetch())` = HTTP calls karne ke liye (backend se baat)
- `provideClientHydration()` = SSR (Server Side Rendering) ke liye

**provideHttpClient kya hai?**
Jab hum backend se data lena ho (GET/POST/PUT/DELETE), toh Angular ka HttpClient 
use karte hain. Use karne ke liye yahan `provideHttpClient()` add karna zaroori hai.
Bina iske HttpClient inject nahi hoga.

---

#### 3. `src/app/app.routes.ts`
```typescript
export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent)
  },
  {
    path: '**',
    redirectTo: ''
  }
];
```

**Kya hai Routing?**
Angular ek SPA (Single Page Application) hai. Matlab page reload nahi hota, sirf
content change hota hai. Routing system manage karta hai ki kaunsi URL pe kaunsa
component dikhega.

**Route Object ka Anatomy:**
```
{
  path: 'home',           ← URL ka path (localhost:4200/home)
  loadComponent: () =>    ← Lazy loading (tabhi load ho jab jarurat ho)
    import('./pages/home/home')   ← File ko dynamically import karo
    .then(m => m.HomeComponent)   ← us file se HomeComponent lo
}
```

**Lazy Loading kyun?**
- Bina lazy loading: App start hone par sab components load ho jaate hain (slow)
- Lazy loading ke saath: Component tabhi load hoga jab user us page pe jaye (fast)

**`**` (double asterisk) kya hai?**
Wildcard route - koi bhi URL match ho jaaye jo pehle define nahi hua.
Yeh hamesha last mein rakhte hain.

---

#### 4. `src/app/app.ts` (Root Component)
```typescript
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {}
```

**Decorator kya hota hai?**
`@Component` ek decorator hai. TypeScript mein decorator `@` se shuru hota hai.
Yeh class ko extra metadata deta hai.

**selector:**
- `selector: 'app-root'` = Yeh component HTML mein `<app-root>` tag se use hoga
- `index.html` mein `<app-root></app-root>` hai - wahan yeh component render hoga

**imports array (Standalone Components):**
Angular 17+ mein NgModules ki jagah standalone components use hote hain.
Har component apne imports khud declare karta hai.
- `RouterOutlet` = router-outlet tag ke liye
- `HeaderComponent` = `<app-header>` tag ke liye
- `FooterComponent` = `<app-footer>` tag ke liye

**Agar import na karo to kya hoga?**
Error aayega: "app-header is not a known element"

---

#### 5. `src/app/app.html` (Root Template)
```html
<app-header></app-header>

<main class="main-content">
  <router-outlet></router-outlet>
</main>

<app-footer></app-footer>
```

**router-outlet kya hai?**
Yeh Angular ka placeholder hai. Jis URL pe ho, us URL ka component yahan render hoga.
- `/` pe → HomeComponent yahan dikhega
- `/login` pe → LoginComponent yahan dikhega (Phase 2 mein)

Socho jaise ek "frame" hai. Header aur Footer fix hai, sirf beech wala content badalta hai.

---

#### 6. `src/styles.scss` (Global Styles)

**CSS Variables kya hain?**
```css
:root {
  --primary-color: #1a73e8;
}
```
`:root` = poori document ka root element (html tag).
`--primary-color` = CSS variable (double hyphen se shuru hota hai).
Use karna: `color: var(--primary-color);`

**Faida:** Ek jagah color change karo, poori app mein change ho jaayega.

---

#### 7. Header Component

**RouterLink kya hai?**
```html
<a routerLink="/">Home</a>
```
Normal `<a href="/">` se page reload hota hai. `routerLink` se Angular routing use hoti hai
- no page reload, smooth navigation.

**RouterLinkActive kya hai?**
```html
<a routerLink="/" routerLinkActive="active">Home</a>
```
Jab current URL is link ke path se match kare, tab `active` CSS class automatically add ho jaati hai.

**Event Binding:**
```html
<button (click)="toggleMenu()">Menu</button>
```
`(click)` = Angular event binding. Parentheses = event.
Button click hone par `toggleMenu()` function call hoga.

**Class Binding:**
```html
<nav [class.open]="isMenuOpen">
```
`[class.open]` = Angular class binding. Square brackets = property binding.
`isMenuOpen` true ho to `open` class add ho jaaye, false ho to remove ho jaaye.

---

#### 8. Home Component

**TypeScript Array in Component:**
```typescript
features = [
  { icon: '🚗', title: 'Basic Wash', description: '...' },
  ...
];
```
Yeh TypeScript class property hai - ek array of objects.
HTML mein is array ko loop karke display karenge.

**@for loop (Angular 17+):**
```html
@for (feature of features; track feature.title) {
  <div>{{ feature.title }}</div>
}
```
- `@for` = Angular ka new template syntax for loops
- `feature of features` = features array se ek ek item nikalo, naam do "feature"
- `track feature.title` = Angular ko batao ki unique identifier kya hai (performance)
- `{{ feature.title }}` = Interpolation - TypeScript variable HTML mein dikhao

**Purana syntax (ngFor):**
```html
<div *ngFor="let feature of features">{{ feature.title }}</div>
```
Dono kaam karte hain, but naya @for syntax Angular 17+ ka preferred style hai.

---

### ANGULAR FLOW - Phase 1 Summary

```
Browser URL: localhost:4200/
         ↓
index.html loads
         ↓
<app-root> tag milta hai
         ↓
app.ts (App component) load hota hai
         ↓
app.html render hota hai:
  <app-header> → HeaderComponent
  <router-outlet> → Route check karo
  <app-footer> → FooterComponent
         ↓
Routes check: path '' → HomeComponent
         ↓
HomeComponent lazy load hota hai
         ↓
Home page dikhti hai
```

---

### COMPONENT COMMUNICATION (Phase 1 mein simple hai)

Abhi Phase 1 mein koi component-to-component communication nahi hai.
Sirf routing based navigation hai.

Phase 2 mein seekhenge:
- **@Input()** = Parent se Child ko data dena
- **@Output()** = Child se Parent ko event bhejna
- **Services** = Alag components ke beech data share karna

---

### FOLDER STRUCTURE - Kyun Yeh Structure?

```
core/       ← App-wide singleton services (sirf ek instance hoga)
shared/     ← Reusable components (kai jagah use ho sakti hain)
pages/      ← Page-level components (ek ek route ke liye)
```

**Rules:**
- `core/` mein services aur guards jaate hain - woh Angular ke root level pe inject hote hain
- `shared/` mein header, footer, buttons jaisi cheezein jaati hain - jo kai pages pe use honi hain
- `pages/` mein sirf page components - jo routes pe map hote hain

---

## PHASE 2 - AUTHENTICATION {#phase-2}
**Status: ✅ COMPLETED**

---

### PHASE 2 MEIN KYA KYA BANAYA — COMPLETE FILE LIST

#### Naye Files (New):
| File Location | Kaam (Purpose) |
|---|---|
| `src/app/models/auth.models.ts` | TypeScript interfaces — data ka blueprint |
| `src/app/core/services/auth.service.ts` | Backend se baat karna — login/register/logout logic |
| `src/app/core/guards/auth.guard.ts` | Routes protect karna — darwaan |
| `src/app/core/interceptors/auth.interceptor.ts` | Har request mein auto token lagana |
| `src/app/pages/auth/login/login.ts` | Login page ka logic |
| `src/app/pages/auth/login/login.html` | Login page ka HTML form |
| `src/app/pages/auth/login/login.scss` | Login page ki styling |
| `src/app/pages/auth/register/register.ts` | Register page ka logic |
| `src/app/pages/auth/register/register.html` | Register page ka HTML form |
| `src/app/pages/auth/register/register.scss` | Register page ki styling |
| `src/app/pages/profile/profile.ts` | Profile page ka logic |
| `src/app/pages/profile/profile.html` | Profile page ka HTML |
| `src/app/pages/profile/profile.scss` | Profile page ki styling |

#### Updated Files (Purane files mein changes):
| File Location | Kya Badla |
|---|---|
| `src/app/app.config.ts` | Interceptor add kiya |
| `src/app/app.routes.ts` | Login, Register, Profile routes add kiye |
| `src/app/shared/components/header/header.ts` | AuthService inject kiya |
| `src/app/shared/components/header/header.html` | Logged-in/out conditional nav |

---

### PHASE 2 — HAR FILE KI DETAIL (Location + Line-by-Line)

---

#### FILE 1 — `src/app/models/auth.models.ts`
**Kaam:** Backend ke saath data exchange karne ke liye TypeScript blueprints (interfaces)

```typescript
// LINE 1-3: LoginRequest interface
// Jab user login kare, backend ko yeh 2 cheezein bhejni hain
export interface LoginRequest {
  email: string;      // string = text value
  password: string;   // string = text value
}

// LINE 5-10: RegisterRequest interface
// Jab user register kare, backend ko yeh 4 cheezein bhejni hain
export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
}

// LINE 12-19: AuthResponse interface
// Backend login/register ke BAAD yeh data wapas bhejta hai
export interface AuthResponse {
  userId: number;    // number = numeric value
  fullName: string;
  email: string;
  role: string;      // "Customer", "Admin", ya "Washer"
  token: string;     // JWT Token — yeh sab se important hai
  message: string;   // "Login successful" jaisi message
}

// LINE 21-28: ProfileResponse interface
// GET /api/auth/profile se yeh data milta hai
export interface ProfileResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  phone: string;
  message: string;
}
```

**Interface kyun banaya?**
Bina interface ke TypeScript ko nahi pata ki backend se kya aayega. Interface se:
- Autocomplete milti hai editor mein
- Galat field name likho to compile-time error aata hai (runtime se pehle)
- Backend ke DTOs se exactly match karta hai

---

#### FILE 2 — `src/app/core/services/auth.service.ts`
**Kaam:** Poori auth logic — login, register, logout, token store, current user state

```typescript
// LINE 1: Injectable decorator import
import { Injectable, signal, PLATFORM_ID, inject } from '@angular/core';
// Injectable = yeh class Angular mein inject ho sakti hai
// signal = reactive state ke liye
// PLATFORM_ID = browser hai ya server? check karne ke liye
// inject = Angular 15+ ka modern injection tarika

// LINE 2: HttpClient import — backend se baat karne ke liye
import { HttpClient } from '@angular/common/http';

// LINE 3: isPlatformBrowser import — server pe localStorage nahi hota
import { isPlatformBrowser } from '@angular/common';

// LINE 4: Observable aur tap import (RxJS se)
import { Observable, tap } from 'rxjs';
// Observable = async data stream (HTTP response)
// tap = data dekho, change mat karo, side effect karo

// LINE 5: Environment se API URL lo
import { environment } from '../../../environments/environment';
// '../../../' = teen folders upar jao (services → core → app → src)
// Tab 'environments/environment' folder milta hai

// LINE 6: Interfaces import karo
import { AuthResponse, LoginRequest, ProfileResponse, RegisterRequest }
  from '../../models/auth.models';
// '../../' = do folders upar (services → core → app)
// Tab 'models/auth.models' milta hai

// LINE 8: @Injectable decorator
@Injectable({ providedIn: 'root' })
// providedIn: 'root' = Angular poori app mein sirf EK instance banayega
// Singleton pattern — ek hi AuthService sab components share karein

export class AuthService {

  // LINE 10: API base URL
  private apiUrl = environment.apiUrl;
  // private = sirf is class ke andar use ho sakta hai
  // environment.apiUrl = 'http://localhost:5001/api'

  // LINE 12: PLATFORM_ID inject karo
  private platformId = inject(PLATFORM_ID);
  // inject() = Angular 15+ ka functional way to inject services
  // PLATFORM_ID = Angular ko pata hai ki app browser mein hai ya server pe

  // LINE 13: HttpClient inject karo
  private http = inject(HttpClient);
  // HttpClient = Angular ka HTTP library — GET, POST, PUT, DELETE karne ke liye

  // LINE 15: Signal — current logged-in user ka reactive state
  currentUser = signal<AuthResponse | null>(this.loadUserFromStorage());
  // signal<Type>(initialValue) = reactive variable banao
  // AuthResponse | null = ya to user object hai ya null (not logged in)
  // this.loadUserFromStorage() = page refresh ke baad bhi user rehta hai

  // LINE 17-21: localStorage se user load karo (private method)
  private loadUserFromStorage(): AuthResponse | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    // isPlatformBrowser check — SSR (server) pe localStorage nahi hota
    // Server pe null return karo
    const saved = localStorage.getItem('carwash_user');
    // localStorage.getItem() = browser storage se value nikalo
    return saved ? JSON.parse(saved) : null;
    // saved hai to JSON parse karo, nahi hai to null return karo
    // JSON.parse() = JSON string ko JavaScript object mein badlo
  }

  // LINE 23-35: LOGIN method
  login(data: LoginRequest): Observable<AuthResponse> {
    // data: LoginRequest = sirf LoginRequest shape ka object accept karo
    // Observable<AuthResponse> = async response return hoga
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, data)
    // this.http.post<T>(url, body) = POST request bhejo
    // Template literal: backtick se URL banao, ${} mein variable
    // → POST http://localhost:5001/api/auth/login
    .pipe(
      // .pipe() = Observable pe operators lagao
      tap(response => {
        // tap() = response ko dekho, change mat karo
        if (isPlatformBrowser(this.platformId)) {
          localStorage.setItem('carwash_token', response.token);
          // Token alag save karo — Interceptor yahan se uthayega
          localStorage.setItem('carwash_user', JSON.stringify(response));
          // Poora user object save karo — page refresh ke baad bhi login rahe
          // JSON.stringify() = object ko JSON string mein badlo
        }
        this.currentUser.set(response);
        // Signal update karo → Header automatically update hoga
        // .set() = signal ki value change karo
      })
    );
  }

  // LINE 37-39: REGISTER method
  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, data);
    // Simple POST request — koi tap() nahi kyunki register ke baad
    // hum directly login page pe redirect karte hain
  }

  // LINE 41-43: GET PROFILE method (JWT required)
  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiUrl}/auth/profile`);
    // GET request — Interceptor automatically Bearer token add karega
    // Hume manually token add nahi karna — Interceptor ka kaam hai
  }

  // LINE 45-51: LOGOUT method
  logout(): void {
    // void = yeh function kuch return nahi karta
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem('carwash_token');   // Token delete karo
      localStorage.removeItem('carwash_user');    // User data delete karo
    }
    this.currentUser.set(null);
    // Signal null karo → Header "Login/Register" buttons dikhayega
  }

  // LINE 53-55: Token getter
  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return localStorage.getItem('carwash_token');
    // Token hai to return karo, nahi hai to null
  }

  // LINE 57-59: isLoggedIn check
  isLoggedIn(): boolean {
    return !!this.getToken();
    // !! = double NOT — string ko boolean mein convert karo
    // !!null = false (not logged in)
    // !!"abc123" = true (logged in)
  }

  // LINE 61-63: User ka role
  getUserRole(): string | null {
    return this.currentUser()?.role ?? null;
    // this.currentUser() = signal read karo (parentheses lagao)
    // ?. = optional chaining — null ho to crash mat karo
    // ?? = nullish coalescing — null/undefined ho to null return karo
  }
}
```

---

#### FILE 3 — `src/app/core/interceptors/auth.interceptor.ts`
**Kaam:** Har outgoing HTTP request mein automatically JWT token lagana

```typescript
// LINE 1: HttpInterceptorFn = Angular 15+ ka functional interceptor type
import { HttpInterceptorFn } from '@angular/common/http';
import { inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

// LINE 5: Interceptor function — Angular khud call karta hai har request pe
export const authInterceptor: HttpInterceptorFn = (req, next) => {
// req = current HTTP request (readonly)
// next = function jo request ko aage bhejta hai (next middleware/server)

  const platformId = inject(PLATFORM_ID);

  // LINE 8: SSR check — server pe localStorage nahi
  if (!isPlatformBrowser(platformId)) {
    return next(req);  // Server pe original request aage bhejo
  }

  // LINE 12: localStorage se token lo
  const token = localStorage.getItem('carwash_token');

  // LINE 14: Token hai to request modify karo
  if (token) {
    const authReq = req.clone({
    // req.clone() = request ki copy banao
    // HTTP Request immutable hoti hai — directly modify nahi kar sakte
      headers: req.headers.set('Authorization', `Bearer ${token}`)
      // headers.set() = naya header add karo
      // 'Authorization' = header ka naam (backend is header ko check karta hai)
      // 'Bearer xyz123' = JWT ka standard format
      // Bearer = keyword, token = actual JWT string
    });
    return next(authReq);  // Modified request aage bhejo
  }

  return next(req);  // Token nahi → original request bhejo as-is
};
```

**Yeh kahan register hua?**
`src/app/app.config.ts` mein:
```typescript
provideHttpClient(withFetch(), withInterceptors([authInterceptor]))
//                                              ^^^^^^^^^^^^^^^^^^^
//                                              Yahan register kiya
```

---

#### FILE 4 — `src/app/core/guards/auth.guard.ts`
**Kaam:** Routes protect karna — unauthorized access rokna

```typescript
import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
// CanActivateFn = guard function ka type

import { AuthService } from '../services/auth.service';

// GUARD 1: authGuard — sirf logged-IN users ke liye
export const authGuard: CanActivateFn = () => {
// () = route activate hone se pehle Angular yeh function call karta hai

  const authService = inject(AuthService);
  // inject() = AuthService ka existing instance lo (nayi nahi banao)

  const router = inject(Router);
  // Router = navigate karne ke liye

  if (authService.isLoggedIn()) {
    return true;  // ✅ Logged in hai — access allow karo
  }

  return router.createUrlTree(['/login']);
  // ❌ Logged in nahi — /login URL tree banao
  // Angular is UrlTree ko dekh ke /login pe redirect karta hai
};

// GUARD 2: guestGuard — sirf logged-OUT users ke liye (login/register pages)
export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isLoggedIn()) {
    return true;  // ✅ Logout hai — login/register page dikhaao
  }

  return router.createUrlTree(['/']);
  // ❌ Pehle se login hai — home pe bhejo
  // Agar login hai aur /login URL type karo to home pe redirect ho
};
```

**Guards kahan use hue?**
`src/app/app.routes.ts` mein:
```typescript
{ path: 'login',   canActivate: [guestGuard], loadComponent: ... }
{ path: 'register',canActivate: [guestGuard], loadComponent: ... }
{ path: 'profile', canActivate: [authGuard],  loadComponent: ... }
//                 ^^^^^^^^^^^^^^^^^^^^^^^^^^^
//                 Route activate hone se pehle guard check hota hai
```

---

#### FILE 5 — `src/app/pages/auth/login/login.ts`
**Kaam:** Login page ka TypeScript logic — form data, API call, redirect

```typescript
import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
// FormsModule = [(ngModel)] aur form validation ke liye

import { Router, RouterLink } from '@angular/router';
// Router = programmatically navigate karne ke liye
// RouterLink = HTML mein routerLink directive ke liye

import { AuthService } from '../../../core/services/auth.service';
// '../../../' = teen folders upar: login → auth → pages → app
// Tab 'core/services/auth.service' milta hai

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  // FormsModule import karo → [(ngModel)] template mein kaam karega
  templateUrl: './login.html',
  styleUrl: './login.scss'
})
export class LoginComponent {

  // Form data object — [(ngModel)] in properties se bind hoga
  formData = {
    email: '',      // Initial value empty string
    password: ''    // Initial value empty string
  };
  // Jab user email field mein type kare → formData.email update hoga
  // Yeh two-way binding hai

  isLoading = false;     // true hone par button disable hoga
  errorMessage = '';     // Backend se error aaye to yahan store karo

  constructor(
    private authService: AuthService,
    // private = sirf is class mein use ho sakta hai
    // Angular automatically AuthService ka instance dega (DI)
    private router: Router
    // Router = navigate() function ke liye
  ) {}

  onSubmit(): void {
  // Yeh function login.html mein (ngSubmit) se call hota hai

    this.isLoading = true;
    // Button disable karo — double submit rokne ke liye

    this.errorMessage = '';
    // Pehle wali error clear karo

    this.authService.login(this.formData).subscribe({
    // authService.login(data) = Observable return karta hai
    // .subscribe() se Observable activate hoti hai — HTTP request jaati hai

      next: (response) => {
      // next: = success hone par yeh callback chalega
        if (response.role === 'Admin') {
          this.router.navigate(['/admin']);
          // Admin ko admin panel pe bhejo (Phase 8 mein banayenge)
        } else if (response.role === 'Washer') {
          this.router.navigate(['/washer']);
          // Washer ko washer panel pe bhejo (Phase 9 mein banayenge)
        } else {
          this.router.navigate(['/']);
          // Customer ko home pe bhejo
        }
      },

      error: (err) => {
      // error: = kuch galat hone par yeh callback chalega
        this.errorMessage = err.error?.message || 'Login failed. Please try again.';
        // err.error = backend ka response body (JSON)
        // err.error?.message = message field — agar hai to use karo
        // || = OR — agar message nahi hai to default text use karo
        this.isLoading = false;
        // Error pe loading false karo — button dobara enable ho
      }
    });
  }
}
```

---

#### FILE 6 — `src/app/pages/auth/login/login.html`
**Kaam:** Login form ka HTML — Angular bindings ke saath

```html
<!-- (ngSubmit) = form ka submit event — Enter ya button click pe -->
<!-- #loginForm="ngForm" = form ka reference variable -->
<!--   loginForm.invalid = koi bhi required field invalid ho to true -->
<form (ngSubmit)="onSubmit()" #loginForm="ngForm">

  <div class="form-group">
    <label for="email">Email Address</label>
    <input
      type="email"
      id="email"
      name="email"
      <!-- name="email" = Angular ko pata ho ki yeh field kaunsi hai -->

      [(ngModel)]="formData.email"
      <!-- [(ngModel)] = two-way binding -->
      <!-- User type kare → formData.email update -->
      <!-- formData.email change ho → input update -->

      required
      <!-- required = yeh field mandatory hai -->
      email
      <!-- email = valid email format chahiye -->

      #emailField="ngModel"
      <!-- #emailField = is input ka reference variable -->
      <!-- emailField.invalid = validation fail hai? -->
      <!-- emailField.touched = user ne field touch kiya? -->
    />

    @if (emailField.invalid && emailField.touched) {
    <!-- @if = Angular 17+ conditional — sirf tab dikhao jab: -->
    <!-- invalid: validation fail aur touched: user ne touch kiya -->
      <span class="field-error">Valid email is required</span>
    }
  </div>

  <button
    type="submit"
    [disabled]="isLoading || loginForm.invalid"
    <!-- [disabled] = property binding -->
    <!-- isLoading true hai YA form invalid hai → button disable -->
  >
    @if (isLoading) {
      Logging in...   <!-- Loading ke waqt text change karo -->
    } @else {
      Login
    }
  </button>

</form>
```

---

#### FILE 7 — `src/app/pages/auth/register/register.ts`
**Kaam:** Register page ka logic — extra field (confirmPassword) validation bhi

```typescript
// register.ts mein ek extra cheez hai — password confirm check
formData = {
  fullName: '',
  email: '',
  phone: '',
  password: '',
  confirmPassword: ''  // Sirf frontend ke liye — backend ko nahi bhejenge
};

onSubmit(): void {
  // Frontend validation: password match check
  if (this.formData.password !== this.formData.confirmPassword) {
    this.errorMessage = 'Passwords do not match!';
    return;  // Aage mat jao — backend call mat karo
  }

  // Backend ke liye sirf zaroori fields — confirmPassword exclude
  const registerData = {
    fullName: this.formData.fullName,
    email: this.formData.email,
    phone: this.formData.phone,
    password: this.formData.password
    // confirmPassword yahan nahi — backend ka RegisterRequest isme nahi hai
  };

  this.authService.register(registerData).subscribe({
    next: () => {
      this.successMessage = 'Registration successful! Redirecting to login...';
      setTimeout(() => this.router.navigate(['/login']), 2000);
      // setTimeout = 2000ms (2 seconds) baad /login pe redirect
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

#### FILE 8 — `src/app/pages/profile/profile.ts`
**Kaam:** Protected page — backend se profile data fetch karo aur display karo

```typescript
// implements OnInit = Angular lifecycle hook use karenge
export class ProfileComponent implements OnInit {

  profile: ProfileResponse | null = null;
  // profile variable — shuru mein null, API se data aane ke baad set hoga

  isLoading = true;
  // Shuru mein true — data load ho raha hai

  ngOnInit(): void {
  // Angular khud yeh function call karta hai component ready hone ke baad
  // API calls yahan karte hain, constructor mein nahi
    this.loadProfile();
  }

  loadProfile(): void {
    this.authService.getProfile().subscribe({
    // GET /api/auth/profile
    // Interceptor automatically Bearer token add karega
    // Backend token verify karega aur profile data return karega
      next: (data) => {
        this.profile = data;    // Data store karo
        this.isLoading = false; // Loading khatam
      },
      error: () => {
        this.errorMessage = 'Could not load profile.';
        this.isLoading = false;
      }
    });
  }

  logout(): void {
    this.authService.logout();     // Token delete, signal null
    this.router.navigate(['/login']); // Login page pe bhejo
  }
}
```

---

#### FILE 9 — `src/app/app.config.ts` (Updated)
**Kaam:** Interceptor globally register kiya

```typescript
// BEFORE (Phase 1):
provideHttpClient(withFetch())

// AFTER (Phase 2):
provideHttpClient(withFetch(), withInterceptors([authInterceptor]))
//                             ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
//                             withInterceptors() = interceptors ki list
//                             [authInterceptor] = hamara interceptor

// Ab har HTTP request se pehle authInterceptor run hoga
// Token hai → Authorization header add hoga
// Token nahi → request as-is jaayegi
```

---

#### FILE 10 — `src/app/app.routes.ts` (Updated)
**Kaam:** Naye routes add kiye — guards ke saath

```typescript
export const routes: Routes = [

  // EXISTING — Phase 1
  { path: '', loadComponent: () => import('./pages/home/home')... },

  // NEW — Phase 2: Guest only (logout hona chahiye)
  {
    path: 'login',
    canActivate: [guestGuard],
    // canActivate = route activate hone se PEHLE guard check karo
    // guestGuard: login nahi hai → allow, login hai → home redirect
    loadComponent: () => import('./pages/auth/login/login')
                         .then(m => m.LoginComponent)
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadComponent: () => import('./pages/auth/register/register')
                         .then(m => m.RegisterComponent)
  },

  // NEW — Phase 2: Protected (login hona chahiye)
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

#### FILE 11 — `src/app/shared/components/header/header.ts` (Updated)
**Kaam:** AuthService inject kiya — login/logout state ke hisaab se nav change hoga

```typescript
export class HeaderComponent {

  // inject() = modern Angular DI — constructor ki jagah
  authService = inject(AuthService);
  // authService = public (no 'private') kyunki template mein bhi use hoga
  // authService.isLoggedIn() → template mein call karenge
  // authService.currentUser() → user ka naam dikhayenge

  private router = inject(Router);
  // private = template mein use nahi hoga, sirf logout() mein

  logout(): void {
    this.authService.logout();      // Token delete, signal null
    this.router.navigate(['/login']); // Login pe redirect
  }
}
```

---

#### FILE 12 — `src/app/shared/components/header/header.html` (Updated)
**Kaam:** Logged-in aur logged-out states ke liye alag nav items

```html
<!-- @if = Angular ka conditional block -->
@if (authService.isLoggedIn()) {
<!-- authService.isLoggedIn() = localStorage mein token hai? -->

  <!-- Logged IN state: user name + logout button -->
  <a routerLink="/profile" class="nav-link user-link">
    👤 {{ authService.currentUser()?.fullName }}
    <!--  authService.currentUser() = Signal read karo -->
    <!--  ?. = null safety — agar null ho to crash nahi -->
    <!--  .fullName = user ka naam -->
    <!--  Jab login hoga → Signal update → naam yahan dikhe ga -->
  </a>

  <button (click)="logout()">Logout</button>

} @else {

  <!-- Logged OUT state: Login + Register buttons -->
  <a routerLink="/login" class="btn btn-secondary">Login</a>
  <a routerLink="/register" class="btn btn-primary">Register</a>

}
```

**Signal ka magic yahan:**
Login hone par `authService.currentUser.set(response)` call hota hai.
Header ka template `authService.currentUser()` read kar raha hai.
Signal change hone par Angular automatically header re-render karta hai.
Koi extra code nahi — reactive!

---

#### Template-Driven Forms

```html
<form (ngSubmit)="onSubmit()" #loginForm="ngForm">
  <input name="email" [(ngModel)]="formData.email" required email />
</form>
```

**`[(ngModel)]` = Two-way binding:**
- `[ngModel]` (Property binding) = TypeScript → HTML (TS variable ka value input mein dikhao)
- `(ngModel)` (Event binding) = HTML → TypeScript (user kuch type kare to TS variable update karo)
- `[(ngModel)]` = dono direction mein — user type kare ya TS change kare, dono sync rahein

**`#loginForm="ngForm"`:**
`#loginForm` = template reference variable — form object ko `loginForm` naam diya.
`loginForm.invalid` = form mein koi bhi required field empty ho to true.

**`#emailField="ngModel"`:**
Individual field ka reference — `emailField.invalid`, `emailField.touched` check kar sakte hain.
- `invalid` = validation fail ho
- `touched` = user ne field pe click karke bahar gaya ho

**Validation directives:**
- `required` = field khali nahi ho sakti
- `email` = valid email format chahiye
- `minlength="6"` = minimum 6 characters chahiye
- `pattern="[0-9]{10,11}"` = regex pattern match karo

---

#### CSS Classes: `ng-invalid` aur `ng-touched`

Angular automatically yeh CSS classes add karta hai inputs pe:
- `ng-valid` / `ng-invalid` = validation pass/fail
- `ng-touched` / `ng-untouched` = user ne touch kiya ya nahi
- `ng-dirty` / `ng-pristine` = user ne value change ki ya nahi

Hum CSS mein:
```css
input.ng-invalid.ng-touched {
  border-color: red;
}
```
= Jab field invalid ho AND user ne touch ki ho tab red border.

---

#### HTTP Interceptor kya hota hai?

```
[Component] → HTTP Request → [Interceptor] → (token add karo) → [Backend]
[Backend] → HTTP Response → [Interceptor] → (response modify karo) → [Component]
```

Socho airport security jaisi: har passenger (request) ko check karo, boarding pass (token) attach karo.

**Functional Interceptor (Angular 15+):**
```typescript
export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const token = localStorage.getItem('carwash_token');
  if (token) {
    const authReq = req.clone({
      headers: req.headers.set('Authorization', `Bearer ${token}`)
    });
    return next(authReq); // Modified request aage bhejo
  }
  return next(req); // Token nahi → original request bhejo
};
```

`req.clone()` = Request immutable hoti hai - copy banao, modify karo, aage bhejo.

**App.config mein register karo:**
```typescript
provideHttpClient(withFetch(), withInterceptors([authInterceptor]))
```

---

#### Route Guard kya hota hai?

```typescript
export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isLoggedIn()) return true;
  return router.createUrlTree(['/login']); // Redirect
};
```

Guard = Route ka darwaan.
- `true` return karo → access allow hai
- `UrlTree` return karo → us URL pe redirect kar do

**Routes mein use:**
```typescript
{
  path: 'profile',
  canActivate: [authGuard], // Guard laga diya
  loadComponent: () => ...
}
```

**Two guards banaye:**
- `authGuard` = Login zaroori hai (profile, bookings, cars)
- `guestGuard` = Logout hona zaroori hai (login, register pages) — logged-in user ko login page nahi dikhana

---

#### isPlatformBrowser kya hai? (SSR Fix)

```typescript
import { isPlatformBrowser } from '@angular/common';
import { PLATFORM_ID, inject } from '@angular/core';

private platformId = inject(PLATFORM_ID);

if (isPlatformBrowser(this.platformId)) {
  localStorage.setItem('token', value); // Sirf browser mein
}
```

Angular SSR (Server Side Rendering) mein:
- Code pehle **server pe** (Node.js mein) run hota hai HTML generate karne ke liye
- Server pe `localStorage` exist nahi karta (yeh browser ka feature hai)
- `isPlatformBrowser()` check karo → server pe skip karo, browser pe run karo

---

#### ngOnInit Lifecycle Hook

```typescript
export class ProfileComponent implements OnInit {
  ngOnInit(): void {
    this.loadProfile(); // Component banne ke baad API call karo
  }
}
```

Angular components ki "life" hoti hai:
1. Component create hota hai (constructor)
2. **ngOnInit** - Component ready hai, ab API calls karo ← Yahan best hai
3. Component updates hoti hain
4. **ngOnDestroy** - Component destroy hone wala hai

API calls `ngOnInit` mein karte hain, constructor mein nahi.

---

---

> **Detailed workflow (file locations + har line ka code):**
> dekho `auth-workflow.md` — `d:\colllege\project\auth-workflow.md`

---

## PHASE 3 - SERVICES & ADD-ONS {#phase-3}
**Status: ✅ COMPLETED**

---

### PHASE 3 MEIN KYA KYA BANAYA — COMPLETE FILE LIST

#### Naye Files (New):
| File Location | Kaam (Purpose) |
|---|---|
| `src/app/models/services.models.ts` | TypeScript interfaces — ServicePlan + AddOn ka blueprint |
| `src/app/core/services/services.service.ts` | Backend se plans + addons fetch karna |
| `src/app/pages/services/services.ts` | Services page ka logic — 3 states manage karna |
| `src/app/pages/services/services.html` | Plans grid + AddOns grid + 3 states ka HTML |
| `src/app/pages/services/services.scss` | Cards, skeleton animation, grid layout |

#### Updated Files (Purane files mein changes):
| File Location | Kya Badla |
|---|---|
| `src/app/app.routes.ts` | `/services` public route add kiya — koi guard nahi |

---

### PHASE 3 — HAR FILE KI DETAIL (Location + Line-by-Line)

---

#### FILE 1 — `src/app/models/services.models.ts`
**Kaam:** Backend ke C# DTOs se match karne wale TypeScript interfaces

```typescript
// LINE 1-8: ServicePlan interface
// Backend: DTOs/ServicePlan/ServicePlanResponse.cs se exactly match karta hai
export interface ServicePlan {
  id: number;           // number = numeric (1, 2, 3...)
  name: string;         // "Basic Wash", "Premium" etc
  description: string;  // Plan ki detail
  price: number;        // Rupees mein — decimal bhi ho sakta hai
  isActive: boolean;    // true/false — sirf true wale dikhayenge
  message: string;      // Backend ka message
}

// LINE 10-16: AddOn interface
// Backend: DTOs/AddOn/AddOnResponse.cs se match karta hai
export interface AddOn {
  id: number;
  name: string;         // "Interior Cleaning", "Waxing" etc
  price: number;
  isActive: boolean;
  message: string;
}
```

**Interface kyun banaya?**
- Backend ka C# `ServicePlanResponse` → Angular ka `ServicePlan` — dono same fields
- Galat field name likho (jaise `planName` ki jagah `name`) → TypeScript compile-time error
- Editor mein autocomplete milti hai — `plan.` likhte hi sab fields suggest hoti hain

---

#### FILE 2 — `src/app/core/services/services.service.ts`
**Kaam:** `/api/serviceplans` aur `/api/addons` se data fetch karna — yeh service ka BRAIN hai

```typescript
// LINE 1: Injectable + inject import
import { Injectable, inject } from '@angular/core';
// Injectable = @Injectable decorator ke liye
// inject     = Angular 15+ ka functional DI

// LINE 2: HttpClient — backend se baat karne ke liye
import { HttpClient } from '@angular/common/http';

// LINE 3: Observable — async response ka type
import { Observable } from 'rxjs';

// LINE 4: Environment — API URL
import { environment } from '../../../environments/environment';
// '../../../' = teen folders upar: services → core → app → src

// LINE 5: Interfaces import
import { ServicePlan, AddOn } from '../../models/services.models';
// '../../' = do folders upar: services → core → app

// LINE 7: Injectable decorator
@Injectable({ providedIn: 'root' })
// providedIn: 'root' = Singleton — poori app mein ek hi instance

export class ServicesService {
  private http = inject(HttpClient);      // HTTP calls ke liye
  private apiUrl = environment.apiUrl;   // 'http://localhost:5001/api'

  // LINE 12-14: Plans fetch karo
  getServicePlans(): Observable<ServicePlan[]> {
  // Observable<ServicePlan[]> = async response mein ServicePlan array milega
    return this.http.get<ServicePlan[]>(`${this.apiUrl}/serviceplans`);
    // GET http://localhost:5001/api/serviceplans
    // Backend mein [AllowAnonymous] → koi token check nahi hota
  }

  // LINE 16-18: AddOns fetch karo
  getAddOns(): Observable<AddOn[]> {
    return this.http.get<AddOn[]>(`${this.apiUrl}/addons`);
    // GET http://localhost:5001/api/addons
    // [AllowAnonymous] → public endpoint
  }
}
```

---

#### FILE 3 — `src/app/pages/services/services.ts`
**Kaam:** Services page ka poora logic — data load karo, 3 states manage karo

```typescript
// LINE 1: Imports
import { Component, OnInit, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
// RouterLink = template mein routerLink="/login" ke liye zaroori

import { ServicesService } from '../../core/services/services.service';
import { ServicePlan, AddOn } from '../../models/services.models';

// LINE 8: Component decorator
@Component({
  selector: 'app-services',
  standalone: true,
  imports: [RouterLink],    // Template mein routerLink use kiya hai
  templateUrl: './services.html',
  styleUrl: './services.scss'
})
export class ServicesComponent implements OnInit {
// implements OnInit = ngOnInit() lifecycle hook use karna hai

  private servicesService = inject(ServicesService);

  // LINE 15-20: 6 Signals — teen states ke liye (data, loading, error)
  plans = signal<ServicePlan[]>([]);    // Initial: empty array
  addOns = signal<AddOn[]>([]);
  isLoadingPlans = signal(true);        // Initial: true → skeleton dikhao
  isLoadingAddOns = signal(true);
  errorPlans = signal('');              // Initial: '' → koi error nahi
  errorAddOns = signal('');

  // LINE 22-26: ngOnInit — component ready hone ke baad
  ngOnInit(): void {
    this.loadPlans();
    this.loadAddOns();
    // Dono parallel call kiye — ek doosre ka wait nahi karte
  }

  // LINE 28-41: Plans fetch
  loadPlans(): void {
    this.servicesService.getServicePlans().subscribe({
      next: (data) => {
        this.plans.set(data.filter(p => p.isActive));
        // .filter() = sirf isActive:true wale rakho → inactive hide karo
        // .set()    = signal update karo → UI auto re-render
        this.isLoadingPlans.set(false);
      },
      error: () => {
        this.errorPlans.set('Service plans load nahi ho sake. Backend chal raha hai?');
        this.isLoadingPlans.set(false);
      }
    });
  }

  // LINE 43-52: AddOns fetch — same pattern
  loadAddOns(): void {
    this.servicesService.getAddOns().subscribe({
      next: (data) => {
        this.addOns.set(data.filter(a => a.isActive));
        this.isLoadingAddOns.set(false);
      },
      error: () => {
        this.errorAddOns.set('Add-ons load nahi ho sake. Backend chal raha hai?');
        this.isLoadingAddOns.set(false);
      }
    });
  }

  // LINE 54-60: Plan naam se icon return karo
  getPlanIcon(name: string): string {
    const lower = name.toLowerCase();
    // toLowerCase() = "BASIC" → "basic" (case insensitive match)
    if (lower.includes('basic') || lower.includes('standard')) return '🚿';
    if (lower.includes('premium') || lower.includes('deluxe')) return '✨';
    if (lower.includes('ultra') || lower.includes('full'))    return '💎';
    return '🚗'; // default
  }
}
```

---

#### FILE 4 — `src/app/pages/services/services.html`
**Kaam:** 3 states handle karna — loading skeleton → error message → data grid

```html
<!-- === STATE 1: LOADING === -->
@if (isLoadingPlans()) {
<!-- isLoadingPlans() = Signal read karo (parentheses zaroori) -->
<!-- true → yeh block render hoga -->
  <div class="skeleton-card"></div>
  <!-- Skeleton = shimmer animation wala fake card -->
  <!-- Data aate hi yeh block @if condition false hone par hat jaayega -->
}

<!-- === STATE 2: ERROR === -->
@if (errorPlans()) {
<!-- errorPlans() = '' falsy → nahi dikhega -->
<!--             = 'error msg' truthy → dikhega -->
  <div class="error-message">{{ errorPlans() }}</div>
  <!-- {{ errorPlans() }} = signal ki value interpolate karo -->
}

<!-- === STATE 3: DATA (loading khatam + koi error nahi) === -->
@if (!isLoadingPlans() && !errorPlans()) {

  @if (plans().length === 0) {
  <!-- plans() = signal array read karo -->
  <!-- .length === 0 = koi plan nahi → empty state -->
    <div class="empty-state">Abhi koi plan available nahi.</div>

  } @else {
    <div class="plans-grid">
      @for (plan of plans(); track plan.id) {
      <!-- @for = Angular 17+ loop — plans array se ek ek item nikalo -->
      <!-- track plan.id = Angular ko unique key batao (performance) -->

        <div class="plan-card">
          <div class="plan-icon">{{ getPlanIcon(plan.name) }}</div>
          <!-- getPlanIcon() = method call → emoji return -->
          <h3>{{ plan.name }}</h3>      <!-- interpolation = string display -->
          <p>{{ plan.description }}</p>
          <span>Rs. {{ plan.price }}</span>
        </div>
      }
    </div>
  }
}

<!-- AddOns section bhi same 3-state pattern follow karta hai -->
@for (addon of addOns(); track addon.id) {
  <div class="addon-card">
    <span>{{ addon.name }}</span>
    <span>Rs. {{ addon.price }}</span>
  </div>
}
```

---

#### FILE 5 — `src/app/pages/services/services.scss`
**Kaam:** CSS Grid layout, hover card effect, skeleton shimmer animation

```scss
// Plans Grid — responsive layout
.plans-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  // auto-fit = jitne cards fit hon utne columns bana lo
  // minmax(280px, 1fr) = minimum 280px, maximum equal share
  // Mobile pe: 1 column, Desktop pe: 3 columns — automatic!
  gap: 24px;
}

// Hover effect
.plan-card {
  transition: transform 0.2s, box-shadow 0.2s;
  // transition = smooth animation — 0.2s mein change hoga
  &:hover {
    transform: translateY(-4px);  // 4px upar uthao — floating effect
    border-color: var(--primary-color);  // Blue border aayegi
  }
}

// Skeleton loading animation
.skeleton-card {
  height: 280px;
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  animation: shimmer 1.5s infinite;  // shimmer naam ka animation loop karo
}

@keyframes shimmer {
  0%   { background-position: 200% 0; }   // Start: right side
  100% { background-position: -200% 0; }  // End: left side
  // Effect: gradient slide karta hai → shimmer dikhta hai
}
```

---

#### FILE 6 — `src/app/app.routes.ts` (Updated)
**Kaam:** `/services` public route add kiya — koi guard nahi

```typescript
// PHASE 3 NEW — Public route (no canActivate guard)
{
  path: 'services',
  // canActivate nahi lagaya → koi bhi access kar sakta hai
  // Login ho ya na ho — dono dekh sakte hain
  loadComponent: () => import('./pages/services/services')
                       .then(m => m.ServicesComponent)
  // Lazy loading = sirf tab download ho jab user /services pe jaye
}

// Teen types ke routes ab hain:
// Public  → koi guard nahi  → '/', '/services'
// Guest   → guestGuard      → '/login', '/register'
// Auth    → authGuard        → '/profile'
```

---

#### Signal aur 3-State Pattern — Phase 3 ka Core

```
Shuru mein:        isLoading=true,  error='',    data=[]  → SKELETON
Data aaya:         isLoading=false, error='',    data=[…] → GRID
Backend down:      isLoading=false, error='msg', data=[]  → ERROR
Backend empty:     isLoading=false, error='',    data=[]  → EMPTY STATE
```

Header re-render, loading dikhna, error dikhna — sab **Signal `.set()` se hota hai**.
Manually DOM update karne ki zaroorat nahi.

---

#### Observable aur .subscribe() kya hota hai?

```typescript
this.http.get<ServicePlan[]>(url)   // ← Observable banao (request nahi gayi abhi)
  .subscribe({                       // ← yahan subscribe karo → request JAATI HAI
    next: (data) => { ... },         // success → data aaya
    error: () => { ... }             // failure → kuch galat hua
  });
```

**Observable = Lazy hai:** Jab tak `.subscribe()` na karo, request nahi jaati.
**Promise se farak:** Promise turant start hoti hai, Observable ko subscribe chahiye.

---

#### Array.filter() — Sirf Active Items Dikhao

```typescript
data.filter(p => p.isActive)
// data     = backend se aaya poora array (active + inactive dono)
// .filter  = ek naya array banao
// p => p.isActive = arrow function — har item check karo
//   isActive: true  → rakho
//   isActive: false → hatao
```

**Example:**
```
Input:  [Basic(true), OldPlan(false), Ultra(true)]
Output: [Basic(true), Ultra(true)]   ← OldPlan filter ho gaya
```

Admin inactive kare kisi plan ko → frontend automatically chhupa deta hai.

---

#### Parallel API Calls kyu kiye?

```typescript
ngOnInit(): void {
  this.loadPlans();   // Request 1 start — background mein
  this.loadAddOns();  // Request 2 start — background mein (wait nahi kiya)
}
```

Agar sequential karte:
```
loadPlans() → 500ms wait → loadAddOns() → 500ms wait → Total: 1000ms
```
Parallel mein:
```
loadPlans()  ─── 500ms ───┐
loadAddOns() ─── 500ms ───┘  Total: 500ms  (same time, faster!)
```

---

> **Detailed workflow (file locations + har line ka code + complete flows):**
> dekho `services-workflow.md` — `d:\colllege\project\services-workflow.md`

---

## PHASE 4 - CARS MANAGEMENT {#phase-4}
**Status: ✅ COMPLETED**

---

### PHASE 4 MEIN KYA KYA BANAYA — COMPLETE FILE LIST

#### Naye Files (New):
| File Location | Kaam (Purpose) |
|---|---|
| `src/app/models/car.models.ts` | Car, CreateCarRequest, UpdateCarRequest interfaces |
| `src/app/core/services/car.service.ts` | 4 API calls — GET, POST, PUT, DELETE |
| `src/app/pages/cars/cars.ts` | Full CRUD logic + form states (add/edit/delete) |
| `src/app/pages/cars/cars.html` | Cars list + inline Add/Edit form |
| `src/app/pages/cars/cars.scss` | Cards, form layout, grid, skeleton |

#### Updated Files (Purane files mein changes):
| File Location | Kya Badla |
|---|---|
| `src/app/app.routes.ts` | `/cars` route add kiya with `authGuard` |
| `src/app/shared/components/header/header.html` | "My Cars" nav link — sirf logged-in users ko |

---

### PHASE 4 — HAR FILE KI DETAIL (Location + Line-by-Line)

---

#### FILE 1 — `src/app/models/car.models.ts`
**Kaam:** 3 interfaces — response ka shape + create/update request ka shape

```typescript
// LINE 1-10: Car interface — backend CarResponse se match karta hai
export interface Car {
  id: number;
  userId: number;
  carNumber: string;   // "MH12AB1234" — number plate
  brand: string;       // "Toyota", "Honda"
  model: string;       // "Corolla", "City"
  carType: string;     // "Sedan", "SUV", "Hatchback" etc
  imageUrl: string;    // Optional car image URL
  isActive: boolean;
  message: string;
}

// LINE 12-18: CreateCarRequest — POST /api/cars ko bhejenge
export interface CreateCarRequest {
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string;   // Optional — empty string bhi chalega
}

// LINE 20-26: UpdateCarRequest — PUT /api/cars/{id} ko bhejenge
// CreateCarRequest ke saath same fields — backend dono mein same DTOs use karta hai
export interface UpdateCarRequest {
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string;
}
```

---

#### FILE 2 — `src/app/core/services/car.service.ts`
**Kaam:** CRUD ke liye 4 methods — ek service mein sab

```typescript
// LINE 1-6: Imports
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Car, CreateCarRequest, UpdateCarRequest } from '../../models/car.models';

// LINE 8: Singleton service
@Injectable({ providedIn: 'root' })
export class CarService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // LINE 13: READ — apni saari cars lo
  getMyCars(): Observable<Car[]> {
    return this.http.get<Car[]>(`${this.apiUrl}/cars/my`);
    // GET /api/cars/my — Interceptor token add karega (authGuard protected)
  }

  // LINE 17: CREATE — nayi car add karo
  createCar(data: CreateCarRequest): Observable<Car> {
    return this.http.post<Car>(`${this.apiUrl}/cars`, data);
    // POST /api/cars — body mein formData bhejenge
  }

  // LINE 21: UPDATE — existing car edit karo
  updateCar(id: number, data: UpdateCarRequest): Observable<Car> {
    return this.http.put<Car>(`${this.apiUrl}/cars/${id}`, data);
    // PUT /api/cars/5 — URL mein id, body mein updated data
    // Template literal: `${this.apiUrl}/cars/${id}`
  }

  // LINE 25: DELETE — car hatao
  deleteCar(id: number): Observable<Car> {
    return this.http.delete<Car>(`${this.apiUrl}/cars/${id}`);
    // DELETE /api/cars/5 — sirf URL mein id, koi body nahi
  }
}
```

---

#### FILE 3 — `src/app/pages/cars/cars.ts`
**Kaam:** Add + Edit + Delete + List — sab ek component mein

```typescript
// LINE 1: Imports
import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
// FormsModule = [(ngModel)] ke liye — same as Phase 2

import { CarService } from '../../core/services/car.service';
import { Car } from '../../models/car.models';

@Component({
  selector: 'app-cars',
  standalone: true,
  imports: [FormsModule],   // Template-driven forms use karenge
  templateUrl: './cars.html',
  styleUrl: './cars.scss'
})
export class CarsComponent implements OnInit {
  private carService = inject(CarService);

  // LINE 15-20: List signals
  cars = signal<Car[]>([]);
  isLoading = signal(true);
  errorMsg = signal('');

  // LINE 22-28: Form signals — add/edit state manage karna
  showForm = signal(false);           // Form dikhao ya chhupao
  isEditing = signal(false);          // Add mode ya Edit mode
  editingCarId = signal<number | null>(null);  // Kaunsi car edit ho rahi hai
  isSubmitting = signal(false);       // API call chal rahi hai
  formError = signal('');
  successMsg = signal('');

  // LINE 30-36: Form data object — [(ngModel)] se bind hoga
  formData = {
    carNumber: '',
    brand: '',
    model: '',
    carType: '',
    imageUrl: ''
  };

  // LINE 38: Car type options — select dropdown ke liye
  carTypes = ['Sedan', 'SUV', 'Hatchback', 'Pickup', 'Van', 'Truck', 'Motorcycle', 'Other'];

  // LINE 40: ngOnInit — cars load karo
  ngOnInit(): void {
    this.loadCars();
  }

  // LINE 43-52: Cars fetch karo
  loadCars(): void {
    this.isLoading.set(true);
    this.carService.getMyCars().subscribe({
      next: (data) => { this.cars.set(data); this.isLoading.set(false); },
      error: () => { this.errorMsg.set('Cars load nahi ho sake.'); this.isLoading.set(false); }
    });
  }

  // LINE 54-59: Add mode ke liye form open karo
  openAddForm(): void {
    this.resetForm();          // Form fields clear karo
    this.isEditing.set(false); // Add mode
    this.editingCarId.set(null);
    this.showForm.set(true);   // Form dikhao
  }

  // LINE 61-71: Edit mode ke liye form open karo — existing car ka data bharo
  openEditForm(car: Car): void {
    this.formData = {          // Form mein car ka existing data bharo
      carNumber: car.carNumber,
      brand: car.brand,
      model: car.model,
      carType: car.carType,
      imageUrl: car.imageUrl
    };
    this.isEditing.set(true);     // Edit mode
    this.editingCarId.set(car.id); // Yaad rakho kaunsa car hai
    this.showForm.set(true);
  }

  // LINE 73-76: Form band karo
  closeForm(): void {
    this.showForm.set(false);
    this.resetForm();
  }

  // LINE 78-103: Submit — Add ya Edit decide karo
  onSubmit(): void {
    this.isSubmitting.set(true);

    if (this.isEditing()) {
    // isEditing() true hai → PUT request (update)
      this.carService.updateCar(this.editingCarId()!, this.formData).subscribe({
      // editingCarId()! = ! (non-null assertion) — TypeScript ko guarantee do ki null nahi
        next: () => {
          this.successMsg.set('Car updated!');
          this.closeForm();
          this.loadCars(); // List refresh karo — updated data dikhao
        },
        error: (err) => { this.formError.set(err.error?.message || 'Update failed.'); }
      });
    } else {
    // isEditing() false hai → POST request (create)
      this.carService.createCar(this.formData).subscribe({
        next: () => {
          this.successMsg.set('Car added!');
          this.closeForm();
          this.loadCars(); // List refresh karo — nayi car dikhao
        },
        error: (err) => { this.formError.set(err.error?.message || 'Add failed.'); }
      });
    }
  }

  // LINE 105-115: Delete — confirm karo phir delete karo
  deleteCar(car: Car): void {
    if (!confirm(`"${car.brand} ${car.model}" delete karna chahte ho?`)) return;
    // confirm() = browser ka built-in dialog — Cancel → false → return (ruk jao)

    this.carService.deleteCar(car.id).subscribe({
      next: () => { this.successMsg.set('Car deleted!'); this.loadCars(); },
      error: () => { this.errorMsg.set('Delete failed.'); }
    });
  }
}
```

---

#### FILE 4 — `src/app/pages/cars/cars.html`
**Kaam:** Cars list + Add/Edit form — showForm signal se toggle hota hai

```html
<!-- ADD FORM — showForm() true ho to dikhao -->
@if (showForm()) {
  <form (ngSubmit)="onSubmit()" #carForm="ngForm" id="car-form">

    <!-- isEditing() se heading change hoti hai -->
    <h2>{{ isEditing() ? 'Edit Car' : 'Add New Car' }}</h2>

    <input name="carNumber" [(ngModel)]="formData.carNumber" required
           #carNumberField="ngModel" />
    @if (carNumberField.invalid && carNumberField.touched) {
      <span>Car number required hai</span>
    }

    <!-- SELECT DROPDOWN — car type choose karo -->
    <select name="carType" [(ngModel)]="formData.carType" required>
      <option value="">-- Select Type --</option>
      @for (type of carTypes; track type) {
        <option [value]="type">{{ type }}</option>
        <!--   [value]="type" = property binding — har option ki value -->
      }
    </select>

    <button type="submit"
            [disabled]="isSubmitting() || carForm.invalid">
      {{ isEditing() ? 'Update Car' : 'Add Car' }}
    </button>

  </form>
}

<!-- CARS GRID — har car ka card -->
@for (car of cars(); track car.id) {
  <div class="car-card">
    <!-- Image ya icon dikhao -->
    @if (car.imageUrl) {
      <img [src]="car.imageUrl" [alt]="car.brand + ' ' + car.model" />
      <!-- [src] = property binding — dynamic URL -->
    } @else {
      <div>{{ getCarIcon(car.carType) }}</div>
    }

    <h3>{{ car.brand }} {{ car.model }}</h3>

    <button (click)="openEditForm(car)">Edit</button>
    <!-- openEditForm(car) = car object pass karo — form fill hoga -->

    <button (click)="deleteCar(car)">Delete</button>
  </div>
}
```

---

#### FILE 5 — `src/app/pages/cars/cars.scss`
**Kaam:** 2-column form grid, car cards, edit/delete buttons

```scss
// 2-column form grid
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  // 1fr 1fr = do equal columns
  gap: 16px;

  @media (max-width: 600px) {
    grid-template-columns: 1fr;
    // Mobile pe → single column
  }
}

// Full width field — image URL
.full-width {
  grid-column: 1 / -1;
  // 1 = first column se start, -1 = last column tak stretch
}

// Car image
.car-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  // object-fit: cover = image crop karo without distortion
  // Jaise background-size: cover hota hai
}

// Edit button — blue
.btn-edit {
  background: #eff6ff;
  color: #1d4ed8;
  border: 1px solid #bfdbfe;
  &:hover { background: #dbeafe; }
}

// Delete button — red
.btn-delete {
  background: #fef2f2;
  color: #dc2626;
  border: 1px solid #fecaca;
  &:hover { background: #fee2e2; }
}
```

---

#### FILE 6 — `src/app/app.routes.ts` (Updated)
**Kaam:** `/cars` route add kiya — `authGuard` laga hai (login zaroori)

```typescript
{
  path: 'cars',
  canActivate: [authGuard],
  // authGuard = login zaroori — Customer ka token verify hoga
  // Backend bhi [Authorize(Roles = "Customer")] check karta hai
  loadComponent: () => import('./pages/cars/cars').then(m => m.CarsComponent)
}
```

---

#### CRUD Operations — HTTP Methods ka Farak

```typescript
GET    /api/cars/my      → Read   — data mangna
POST   /api/cars         → Create — naya data banana
PUT    /api/cars/{id}    → Update — purana data badalna
DELETE /api/cars/{id}    → Delete — data hatana
```

**Interceptor** har request mein `Authorization: Bearer token` add karta hai — isliye backend ko pata hota hai ki kaun sa user hai, aur woh sirf usi user ki cars return karta hai.

---

#### isEditing Signal — Ek Form Do Kaam

```typescript
isEditing = signal(false);
// false → Add mode: POST request, "Add Car" button
// true  → Edit mode: PUT request, "Update Car" button
```

```html
<h2>{{ isEditing() ? 'Edit Car' : 'Add New Car' }}</h2>
<button>{{ isEditing() ? 'Update Car' : 'Add Car' }}</button>
```

Ternary operator `? :` — agar isEditing() true → pehla value, false → doosra value.
Ek hi form add aur edit dono ke liye kaam karta hai — code duplicate nahi hua.

---

#### confirm() — Browser Native Delete Confirmation

```typescript
if (!confirm(`"${car.brand} ${car.model}" delete karna chahte ho?`)) return;
// confirm() = browser ka built-in popup
// User "OK" click kare → true → aage chalo
// User "Cancel" click kare → false → !false = true → return (ruk jao)
```

Ek line mein delete protection — koi extra modal component banane ki zaroorat nahi.

---

#### loadCars() after Create/Update/Delete

```typescript
next: () => {
  this.closeForm();
  this.loadCars();   // ← List refresh karo
}
```

Create/Update/Delete ke baad `loadCars()` dobara call karte hain.
Kyun? Signal mein purani list hai — nayi list backend se mangni padegi.
`loadCars()` fresh GET request bhejti hai → updated list aati hai → UI update hoti hai.

---

> **Detailed workflow (file locations + har line ka code + complete flows):**
> dekho `cars-workflow.md` — `d:\colllege\project\cars-workflow.md`

---

## PHASE 5 - BOOKINGS {#phase-5}
**Status: ✅ COMPLETED**

---

### FILES CREATED/UPDATED

| # | File | Kaam |
|---|------|------|
| 1 | `src/app/models/booking.models.ts` | Booking + CreateBookingRequest + nested interfaces |
| 2 | `src/app/core/services/booking.service.ts` | GET/POST/PUT API calls |
| 3 | `src/app/pages/bookings/bookings.ts` | List + form + cancel logic |
| 4 | `src/app/pages/bookings/bookings.html` | Bookings list + create form |
| 5 | `src/app/pages/bookings/bookings.scss` | Status badges, plan cards, addon pills |
| 6 | `src/app/app.routes.ts` | `/bookings` route + authGuard |
| 7 | `src/app/shared/components/header/header.html` | "My Bookings" nav link |

---

### FILE 1 — `booking.models.ts`

```typescript
// LINE 1-4: Nested interfaces (small objects inside Booking)
export interface BookingAddOn { id: number; name: string; price: number; }
export interface BookingCar { id: number; brand: string; model: string; carNumber: string; carType: string; }
export interface BookingServicePlan { id: number; name: string; price: number; }

// LINE 6-18: Main Booking interface — nested objects included
export interface Booking {
  id: number; userId: number; carId: number; servicePlanId: number;
  scheduledDate: string;  // LINE 10: ISO string "2026-05-25T00:00:00"
  status: string;         // LINE 11: "Pending"|"Confirmed"|"InProgress"|"Completed"|"Cancelled"
  totalAmount: number; notes: string; message: string;
  car: BookingCar;             // LINE 15: Nested — car data embedded
  servicePlan: BookingServicePlan;  // LINE 16: Nested — plan data embedded
  addOns: BookingAddOn[];      // LINE 17: Array — selected addons
}

// LINE 20-26: CreateBookingRequest — POST /api/bookings ko bhejenge
export interface CreateBookingRequest {
  carId: number; servicePlanId: number;
  addOnIds: number[];   // LINE 23: Sirf IDs — backend objects fetch karega
  scheduledDate: string; notes: string;
}
```

**CONCEPT: Nested Interface** — Booking ke andar Car aur ServicePlan ka poora object hota hai. Alag interface isliye: `Car` interface mein extra fields (isActive, imageUrl) hain jo booking mein nahi chahiye.

---

### FILE 2 — `booking.service.ts`

```typescript
// LINE 8-10: @Injectable singleton
@Injectable({ providedIn: 'root' })
export class BookingService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // LINE 13: GET — my bookings
  getMyCars(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/bookings/my`);
    // LINE 15: [Authorize] → JWT token → sirf is user ki bookings
  }

  // LINE 18: POST — create booking
  createBooking(data: CreateBookingRequest): Observable<Booking> {
    return this.http.post<Booking>(`${this.apiUrl}/bookings`, data);
  }

  // LINE 22: PUT — cancel booking
  cancelBooking(id: number): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/bookings/${id}/cancel`, {});
    // LINE 24: {} = empty body — PUT mein body zaroori hai Angular ke liye
  }
}
```

---

### FILE 3 — `bookings.ts` (Key Parts)

```typescript
// LINE 14-16: Teen services inject — bookings + cars + plans/addons
private bookingService = inject(BookingService);
private carService = inject(CarService);
private servicesService = inject(ServicesService);

// LINE 18-21: List state signals
bookings = signal<Booking[]>([]);
isLoadingBookings = signal(true);
listError = signal('');

// LINE 23-26: Form dropdown data signals
myCars = signal<Car[]>([]);
plans = signal<ServicePlan[]>([]);
addOns = signal<AddOn[]>([]);

// LINE 36: formData — carId:0, servicePlanId:0 default (0 = nothing selected)
formData = { carId: 0, servicePlanId: 0, scheduledDate: '', notes: '' };

// LINE 39: selectedAddOnIds — Signal (plain array nahi) — reactive update ke liye
selectedAddOnIds = signal<number[]>([]);

// LINE 45-46: Parallel loading
ngOnInit(): void { this.loadBookings(); this.loadFormData(); }

// LINE 55-61: loadFormData — teen parallel calls
loadFormData(): void {
  this.carService.getMyCars().subscribe({ next: (d) => this.myCars.set(d) });
  this.servicesService.getServicePlans().subscribe({ next: (d) => this.plans.set(d.filter(p => p.isActive)) });
  this.servicesService.getAddOns().subscribe({ next: (d) => this.addOns.set(d.filter(a => a.isActive)) });
}

// LINE 64-71: toggleAddOn — checkbox ki array manage karo
toggleAddOn(addonId: number): void {
  const current = this.selectedAddOnIds();
  if (current.includes(addonId)) {
    this.selectedAddOnIds.set(current.filter(id => id !== addonId)); // LINE 68: Remove
  } else {
    this.selectedAddOnIds.set([...current, addonId]); // LINE 70: Add — spread operator
  }
}

// LINE 78-87: getter — computed total price
get totalAmount(): number {
  const plan = this.plans().find(p => p.id === this.formData.servicePlanId); // LINE 80: .find()
  const planPrice = plan?.price ?? 0; // LINE 81: ?? = nullish coalescing
  const addonTotal = this.addOns()
    .filter(a => this.selectedAddOnIds().includes(a.id))
    .reduce((sum, a) => sum + a.price, 0); // LINE 85: .reduce() — array sum
  return planPrice + addonTotal;
}

// LINE 89-91: minDate getter — past dates disable karo
get minDate(): string {
  return new Date().toISOString().split('T')[0]; // "2026-05-21"
}

// LINE 118-121: getStatusClass — switch se dynamic CSS class
getStatusClass(status: string): string {
  switch (status.toLowerCase()) {
    case 'pending': return 'status-badge status-pending'; // Yellow
    case 'completed': return 'status-badge status-completed'; // Green
    // ... etc
  }
}
```

**CONCEPTS:**
- `get totalAmount()` → TypeScript getter → template mein `{{ totalAmount }}` (no brackets)
- `Array.reduce()` → array ko ek value mein convert karo (sum, product, etc.)
- `Array.find()` → pehla matching item return karo (ya undefined)
- `??` nullish coalescing → null/undefined ho to default value

---

### FILE 4 — `bookings.html` (Key Parts)

```html
<!-- LINE 30-35: Car Dropdown — [ngValue] number preserve karta hai -->
<select name="carId" [(ngModel)]="formData.carId">
  <option [ngValue]="0">-- Apni car chunao --</option>
  @for (car of myCars(); track car.id) {
    <option [ngValue]="car.id">{{ car.brand }} {{ car.model }}</option>
    <!-- LINE 34: [ngValue] → number 5, [value] → string "5" galat hoga -->
  }
</select>

<!-- LINE 38-40: Date Input — min date se past disable -->
<input type="date" name="scheduledDate"
       [(ngModel)]="formData.scheduledDate" [min]="minDate" />

<!-- LINE 44-50: Radio Buttons — ek hi select ho sakta hai -->
@for (plan of plans(); track plan.id) {
  <label [class.selected]="formData.servicePlanId === plan.id">
    <input type="radio" name="servicePlanId"
           [value]="plan.id" [(ngModel)]="formData.servicePlanId" />
    {{ plan.name }} — Rs. {{ plan.price }}
  </label>
}

<!-- LINE 54-60: Checkboxes — multiple select, manual approach -->
@for (addon of addOns(); track addon.id) {
  <label [class.selected]="isAddOnSelected(addon.id)">
    <input type="checkbox"
           [checked]="isAddOnSelected(addon.id)"
           (change)="toggleAddOn(addon.id)" />
    <!-- LINE 59: [(ngModel)] array ke liye complex — [checked]+(change) better -->
    {{ addon.name }} +Rs. {{ addon.price }}
  </label>
}

<!-- LINE 65-67: Total — getter se (no brackets) -->
@if (formData.servicePlanId) {
  <div>Total: Rs. {{ totalAmount }}</div>
}

<!-- LINE 85: Status Badge — dynamic class -->
<span [class]="getStatusClass(booking.status)">{{ booking.status }}</span>

<!-- LINE 88-90: Nested object access -->
{{ booking.car?.brand }} {{ booking.car?.model }}

<!-- LINE 97: Conditional cancel button -->
@if (canCancel(booking.status)) {
  <button (click)="cancelBooking(booking)">Cancel Booking</button>
}
```

**CONCEPTS:**
- `[ngValue]` → number type preserve karta hai (vs `[value]` jo string deta hai)
- `type="radio"` → ek group mein ek hi select
- `type="checkbox"` → multiple select — `[checked]` + `(change)` pattern
- `[class]="expression"` → dynamic full class binding
- `booking.car?.brand` → nested object optional chaining

---

### FILE 5 — `bookings.scss` (Key Parts)

```scss
// Status Badges — har status ka alag color
.status-pending    { background: #fef9c3; color: #854d0e; }  // Yellow
.status-confirmed  { background: #dbeafe; color: #1e40af; }  // Blue
.status-inprogress { background: #ede9fe; color: #6d28d9; }  // Purple
.status-completed  { background: #dcfce7; color: #15803d; }  // Green
.status-cancelled  { background: #f3f4f6; color: #6b7280; }  // Grey

// Plan Radio Cards — border highlight on selection
.plan-option {
  border: 2px solid #e5e7eb;
  &.selected { border-color: var(--primary-color); background: #eff6ff; }
  // .selected → [class.selected]="formData.servicePlanId === plan.id"
}

// Addon Pills — horizontal pill style
.addon-option { border-radius: 20px; border: 2px solid #e5e7eb; }
.addon-option.selected { border-color: var(--primary-color); background: #eff6ff; }

// Total Preview Box
.total-preview {
  background: #f0f4ff; padding: 14px 20px;
  display: flex; justify-content: flex-end;
  strong { color: var(--primary-color); font-size: 1.2rem; }
}

// Bookings — vertical list (column)
.bookings-list { display: flex; flex-direction: column; gap: 16px; }

// Booking body — 2-column grid
.booking-body {
  display: grid; grid-template-columns: 1fr 1fr;
  @media (max-width: 600px) { grid-template-columns: 1fr; }
}
```

---

### FILE 6 — Route + Header Update

```typescript
// app.routes.ts — LINE 39-43
{
  path: 'bookings',
  canActivate: [authGuard],
  loadComponent: () => import('./pages/bookings/bookings').then(m => m.BookingsComponent)
}
```

```html
<!-- header.html — My Bookings link (logged-in section mein) -->
<a routerLink="/bookings" routerLinkActive="active" class="nav-link" (click)="closeMenu()">
  My Bookings
</a>
```

---

### PHASE 5 CONCEPTS EXPLAINED

**[ngValue] vs [value]:**
`[value]="5"` → HTML attribute → always string → `formData.carId = "5"` (galat)
`[ngValue]="5"` → Angular property binding → number type → `formData.carId = 5` (sahi)

**Checkbox Array Pattern:**
`[(ngModel)]` single boolean ke liye. Array ke liye: `[checked]` + `(change)` + manual toggle.

**getter:**
`get totalAmount()` → `{{ totalAmount }}` — method ki tarah brackets nahi likhte.

**Array.reduce():**
`[50, 100].reduce((sum, price) => sum + price, 0)` → `150` — running sum nikalna.

**Date:**
`new Date().toISOString().split('T')[0]` → `"2026-05-21"` — min date for past blocking.
`new Date(str).toLocaleDateString('en-IN', {...})` → `"25 May 2026"` — display format.

**switch + [class]:**
`getStatusClass()` returns CSS class string → `[class]="getStatusClass(status)"` → dynamic badge.

> dekho `bookings-workflow.md` — `d:\colllege\project\bookings-workflow.md`

---

## PHASE 6 - PAYMENTS & RECEIPTS {#phase-6}
**Status: ✅ COMPLETED**

---

### FILES CREATED/UPDATED

| # | File | Kaam |
|---|------|------|
| 1 | `src/app/models/payment.models.ts` | Payment, Receipt + nested interfaces |
| 2 | `src/app/core/services/payment.service.ts` | GET payments, POST create, GET receipts |
| 3 | `src/app/pages/payments/payments.ts` | List + form + receipt toggle + print |
| 4 | `src/app/pages/payments/payments.html` | Payment cards + receipt panel + @let |
| 5 | `src/app/pages/payments/payments.scss` | Teal theme + receipt + @media print |
| 6 | `src/app/app.routes.ts` | `/payments` route + authGuard |
| 7 | `src/app/shared/components/header/header.html` | "Payments" nav link |

---

### FILE 1 — `payment.models.ts`

```typescript
// LINE 1-7: PaymentBooking nested interface
export interface PaymentBooking {
  id: number; scheduledDate: string; status: string;
  car: { brand: string; model: string; carNumber: string; };  // LINE 5: inline nested
  servicePlan: { name: string; };
}

// LINE 9-19: Main Payment interface
export interface Payment {
  id: number; bookingId: number; userId: number;
  amount: number;
  paymentMethod: string;  // LINE 13: "Cash"|"Card"|"UPI"|"Online Banking"
  paymentDate: string;
  status: string;         // LINE 15: "Completed"|"Pending"|"Failed"|"Refunded"
  transactionId: string;
  message: string;
  booking: PaymentBooking;  // LINE 18: Nested
}

// LINE 21-22: POST body
export interface CreatePaymentRequest { bookingId: number; paymentMethod: string; }

// LINE 24-38: Receipt interfaces
export interface ReceiptPayment { paymentMethod: string; transactionId: string; paymentDate: string; }
export interface ReceiptAddOn { name: string; price: number; }
export interface ReceiptBooking {
  scheduledDate: string;
  car: { brand: string; model: string; carNumber: string; };
  servicePlan: { name: string; price: number; };
  addOns: ReceiptAddOn[];  // LINE 34: array of addons
}
export interface Receipt {
  id: number; paymentId: number; bookingId: number;
  receiptNumber: string;  // LINE 38: "RCP-2026-00001"
  totalAmount: number; generatedDate: string; message: string;
  payment: ReceiptPayment; booking: ReceiptBooking;
}
```

---

### FILE 2 — `payment.service.ts`

```typescript
// LINE 8-10: Singleton
@Injectable({ providedIn: 'root' })
export class PaymentService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // LINE 13: GET payments list
  getMyPayments(): Observable<Payment[]> {
    return this.http.get<Payment[]>(`${this.apiUrl}/payments/my`);
  }

  // LINE 17: POST — create payment
  createPayment(data: CreatePaymentRequest): Observable<Payment> {
    return this.http.post<Payment>(`${this.apiUrl}/payments`, data);
    // LINE 19: Backend receipt automatically generate karta hai payment ke baad
  }

  // LINE 22: GET receipts
  getMyReceipts(): Observable<Receipt[]> {
    return this.http.get<Receipt[]>(`${this.apiUrl}/receipts/my`);
    // LINE 24: Alag endpoint — receipt = payment ka proof document
  }
}
```

---

### FILE 3 — `payments.ts` (Key Parts)

```typescript
// LINE 10-11: Do services inject — payment + booking (reuse existing)
private paymentService = inject(PaymentService);
private bookingService = inject(BookingService);  // Reuse! Nayi service banana ki zaroorat nahi

// LINE 13-15: Data signals — teen types
payments = signal<Payment[]>([]);
receipts = signal<Receipt[]>([]);
myBookings = signal<Booking[]>([]);

// LINE 22: Toggle signal — kaunsi receipt open hai
viewingReceiptId = signal<number | null>(null);
// null = kuch open nahi, 5 = payment #5 ki receipt open

// LINE 30: Available methods array
paymentMethods = ['Cash', 'Card', 'UPI', 'Online Banking'];

// LINE 33-35: Parallel loading
ngOnInit(): void { this.loadPayments(); this.loadReceipts(); this.loadMyBookings(); }

// LINE 45-48: Filter cancelled bookings
loadMyBookings(): void {
  this.bookingService.getMyBookings().subscribe({
    next: (data) => this.myBookings.set(data.filter(b => b.status !== 'Cancelled'))
    // Cancelled bookings pay nahi hoti — filter karo
  });
}

// LINE 75-80: Receipt toggle
toggleReceipt(paymentId: number): void {
  if (this.viewingReceiptId() === paymentId) {
    this.viewingReceiptId.set(null);       // LINE 77: Already open → band
  } else {
    this.viewingReceiptId.set(paymentId);  // LINE 79: New open
  }
}

// LINE 83-85: Cross-reference receipts array
getReceiptForPayment(paymentId: number): Receipt | undefined {
  return this.receipts().find(r => r.paymentId === paymentId);
  // LINE 85: .find() → pehla match return karo (ya undefined)
}

// LINE 88-90: Browser print
printReceipt(): void {
  window.print();  // LINE 89: @media print CSS apply hogi
}
```

---

### FILE 4 — `payments.html` (Key Parts)

```html
<!-- LINE 45-55: Amount preview — @for + nested @if -->
@if (formData.bookingId) {
  @for (booking of myBookings(); track booking.id) {
    @if (booking.id === formData.bookingId) {
      <strong>Rs. {{ booking.totalAmount }}</strong>
    }
  }
}

<!-- LINE 60: @let — Angular 18+ template variable -->
@for (payment of payments(); track payment.id) {
  @let receipt = getReceiptForPayment(payment.id);
  <!-- Method ek baar call → 'receipt' mein store → baad mein directly use karo -->

  <!-- LINE 70: Receipt button sirf tab dikhao jab receipt exists -->
  @if (receipt) {
    <button (click)="toggleReceipt(payment.id)">
      {{ viewingReceiptId() === payment.id ? 'Hide ▲' : 'View ▼' }}
    </button>
  }

  <!-- LINE 76: Receipt panel — toggle state check -->
  @if (viewingReceiptId() === payment.id && receipt) {
    <div class="receipt-panel">
      {{ receipt.receiptNumber }}
      Rs. {{ receipt.totalAmount }}
      <button (click)="printReceipt()">🖨 Print</button>
      <!-- LINE 83: Nested addons loop -->
      @for (addon of receipt.booking.addOns; track addon.name) {
        <div>{{ addon.name }} — Rs. {{ addon.price }}</div>
      }
    </div>
  }
}
```

---

### FILE 5 — `payments.scss` (Key Parts)

```scss
// LINE 85-93: Payment method grid — 2x2 layout
.method-options { display: grid; grid-template-columns: 1fr 1fr; gap: 8px; }
.method-option {
  border: 2px solid #e5e7eb;
  &.selected { border-color: #0d9488; background: #f0fdfa; }  // Teal theme
}

// LINE 160-170: Receipt panel — nested inside payment card
.receipt-panel { margin: 0 16px 16px; border: 1px solid #e5e7eb; border-radius: 12px; }
.receipt-row { display: flex; justify-content: space-between; padding: 8px 0; }
.receipt-total { display: flex; justify-content: space-between; }

// LINE 195-210: @media print — sirf receipt print karo
@media print {
  .page-header, .form-card, .payment-header,
  .payment-body, .payment-actions, .btn-receipt { display: none !important; }
  .receipt-panel { border: 1px solid #000; margin: 0; }
}
// @media print → printer ke liye alag CSS
// window.print() se trigger hota hai
```

---

### PHASE 6 CONCEPTS EXPLAINED

**@let (Angular 18+):**
`@let receipt = getReceiptForPayment(payment.id);` — template mein local variable banao.
Method ek baar call → result store → repeat calling se bacho.

**Toggle Pattern (signal<T | null>):**
`viewingReceiptId = signal<number | null>(null)` — kaunsa item expanded hai.
Same ID → null (close), new ID → set (open). Ek time pe ek hi open.

**Array.find() cross-reference:**
`receipts().find(r => r.paymentId === payment.id)` — do arrays ko link karo paymentId se.

**window.print() + @media print:**
`window.print()` = browser print dialog. `@media print { display: none }` = screen pe dikhao, print pe chhupaao.

**Service Reuse:**
`BookingService` inject kiya `PaymentsComponent` mein — bookings load karne ke liye.
Angular singleton → same instance → nayi service banana ki zaroorat nahi.

> dekho `payments-workflow.md` — `d:\colllege\project\payments-workflow.md`

---

## PHASE 7 - REVIEWS {#phase-7}
**Status: ⏳ PENDING**

---

## PHASE 8 - ADMIN PANEL {#phase-8}
**Status: ⏳ PENDING**

---

## PHASE 9 - WASHER PANEL {#phase-9}
**Status: ⏳ PENDING**

---

## ANGULAR CONCEPTS DICTIONARY {#concepts}

| Term | Simple Explanation |
|------|--------------------|
| Component | Ek reusable UI piece - HTML + CSS + TypeScript |
| Decorator | `@Component`, `@Injectable` etc - class ko metadata deta hai |
| Template | Component ka HTML part |
| Interpolation | `{{ variable }}` - TypeScript variable HTML mein dikhao |
| Property Binding | `[property]="value"` - HTML property ko TS variable se bind karo |
| Event Binding | `(event)="method()"` - HTML event ko TS function se bind karo |
| Two-way Binding | `[(ngModel)]="variable"` - Form inputs ke liye |
| RouterLink | `routerLink="/path"` - Page reload ke bina navigate karo |
| RouterOutlet | `<router-outlet>` - Yahan page component render hoga |
| Service | Business logic + API calls - components mein inject hoti hai |
| Dependency Injection | Angular automatically service inject karta hai constructor mein |
| Observable | Async data stream - HTTP calls ka result |
| Guard | Route ko protect karta hai - unauthorized access rokta hai |
| Interceptor | Har HTTP request/response ko intercept karta hai |
| Standalone Component | Apne imports khud declare karne wala component (Angular 14+) |
| Lazy Loading | Component tabhi load ho jab user us route pe jaye |
| @Input() | Parent se child ko data pass karo |
| @Output() | Child se parent ko event emit karo |
| Signal | `signal()` - Angular 16+ ka reactive state management |
| @for | Angular 17+ ka new loop syntax (replaces *ngFor) |
| @if | Angular 17+ ka new conditional syntax (replaces *ngIf) |

---

*Last Updated: Phase 3 completion*
*Next Phase: Phase 4 - Cars Management*
