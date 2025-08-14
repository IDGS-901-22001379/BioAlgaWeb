import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { isPlatformBrowser } from '@angular/common';
import { environment } from '../../environments/environment';
import { LoginRequest, LoginResponse } from '../models/auth.models';

const STORAGE_KEY = 'bioalga_user';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);
  private platformId = inject(PLATFORM_ID);

  private currentUserSubject = new BehaviorSubject<LoginResponse | null>(this.readUserFromStorage());
  currentUser$ = this.currentUserSubject.asObservable();

  get isLoggedIn(): boolean {
    return !!this.currentUserSubject.value;
  }

  get currentUser(): LoginResponse | null {
    return this.currentUserSubject.value;
  }

  /** Login contra la API */
  login(payload: LoginRequest): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(`${environment.apiUrl}/api/auth/login`, payload)
      .pipe(tap(user => this.setSession(user)));
  }

  /** Cierra sesión */
  logout(): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(STORAGE_KEY);
    }
    this.currentUserSubject.next(null);
  }

  // =======================
  // Internos
  // =======================
  private setSession(user: LoginResponse): void {
    this.currentUserSubject.next(user);
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(user));
    }
  }

  private readUserFromStorage(): LoginResponse | null {
    if (isPlatformBrowser(this.platformId)) {
      const raw = localStorage.getItem(STORAGE_KEY);
      if (raw) {
        try { return JSON.parse(raw) as LoginResponse; }
        catch { localStorage.removeItem(STORAGE_KEY); }
      }
    }
    return null;
  }
}
