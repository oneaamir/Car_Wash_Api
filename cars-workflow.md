# Phase 4 — Cars Management Workflow
# File Locations + Line-by-Line Code + Complete Flow
> Yeh file sirf Phase 4 ke liye hai — har cheez detail mein

---

## SARI FILES AUR UNKA KAAM

| File | Location | Kaam |
|------|----------|------|
| Models | `src/app/models/car.models.ts` | Car, CreateCarRequest, UpdateCarRequest interfaces |
| Service | `src/app/core/services/car.service.ts` | Backend CRUD endpoints call karna |
| Page TS | `src/app/pages/cars/cars.ts` | List + Form logic, signals, CRUD methods |
| Page HTML | `src/app/pages/cars/cars.html` | Cars grid + Add/Edit form display |
| Page SCSS | `src/app/pages/cars/cars.scss` | Cards, form layout, skeleton animation |
| Routes | `src/app/app.routes.ts` | `/cars` route — authGuard laga |
| Header | `src/app/shared/components/header/header.html` | "My Cars" nav link add kiya |

---

## FILE 1 — `src/app/models/car.models.ts`

**Kaam:** Teen interfaces — Car (response), CreateCarRequest (POST body), UpdateCarRequest (PUT body)

```typescript
// Car interface — Backend se aane wala response
export interface Car {
  id: number;
  // Database mein auto-generated primary key
  // Edit aur Delete ke waqt yeh id use hogi URL mein: /api/cars/5

  userId: number;
  // Yeh car kis user ki hai
  // Backend JWT token se automatically set karta hai

  carNumber: string;
  // Number plate → "MH12AB1234"

  brand: string;
  // Car manufacturer → "Toyota", "Honda", "Maruti"

  model: string;
  // Car model → "Corolla", "City", "Swift"

  carType: string;
  // Type: "Sedan", "SUV", "Hatchback", etc.

  imageUrl: string;
  // Car ki photo URL (optional — empty string bhi ho sakta hai)

  isActive: boolean;
  // true → car visible, false → soft deleted

  message: string;
  // Backend ka response message
}

// CreateCarRequest — POST body ke liye
// id aur userId nahi hote → backend khud set karta hai
export interface CreateCarRequest {
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string;
}

// UpdateCarRequest — PUT body ke liye
// Same fields as CreateCarRequest
// id URL mein jaata hai: PUT /api/cars/5
export interface UpdateCarRequest {
  carNumber: string;
  brand: string;
  model: string;
  carType: string;
  imageUrl: string;
}
```

**Teen interfaces kyun?**
```
Car             → Backend response read karne ke liye
                  (id + userId bhi aata hai)

CreateCarRequest → POST body likhne ke liye
                  (id/userId backend set karta hai)

UpdateCarRequest → PUT body likhne ke liye
                  (id URL mein jaata hai, body mein nahi)
```

---

## FILE 2 — `src/app/core/services/car.service.ts`

**Kaam:** Chaar HTTP methods — GET, POST, PUT, DELETE (poora CRUD)

```typescript
import { Injectable, inject } from '@angular/core';
// Injectable → Angular DI mein register karo
// inject → functional dependency injection (Angular 15+)

import { HttpClient } from '@angular/common/http';
// HTTP requests ke liye

import { Observable } from 'rxjs';
// Return type — async response

import { environment } from '../../../environments/environment';
// apiUrl: 'http://localhost:5001/api'

import { Car, CreateCarRequest, UpdateCarRequest } from '../../models/car.models';
// TypeScript type safety ke liye

@Injectable({ providedIn: 'root' })
// Singleton — poori app mein ek hi instance
export class CarService {

  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getMyCars(): Observable<Car[]> {
  // Return type: Observable<Car[]> → array of Car objects
    return this.http.get<Car[]>(`${this.apiUrl}/cars/my`);
    // GET http://localhost:5001/api/cars/my
    //
    // Backend controller:
    //   [Authorize] → JWT token required
    //   /my → sirf current user ki cars
    //   Auth interceptor JWT header automatically add karta hai
    //   Backend JWT se userId nikalta hai → sirf us user ki cars return
  }

  createCar(data: CreateCarRequest): Observable<Car> {
  // Parameter: data → CreateCarRequest object
  // Return: Observable<Car> → naya create hua Car object
    return this.http.post<Car>(`${this.apiUrl}/cars`, data);
    // POST http://localhost:5001/api/cars
    // Body: { carNumber, brand, model, carType, imageUrl }
    //
    // JSON.stringify automatically hota hai HttpClient se
    // Content-Type: application/json header automatic
    //
    // Backend:
    //   [Authorize] → token zaroori
    //   JWT se userId nikalo
    //   New Car object banao + userId set karo
    //   Database mein save karo
    //   Return: newly created Car with id
  }

  updateCar(id: number, data: UpdateCarRequest): Observable<Car> {
  // id → URL mein jaayega: /cars/5
  // data → PUT body
    return this.http.put<Car>(`${this.apiUrl}/cars/${id}`, data);
    // PUT http://localhost:5001/api/cars/5
    // id URL path variable mein hai
    // Body: { carNumber, brand, model, carType, imageUrl }
    //
    // Backend:
    //   [Authorize] → token zaroori
    //   Car find karo by id
    //   Check: yeh car is user ki hai?
    //   Fields update karo
    //   Return: updated Car
  }

  deleteCar(id: number): Observable<Car> {
  // id → URL mein jaayega: /cars/5
    return this.http.delete<Car>(`${this.apiUrl}/cars/${id}`);
    // DELETE http://localhost:5001/api/cars/5
    // No body — sirf URL pe id
    //
    // Backend:
    //   [Authorize] → token zaroori
    //   Car find karo by id
    //   Check: yeh car is user ki hai?
    //   Soft delete ya hard delete karo
    //   Return: deleted Car
  }
}
```

