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

  payments = signal<Payment[]>([]);
  receipts = signal<Receipt[]>([]);
  myBookings = signal<Booking[]>([]);

  isLoadingPayments = signal(true);
  isLoadingReceipts = signal(true);

  listError = signal('');
  successMsg = signal('');
  showForm = signal(false);
  isSubmitting = signal(false);
  formError = signal('');

  viewingReceiptId = signal<number | null>(null);

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

  openForm(): void {
    this.resetForm();
    this.showForm.set(true);
  }

  closeForm(): void {
    this.showForm.set(false);
    this.resetForm();
  }

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
        this.successMsg.set('Payment processed successfully. Your receipt has been generated.');
        this.isSubmitting.set(false);
        this.closeForm();
        this.loadPayments();
        this.loadReceipts();
      },
      error: (err) => {
        this.formError.set(err.error?.message || 'Payment could not be processed. Please try again.');
        this.isSubmitting.set(false);
      }
    });
  }

  toggleReceipt(paymentId: number): void {
    if (this.viewingReceiptId() === paymentId) {
      this.viewingReceiptId.set(null);
    } else {
      this.viewingReceiptId.set(paymentId);
    }
  }

  getReceiptForPayment(paymentId: number): Receipt | undefined {
    return this.receipts().find(r => r.paymentId === paymentId);
  }

  getBookingById(bookingId: number): Booking | undefined {
    return this.myBookings().find(b => b.id === bookingId);
  }

  printReceipt(): void {
    window.print();
  }

  formatDate(dateStr: string): string {
    if (!dateStr) return '';
    return new Date(dateStr).toLocaleDateString('en-IN', {
      day: '2-digit', month: 'short', year: 'numeric'
    });
  }

  getPaymentStatusClass(status: string): string {
    switch (status?.toLowerCase()) {
      case 'completed': return 'status-completed';
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
