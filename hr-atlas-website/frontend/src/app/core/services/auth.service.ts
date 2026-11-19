import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  ApiResponse,
  ForgotPasswordRequest,
  LoginRequest,
  LoginResponse,
  SendOtpRequest,
  TenantProfile,
  UserProfile,
  VerifyOtpRequest
} from '../models/auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = environment.apiUrl;
  private readonly TOKEN_KEY = 'hr_atlas_auth_token';
  private readonly USER_KEY = 'hr_atlas_auth_user';
  private readonly TENANT_KEY = 'hr_atlas_auth_tenant';

  private currentUserSubject = new BehaviorSubject<UserProfile | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  private currentTenantSubject = new BehaviorSubject<TenantProfile | null>(this.getStoredTenant());
  public currentTenant$ = this.currentTenantSubject.asObservable();

  private isAuthenticatedSubject = new BehaviorSubject<boolean>(!!this.getToken());
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(private http: HttpClient) {}

  login(credentials: LoginRequest): Observable<ApiResponse<LoginResponse>> {
    return this.http.post<ApiResponse<LoginResponse>>(`${this.apiUrl}/login`, credentials).pipe(
      tap(response => {
        if (response.success && response.data) {
          this.setSession(response.data);
        }
      })
    );
  }

  sendOtp(request: SendOtpRequest): Observable<ApiResponse<string>> {
    return this.http.post<ApiResponse<string>>(`${this.apiUrl}/user/sendOtp`, request);
  }

  verifyOtp(request: VerifyOtpRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/user/verifyOtp`, request);
  }

  forgotPassword(request: ForgotPasswordRequest): Observable<ApiResponse<boolean>> {
    return this.http.put<ApiResponse<boolean>>(`${this.apiUrl}/user/forgotPassword`, request);
  }

  setSession(authData: LoginResponse): void {
    localStorage.setItem(this.TOKEN_KEY, authData.token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(authData.user));
    localStorage.setItem(this.TENANT_KEY, JSON.stringify(authData.tenant));

    this.currentUserSubject.next(authData.user);
    this.currentTenantSubject.next(authData.tenant);
    this.isAuthenticatedSubject.next(true);
  }

  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    localStorage.removeItem(this.TENANT_KEY);

    this.currentUserSubject.next(null);
    this.currentTenantSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  private getStoredUser(): UserProfile | null {
    const raw = localStorage.getItem(this.USER_KEY);
    try {
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }

  private getStoredTenant(): TenantProfile | null {
    const raw = localStorage.getItem(this.TENANT_KEY);
    try {
      return raw ? JSON.parse(raw) : null;
    } catch {
      return null;
    }
  }
}

