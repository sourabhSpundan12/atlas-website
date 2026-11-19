import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/auth.model';
import {
  CheckoutSessionResponse,
  ContactInquiryRequest,
  CreateCheckoutRequest,
  OrganisationUsage,
  SubscriptionPlan,
  TenantRegistrationRequest,
  TenantRegistrationResponse
} from '../models/saas.model';

@Injectable({
  providedIn: 'root'
})
export class SaasService {
  private readonly apiUrl = `${environment.apiUrl}/saas`;

  constructor(private http: HttpClient) {}

  registerTenant(request: TenantRegistrationRequest): Observable<ApiResponse<TenantRegistrationResponse>> {
    return this.http.post<ApiResponse<TenantRegistrationResponse>>(`${this.apiUrl}/register-tenant`, request);
  }

  checkSlugAvailability(slug: string): Observable<ApiResponse<boolean>> {
    return this.http.get<ApiResponse<boolean>>(`${this.apiUrl}/check-slug/${encodeURIComponent(slug)}`);
  }

  getSubscriptionPlans(): Observable<ApiResponse<SubscriptionPlan[]>> {
    return this.http.get<ApiResponse<SubscriptionPlan[]>>(`${this.apiUrl}/subscription/plans`);
  }

  createCheckout(request: CreateCheckoutRequest): Observable<ApiResponse<CheckoutSessionResponse>> {
    return this.http.post<ApiResponse<CheckoutSessionResponse>>(`${this.apiUrl}/subscription/create-checkout`, request);
  }

  submitContactInquiry(request: ContactInquiryRequest): Observable<ApiResponse<boolean>> {
    return this.http.post<ApiResponse<boolean>>(`${this.apiUrl}/contact-inquiry`, request);
  }

  getOrganisationUsage(): Observable<ApiResponse<OrganisationUsage>> {
    return this.http.get<ApiResponse<OrganisationUsage>>(`${this.apiUrl}/organisation/usage`);
  }
}

