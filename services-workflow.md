# Phase 3 — Services & Add-Ons Workflow
# File Locations + Line-by-Line Code + Complete Flow
> Yeh file sirf Phase 3 ke liye hai — har cheez detail mein

---

## SARI FILES AUR UNKA KAAM

| File | Location | Kaam |
|------|----------|------|
| Models | `src/app/models/services.models.ts` | ServicePlan + AddOn ka blueprint |
| Service | `src/app/core/services/services.service.ts` | Backend se data fetch karna |
| Page TS | `src/app/pages/services/services.ts` | Loading, error, data logic |
| Page HTML | `src/app/pages/services/services.html` | Plans + AddOns display |
| Page SCSS | `src/app/pages/services/services.scss` | Cards, skeletons, layout styling |
| Routes | `src/app/app.routes.ts` | `/services` route add kiya |

---

## FILE 1 — `src/app/models/services.models.ts`

**Kaam:** Backend ke DTOs se match karne wale TypeScript interfaces — data ka blueprint

```typescript
// ServicePlan interface
// Backend C# class: ServicePlanResponse.cs se exactly match karta hai
export interface ServicePlan {
  id: number;
  // number → numeric value (1, 2, 3...)
  // Backend mein: public int Id { get; set; }

  name: string;
  // Plan ka naam → "Basic Wash", "Premium", "Ultra" etc
  // Backend mein: public string Name { get; set; }

  description: string;
  // Plan ki description — kya kya milega is wash mein
  // Backend mein: public string Description { get; set; }

  price: number;
  // Price in rupees — decimal bhi ho sakta hai (e.g., 299.99)
  // Backend mein: public decimal Price { get; set; }

  isActive: boolean;
  // true → plan available hai, false → admin ne disable kiya
  // Hum sirf isActive: true wale dikhayenge
  // Backend mein: public bool IsActive { get; set; }

  message: string;
  // Backend se aane wala message (e.g., "Plan retrieved successfully")
}

// AddOn interface
// Backend C# class: AddOnResponse.cs se exactly match karta hai
export interface AddOn {
  id: number;

  name: string;
  // AddOn ka naam → "Interior Cleaning", "Waxing" etc

  price: number;
  // Extra charge for this add-on

  isActive: boolean;
  // Sirf active add-ons dikhayenge

  message: string;
}
```

**Backend DTOs kahan hain?**
```
CarWash.Backend/
├── DTOs/
│   ├── ServicePlan/
│   │   └── ServicePlanResponse.cs  ← Yahan se copy kiya structure
│   └── AddOn/
│       └── AddOnResponse.cs        ← Yahan se copy kiya structure
```

**Kya hoga agar interface galat ho?**
```typescript
// Agar yeh likha:
interface ServicePlan {
  planName: string;  // Backend mein "name" hai, "planName" nahi
}

// To TypeScript error dega:
// Property 'planName' does not exist on type 'ServicePlan'
// Compile time par → runtime se pehle → better!
```

---

## FILE 2 — `src/app/core/services/services.service.ts`

**Kaam:** Backend ke `/api/serviceplans` aur `/api/addons` endpoints se data fetch karna

