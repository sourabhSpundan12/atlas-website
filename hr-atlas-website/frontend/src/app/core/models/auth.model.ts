export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
  errors?: string[];
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface UserProfile {
  id: string;
  email: string;
  fullName: string;
  role: string;
  jobTitle: string;
}

export interface TenantProfile {
  tenantId: string;
  companyName: string;
  slug: string;
  planId: string;
  subscriptionStatus: string;
  workspaceUrl: string;
}

export interface LoginResponse {
  token: string;
  expiresIn: number;
  user: UserProfile;
  tenant: TenantProfile;
}

export interface SendOtpRequest {
  email: string;
  purpose: 'TenantRegistration' | 'PasswordReset';
}

export interface VerifyOtpRequest {
  email: string;
  otp: string;
  purpose: 'TenantRegistration' | 'PasswordReset';
}

export interface ForgotPasswordRequest {
  email: string;
  otp: string;
  newPassword: string;
}

