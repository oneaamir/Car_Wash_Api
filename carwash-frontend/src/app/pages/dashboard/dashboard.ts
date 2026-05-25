import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { BookingService } from '../../core/services/booking.service';
import { PaymentService } from '../../core/services/payment.service';
import { AuthService } from '../../core/services/auth.service';
import { Booking } from '../../models/booking.models';
import { Payment } from '../../models/payment.models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss'
})
export class DashboardComponent implements OnInit {
  private bookingService = inject(BookingService);
  private paymentService = inject(PaymentService);
  private auth = inject(AuthService);

  bookings = signal<Booking[]>([]);
  payments = signal<Payment[]>([]);
  isLoading = signal(true);
  pageError = signal('');

  liveBooking = computed(() =>
    this.bookings().find(b => b.status === 'Confirmed' || b.status === 'InProgress') ?? null
  );

  lifetimeWashes = computed(() => this.bookings().length);

  totalSpent = computed(() =>
    this.payments()
      .filter(p => p.paymentStatus === 'Success')
      .reduce((sum, p) => sum + p.amount, 0)
  );

  recentActivity = computed(() =>
    [...this.bookings()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 3)
  );

  greeting = computed(() => {
    const h = new Date().getHours();
    const firstName = this.auth.currentUser()?.fullName?.split(' ')[0] ?? '';
    const time = h < 12 ? 'morning' : h < 17 ? 'afternoon' : 'evening';
    return `Good ${time}, ${firstName}`;
  });

  ngOnInit(): void {
    forkJoin({
      bookings: this.bookingService.getMyBookings(),
      payments: this.paymentService.getMyPayments()
    }).subscribe({
      next: ({ bookings, payments }) => {
        this.bookings.set(bookings);
        this.payments.set(payments);
        this.isLoading.set(false);
      },
      error: () => {
        this.pageError.set('Failed to load dashboard data. Please refresh.');
        this.isLoading.set(false);
      }
    });
  }

  statusStep(status: string): number {
    const map: Record<string, number> = {
      Pending: 0, Confirmed: 1, InProgress: 2, Completed: 3
    };
    return map[status] ?? -1;
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric'
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

  readonly progressSteps = ['Pending', 'Confirmed', 'InProgress', 'Completed'];
}