```typescript
import { Injectable, inject } from '@angular/core';
// Injectable → @Injectable decorator ke liye — class inject ho sakti hai
// inject     → Angular 15+ ka functional dependency injection

import { HttpClient } from '@angular/common/http';
// HttpClient → Angular ka built-in HTTP library
// GET, POST, PUT, DELETE requests karne ke liye

import { Observable } from 'rxjs';
// Observable → async data stream
// HTTP response ek Observable mein wrap hota hai

import { environment } from '../../../environments/environment';
// Relative path: services.service.ts ki location se
//   services/  ← current folder
//   core/      ← ek upar
//   app/       ← do upar
//   src/       ← teen upar
// phir environments/environment.ts → apiUrl: 'http://localhost:5001/api'

import { ServicePlan, AddOn } from '../../models/services.models';
// Relative path:
//   services/  ← current folder
//   core/      ← ek upar
//   app/       ← do upar
// phir models/services.models.ts

@Injectable({ providedIn: 'root' })
// @Injectable → yeh class Angular DI system mein register hoti hai
// providedIn: 'root' → Singleton pattern
//   Angular sirf EK instance banayega poori app ke liye
//   Agar ServicesComponent aur koi aur component bhi inject kare
//   → dono ko SAME instance milega (nayi nahi banegi)

export class ServicesService {

  private http = inject(HttpClient);
  // HttpClient inject kiya
  // inject() → Angular khud HttpClient ka instance deta hai
  // private → sirf is class ke andar use hoga

  private apiUrl = environment.apiUrl;
  // 'http://localhost:5001/api'
  // private → sirf yahan use hoga

  getServicePlans(): Observable<ServicePlan[]> {
  // Return type: Observable<ServicePlan[]>
  //   Observable → async (response turant nahi aata)
  //   ServicePlan[] → array of ServicePlan objects ([] matlab array)

    return this.http.get<ServicePlan[]>(`${this.apiUrl}/serviceplans`);
    // this.http.get<T>(url) → GET request bhejo
    // <ServicePlan[]> → TypeScript ko batao ki response kaisa hoga
    // Template literal: `${this.apiUrl}/serviceplans`
    //   = 'http://localhost:5001/api' + '/serviceplans'
    //   = 'http://localhost:5001/api/serviceplans'
    //
    // Backend controller:
    //   [AllowAnonymous] → No auth needed
    //   [HttpGet] → GET request handle karta hai
    //   Returns: List<ServicePlanResponse>
  }

  getAddOns(): Observable<AddOn[]> {
    return this.http.get<AddOn[]>(`${this.apiUrl}/addons`);
    // GET http://localhost:5001/api/addons
    // Backend mein [AllowAnonymous] → No auth needed
  }
}
```

**Auth.service vs Services.service — Farak:**

| | auth.service.ts | services.service.ts |
|---|---|---|
| Methods | login(), register(), logout() | getServicePlans(), getAddOns() |
| State | currentUser signal rakhta hai | Koi state nahi — sirf fetch karo |
| Token | localStorage mein save karta hai | Token se koi kaam nahi |
| Side effects | tap() se token save karta hai | Direct return — koi side effect nahi |

---

## FILE 3 — `src/app/pages/services/services.ts`

**Kaam:** Services page ka poora brain — data fetch karo, states manage karo

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
// Component → @Component decorator
// OnInit    → ngOnInit lifecycle hook interface
// inject    → functional DI
// signal    → reactive state

import { RouterLink } from '@angular/router';
// RouterLink → template mein routerLink directive ke liye
// Jab <a routerLink="/login"> likhte hain to RouterLink directive handle karta hai

import { ServicesService } from '../../core/services/services.service';
// Relative path se services page ki location:
//   services/  ← current folder
//   pages/     ← ek upar
//   app/       ← do upar
// phir core/services/services.service

import { ServicePlan, AddOn } from '../../models/services.models';
// TypeScript type checking ke liye

@Component({
  selector: 'app-services',
  // Agar koi aur component mein <app-services> likhein to yeh render hoga

  standalone: true,
  // Angular 17+ ka pattern — NgModule ki zaroorat nahi

  imports: [RouterLink],
  // RouterLink import kiya → template mein routerLink kaam karega
  // Bina import ke: "routerLink is not a known property" error

  templateUrl: './services.html',
  // HTML file ka path — same folder mein

  styleUrl: './services.scss'
  // SCSS file ka path — same folder mein
})
export class ServicesComponent implements OnInit {
// implements OnInit → TypeScript ko batao ki ngOnInit() method hogi
// Agar implements likho aur ngOnInit() na likho → TypeScript error

  private servicesService = inject(ServicesService);
  // ServicesService inject kiya
  // private → template mein nahi use hoga

  // === SIGNALS — Reactive State ===

  plans = signal<ServicePlan[]>([]);
  // signal<ServicePlan[]> → type specify kiya: ServicePlan ka array
  // ([]) → initial value: empty array (koi plan nahi initially)
  // plans() → read karo (parentheses)
  // plans.set([...]) → update karo

  addOns = signal<AddOn[]>([]);
  // AddOns ke liye same

  isLoadingPlans = signal(true);
  // true → shuru mein loading dikhao
  // Jab data aaye ya error aaye → false karo

  isLoadingAddOns = signal(true);
  // Plans aur AddOns ki loading alag track ki
  // Dono independent hain → ek load ho jaaye to dikhao, doosre ka wait na karo

  errorPlans = signal('');
  // '' (empty string) → koi error nahi → template mein nahi dikhega
  // 'Error message' → error aaya → template mein dikhega
  // @if(errorPlans()) → empty string falsy → nahi dikhega
  //                   → non-empty string truthy → dikhega

