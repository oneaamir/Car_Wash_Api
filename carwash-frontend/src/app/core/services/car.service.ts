import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Car, CreateCarRequest, UpdateCarRequest } from '../../models/car.models';

@Injectable({ providedIn: 'root' })
export class CarService {
  private http = inject(HttpClient);
  private apiUrl = environment.apiUrl;

  getMyCars(): Observable<Car[]> {
    return this.http.get<Car[]>(`${this.apiUrl}/cars/my`);
  }

  createCar(data: CreateCarRequest): Observable<Car> {
    return this.http.post<Car>(`${this.apiUrl}/cars`, data);
  }

  updateCar(id: number, data: UpdateCarRequest): Observable<Car> {
    return this.http.put<Car>(`${this.apiUrl}/cars/${id}`, data);
  }

  deleteCar(id: number): Observable<Car> {
    return this.http.delete<Car>(`${this.apiUrl}/cars/${id}`);
  }
}