**CRUD = 4 HTTP Methods:**
```
C = Create → POST   → /api/cars        (body mein data)
R = Read   → GET    → /api/cars/my     (koi body nahi)
U = Update → PUT    → /api/cars/5      (id URL + body mein data)
D = Delete → DELETE → /api/cars/5      (id URL, koi body nahi)
```

---

## FILE 3 — `src/app/pages/cars/cars.ts`

**Kaam:** Cars page ka poora brain — list state + form state + CRUD methods

```typescript
import { Component, OnInit, inject, signal } from '@angular/core';
// Component → @Component decorator
// OnInit → ngOnInit lifecycle hook
// inject → DI
// signal → reactive state

import { FormsModule } from '@angular/forms';
// Template-driven forms ke liye zaroori
// [(ngModel)] directive FormsModule se aata hai

import { CarService } from '../../core/services/car.service';
import { Car } from '../../models/car.models';

@Component({
  selector: 'app-cars',
  standalone: true,
  imports: [FormsModule],
  // FormsModule import kiya → [(ngModel)] kaam karega
  // Bina import ke: "ngModel is not a known property" error

  templateUrl: './cars.html',
  styleUrl: './cars.scss'
})
export class CarsComponent implements OnInit {

  private carService = inject(CarService);
  // private → template mein use nahi hoga
  // Angular DI se CarService ka singleton instance milega

  // ======= LIST STATE SIGNALS =======

  cars = signal<Car[]>([]);
  // Initial: empty array
  // loadCars() ke baad: user ki cars ka array
  // template mein: cars() → array read karo

  isLoading = signal(true);
  // true → skeleton dikhao
  // false → cars ya empty state dikhao

  errorMsg = signal('');
  // '' → koi error nahi
  // 'message' → error dikhao

  // ======= FORM STATE SIGNALS =======

  showForm = signal(false);
  // false → form hidden
  // true → form visible

  isEditing = signal(false);
  // false → Add mode (POST)
  // true → Edit mode (PUT)

  editingCarId = signal<number | null>(null);
  // null → Add mode (koi car edit nahi ho rahi)
  // 5 → car id=5 edit ho rahi hai (PUT /api/cars/5)

  isSubmitting = signal(false);
  // false → button clickable
  // true → "Saving..." dikhao, button disable

  formError = signal('');
  // Form submit error message

  successMsg = signal('');
  // Success ke baad: "Car successfully added!"

  // ======= FORM DATA (NOT a Signal) =======

  formData = {
    carNumber: '',
    brand: '',
    model: '',
    carType: '',
    imageUrl: ''
  };
  // Plain object — signal nahi
  // Kyun? [(ngModel)] plain objects ke saath better kaam karta hai
  // Template mein: [(ngModel)]="formData.brand"
  //   User type kare → formData.brand automatically update
  //   formData.brand change ho → input field automatically update

  carTypes = ['Sedan', 'SUV', 'Hatchback', 'Pickup', 'Van', 'Truck', 'Motorcycle', 'Other'];
  // <select> dropdown ke options
  // @for (type of carTypes; track type) → template mein loop

  ngOnInit(): void {
    this.loadCars();
    // Component load → turant cars fetch karo
    // Services page se farak: wahan 2 parallel calls (plans + addons)
    // Yahan sirf 1 call: getMyCars()
  }

  loadCars(): void {
    this.isLoading.set(true);
    // Pehle loading true karo
    // Add/Edit/Delete ke baad bhi call hota hai → loading reset
    
    this.carService.getMyCars().subscribe({
      next: (data) => {
        this.cars.set(data);
        // Backend ne saari cars return ki → set karo
        // Services page ki tarah .filter() nahi kiya
        // Kyun? /cars/my sirf current user ki active cars return karta hai
        
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMsg.set('Cars load nahi ho sake. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  openAddForm(): void {
    this.resetForm();
    // formData clear karo (pehle wala data na rahe)
    
    this.isEditing.set(false);
    // Add mode
    
    this.editingCarId.set(null);
    // Koi car edit nahi ho rahi
    
    this.showForm.set(true);
    // Form dikhao
  }

  openEditForm(car: Car): void {
  // car → template se pass hota hai: (click)="openEditForm(car)"
  // car = poora Car object (id bhi hai, brand bhi, model bhi)

    this.formData = {
      carNumber: car.carNumber,
      brand: car.brand,
      model: car.model,
      carType: car.carType,
      imageUrl: car.imageUrl
    };
    // formData mein existing car ki values bharo
    // Yahi values input fields mein dikhenge → user sirf edit karega

    this.isEditing.set(true);
    // Edit mode → onSubmit() mein PUT use hoga

    this.editingCarId.set(car.id);
    // Car ki id save karo → PUT /api/cars/{id} mein use hogi

    this.formError.set('');
    this.successMsg.set('');
    // Pehle ke messages clear karo

    this.showForm.set(true);
    // Form dikhao

    setTimeout(() => {
      document.getElementById('car-form')?.scrollIntoView({ behavior: 'smooth' });
    }, 100);
    // Smooth scroll to form
    // setTimeout(fn, 100) → 100ms baad run karo
    //   Kyun 100ms? showForm.set(true) ke baad Angular DOM update karta hai
    //   Agar 0ms mein scroll karo → element abhi DOM mein nahi hoga
    //   100ms wait → Angular render kare → phir scroll
    //
    // document.getElementById('car-form') → HTML element dhundo id se
    // ?. → optional chaining: null ho to crash mat karo
    // .scrollIntoView({ behavior: 'smooth' }) → smooth scroll
  }

  closeForm(): void {
    this.showForm.set(false);
    // Form chhupaao
    
    this.resetForm();
    // Data clear karo
  }

  onSubmit(): void {
    this.isSubmitting.set(true);
    // Button disable karo + "Saving..." dikhao
    
    this.formError.set('');
    this.successMsg.set('');
    // Pehle ke messages clear karo

    if (this.isEditing() && this.editingCarId() !== null) {
    // isEditing() → signal read karo
    // editingCarId() !== null → id hai? (null nahi?)

      // ===== UPDATE (PUT) =====
      this.carService.updateCar(this.editingCarId()!, this.formData).subscribe({
      // this.editingCarId()! → non-null assertion: ! matlab "main guarantee karta hoon yeh null nahi hai"
      //   TypeScript ko trust dilana → null check pehle hi kiya tha if mein
      // this.formData → UpdateCarRequest compatible hai (same fields)

        next: () => {
        // next: () → () matlab response object ignore kar rahe hain
        // Hume sirf pata chahiye: success hua ya nahi
          this.successMsg.set('Car successfully updated!');
          this.isSubmitting.set(false);
          this.closeForm();
          // Form band karo + data clear karo
          this.loadCars();
          // Fresh list fetch karo → updated car dikhega
        },
        error: (err) => {
          this.formError.set(err.error?.message || 'Update failed. Please try again.');
          // err.error?.message → Backend ka error message
          //   err → HTTP error response object
          //   err.error → response body
          //   err.error?.message → body mein message field hai?
          // || 'Update failed...' → message nahi mila to fallback
          this.isSubmitting.set(false);
        }
      });

    } else {
      // ===== CREATE (POST) =====
      this.carService.createCar(this.formData).subscribe({
      // formData → CreateCarRequest compatible hai

        next: () => {
          this.successMsg.set('Car successfully added!');
          this.isSubmitting.set(false);
          this.closeForm();
          this.loadCars();
          // Same pattern: close + refresh
        },
        error: (err) => {
          this.formError.set(err.error?.message || 'Could not add car. Please try again.');
          this.isSubmitting.set(false);
        }
      });
    }
  }

  deleteCar(car: Car): void {
  // car → template se: (click)="deleteCar(car)"

    if (!confirm(`"${car.brand} ${car.model}" delete karna chahte ho?`)) return;
    // confirm() → browser built-in dialog box dikhata hai
    //   Message: "Toyota Corolla delete karna chahte ho?"
    //   Two buttons: OK aur Cancel
    //   OK → true return, Cancel → false return
    //
    // !confirm(...) → negate karo
    //   Cancel → !false → true → return → function exit (kuch nahi hoga)
    //   OK    → !true  → false → return skip → delete aage badhega
    //
    // Template literal: `"${car.brand} ${car.model}"` 
    //   = `"Toyota Corolla"` → dynamic message

    this.carService.deleteCar(car.id).subscribe({
    // car.id → DELETE /api/cars/5

      next: () => {
        this.successMsg.set('Car deleted successfully.');
        this.loadCars();
        // Refresh list → deleted car nahi dikhegi
        // closeForm() nahi → form open ho to bhi delete kar sakte hain
      },
      error: () => {
        this.errorMsg.set('Delete failed. Please try again.');
      }
    });
  }

  getCarIcon(carType: string): string {
  // Car type se emoji return karo
  // Template mein: {{ getCarIcon(car.carType) }}
    const type = carType.toLowerCase();
    // "SUV" → "suv", "Truck" → "truck"
    
    if (type === 'suv') return '🚙';
    if (type === 'truck' || type === 'pickup') return '🚚';
    if (type === 'motorcycle') return '🏍️';
    if (type === 'van') return '🚐';
    return '🚗';
    // Default for Sedan, Hatchback, Other
  }

  private resetForm(): void {
  // private → sirf is class ke methods call kar sakte hain
  // Template se nahi call hota
    this.formData = { carNumber: '', brand: '', model: '', carType: '', imageUrl: '' };
    // Naya empty object assign karo
    this.formError.set('');
    this.successMsg.set('');
    this.isSubmitting.set(false);
  }
}
```

