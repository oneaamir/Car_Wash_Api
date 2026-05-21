import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AdminBooking, AdminUser,
  AssignWasherRequest, UpdateBookingStatusRequest, UpdatePaymentStatusRequest
} from '../../models/admin.models';
import { Payment } from '../../models/payment.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getUsers(): Observable<AdminUser[]> {
    return this.http.get<AdminUser[]>(`${this.apiUrl}/admin/users`);
  }

  getAllBookings(): Observable<AdminBooking[]> {
    return this.http.get<AdminBooking[]>(`${this.apiUrl}/admin/bookings`);
  }

  assignWasher(bookingId: number, data: AssignWasherRequest): Observable<AdminBooking> {
    return this.http.put<AdminBooking>(`${this.apiUrl}/admin/bookings/${bookingId}/assign-washer`, data);
  }

  updateBookingStatus(bookingId: number, data: UpdateBookingStatusRequest): Observable<AdminBooking> {
    return this.http.put<AdminBooking>(`${this.apiUrl}/admin/bookings/${bookingId}/status`, data);
  }

  updatePaymentStatus(paymentId: number, data: UpdatePaymentStatusRequest): Observable<Payment> {
    return this.http.put<Payment>(`${this.apiUrl}/admin/payments/${paymentId}/status`, data);
  }
}