  errorAddOns = signal('');

  ngOnInit(): void {
  // Angular lifecycle hook:
  //   1. Component create hota hai (constructor)
  //   2. ngOnInit() → yahan API calls karo   ← YEH WALA
  //   3. Component updates
  //   4. ngOnDestroy()
  //
  // API calls constructor mein kyun nahi?
  //   Constructor mein DOM ready nahi hota
  //   Angular dependency injection poora nahi hota
  //   ngOnInit mein sab ready hota hai

    this.loadPlans();
    this.loadAddOns();
    // Dono ek hi time pe call kiye → Parallel execution
    // JavaScript single-threaded hai but HTTP calls async hain
    // loadPlans() call kiya → HTTP request background mein gayi
    // Turant loadAddOns() call kiya → yeh bhi background mein gayi
    // Dono response ka wait ek saath kiya → faster!
  }

  loadPlans(): void {
    this.servicesService.getServicePlans().subscribe({
    // .getServicePlans() → Observable return karta hai (request nahi gayi abhi)
    // .subscribe() → Observable activate kiya → AB request jaayegi

      next: (data) => {
      // next: callback → HTTP 200 OK response pe chalta hai
      // data → backend ka response: ServicePlan[] array

        this.plans.set(data.filter(p => p.isActive));
        // data.filter() → Array method → ek naya filtered array return karta hai
        // Arrow function: p => p.isActive
        //   p → har ek ServicePlan item
        //   p.isActive → true ya false
        //   filter → sirf true wale rakho
        //
        // Example:
        // data = [
        //   { id:1, name:'Basic', isActive: true },
        //   { id:2, name:'Old',   isActive: false },
        //   { id:3, name:'Ultra', isActive: true }
        // ]
        // After filter = [
        //   { id:1, name:'Basic', isActive: true },
        //   { id:3, name:'Ultra', isActive: true }
        // ]
        //
        // plans.set([...]) → signal update → template auto re-render

        this.isLoadingPlans.set(false);
        // Loading khatam → skeleton hata do, plans dikhao
      },

      error: () => {
      // error: callback → network error, backend down, timeout pe chalta hai
      // () → error object ignore kar rahe hain (just message dikhana hai)

        this.errorPlans.set('Service plans load nahi ho sake. Backend chal raha hai?');
        // Error signal set karo → template mein error message dikhega

        this.isLoadingPlans.set(false);
        // Loading hatao → error message dikhao skeleton ki jagah
      }
    });
  }

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

  getPlanIcon(name: string): string {
  // Plan ke naam se icon return karo
  // Template mein call hota hai: {{ getPlanIcon(plan.name) }}

    const lower = name.toLowerCase();
    // toLowerCase() → case insensitive comparison
    // "BASIC" → "basic", "Premium" → "premium"

    if (lower.includes('basic') || lower.includes('standard')) return '🚿';
    // .includes() → string mein yeh word hai?
    // "basic wash".includes('basic') → true → '🚿' return

    if (lower.includes('premium') || lower.includes('deluxe')) return '✨';
    if (lower.includes('ultra') || lower.includes('full')) return '💎';
    return '🚗';
    // Default → koi match nahi mila
  }
}
```

---

## FILE 4 — `src/app/pages/services/services.html`

**Kaam:** 3 states handle karna (loading → error → data) aur plans + addons display karna

```html
<!-- ====== HERO SECTION ====== -->
<div class="services-page">
<section class="services-hero">
  <h1>Our Services</h1>
  <p>Premium car wash packages aur extra add-ons choose karo</p>
  <!-- Static content — koi binding nahi -->
</section>