---

## FILE 4 — `src/app/pages/cars/cars.html`

**Kaam:** 3 sections — Page header, Add/Edit Form, Cars Grid

```html
<div class="cars-page">

  <!-- ====== PAGE HEADER ====== -->
  <div class="page-header">
    <div class="container">
      <div class="header-row">
        <div>
          <h1>My Cars</h1>
          <p>Apni saari gaadiyaan yahan manage karo</p>
        </div>

        @if (!showForm()) {
        <!-- showForm() → Signal read karo -->
        <!-- Form open nahi hai → "Add New Car" button dikhao -->
        <!-- Form open hai → button mat dikhao (page cluttered nahi hoga) -->

          <button class="btn btn-primary" (click)="openAddForm()">
            + Add New Car
          </button>
          <!-- (click)="openAddForm()" → Event binding -->
          <!-- Button click → openAddForm() method call -->
        }
      </div>
    </div>
  </div>

  <div class="container">

    <!-- ====== MESSAGES ====== -->
    @if (successMsg()) {
    <!-- successMsg() → '' = falsy = nahi dikhega -->
    <!-- 'Car added!' = truthy = dikhega -->
      <div class="success-message">{{ successMsg() }}</div>
    }

    @if (errorMsg()) {
      <div class="error-message">{{ errorMsg() }}</div>
    }

    <!-- ====== ADD / EDIT FORM ====== -->
    @if (showForm()) {
    <!-- showForm() true → form dikhao -->

      <div class="form-card" id="car-form">
      <!-- id="car-form" → scrollIntoView() ke liye target -->
      <!-- openEditForm() mein: document.getElementById('car-form').scrollIntoView() -->

        <div class="form-card-header">
          <h2>{{ isEditing() ? 'Edit Car' : 'Add New Car' }}</h2>
          <!-- Ternary operator: condition ? valueIfTrue : valueIfFalse -->
          <!-- isEditing() true → 'Edit Car' -->
          <!-- isEditing() false → 'Add New Car' -->
          <!-- Ek hi template → Add aur Edit dono handle karta hai -->

          <button class="close-btn" (click)="closeForm()">✕</button>
        </div>

        @if (formError()) {
          <div class="error-message">{{ formError() }}</div>
        }

        <form (ngSubmit)="onSubmit()" #carForm="ngForm">
        <!-- (ngSubmit) → form submit event (Enter ya button type="submit" press) -->
        <!-- onSubmit() → method call hoga submit pe -->
        <!--
             #carForm="ngForm" → Template Reference Variable
             carForm → NgForm directive ka instance
             NgForm → FormsModule se aata hai
             carForm.invalid → koi bhi required field empty hai?
             carForm.valid → sab fields valid hain?
        -->

          <div class="form-grid">

            <div class="form-group">
              <label for="carNumber">Car Number *</label>
              <input
                type="text"
                id="carNumber"
                name="carNumber"
                [(ngModel)]="formData.carNumber"
                <!-- Two-way binding: -->
                <!-- User type kare → formData.carNumber update -->
                <!-- formData.carNumber change ho → input update -->
                required
                <!-- HTML5 required attribute + Angular validation -->
                placeholder="MH12AB1234"
                #carNumberField="ngModel"
                <!-- Template ref var → is input ki NgModel instance -->
                <!-- carNumberField.invalid → field empty hai? -->
                <!-- carNumberField.touched → user ne click karke focus hataya? -->
              />
              @if (carNumberField.invalid && carNumberField.touched) {
              <!-- invalid && touched → Dono conditions zaroori -->
              <!-- Sirf invalid → Page load pe hi error dikhega (bad UX) -->
              <!-- invalid && touched → User ne field chhodi tab dikhao -->
                <span class="field-error">Car number required hai</span>
              }
            </div>

            <!-- Brand, Model → Same pattern as carNumber -->

            <div class="form-group">
              <label for="carType">Car Type *</label>
              <select
                id="carType"
                name="carType"
                [(ngModel)]="formData.carType"
                required
                #carTypeField="ngModel"
              >
                <option value="">-- Select Type --</option>
                <!-- Default empty option → carType initially '' → required validation fail → user forced to select -->

                @for (type of carTypes; track type) {
                <!-- carTypes = ['Sedan', 'SUV', 'Hatchback', ...] → TS mein defined -->
                <!-- @for → Angular loop → har type ke liye ek <option> -->
                <!-- track type → type string unique hai → use it as key -->
                <!--   (id nahi hai kyunki yeh plain strings hain) -->

                  <option [value]="type">{{ type }}</option>
                  <!-- [value]="type" → Property binding → dynamic value -->
                  <!--   [value] → Angular binding → type variable ki value -->
                  <!--   value="type" → Literal string "type" → GALAT HOGA -->
                  <!-- {{ type }} → Interpolation → display text -->
                }
              </select>
            </div>

            <div class="form-group full-width">
            <!-- full-width → SCSS mein: grid-column: 1 / -1 → poori row le lo -->
              <label for="imageUrl">Image URL <span class="optional">(optional)</span></label>
              <input
                type="url"
                id="imageUrl"
                name="imageUrl"
                [(ngModel)]="formData.imageUrl"
                <!-- required nahi → optional field -->
                placeholder="https://example.com/car.jpg"
              />
            </div>

          </div>

          <div class="form-actions">
            <button
              type="submit"
              class="btn btn-primary"
              [disabled]="isSubmitting() || carForm.invalid"
            >
            <!-- [disabled] → Property binding → true/false -->
            <!-- isSubmitting() → API call chal rahi hai? → disable -->
            <!-- carForm.invalid → required fields empty hain? → disable -->
            <!-- || → OR: koi bhi ek true ho → button disabled -->

              @if (isSubmitting()) {
                Saving...
              } @else {
                {{ isEditing() ? 'Update Car' : 'Add Car' }}
                <!-- Ternary again → Edit mode: "Update Car", Add mode: "Add Car" -->
              }
            </button>
            <button type="button" class="btn btn-secondary" (click)="closeForm()">
              Cancel
              <!-- type="button" → form submit nahi hoga is button se -->
              <!-- type="submit" nahi → sirf closeForm() call hoga -->
            </button>
          </div>

        </form>
      </div>
    }

    <!-- ====== CARS LIST ====== -->

    @if (isLoading()) {
      <div class="loading-grid">
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
        <div class="skeleton-card"></div>
        <!-- 3 fake cards → shimmer animation -->
      </div>
    }

    @if (!isLoading() && !errorMsg()) {
    <!-- Loading khatam + koi error nahi → cars ya empty state dikhao -->

      @if (cars().length === 0) {
        <div class="empty-state">
          <span class="empty-icon">🚗</span>
          <h3>Koi car nahi hai abhi</h3>
          <p>Apni pehli car add karo aur booking shuru karo!</p>
          @if (!showForm()) {
            <button class="btn btn-primary" (click)="openAddForm()">+ Add Your First Car</button>
          }
        </div>

      } @else {
        <div class="cars-grid">
          @for (car of cars(); track car.id) {
          <!-- car.id → unique database id → proper tracking key -->
          <!-- Agar track $index use karein → Add/Delete pe animation glitches -->

            <div class="car-card">

              <div class="car-visual">
                @if (car.imageUrl) {
                <!-- car.imageUrl → truthy (non-empty string) → image dikhao -->
                <!-- car.imageUrl = '' → falsy → icon dikhao -->

                  <img [src]="car.imageUrl" [alt]="car.brand + ' ' + car.model" class="car-image" />
                  <!-- [src] → Property binding → dynamic URL -->
                  <!--   src="car.imageUrl" → Literal "car.imageUrl" → GALAT HOGA -->
                  <!-- [alt] → Expression: brand + ' ' + model → "Toyota Corolla" -->

                } @else {
                  <div class="car-icon-placeholder">{{ getCarIcon(car.carType) }}</div>
                  <!-- getCarIcon("SUV") → '🚙' -->
                }
              </div>

              <div class="car-info">
                <div class="car-header">
                  <h3 class="car-name">{{ car.brand }} {{ car.model }}</h3>
                  <!-- "Toyota" + " " + "Corolla" → "Toyota Corolla" -->
                  <span class="car-type-badge">{{ car.carType }}</span>
                </div>
                <p class="car-number">🔢 {{ car.carNumber }}</p>
              </div>

              <div class="car-actions">
                <button class="btn-edit" (click)="openEditForm(car)">Edit</button>
                <!-- car → poora Car object pass ho raha hai method mein -->
                <!-- openEditForm(car) → car.id, car.brand, car.model sab milega -->

                <button class="btn-delete" (click)="deleteCar(car)">Delete</button>
                <!-- car → poora object pass → deleteCar(car) mein car.id use hoga -->
              </div>

            </div>
          }
        </div>
      }
    }

  </div>
</div>
```

