import { Injectable, signal, PLATFORM_ID, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { isPlatformBrowser } from '@angular/common';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { AuthResponse, LoginRequest, ProfileResponse, RegisterRequest } from '../../models/auth.models';

@Injectable({ providedIn: 'root' })
export class AuthService {

  private apiUrl = environment.apiUrl;

  // PLATFORM_ID = Angular ko pata hota hai ki app browser mein hai ya server pe
  // isPlatformBrowser() = true agar browser hai, false agar server (SSR) hai
  // SSR ke waqt sessionStorage available nahi hota - isliye check karna zaroori hai
  private platformId = inject(PLATFORM_ID);
  private http = inject(HttpClient);

  currentUser = signal<AuthResponse | null>(this.loadUserFromStorage());

  private loadUserFromStorage(): AuthResponse | null {
    // SSR check: server pe sessionStorage nahi hota
    if (!isPlatformBrowser(this.platformId)) return null;
    const saved = sessionStorage.getItem('carwash_user');
    return saved ? JSON.parse(saved) : null;
  }

  login(data: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/login`, data).pipe(
      tap(response => {
        if (isPlatformBrowser(this.platformId)) {
          sessionStorage.setItem('carwash_token', response.token);
          sessionStorage.setItem('carwash_user', JSON.stringify(response));
        }
        this.currentUser.set(response);
      })
    );
  }

  register(data: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/auth/register`, data);
  }

  getProfile(): Observable<ProfileResponse> {
    return this.http.get<ProfileResponse>(`${this.apiUrl}/auth/profile`);
  }

  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      sessionStorage.removeItem('carwash_token');
      sessionStorage.removeItem('carwash_user');
    }
    this.currentUser.set(null);
  }

  getToken(): string | null {
    if (!isPlatformBrowser(this.platformId)) return null;
    return sessionStorage.getItem('carwash_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }

  getUserRole(): string | null {
    return this.currentUser()?.role ?? null;
  }
}