<!-- ====== SERVICE PLANS SECTION ====== -->
<section class="section">
  <div class="container">
    <h2 class="section-title">Wash Packages</h2>

    <!-- STATE 1: LOADING -->
    @if (isLoadingPlans()) {
    <!-- isLoadingPlans() → Signal read karo, parentheses zaroori -->
    <!-- true ho to yeh block render hoga -->

      <div class="loading-grid">
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
        <!-- 3 fake cards — shimmer animation SCSS mein hai -->
      </div>
    }

    <!-- STATE 2: ERROR -->
    @if (errorPlans()) {
    <!-- errorPlans() → empty string '' = falsy = nahi dikhega -->
    <!--              → 'error text' = truthy = dikhega -->

      <div class="error-message">{{ errorPlans() }}</div>
      <!-- {{ errorPlans() }} → Signal ki value interpolate karo -->
    }

    <!-- STATE 3: DATA (loading khatam + koi error nahi) -->
    @if (!isLoadingPlans() && !errorPlans()) {
    <!-- !isLoadingPlans() → loading false ho -->
    <!-- && → AND condition → dono true honi chahiye -->
    <!-- !errorPlans() → error empty ho -->

      <!-- EMPTY STATE: Data hai but koi plan nahi -->
      @if (plans().length === 0) {
      <!-- plans() → Signal read karo → array milega -->
      <!-- .length → kitne items hain -->
      <!-- === 0 → strict equality → exactly zero -->

        <div class="empty-state">
          <span class="empty-icon">📋</span>
          <p>Abhi koi service plan available nahi hai.</p>
        </div>

      } @else {
      <!-- Plans hain → grid dikhao -->

        <div class="plans-grid">

          @for (plan of plans(); track plan.id) {
          <!-- @for → Angular 17+ loop syntax -->
          <!-- plan of plans() → plans() array se ek ek item nikalo -->
          <!--   first iteration: plan = plans()[0] -->
          <!--   second iteration: plan = plans()[1] -->
          <!--   ... -->
          <!-- track plan.id → Angular ko unique identifier batao -->
          <!--   Id unique hai har plan ka → Angular efficiently update kar sakta hai -->

            <div class="plan-card">

              <div class="plan-icon">{{ getPlanIcon(plan.name) }}</div>
              <!-- getPlanIcon(plan.name) → method call → emoji string return -->
              <!-- {{ ... }} → interpolation → string HTML mein render hoga -->

              <h3 class="plan-name">{{ plan.name }}</h3>
              <!-- plan.name → is iteration ka plan object ka naam -->

              <p class="plan-description">{{ plan.description }}</p>

              <div class="plan-price">
                <span class="price-amount">Rs. {{ plan.price }}</span>
                <!-- plan.price → number → Angular number ko string mein convert karta hai -->
              </div>

              <div class="plan-footer">
                <span class="active-badge">Available</span>
                <!-- Sirf active plans filter kiye hain → sab "Available" hain -->
              </div>

            </div>
          }
          <!-- @for end -->

        </div>
      }
    }
    <!-- STATE 3 end -->

  </div>
</section>

<!-- ====== ADD-ONS SECTION ====== -->
<section class="section addons-section">
  <div class="container">
    <h2 class="section-title">Extra Add-Ons</h2>

    <!-- Same 3 states: loading, error, data -->
    @if (isLoadingAddOns()) {
      <div class="loading-grid">
        <div class="skeleton-card small"></div>
        <div class="skeleton-card small"></div>
        <div class="skeleton-card small"></div>
        <div class="skeleton-card small"></div>
      </div>
    }

    @if (errorAddOns()) {
      <div class="error-message">{{ errorAddOns() }}</div>
    }

    @if (!isLoadingAddOns() && !errorAddOns()) {
      @if (addOns().length === 0) {
        <div class="empty-state">
          <span class="empty-icon">🔧</span>
          <p>Abhi koi add-on available nahi hai.</p>
        </div>
      } @else {
        <div class="addons-grid">
          @for (addon of addOns(); track addon.id) {
            <div class="addon-card">
              <div class="addon-info">
                <span class="addon-icon">➕</span>
                <span class="addon-name">{{ addon.name }}</span>
              </div>
              <span class="addon-price">Rs. {{ addon.price }}</span>
            </div>
          }
        </div>
      }
    }

  </div>
</section>

<!-- ====== CTA SECTION ====== -->
<section class="cta-section">
  <div class="container">
    <h2>Ready to Book?</h2>
    <p>Apna package choose karo aur aaj hi apni car sparkle karwao</p>
    <a routerLink="/login" class="btn btn-primary btn-large">Book Now</a>
    <!-- routerLink="/login" → RouterLink directive → /login pe navigate karo -->
    <!-- RouterLink import kiya tha services.ts ke imports array mein -->
  </div>
</section>
</div>
```

---

## FILE 5 — `src/app/pages/services/services.scss`

**Kaam:** Services page ki visual styling — cards, grid, skeleton animation

```scss
// SCSS = CSS ka superset — variables, nesting, &:hover use kar sakte hain