---

## FILE 5 — `src/app/pages/cars/cars.scss`

**Kaam:** Cars page styling — page header, form card, 2-col form grid, car cards, skeleton

```scss
// Page Header — blue gradient banner
.page-header {
  background: linear-gradient(135deg, var(--primary-color), #1a56db);
  color: white;
  padding: 40px 0;
}

.header-row {
  display: flex;
  // Flexbox → horizontal layout
  align-items: center;
  // Vertical center alignment
  justify-content: space-between;
  // Title left pe, button right pe
  gap: 16px;
  flex-wrap: wrap;
  // Small screen pe wrap ho jaaye
}

// Form Card — white card with shadow
.form-card {
  background: white;
  border-radius: 16px;
  padding: 28px;
  margin: 24px 0;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.08);
  // Subtle shadow → card looks elevated
}

// Form Grid — 2 columns on desktop, 1 on mobile
.form-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  // 2 equal columns
  gap: 16px;

  @media (max-width: 600px) {
  // Screen 600px se chhota → mobile
    grid-template-columns: 1fr;
    // 1 column → fields stack vertically
  }
}

.full-width {
  grid-column: 1 / -1;
  // 1 = column 1 se start
  // -1 = last column tak
  // Matlab: poori row le lo (Image URL field ke liye)
}

// Cars Grid — responsive, auto-fill columns
.cars-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  // auto-fill → jitne columns fit hon
  // minmax(300px, 1fr) → min 300px, max equal share
  gap: 20px;
  padding: 24px 0;
}

// Individual Car Card
.car-card {
  background: white;
  border-radius: 16px;
  overflow: hidden;
  // Image ka border-radius ke bahar niklena rokta hai
  box-shadow: 0 2px 12px rgba(0, 0, 0, 0.07);
  transition: transform 0.2s, box-shadow 0.2s;
  // 0.2s smooth animation on hover

  &:hover {
    transform: translateY(-3px);
    // 3px upar uthao → floating effect
    box-shadow: 0 6px 24px rgba(0, 0, 0, 0.12);
    // Deeper shadow → more elevated look
  }
}

// Car visual area — image ya icon
.car-visual {
  height: 160px;
  background: linear-gradient(135deg, #f0f4ff, #e8efff);
  // Light blue gradient background jab image nahi hoti
  display: flex;
  align-items: center;
  justify-content: center;
}

.car-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  // Image crop karo container fit karne ke liye
  // Aspect ratio preserve nahi → fill karo
  // vs object-fit: contain → full image dikhao (letterboxing)
}

// Edit/Delete action buttons
.btn-edit {
  flex: 1;
  // Dono buttons equal width
  background: #eff6ff; // Light blue
  color: #1d4ed8;      // Dark blue text
  border: 1px solid #bfdbfe;

  &:hover { background: #dbeafe; }
}

.btn-delete {
  flex: 1;
  background: #fef2f2; // Light red
  color: #dc2626;      // Dark red text
  border: 1px solid #fecaca;

  &:hover { background: #fee2e2; }
}

// Skeleton Animation — shimmer effect
.skeleton-card {
  height: 260px;
  background: linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%);
  background-size: 200% 100%;
  // Background 2x wide → slide karne pe shimmer
  animation: shimmer 1.5s infinite;
  border-radius: 16px;
}

@keyframes shimmer {
  0%   { background-position: 200% 0; }
  // Gradient right side se start
  100% { background-position: -200% 0; }
  // Gradient left side pe end
  // Effect: light band right se left slide karta hai
}
```

