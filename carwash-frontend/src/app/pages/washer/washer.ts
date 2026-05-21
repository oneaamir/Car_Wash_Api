import { Component, OnInit, inject, signal } from '@angular/core';
import { WasherService } from '../../core/services/washer.service';
import { Booking } from '../../models/booking.models';

@Component({
  selector: 'app-washer',
  standalone: true,
  imports: [],
  templateUrl: './washer.html',
  styleUrl: './washer.scss'
})
export class WasherComponent implements OnInit {
  private washerService = inject(WasherService);

  bookings = signal<Booking[]>([]);
  isLoading = signal(true);
  pageError = signal('');
  successMsg = signal('');
  updatingId = signal<number | null>(null);
  updateError = signal('');
  statusFilter = signal('All');

  statusFilters = ['All', 'Confirmed', 'InProgress', 'Completed'];

  ngOnInit(): void {
    this.loadBookings();
  }

  loadBookings(): void {
    this.isLoading.set(true);
    this.washerService.getAssignedBookings().subscribe({
      next: (data) => {
        this.bookings.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.pageError.set('Unable to load assigned bookings.');
        this.isLoading.set(false);
      }
    });
  }

  get filteredBookings(): Booking[] {
    const f = this.statusFilter();
    if (f === 'All') return this.bookings();
    return this.bookings().filter(b => b.status === f);
  }

  // Washer can only: Confirmed → InProgress → Completed
  getNextStatus(current: string): string | null {
    switch (current.toLowerCase()) {
      case 'confirmed':  return 'InProgress';
      case 'inprogress': return 'Completed';
      default:           return null;
    }
  }

  getNextLabel(current: string): string {
    switch (current.toLowerCase()) {
      case 'confirmed':  return '▶ Start Job';
      case 'inprogress': return '✔ Mark Complete';
      default:           return '';
    }
  }

  updateStatus(booking: Booking): void {
    const next = this.getNextStatus(booking.status);
    if (!next) return;

    this.updatingId.set(booking.id);
    this.updateError.set('');

    this.washerService.updateBookingStatus(booking.id, { status: next }).subscribe({
      next: (updated) => {
        this.bookings.update(list =>
          list.map(b => b.id === booking.id ? updated : b)
        );
        this.successMsg.set(`Booking #${booking.id} updated to ${next}.`);
        this.updatingId.set(null);
        setTimeout(() => this.successMsg.set(''), 3000);
      },
      error: (err) => {
        this.updateError.set(err.error?.message || err.error || 'Failed to update status.');
        this.updatingId.set(null);
      }
    });
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric'
    });
  }

  getStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'confirmed':  return 'badge-confirmed';
      case 'inprogress': return 'badge-inprogress';
      case 'completed':  return 'badge-completed';
      default:           return 'badge-pending';
    }
  }
}