// Hero Section
.services-hero {
  background: linear-gradient(135deg, var(--primary-color), #1a56db);
  // var(--primary-color) → CSS variable → styles.scss mein defined
  // linear-gradient → do colors ke beech gradient

  color: white;
  text-align: center;
  padding: 80px 20px 60px;
  // 80px top, 20px sides, 60px bottom

  h1 {
    font-size: 2.8rem;
    // rem = root em = relative to root font-size (16px default)
    // 2.8rem = 2.8 × 16px = 44.8px
  }
}

// Plans Grid — responsive layout
.plans-grid {
  display: grid;
  // CSS Grid Layout — powerful 2D layout system

  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  // auto-fit → jitne cards fit hon utne columns
  // minmax(280px, 1fr) → minimum 280px, maximum 1fr (equal share)
  // Ek screen pe: 3 cards → 3 columns
  // Mobile mein: 1 card → 1 column (280px fit nahi hota side by side)

  gap: 24px;
  // Cards ke beech space
}

// Individual Plan Card
.plan-card {
  background: white;
  border-radius: 16px;
  // Corners round karo → modern look

  padding: 32px 24px;
  // 32px top-bottom, 24px left-right

  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  // Shadow → card ka 3D effect
  // rgba(0,0,0,0.08) → black with 8% opacity → subtle

  transition: transform 0.2s, box-shadow 0.2s;
  // Hover pe smooth animation → 0.2 second mein change

  border: 2px solid transparent;
  // Transparent border — hover pe color aayega

  &:hover {
  // & → parent selector (.plan-card)
  // .plan-card:hover → mouse hover pe

    transform: translateY(-4px);
    // 4px upar uthao → floating effect

    box-shadow: 0 8px 30px rgba(0, 0, 0, 0.12);
    // Deeper shadow → utha hua lagta hai

    border-color: var(--primary-color);
    // Blue border appear hogi
  }
}

// Skeleton Loading Animation
.skeleton-card {
  height: 280px;
  background: linear-gradient(
    90deg,
    #f0f0f0 25%,   // Light grey
    #e0e0e0 50%,   // Slightly darker — shimmer effect
    #f0f0f0 75%    // Light grey again
  );
  background-size: 200% 100%;
  // Background 2x wide — animation mein slide karega

  animation: shimmer 1.5s infinite;
  // shimmer → @keyframes defined below
  // 1.5s → 1.5 second mein ek cycle
  // infinite → repeat karte raho
  
  border-radius: 16px;

  &.small {
  // .skeleton-card.small → dono classes saath ho
    height: 70px;
    // AddOns cards chhote hain → chhota skeleton
  }
}

@keyframes shimmer {
  0%   { background-position: 200% 0; }
  // Start: gradient right side pe
  100% { background-position: -200% 0; }
  // End: gradient left side pe
  // Effect: gradient right se left slide karta hai → shimmer
}
```

---

## FILE 6 — `src/app/app.routes.ts` (Updated)

**Kaam:** `/services` route add kiya

```typescript
import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/guards/auth.guard';

export const routes: Routes = [

  // Phase 1 — Public
  { path: '', loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent) },

  // Phase 3 NEW — Public (koi guard nahi)
  {
    path: 'services',
    // No canActivate → public route → koi bhi dekh sakta hai
    // Login kiya ho ya na kiya ho
    loadComponent: () => import('./pages/services/services')
                         .then(m => m.ServicesComponent)
    // Lazy loading:
    // import('./pages/services/services') → file dynamically load karo
    // .then(m => m.ServicesComponent)     → ServicesComponent class nikalo
    // Jab tak user /services nahi jaata → yeh file download nahi hogi
  },

  // Phase 2 — Guest only
  { path: 'login',    canActivate: [guestGuard], loadComponent: () => import('./pages/auth/login/login').then(m => m.LoginComponent) },
  { path: 'register', canActivate: [guestGuard], loadComponent: () => import('./pages/auth/register/register').then(m => m.RegisterComponent) },

  // Phase 2 — Auth required
  { path: 'profile',  canActivate: [authGuard],  loadComponent: () => import('./pages/profile/profile').then(m => m.ProfileComponent) },

  { path: '**', redirectTo: '' }
];
```

**Teen types ke routes summary:**
```
| Route type  | canActivate   | Accessible to         |
|-------------|---------------|-----------------------|
| Public      | (none)        | Sab log               |
| guestGuard  | [guestGuard]  | Sirf logged-OUT       |
| authGuard   | [authGuard]   | Sirf logged-IN        |
|-------------|---------------|-----------------------|
| /           | none          | Sab                   |
| /services   | none          | Sab  ← Phase 3        |
| /login      | guestGuard    | Logged out users      |
| /register   | guestGuard    | Logged out users      |
| /profile    | authGuard     | Logged in users       |
```

---

## COMPLETE FLOW 1 — User /services kholta hai (Data milta hai)

```
User browser mein type karta hai: localhost:4200/services
        ↓
