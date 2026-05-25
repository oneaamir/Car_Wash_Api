import { Component, OnInit, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { PaymentService } from '../../core/services/payment.service';
import { BookingService } from '../../core/services/booking.service';
import { Payment, Receipt } from '../../models/payment.models';
import { Booking } from '../../models/booking.models';

@Component({
  selector: 'app-payments',
  standalone: true,
  imports: [FormsModule, RouterLink],
  templateUrl: './payments.html',
  styleUrl: './payments.scss'
})
export class PaymentsComponent implements OnInit {
  private paymentService = inject(PaymentService);
  private bookingService = inject(BookingService);

  payments  = signal<Payment[]>([]);
  receipts  = signal<Receipt[]>([]);
  myBookings = signal<Booking[]>([]);
  paymentFilter = signal('All');
  paymentFilters = ['All', 'Pending', 'Success', 'Failed'];
  sortOrder = signal('newest');
  sortOptions = [
    { value: 'newest',      label: 'Newest First' },
    { value: 'oldest',      label: 'Oldest First' },
    { value: 'amount-high', label: 'Amount: High to Low' },
    { value: 'amount-low',  label: 'Amount: Low to High' }
  ];

  isLoadingPayments = signal(true);
  isLoadingReceipts = signal(true);

  listError  = signal('');
  successMsg = signal('');
  showForm   = signal(false);
  isSubmitting = signal(false);
  formError  = signal('');

  viewingReceiptId   = signal<number | null>(null);
  generatingReceiptId = signal<number | null>(null);
  receiptGenError    = signal('');

  formData = { bookingId: 0, paymentMethod: '', transactionRef: '' };
  paymentMethods = ['Cash', 'Card', 'UPI', 'Online Banking'];

  ngOnInit(): void {
    this.loadPayments();
    this.loadReceipts();
    this.loadMyBookings();
  }

  loadPayments(): void {
    this.isLoadingPayments.set(true);
    this.paymentService.getMyPayments().subscribe({
      next: (data) => { this.payments.set(data); this.isLoadingPayments.set(false); },
      error: () => { this.listError.set('Unable to load payment history. Please try again.'); this.isLoadingPayments.set(false); }
    });
  }

  loadReceipts(): void {
    this.isLoadingReceipts.set(true);
    this.paymentService.getMyReceipts().subscribe({
      next: (data) => { this.receipts.set(data); this.isLoadingReceipts.set(false); },
      error: () => { this.isLoadingReceipts.set(false); }
    });
  }

  loadMyBookings(): void {
    this.bookingService.getMyBookings().subscribe({
      next: (data) => {
        this.myBookings.set(data.filter(b => b.status !== 'Cancelled'));
      }
    });
  }

  get filteredPayments(): Payment[] {
    const f = this.paymentFilter();
    let list = f === 'All' ? this.payments() : this.payments().filter(p => p.paymentStatus === f);
    switch (this.sortOrder()) {
      case 'oldest':      return [...list].sort((a, b) => new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime());
      case 'amount-high': return [...list].sort((a, b) => b.amount - a.amount);
      case 'amount-low':  return [...list].sort((a, b) => a.amount - b.amount);
      default:            return [...list].sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
    }
  }

  get totalPaid(): number {
    return this.payments()
      .filter(p => p.paymentStatus === 'Success')
      .reduce((sum, p) => sum + p.amount, 0);
  }

  // Only show bookings that don't have a payment yet
  get unpaidBookings(): Booking[] {
    const paidIds = new Set(this.payments().map(p => p.bookingId));
    return this.myBookings().filter(b => !paidIds.has(b.id));
  }

  openForm(): void { this.resetForm(); this.showForm.set(true); }
  closeForm(): void { this.showForm.set(false); this.resetForm(); }

  onSubmit(): void {
    if (!this.formData.bookingId || !this.formData.paymentMethod) {
      this.formError.set('Please select a booking and payment method.');
      return;
    }
    this.isSubmitting.set(true);
    this.formError.set('');
    this.paymentService.createPayment({
      bookingId: this.formData.bookingId,
      paymentMethod: this.formData.paymentMethod,
      transactionRef: this.formData.transactionRef.trim()
    }).subscribe({
      next: () => {
        this.successMsg.set('Payment submitted successfully. Admin will verify and mark it as successful.');
        this.isSubmitting.set(false);
        this.closeForm();
        this.loadPayments();
        this.loadMyBookings();
        setTimeout(() => this.successMsg.set(''), 5000);
      },
      error: (err) => {
        this.formError.set(err.error?.message || err.error || 'Payment could not be processed. Please try again.');
        this.isSubmitting.set(false);
      }
    });
  }

  // Called after admin marks payment as Success — customer generates receipt
  generateReceipt(payment: Payment): void {
    this.generatingReceiptId.set(payment.id);
    this.receiptGenError.set('');
    this.paymentService.generateReceipt(payment.bookingId).subscribe({
      next: (receipt) => {
        this.receipts.update(list => [...list, receipt]);
        this.generatingReceiptId.set(null);
        this.viewingReceiptId.set(payment.id);
      },
      error: (err) => {
        this.receiptGenError.set(err.error?.message || err.error || 'Could not generate receipt.');
        this.generatingReceiptId.set(null);
      }
    });
  }

  toggleReceipt(paymentId: number): void {
    this.viewingReceiptId.set(this.viewingReceiptId() === paymentId ? null : paymentId);
  }

  getReceiptForPayment(paymentId: number): Receipt | undefined {
    return this.receipts().find(r => r.paymentId === paymentId);
  }

  getBookingById(bookingId: number): Booking | undefined {
    return this.myBookings().find(b => b.id === bookingId);
  }

  printReceipt(): void { window.print(); }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric'
    });
  }

  getPaymentStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'success':   return 'status-completed';
      case 'pending':   return 'status-pending';
      case 'failed':    return 'status-failed';
      case 'refunded':  return 'status-refunded';
      default:          return 'status-pending';
    }
  }

  private resetForm(): void {
    this.formData = { bookingId: 0, paymentMethod: '', transactionRef: '' };
    this.formError.set('');
    this.successMsg.set('');
    this.isSubmitting.set(false);
  }
}
