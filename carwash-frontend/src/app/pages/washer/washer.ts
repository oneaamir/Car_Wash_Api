import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { WasherService } from '../../core/services/washer.service';
import { Booking } from '../../models/booking.models';

@Component({
  selector: 'app-washer',
  standalone: true,
  imports: [FormsModule],
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
  sortOrder = signal('date-asc');
  sortOptions = [
    { value: 'date-asc',    label: 'Date: Earliest First' },
    { value: 'date-desc',   label: 'Date: Latest First' },
    { value: 'amount-high', label: 'Amount: High to Low' }
  ];

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
    let list = f === 'All' ? this.bookings() : this.bookings().filter(b => b.status === f);
    switch (this.sortOrder()) {
      case 'date-desc':   return [...list].sort((a, b) => new Date(b.bookingDate).getTime() - new Date(a.bookingDate).getTime());
      case 'amount-high': return [...list].sort((a, b) => b.totalAmount - a.totalAmount);
      default:            return [...list].sort((a, b) => new Date(a.bookingDate).getTime() - new Date(b.bookingDate).getTime());
    }
  }

  get todaysSummary(): { total: number; confirmed: number; inProgress: number; completed: number } {
    const today = new Date().toISOString().split('T')[0];
    const todayJobs = this.bookings().filter(b => b.bookingDate.startsWith(today));
    return {
      total:      todayJobs.length,
      confirmed:  todayJobs.filter(b => b.status === 'Confirmed').length,
      inProgress: todayJobs.filter(b => b.status === 'InProgress').length,
      completed:  todayJobs.filter(b => b.status === 'Completed').length
    };
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