FILE: src/app/app.routes.ts
  { path: 'services', loadComponent: ... }
  No canActivate → koi guard check nahi
  Route match → ServicesComponent lazy load karo
        ↓
FILE: src/app/pages/services/services.ts → ServicesComponent class
  @Component creates — signals initialize hote hain:
    plans          = signal([])   → empty array
    addOns         = signal([])   → empty array
    isLoadingPlans = signal(true) → loading start
    isLoadingAddOns= signal(true) → loading start
    errorPlans     = signal('')   → no error
    errorAddOns    = signal('')   → no error
        ↓
Angular lifecycle: ngOnInit() call hota hai
  this.loadPlans()    → start
  this.loadAddOns()   → start (dono parallel)
        ↓
FILE: src/app/pages/services/services.html
  PEHLE render (data aane se PEHLE):
    @if (isLoadingPlans()) → true → Skeleton cards dikhte hain
    @if (isLoadingAddOns()) → true → Skeleton cards dikhte hain
  User ko shimmer animation dikhti hai
        ↓
BACKGROUND MEIN:

FILE: src/app/core/services/services.service.ts → getServicePlans()
  this.http.get<ServicePlan[]>('http://localhost:5001/api/serviceplans')
  Observable activate → HTTP GET request jaati hai
        ↓
FILE: src/app/core/interceptors/auth.interceptor.ts
  token = localStorage.getItem('carwash_token')
  → Token hai? 
    [Logged in user] → token hai → Authorization: Bearer eyJ... header add
    [Logged out user] → token nahi → request as-is jaati hai
  → Dono cases mein request aage jaati hai (AllowAnonymous endpoint)
        ↓
Backend: GET /api/serviceplans
  ServicePlansController.GetAllPlans()
  [AllowAnonymous] → No auth check
  Database se sare plans fetch karo
  Return: List<ServicePlanResponse>
  Response: [
    { id:1, name:"Basic Wash", price:199, isActive:true, ... },
    { id:2, name:"Premium",    price:399, isActive:true, ... },
    { id:3, name:"Old Plan",   price:99,  isActive:false, ... }
  ]
        ↓
FILE: src/app/core/services/services.service.ts
  HTTP 200 OK → Observable emit karta hai response
        ↓
FILE: src/app/pages/services/services.ts → loadPlans() → next() callback
  data = [3 plans received]
  data.filter(p => p.isActive):
    id:1 isActive:true  → rakho
    id:2 isActive:true  → rakho
    id:3 isActive:false → hatao
  filtered = [Basic Wash, Premium]
  this.plans.set([Basic Wash, Premium])
  → Signal update → Angular template ke saare plan() calls re-evaluate
  this.isLoadingPlans.set(false)
  → isLoadingPlans Signal change → template update
        ↓
FILE: src/app/pages/services/services.html — AUTO RE-RENDER
  @if (isLoadingPlans()) → false → Skeleton HATA do
  @if (!isLoadingPlans() && !errorPlans()) → true → Plans section dikhao
    @if (plans().length === 0) → 2 plans hain → false → skip
    @else → Plans grid dikhao
      @for (plan of plans(); track plan.id)
        Card 1: "Basic Wash" — Rs. 199
        Card 2: "Premium" — Rs. 399
        (Old Plan filter ho gaya — nahi dikhega)
        ↓
AddOns bhi same parallel flow se load hote hain
```

---

## COMPLETE FLOW 2 — Backend Down hai (Error state)

```
User /services kholta hai
        ↓
ngOnInit() → loadPlans() + loadAddOns() call
        ↓
Template: isLoadingPlans = true → Skeleton dikhta hai
        ↓
FILE: services.service.ts → http.get('/api/serviceplans')
HTTP request jaati hai... backend down hai
        ↓
Network error / Connection refused / Timeout
        ↓
