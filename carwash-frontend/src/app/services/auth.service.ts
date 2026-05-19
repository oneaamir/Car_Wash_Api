import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  fullName: string;
  email: string;
  phone: string;
  password: string;
}

export interface AuthResponse {
  userId: number;
  fullName: string;
  email: string;
  role: string;
  token: string;
  message: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private apiUrl = 'http://localhost:5001/api/auth';

  login(req: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, req).pipe(
      tap(res => this.saveSession(res))
    );
  }

  register(req: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, req).pipe(
      tap(res => this.saveSession(res))
    );
  }

  logout(): void {
    localStorage.removeItem('token');
    localStorage.removeItem('role');
    localStorage.removeItem('fullName');
  }

  getToken(): string | null { return localStorage.getItem('token'); }
  getRole(): string | null  { return localStorage.getItem('role'); }
  getFullName(): string     { return localStorage.getItem('fullName') ?? ''; }
  isLoggedIn(): boolean     { return !!this.getToken(); }

  private saveSession(res: AuthResponse): void {
    localStorage.setItem('token', res.token);
    localStorage.setItem('role', res.role);
    localStorage.setItem('fullName', res.fullName);
  }
}
