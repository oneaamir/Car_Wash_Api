import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { BookingService } from '../../core/services/booking.service';
import { CarService } from '../../core/services/car.service';
import { ServicesService } from '../../core/services/services.service';
import { Booking } from '../../models/booking.models';
import { Car } from '../../models/car.models';
import { ServicePlan, AddOn } from '../../models/services.models';

@Component({
  selector: 'app-bookings',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './bookings.html',
  styleUrl: './bookings.scss'
})
export class BookingsComponent implements OnInit {
  private bookingService = inject(BookingService);
  private carService = inject(CarService);
  private servicesService = inject(ServicesService);

  bookings = signal<Booking[]>([]);
  isLoadingBookings = signal(true);
  listError = signal('');
  statusFilter = signal('All');
  statusFilters = ['All', 'Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled'];

  myCars = signal<Car[]>([]);
  plans = signal<ServicePlan[]>([]);
  addOns = signal<AddOn[]>([]);

  showForm = signal(false);
  isSubmitting = signal(false);
  formError = signal('');
  successMsg = signal('');

  formData = {
    carId: 0,
    servicePlanId: 0,
    bookingType: '',
    bookingDate: '',
    address: '',
    promoCode: '',
    notes: ''
  };

  selectedAddOnIds = signal<number[]>([]);

  bookingTypes = ['WalkIn', 'HomeService'];

  ngOnInit(): void {
    this.loadBookings();
    this.loadFormData();
  }

  loadBookings(): void {
    this.isLoadingBookings.set(true);
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.bookings.set(data);
        this.isLoadingBookings.set(false);
      },
      error: () => {
        this.listError.set('Unable to load bookings. Please try again.');
        this.isLoadingBookings.set(false);
      }
    });
  }

  loadFormData(): void {
    this.carService.getMyCars().subscribe({
      next: (data) => this.myCars.set(data)
    });
    this.servicesService.getServicePlans().subscribe({
      next: (data) => this.plans.set(data.filter(p => p.isActive))
    });
    this.servicesService.getAddOns().subscribe({
      next: (data) => this.addOns.set(data.filter(a => a.isActive))
    });
  }

  openForm(): void {
    this.resetForm();
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.resetForm();
  }

  toggleAddOn(addonId: number): void {
    const current = this.selectedAddOnIds();
    if (current.includes(addonId)) {
      this.selectedAddOnIds.set(current.filter(id => id !== addonId));
    } else {
      this.selectedAddOnIds.set([...current, addonId]);
    }
  }

  isAddOnSelected(addonId: number): boolean {
    return this.selectedAddOnIds().includes(addonId);
  }

  get totalAmount(): number {
    const plan = this.plans().find(p => p.id === this.formData.servicePlanId);
    const planPrice = plan?.price ?? 0;
    const addonTotal = this.addOns()
      .filter(a => this.selectedAddOnIds().includes(a.id))
      .reduce((sum, a) => sum + a.price, 0);
    return planPrice + addonTotal;
  }

  get minDate(): string {
    return new Date().toISOString().split('T')[0];
  }

  onSubmit(): void {
    if (!this.formData.carId || !this.formData.servicePlanId ||
        !this.formData.bookingType || !this.formData.bookingDate ||
        !this.formData.address.trim()) {
      this.formError.set('Please fill in all required fields: vehicle, service plan, booking type, date, and address.');
      return;
    }

    this.isSubmitting.set(true);
    this.formError.set('');

    this.bookingService.createBooking({
      carId: this.formData.carId,
      servicePlanId: this.formData.servicePlanId,
      addOnIds: this.selectedAddOnIds(),
      bookingType: this.formData.bookingType,
      bookingDate: this.formData.bookingDate,
      address: this.formData.address.trim(),
      promoCode: this.formData.promoCode.trim(),
      notes: this.formData.notes
    }).subscribe({
      next: () => {
        this.successMsg.set('Booking confirmed successfully.');
        this.isSubmitting.set(false);
        this.closeForm();
        this.loadBookings();
      },
      error: (err) => {
        this.formError.set(err.error?.message || 'Unable to create booking. Please try again.');
        this.isSubmitting.set(false);
      }
    });
  }

  cancelBooking(booking: Booking): void {
    if (!confirm(`Are you sure you want to cancel Booking #${booking.id}?`)) return;

    this.bookingService.cancelBooking(booking.id).subscribe({
      next: () => {
        this.successMsg.set('Booking cancelled successfully.');
        this.loadBookings();
      },
      error: () => {
        this.listError.set('Unable to cancel booking. Please try again.');
      }
    });
  }

  get filteredBookings(): Booking[] {
    const f = this.statusFilter();
    return f === 'All' ? this.bookings() : this.bookings().filter(b => b.status === f);
  }

  canCancel(status: string): boolean {
    return status === 'Pending' || status === 'Confirmed';
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'pending':    return 'status-pending';
      case 'confirmed':  return 'status-confirmed';
      case 'inprogress': return 'status-inprogress';
      case 'completed':  return 'status-completed';
      case 'cancelled':  return 'status-cancelled';
      default:           return 'status-pending';
    }
  }

  private resetForm(): void {
    this.formData = { carId: 0, servicePlanId: 0, bookingType: '', bookingDate: '', address: '', promoCode: '', notes: '' };
    this.selectedAddOnIds.set([]);
    this.formError.set('');
    this.successMsg.set('');
    this.isSubmitting.set(false);
  }
}
