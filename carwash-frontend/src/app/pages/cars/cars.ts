import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CarService } from '../../core/services/car.service';
import { Car } from '../../models/car.models';

@Component({
  selector: 'app-cars',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './cars.html',
  styleUrl: './cars.scss'
})
export class CarsComponent implements OnInit {
  private carService = inject(CarService);

  // List state
  cars = signal<Car[]>([]);
  isLoading = signal(true);
  errorMsg = signal('');

  // Form state
  showForm = signal(false);
  isEditing = signal(false);
  editingCarId = signal<number | null>(null);
  isSubmitting = signal(false);
  formError = signal('');
  successMsg = signal('');

  // Form data object — [(ngModel)] isse bind hoga
  formData = {
    carNumber: '',
    brand: '',
    model: '',
    carType: '',
    imageUrl: ''
  };

  carTypes = ['Sedan', 'SUV', 'Hatchback', 'Pickup', 'Van', 'Truck', 'Motorcycle', 'Other'];

  ngOnInit(): void {
    this.loadCars();
  }

  loadCars(): void {
    this.isLoading.set(true);
    this.carService.getMyCars().subscribe({
      next: (data) => {
        this.cars.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.errorMsg.set('Unable to load vehicles. Please try again.');
        this.isLoading.set(false);
      }
    });
  }

  openAddForm(): void {
    this.resetForm();
    this.isEditing.set(false);
    this.editingCarId.set(null);
    this.showForm.set(true);
  }

  openEditForm(car: Car): void {
    this.formData = {
      carNumber: car.carNumber,
      brand: car.brand,
      model: car.model,
      carType: car.carType,
      imageUrl: car.imageUrl
    };
    this.isEditing.set(true);
    this.editingCarId.set(car.id);
    this.formError.set('');
    this.successMsg.set('');
    this.showForm.set(true);
    // Form ke paas scroll karo
    setTimeout(() => {
      document.getElementById('car-form')?.scrollIntoView({ behavior: 'smooth' });
    }, 100);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.resetForm();
  }

  onSubmit(): void {
    this.isSubmitting.set(true);
    this.formError.set('');
    this.successMsg.set('');

    const payload = {
      ...this.formData,
      imageUrl: this.formData.imageUrl.trim() || null
    };

    if (this.isEditing() && this.editingCarId() !== null) {
      // UPDATE
      this.carService.updateCar(this.editingCarId()!, payload).subscribe({
        next: () => {
          this.successMsg.set('Vehicle details updated successfully.');
          this.isSubmitting.set(false);
          this.closeForm();
          this.loadCars();
        },
        error: (err) => {
          this.formError.set(err.error?.message || 'Update failed. Please try again.');
          this.isSubmitting.set(false);
        }
      });
    } else {
      // CREATE
      this.carService.createCar(payload).subscribe({
        next: () => {
          this.successMsg.set('Vehicle registered successfully.');
          this.isSubmitting.set(false);
          this.closeForm();
          this.loadCars();
        },
        error: (err) => {
          this.formError.set(err.error?.message || 'Could not add car. Please try again.');
          this.isSubmitting.set(false);
        }
      });
    }
  }

  deleteCar(car: Car): void {
    if (!confirm(`Are you sure you want to remove "${car.brand} ${car.model}" from your account?`)) return;

    this.carService.deleteCar(car.id).subscribe({
      next: () => {
        this.successMsg.set('Vehicle removed successfully.');
        this.loadCars();
      },
      error: () => {
        this.errorMsg.set('Failed to remove vehicle. Please try again.');
      }
    });
  }

  getCarIcon(carType: string): string {
    const type = carType.toLowerCase();
    if (type === 'suv') return '🚙';
    if (type === 'truck' || type === 'pickup') return '🚚';
    if (type === 'motorcycle') return '🏍️';
    if (type === 'van') return '🚐';
    return '🚗';
  }

  private resetForm(): void {
    this.formData = { carNumber: '', brand: '', model: '', carType: '', imageUrl: '' };
    this.formError.set('');
    this.successMsg.set('');
    this.isSubmitting.set(false);
  }
}