---

## FILE 6 — `src/app/app.routes.ts` (Updated) + `header.html` (Updated)

**Routes mein change:**
```typescript
// Phase 4 NEW — Protected (authGuard laga)
{
  path: 'cars',
  canActivate: [authGuard],
  // authGuard → agar token nahi → /login redirect
  // Sirf logged-in users apni cars dekh sakte hain
  loadComponent: () => import('./pages/cars/cars').then(m => m.CarsComponent)
}
```

**Header mein change:**
```html
@if (authService.isLoggedIn()) {
<!-- Sirf logged-in users ko dikhega yeh section -->

  <a routerLink="/cars" routerLinkActive="active" class="nav-link" (click)="closeMenu()">
    My Cars
  </a>
  <!-- authGuard guard → header link bhi conditionally dikhao -->
  <!-- Logged-out users ko "My Cars" link nahi dikhega -->

  <a routerLink="/profile" ...>👤 {{ authService.currentUser()?.fullName }}</a>
  <button (click)="logout()">Logout</button>
}
```

**Route Guard table (sab routes):**
```
| Route      | Guard       | Access        |
|------------|-------------|---------------|
| /          | (none)      | Sab           |
| /services  | (none)      | Sab           |
| /login     | guestGuard  | Logged-OUT    |
| /register  | guestGuard  | Logged-OUT    |
| /profile   | authGuard   | Logged-IN     |
| /cars      | authGuard   | Logged-IN ← Phase 4 |
```

