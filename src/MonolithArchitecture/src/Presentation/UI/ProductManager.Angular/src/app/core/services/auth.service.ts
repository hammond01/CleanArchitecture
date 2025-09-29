import { Injectable, inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, map, tap, catchError, throwError } from 'rxjs';
import { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse, User, UserProfile } from '../../shared/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly platformId = inject(PLATFORM_ID);
  private readonly TOKEN_KEY = 'auth_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'user_profile';

  private currentUserSubject!: BehaviorSubject<UserProfile | null>;
  public currentUser$!: Observable<UserProfile | null>;

  private isAuthenticatedSubject!: BehaviorSubject<boolean>;
  public isAuthenticated$!: Observable<boolean>;

  constructor() {
    // Initialize subjects with safe values
    this.currentUserSubject = new BehaviorSubject<UserProfile | null>(this.getUserFromStorage());
    this.currentUser$ = this.currentUserSubject.asObservable();

    this.isAuthenticatedSubject = new BehaviorSubject<boolean>(this.hasValidToken());
    this.isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

    // Check token validity on service initialization only in browser
    if (this.isBrowser()) {
      this.checkTokenValidity();
    }
  }

  private isBrowser(): boolean {
    return isPlatformBrowser(this.platformId);
  }

  private safeLocalStorageGetItem(key: string): string | null {
    if (this.isBrowser()) {
      return localStorage.getItem(key);
    }
    return null;
  }

  private safeLocalStorageSetItem(key: string, value: string): void {
    if (this.isBrowser()) {
      localStorage.setItem(key, value);
    }
  }

  private safeLocalStorageRemoveItem(key: string): void {
    if (this.isBrowser()) {
      localStorage.removeItem(key);
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    // For now, simulate API call - replace with actual API endpoint
    return this.simulateLogin(credentials).pipe(
      tap(response => {
        if (response.success && response.token && response.user) {
          this.setSession(response);
        }
      }),
      catchError(error => {
        console.error('Login error:', error);
        return throwError(() => error);
      })
    );
  }

  register(registerData: RegisterRequest): Observable<RegisterResponse> {
    // For now, simulate API call - replace with actual API endpoint
    return this.simulateRegister(registerData).pipe(
      catchError(error => {
        console.error('Register error:', error);
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    this.clearSession();
    // Optional: Call logout API endpoint
    // return this.http.post('/api/auth/logout', {});
  }

  refreshToken(): Observable<LoginResponse> {
    const refreshToken = this.getRefreshToken();
    if (!refreshToken) {
      this.logout();
      return throwError(() => new Error('No refresh token available'));
    }

    // Replace with actual API call
    return this.http.post<LoginResponse>('/api/auth/refresh', { refreshToken }).pipe(
      tap(response => {
        if (response.success && response.token) {
          this.setSession(response);
        }
      }),
      catchError(error => {
        this.logout();
        return throwError(() => error);
      })
    );
  }

  getToken(): string | null {
    return this.safeLocalStorageGetItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return this.safeLocalStorageGetItem(this.REFRESH_TOKEN_KEY);
  }

  getCurrentUser(): UserProfile | null {
    return this.currentUserSubject.value;
  }

  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  private setSession(response: LoginResponse): void {
    if (response.token) {
      this.safeLocalStorageSetItem(this.TOKEN_KEY, response.token);
    }

    if (response.refreshToken) {
      this.safeLocalStorageSetItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    }

    if (response.user) {
      const userProfile: UserProfile = {
        id: response.user.id,
        username: response.user.username,
        email: response.user.email,
        firstName: response.user.firstName,
        lastName: response.user.lastName,
        role: response.user.role
      };
      this.safeLocalStorageSetItem(this.USER_KEY, JSON.stringify(userProfile));
      this.currentUserSubject.next(userProfile);
    }

    this.isAuthenticatedSubject.next(true);
  }

  private clearSession(): void {
    this.safeLocalStorageRemoveItem(this.TOKEN_KEY);
    this.safeLocalStorageRemoveItem(this.REFRESH_TOKEN_KEY);
    this.safeLocalStorageRemoveItem(this.USER_KEY);
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  private getUserFromStorage(): UserProfile | null {
    const userStr = this.safeLocalStorageGetItem(this.USER_KEY);
    if (userStr) {
      try {
        return JSON.parse(userStr);
      } catch {
        return null;
      }
    }
    return null;
  }

  private hasValidToken(): boolean {
    const token = this.getToken();
    if (!token) return false;

    // Add token expiration check here if needed
    return true;
  }

  private checkTokenValidity(): void {
    const isValid = this.hasValidToken();
    this.isAuthenticatedSubject.next(isValid);

    if (!isValid) {
      this.clearSession();
    }
  }

  // Simulate login for development - replace with actual API call
  private simulateLogin(credentials: LoginRequest): Observable<LoginResponse> {
    return new Observable(observer => {
      setTimeout(() => {
        if (credentials.username === 'admin' && credentials.password === 'admin') {
          const response: LoginResponse = {
            success: true,
            token: 'mock-jwt-token-' + Date.now(),
            refreshToken: 'mock-refresh-token-' + Date.now(),
            user: {
              id: '1',
              username: credentials.username,
              email: 'admin@example.com',
              firstName: 'Admin',
              lastName: 'User',
              role: 'admin'
            },
            message: 'Login successful',
            expiresIn: 3600
          };
          observer.next(response);
        } else {
          observer.error({
            success: false,
            message: 'Invalid username or password'
          });
        }
        observer.complete();
      }, 1000); // Simulate network delay
    });
  }

  // Simulate register for development - replace with actual API call
  private simulateRegister(registerData: RegisterRequest): Observable<RegisterResponse> {
    return new Observable(observer => {
      setTimeout(() => {
        // Simulate validation checks
        if (registerData.username === 'admin') {
          observer.error({
            success: false,
            message: 'Username already exists'
          });
          return;
        }

        if (registerData.email === 'admin@example.com') {
          observer.error({
            success: false,
            message: 'Email already exists'
          });
          return;
        }

        // Simulate successful registration
        const response: RegisterResponse = {
          success: true,
          user: {
            id: 'new-user-' + Date.now(),
            username: registerData.username,
            email: registerData.email,
            firstName: registerData.firstName,
            lastName: registerData.lastName
          },
          message: 'Registration successful'
        };
        observer.next(response);
        observer.complete();
      }, 1500); // Simulate network delay
    });
  }
}
