import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  AdminBooking, AdminUser,
  AssignWasherRequest, UpdateBookingStatusRequest, UpdatePaymentStatusRequest,
  CreateServicePlanRequest, UpdateServicePlanRequest,
  CreateAddOnRequest, UpdateAddOnRequest,
  PromoCode, CreatePromoCodeRequest, UpdatePromoCodeRequest,
  ReportFilter, BookingReport, RevenueReport
} from '../../models/admin.models';
import { Payment } from '../../models/payment.models';
import { ServicePlan, AddOn } from '../../models/services.models';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  // ---- Bookings & Users ----
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

  // ---- Service Plans ----
  getAllPlans(): Observable<ServicePlan[]> {
    return this.http.get<ServicePlan[]>(`${this.apiUrl}/serviceplans`);
  }

  createPlan(data: CreateServicePlanRequest): Observable<ServicePlan> {
    return this.http.post<ServicePlan>(`${this.apiUrl}/serviceplans`, data);
  }

  updatePlan(id: number, data: UpdateServicePlanRequest): Observable<ServicePlan> {
    return this.http.put<ServicePlan>(`${this.apiUrl}/serviceplans/${id}`, data);
  }

  deletePlan(id: number): Observable<ServicePlan> {
    return this.http.delete<ServicePlan>(`${this.apiUrl}/serviceplans/${id}`);
  }

  // ---- Add-Ons ----
  getAllAddOns(): Observable<AddOn[]> {
    return this.http.get<AddOn[]>(`${this.apiUrl}/addons`);
  }

  createAddOn(data: CreateAddOnRequest): Observable<AddOn> {
    return this.http.post<AddOn>(`${this.apiUrl}/addons`, data);
  }

  updateAddOn(id: number, data: UpdateAddOnRequest): Observable<AddOn> {
    return this.http.put<AddOn>(`${this.apiUrl}/addons/${id}`, data);
  }

  deleteAddOn(id: number): Observable<AddOn> {
    return this.http.delete<AddOn>(`${this.apiUrl}/addons/${id}`);
  }

  // ---- Promo Codes ----
  getAllPromoCodes(): Observable<PromoCode[]> {
    return this.http.get<PromoCode[]>(`${this.apiUrl}/promocodes`);
  }

  createPromoCode(data: CreatePromoCodeRequest): Observable<PromoCode> {
    return this.http.post<PromoCode>(`${this.apiUrl}/promocodes`, data);
  }

  updatePromoCode(id: number, data: UpdatePromoCodeRequest): Observable<PromoCode> {
    return this.http.put<PromoCode>(`${this.apiUrl}/promocodes/${id}`, data);
  }

  deletePromoCode(id: number): Observable<PromoCode> {
    return this.http.delete<PromoCode>(`${this.apiUrl}/promocodes/${id}`);
  }

  // ---- Reports ----
  getBookingReport(filter: ReportFilter): Observable<BookingReport> {
    let params = new HttpParams();
    if (filter.dateFrom) params = params.set('dateFrom', filter.dateFrom);
    if (filter.dateTo)   params = params.set('dateTo', filter.dateTo);
    return this.http.get<BookingReport>(`${this.apiUrl}/reports/bookings`, { params });
  }

  getRevenueReport(filter: ReportFilter): Observable<RevenueReport> {
    let params = new HttpParams();
    if (filter.dateFrom) params = params.set('dateFrom', filter.dateFrom);
    if (filter.dateTo)   params = params.set('dateTo', filter.dateTo);
    return this.http.get<RevenueReport>(`${this.apiUrl}/reports/revenue`, { params });
  }
}