---

## COMPLETE FLOW 1 — Add Car (Normal Success)

```
User /cars page pe hai (logged in)
        ↓
FILE: app.routes.ts
  { path: 'cars', canActivate: [authGuard] }
  authGuard → token check karta hai localStorage mein
  Token hai → component load karo
        ↓
FILE: cars.ts → ngOnInit()
  this.loadCars() call
        ↓
FILE: cars.ts → loadCars()
  isLoading.set(true)
  carService.getMyCars().subscribe(...)
        ↓
FILE: core/interceptors/auth.interceptor.ts
  Token = localStorage.getItem('carwash_token')
  Authorization: Bearer eyJ... header add kiya
        ↓
Backend: GET /api/cars/my
  [Authorize] → JWT validate
  JWT se userId nikalo (e.g., userId = 3)
  SELECT * FROM Cars WHERE UserId = 3 AND IsActive = true
  Return: [] (initially empty)
        ↓
FILE: cars.ts → next() callback
  cars.set([]) → Signal: empty array
  isLoading.set(false)
        ↓
FILE: cars.html — RENDER
  isLoading() → false → Skeleton hide
  cars().length === 0 → true → EMPTY STATE dikhao
  "Koi car nahi hai abhi" + "Add Your First Car" button
        ↓
User "+ Add Your First Car" button click karta hai
        ↓
FILE: cars.ts → openAddForm()
  resetForm() → formData clear
  isEditing.set(false) → Add mode
  editingCarId.set(null)
  showForm.set(true)
        ↓
FILE: cars.html
  @if (showForm()) → true → Form dikhao
  <h2>Add New Car</h2>  ← isEditing() false → "Add New Car"
  Submit button: "Add Car"
        ↓
User form bharta hai:
  carNumber: "MH12AB1234"
  brand: "Toyota"
  model: "Corolla"
  carType: "Sedan" (dropdown se select)
  imageUrl: "" (optional, chhod diya)
        ↓
User "Add Car" button click karta hai → form submit
        ↓
FILE: cars.ts → onSubmit()
  isSubmitting.set(true) → Button: "Saving..."
  isEditing() → false → CREATE branch

FILE: car.service.ts → createCar(formData)
  POST http://localhost:5001/api/cars
  Body: { carNumber:"MH12AB1234", brand:"Toyota", model:"Corolla", carType:"Sedan", imageUrl:"" }
        ↓
Backend: POST /api/cars
  [Authorize] → JWT validate → userId = 3
  New Car: { UserId:3, CarNumber:"MH12AB1234", Brand:"Toyota", ... }
  Database mein save
  Return: { id:7, userId:3, carNumber:"MH12AB1234", brand:"Toyota", ... }
        ↓
FILE: cars.ts → next() callback
  successMsg.set('Car successfully added!')
  isSubmitting.set(false)
  closeForm() → showForm:false + resetForm()
  loadCars() → fresh GET /api/cars/my
        ↓
Backend: GET /api/cars/my
  Return: [{ id:7, brand:"Toyota", model:"Corolla", ... }]
        ↓
FILE: cars.ts → next()
  cars.set([Toyota Corolla]) → Signal update
  isLoading.set(false)
        ↓
FILE: cars.html — AUTO RE-RENDER
  successMsg() → "Car successfully added!" → dikhao
  cars().length === 1 → @else → Cars grid dikhao
    Card: "Toyota Corolla" | Sedan | MH12AB1234
    🚗 icon (imageUrl empty tha)
    [Edit] [Delete] buttons
```

