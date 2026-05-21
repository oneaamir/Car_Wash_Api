import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../core/services/admin.service';
import { AdminBooking, AdminUser } from '../../models/admin.models';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [FormsModule],
  templateUrl: './admin.html',
  styleUrl: './admin.scss'
})
export class AdminComponent implements OnInit {
  private adminService = inject(AdminService);

  activeTab = signal<'bookings' | 'users'>('bookings');

  allBookings = signal<AdminBooking[]>([]);
  allUsers = signal<AdminUser[]>([]);

  isLoadingBookings = signal(true);
  isLoadingUsers = signal(true);
  pageError = signal('');
  successMsg = signal('');

  statusFilter = signal('All');

  // Inline assign-washer panel: which booking is open
  assigningId = signal<number | null>(null);
  assignWasherId = signal(0);
  isAssigning = signal(false);
  assignError = signal('');

  // Inline status-update panel: which booking is open
  updatingId = signal<number | null>(null);
  isUpdating = signal(false);
  updateError = signal('');

  statusFilters = ['All', 'Pending', 'Confirmed', 'InProgress', 'Completed', 'Cancelled'];

  ngOnInit(): void {
    this.loadBookings();
    this.loadUsers();
  }

  loadBookings(): void {
    this.isLoadingBookings.set(true);
    this.adminService.getAllBookings().subscribe({
      next: (data) => {
        this.allBookings.set(data);
        this.isLoadingBookings.set(false);
      },
      error: () => {
        this.pageError.set('Unable to load bookings.');
        this.isLoadingBookings.set(false);
      }
    });
  }

  loadUsers(): void {
    this.isLoadingUsers.set(true);
    this.adminService.getUsers().subscribe({
      next: (data) => {
        this.allUsers.set(data);
        this.isLoadingUsers.set(false);
      },
      error: () => {
        this.isLoadingUsers.set(false);
      }
    });
  }

  get filteredBookings(): AdminBooking[] {
    const filter = this.statusFilter();
    if (filter === 'All') return this.allBookings();
    return this.allBookings().filter(b => b.status === filter);
  }

  get washers(): AdminUser[] {
    return this.allUsers().filter(u => u.role === 'Washer');
  }

  // Valid next statuses based on current status
  getNextStatuses(current: string): string[] {
    switch (current.toLowerCase()) {
      case 'pending':    return ['Confirmed', 'Cancelled'];
      case 'confirmed':  return ['InProgress', 'Cancelled'];
      case 'inprogress': return ['Completed'];
      default:           return [];
    }
  }

  setTab(tab: 'bookings' | 'users'): void {
    this.activeTab.set(tab);
    this.closeAssign();
    this.closeUpdate();
  }

  setFilter(status: string): void {
    this.statusFilter.set(status);
    this.closeAssign();
    this.closeUpdate();
  }

  // ---- Assign Washer ----
  openAssign(bookingId: number, currentWasherId: number | null): void {
    this.closeUpdate();
    this.assigningId.set(bookingId);
    this.assignWasherId.set(currentWasherId ?? 0);
    this.assignError.set('');
  }

  closeAssign(): void {
    this.assigningId.set(null);
    this.assignWasherId.set(0);
    this.assignError.set('');
  }

  submitAssign(bookingId: number): void {
    if (!this.assignWasherId()) {
      this.assignError.set('Please select a washer.');
      return;
    }
    this.isAssigning.set(true);
    this.adminService.assignWasher(bookingId, { washerId: this.assignWasherId() }).subscribe({
      next: (updated) => {
        this.allBookings.update(list =>
          list.map(b => b.id === bookingId ? updated : b)
        );
        this.successMsg.set(`Washer assigned to Booking #${bookingId}.`);
        this.isAssigning.set(false);
        this.closeAssign();
        setTimeout(() => this.successMsg.set(''), 3000);
      },
      error: (err) => {
        this.assignError.set(err.error?.message || err.error || 'Failed to assign washer.');
        this.isAssigning.set(false);
      }
    });
  }

  // ---- Update Status ----
  openUpdate(bookingId: number): void {
    this.closeAssign();
    this.updatingId.set(bookingId);
    this.updateError.set('');
  }

  closeUpdate(): void {
    this.updatingId.set(null);
    this.updateError.set('');
  }

  submitStatus(bookingId: number, newStatus: string): void {
    this.isUpdating.set(true);
    this.adminService.updateBookingStatus(bookingId, { status: newStatus }).subscribe({
      next: (updated) => {
        this.allBookings.update(list =>
          list.map(b => b.id === bookingId ? updated : b)
        );
        this.successMsg.set(`Booking #${bookingId} status updated to ${newStatus}.`);
        this.isUpdating.set(false);
        this.closeUpdate();
        setTimeout(() => this.successMsg.set(''), 3000);
      },
      error: (err) => {
        this.updateError.set(err.error?.message || err.error || 'Failed to update status.');
        this.isUpdating.set(false);
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
      case 'pending':    return 'badge-pending';
      case 'confirmed':  return 'badge-confirmed';
      case 'inprogress': return 'badge-inprogress';
      case 'completed':  return 'badge-completed';
      case 'cancelled':  return 'badge-cancelled';
      default:           return 'badge-pending';
    }
  }

  getRoleClass(role: string): string {
    switch (role?.toLowerCase()) {
      case 'admin':    return 'role-admin';
      case 'washer':   return 'role-washer';
      default:         return 'role-customer';
    }
  }
}
