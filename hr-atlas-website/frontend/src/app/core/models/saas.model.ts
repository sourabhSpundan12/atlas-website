export interface TenantRegistrationRequest {
  companyName: string;
  slug: string;
  industry: string;
  companySize: string;
  country: string;
  currency: string;
  timezone: string;
  adminFullName: string;
  adminEmail: string;
  adminPhoneNumber: string;
  password: string;
  jobTitle?: string;
  otpCode: string;
  planId: string;
  billingCycle: string;
}

export interface TenantSubscription {
  planId: string;
  planName: string;
  status: string;
  maxSeats: number;
  currentSeats: number;
  trialEndsAt: string;
  currentPeriodEnd: string;
}

export interface TenantRegistrationResponse {
  tenantId: string;
  companyName: string;
  slug: string;
  adminEmail: string;
  workspaceUrl: string;
  token: string;
  subscription: TenantSubscription;
}

export interface SubscriptionPlan {
  id: string;
  name: string;
  tagline: string;
  monthlyPriceINR: number;
  annualPriceINR: number;
  monthlyPriceUSD: number;
  annualPriceUSD: number;
  maxSeats: number;
  isPopular: boolean;
  features: string[];
  highlightBadges: string[];
}

export interface ContactInquiryRequest {
  fullName: string;
  email: string;
  phoneNumber: string;
  companyName: string;
  companySize: string;
  modulesOfInterest?: string[];
  message?: string;
  preferredDate?: string;
  type: 'DemoRequest' | 'ContactEnquiry';
}

export interface CreateCheckoutRequest {
  planId: string;
  billingCycle: 'Monthly' | 'Annual';
  returnUrl?: string;
  cancelUrl?: string;
}

export interface CheckoutSessionResponse {
  sessionId: string;
  checkoutUrl: string;
  planId: string;
  amount: number;
  currency: string;
}

export interface OrganisationUsage {
  tenantId: string;
  companyName: string;
  planId: string;
  planName: string;
  status: string;
  currentSeats: number;
  maxSeats: number;
  seatsRemaining: number;
  seatUtilizationPercent: number;
  isTrialActive: boolean;
  trialDaysRemaining: number;
  subscriptionPeriodEnd: string;
  activeModules: string[];
}