---

## COMPLETE FLOW 2 — Edit Car

```
User "Edit" button click karta hai (Toyota Corolla card pe)
        ↓
FILE: cars.html
  <button (click)="openEditForm(car)">Edit</button>
  car = { id:7, brand:"Toyota", model:"Corolla", carType:"Sedan", ... }
        ↓
FILE: cars.ts → openEditForm(car)
  formData = { carNumber:"MH12AB1234", brand:"Toyota", model:"Corolla", carType:"Sedan", imageUrl:"" }
  isEditing.set(true) → Edit mode
  editingCarId.set(7) → Car id save karo
  showForm.set(true) → Form dikhao
  setTimeout(100ms) → scrollIntoView() → Smooth scroll to form
        ↓
FILE: cars.html — RENDER
  showForm() → true → Form dikhao
  <h2>Edit Car</h2>  ← isEditing() true → "Edit Car"
  Input fields pre-filled:
    carNumber: "MH12AB1234" ← formData.carNumber
    brand: "Toyota"
    model: "Corolla"
    carType: "Sedan" (dropdown selected)
  Submit button: "Update Car"
        ↓
User model "Corolla" → "Camry" mein change karta hai
[(ngModel)] → formData.model = "Camry" automatically
        ↓
User "Update Car" click
        ↓
FILE: cars.ts → onSubmit()
  isSubmitting.set(true)
  isEditing() → true, editingCarId() → 7 → UPDATE branch

FILE: car.service.ts → updateCar(7, formData)
  PUT http://localhost:5001/api/cars/7
  Body: { carNumber:"MH12AB1234", brand:"Toyota", model:"Camry", carType:"Sedan", imageUrl:"" }
        ↓
Backend: PUT /api/cars/7
  [Authorize] → JWT validate
  Car id=7 dhundo
  Check: car.UserId === currentUserId? (ownership verify)
  Fields update: model = "Camry"
  Return: { id:7, brand:"Toyota", model:"Camry", ... }
        ↓
FILE: cars.ts → next()
  successMsg.set('Car successfully updated!')
  isSubmitting.set(false)
  closeForm()
  loadCars() → GET /api/cars/my
        ↓
Cars list refresh → Updated card dikhega:
  "Toyota Camry" | Sedan | MH12AB1234
```

---

## COMPLETE FLOW 3 — Delete Car

```
User "Delete" button click karta hai
        ↓
FILE: cars.html
  <button (click)="deleteCar(car)">Delete</button>
        ↓
FILE: cars.ts → deleteCar(car)
  confirm(`"Toyota Camry" delete karna chahte ho?`)
        ↓
Browser: Dialog box dikhata hai
  ┌─────────────────────────────────┐
  │ "Toyota Camry" delete karna     │
  │ chahte ho?                      │
  │              [Cancel]  [OK]     │
  └─────────────────────────────────┘
        ↓
Case A: User "Cancel" click karta hai
  confirm() → false
  !false → true → return
  Function exit → kuch nahi hoga → car safe
        ↓
Case B: User "OK" click karta hai
  confirm() → true
  !true → false → return skip → delete aage badhega

FILE: car.service.ts → deleteCar(7)
  DELETE http://localhost:5001/api/cars/7
  (Koi body nahi → sirf URL pe id)
        ↓
Backend: DELETE /api/cars/7
  [Authorize] → JWT validate
  Car id=7 dhundo
  Ownership check
  Soft delete: IsActive = false (ya hard delete)
  Return: deleted Car object
        ↓
FILE: cars.ts → next()
  successMsg.set('Car deleted successfully.')
  loadCars() → GET /api/cars/my
        ↓
Backend: [] return (no more cars)
        ↓
cars.set([]) → Empty state dikhao
"Koi car nahi hai abhi"
```

---

## COMPLETE FLOW 4 — Auth Guard (Logged-Out User)