FILE: services.ts → loadPlans() → error() callback
  this.errorPlans.set('Service plans load nahi ho sake. Backend chal raha hai?')
  → Signal update
  this.isLoadingPlans.set(false)
  → Signal update
        ↓
FILE: services.html — AUTO RE-RENDER
  @if (isLoadingPlans()) → false → Skeleton HATA do
  @if (errorPlans()) → 'Service plans load...' truthy → Error message DIKHAO
  @if (!isLoadingPlans() && !errorPlans()) → false (errorPlans truthy) → Plans grid nahi dikhega
        ↓
User ko dikhta hai:
  "Service plans load nahi ho sake. Backend chal raha hai?"
```

---

## COMPLETE FLOW 3 — Backend Mein Koi Plan Nahi (Empty State)

```
ngOnInit() → loadPlans()
        ↓
Backend: GET /api/serviceplans
  Database empty hai → Return: [] (empty array)
        ↓
FILE: services.ts → next() callback
  data = [] (empty array)
  data.filter(p => p.isActive) → still [] (koi item nahi tha)
  this.plans.set([])   → Signal: empty array
  this.isLoadingPlans.set(false)
        ↓
FILE: services.html — AUTO RE-RENDER
  @if (!isLoadingPlans() && !errorPlans()) → true → enter
    @if (plans().length === 0) → 0 === 0 → true
      Empty state dikhao:
        📋
        "Abhi koi service plan available nahi hai."
```

---

## SARI FILES KA CONNECTION MAP

```
src/app/
│
├── app.routes.ts
│   └── { path: 'services', loadComponent: ServicesComponent }
│       No guard → public access
│
├── models/
│   └── services.models.ts
│       ├── ServicePlan  → services.service.ts + services.ts mein use
│       └── AddOn        → services.service.ts + services.ts mein use
│
├── core/
│   ├── services/
│   │   └── services.service.ts
│   │       ├── getServicePlans() → Observable<ServicePlan[]>
│   │       │     → GET /api/serviceplans
│   │       │     → services.ts.loadPlans() subscribe karta hai
│   │       └── getAddOns() → Observable<AddOn[]>
│   │             → GET /api/addons
│   │             → services.ts.loadAddOns() subscribe karta hai
│   │
│   └── interceptors/
│       └── auth.interceptor.ts
│           Har request ko intercept karta hai
│           Token hai → Authorization header add
│           Token nahi → as-is (AllowAnonymous endpoints accept kar lete hain)
│
└── pages/
    └── services/
        ├── services.ts     ← CONTROLLER
        │   signals: plans, addOns, isLoading*, error*
        │   ngOnInit → loadPlans() + loadAddOns() parallel
        │   loadPlans() → servicesService.getServicePlans().subscribe()
        │   loadAddOns() → servicesService.getAddOns().subscribe()
        │
        ├── services.html   ← VIEW
        │   @if(isLoading) → skeleton
        │   @if(error)     → error message
        │   @if(!loading && !error)
        │     @if(plans.length === 0) → empty state
        │     @else → @for plans → plan cards
        │   Same pattern for addons
        │
        └── services.scss   ← STYLING
            plans-grid → CSS Grid (auto-fit)
            plan-card → white card + hover lift effect
            skeleton-card → shimmer animation (@keyframes)
            addons-grid → smaller cards
```

---

## SIGNAL STATE MACHINE — Samajhna Zaroori Hai

```
Initial State:
  isLoading = true
  error     = ''
  data      = []
  UI: SKELETON

After successful API:
  isLoading = false  ← .set(false)
  error     = ''     ← unchanged
  data      = [...]  ← .set(filteredData)
  UI: PLANS GRID

After API error:
  isLoading = false  ← .set(false)
  error     = 'msg'  ← .set('message')
  data      = []     ← unchanged
  UI: ERROR MESSAGE

After API success but empty:
  isLoading = false  ← .set(false)
  error     = ''     ← unchanged
  data      = []     ← .set([]) (filter ke baad bhi empty)
  UI: EMPTY STATE
```

**Signal ka reactive magic:**
- Har `.set()` call → Angular automatically template re-render karta hai
- Hume manually DOM update nahi karna
- Hume `detectChanges()` nahi call karna
- Signal change detect karta hai → sirf wahi part update hota hai jo signal use kar raha hai

---

*Phase 3 Services Workflow — Complete*
*Next: Phase 4 — Cars Management (CRUD operations, authGuard protected)*
