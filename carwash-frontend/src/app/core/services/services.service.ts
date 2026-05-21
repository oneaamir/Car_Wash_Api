import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ServicePlan, AddOn } from '../../models/services.models';

@Injectable({ providedIn: 'root' })
export class ServicesService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getServicePlans(): Observable<ServicePlan[]> {
    return this.http.get<ServicePlan[]>(`${this.apiUrl}/serviceplans`);
  }

  getAddOns(): Observable<AddOn[]> {
    return this.http.get<AddOn[]>(`${this.apiUrl}/addons`);
  }
}