```
Logged-out user browser mein type karta hai: localhost:4200/cars
        ↓
FILE: app.routes.ts
  { path: 'cars', canActivate: [authGuard] }
  authGuard run hota hai
        ↓
FILE: core/guards/auth.guard.ts
  token = localStorage.getItem('carwash_token')
  token nahi hai → null
  → router.navigate(['/login'])
  → return false (navigation cancel)
        ↓
User /login page pe redirect ho jaata hai
  Cars page kabhi load nahi hota
  CarsComponent kabhi create nahi hota
  Koi API call nahi gayi
```

---

## SARI FILES KA CONNECTION MAP

```
src/app/
│
├── app.routes.ts
│   └── { path: 'cars', canActivate: [authGuard], loadComponent: CarsComponent }
│       authGuard → token check → no token = /login redirect
│
├── models/
│   └── car.models.ts
│       ├── Car              → GET response type, openEditForm() parameter
│       ├── CreateCarRequest → POST body type
│       └── UpdateCarRequest → PUT body type
│
├── core/
│   ├── services/
│   │   └── car.service.ts
│   │       ├── getMyCars()   → GET  /api/cars/my   → Observable<Car[]>
│   │       ├── createCar()   → POST /api/cars       → Observable<Car>
│   │       ├── updateCar()   → PUT  /api/cars/{id}  → Observable<Car>
│   │       └── deleteCar()   → DELETE /api/cars/{id} → Observable<Car>
│   │
│   ├── interceptors/
│   │   └── auth.interceptor.ts
│   │       Token nikalo localStorage se
│   │       Authorization: Bearer {token} header add karo
│   │       Sab 4 CRUD requests pe yahi hota hai
│   │
│   └── guards/
│       └── auth.guard.ts
│           Token nahi → /login redirect
│           Token hai → component load karne do
│
├── shared/components/header/
│   └── header.html
│       @if (authService.isLoggedIn())
│         "My Cars" link → routerLink="/cars"
│       Logged-out users ko link nahi dikhega
│
└── pages/cars/
    ├── cars.ts          ← BRAIN
    │   List Signals: cars, isLoading, errorMsg
    │   Form Signals: showForm, isEditing, editingCarId, isSubmitting, formError, successMsg
    │   formData: plain object (ngModel ke liye)
    │   carTypes: string array (select dropdown ke liye)
    │   ngOnInit → loadCars()
    │   openAddForm() → resetForm + isEditing=false + showForm=true
    │   openEditForm(car) → formData fill + isEditing=true + editingCarId=car.id + scroll
    │   closeForm() → showForm=false + resetForm
    │   onSubmit() → isEditing? PUT : POST → success: closeForm+loadCars
    │   deleteCar(car) → confirm? DELETE → loadCars
    │
    ├── cars.html        ← VIEW
    │   @if (!showForm) → "Add New Car" button
    │   @if (showForm) → Form card
    │     isEditing? "Edit Car" : "Add New Car" title
    │     (ngSubmit)="onSubmit()" #carForm="ngForm"
    │     [(ngModel)] bindings + #field="ngModel" refs
    │     @for carTypes → <option> tags
    │     [disabled]="isSubmitting() || carForm.invalid"
    │   @if (isLoading) → Skeleton cards
    │   @if (!loading && !error)
    │     cars.length === 0 → Empty state
    │     else → @for cars → Car cards
    │       @if imageUrl → <img [src]> else → icon
    │       Edit/Delete buttons → openEditForm(car) / deleteCar(car)
    │
    └── cars.scss        ← STYLE
        .page-header → blue gradient
        .form-grid → 2 columns (@media mobile: 1 col)
        .full-width → grid-column: 1/-1
        .cars-grid → auto-fill responsive
        .car-card → white card + hover lift
        .car-image → object-fit: cover
        .skeleton-card → shimmer @keyframes
```

---

## SIGNAL STATE MACHINE — Cars Page

```
=== LIST SIGNALS ===

Initial:
  isLoading = true    → Skeleton dikhao
  errorMsg  = ''      → No error
  cars      = []      → No data yet

After GET success:
  isLoading = false   → Skeleton hide
  errorMsg  = ''      → No error
  cars      = [...]   → Cars grid dikhao

After GET error:
  isLoading = false   → Skeleton hide
  errorMsg  = 'msg'   → Error message dikhao
  cars      = []      → Grid nahi

=== FORM SIGNALS ===

Hidden state:
  showForm   = false  → Form nahi dikhega
  isEditing  = false
  editingCarId = null

Add mode:
  showForm   = true   → Form dikhega
  isEditing  = false  → onSubmit → POST
  editingCarId = null → URL mein id nahi

Edit mode:
  showForm   = true   → Form dikhega
  isEditing  = true   → onSubmit → PUT
  editingCarId = 7    → PUT /api/cars/7

Submitting:
  isSubmitting = true  → Button disabled + "Saving..."

After success:
  showForm = false    → Form close
  loadCars() call     → Refresh
```

**Signal reactive magic recap:**
```
.set() call → Angular detects change → Template auto re-render
              Sirf woh parts update hote hain jo signal use kar rahe hain
              Puri page reload nahi hoti
```

---

*Phase 4 Cars Workflow — Complete*
*Next: Phase 5 — Bookings (Create booking, My bookings list, Cancel booking)*
