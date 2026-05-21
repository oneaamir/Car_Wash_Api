import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Booking } from '../../models/booking.models';
import { UpdateAssignedBookingStatusRequest } from '../../models/washer.models';

@Injectable({ providedIn: 'root' })
export class WasherService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getAssignedBookings(): Observable<Booking[]> {
    return this.http.get<Booking[]>(`${this.apiUrl}/washers/bookings`);
  }

  updateBookingStatus(bookingId: number, data: UpdateAssignedBookingStatusRequest): Observable<Booking> {
    return this.http.put<Booking>(`${this.apiUrl}/washers/bookings/${bookingId}/status`, data);
  }
}
