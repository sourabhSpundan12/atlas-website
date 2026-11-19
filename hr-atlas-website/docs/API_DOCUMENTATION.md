# HR Atlas – SaaS Platform API Documentation

## 1. Overview & Base URLs

- **Development API Base URL**: `http://localhost:5000/api`
- **Swagger / OpenAPI Interactive UI**: `http://localhost:5000/swagger`
- **Health Check**: `GET http://localhost:5000/health`

### Authentication Scheme
All secured endpoints require the `Authorization` HTTP header with a Bearer JWT token:
```http
Authorization: Bearer <token>
```
The token payload contains:
- `sub`: User ID
- `email`: Work Email
- `role`: Role (`TenantAdmin`, `HRManager`, `Employee`)
- `TenantId`: Immutable tenant discriminator
- `OrgSlug`: Subdomain prefix for workspace

---

## 2. Authentication & User APIs

### 2.1 Customer Login
- **Method**: `POST`
- **Path**: `/api/login`
- **Access**: Public
- **Request Body**:
```json
{
  "email": "sarah@acme.com",
  "password": "Password123!"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Login successful. Welcome back to HR Atlas.",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiresIn": 86400,
    "user": {
      "id": "673b5f92a1...",
      "email": "sarah@acme.com",
      "fullName": "Sarah Jenkins",
      "role": "TenantAdmin",
      "jobTitle": "Head of People"
    },
    "tenant": {
      "tenantId": "org_7f9a1b2c",
      "companyName": "Acme Global Technologies",
      "slug": "acme",
      "planId": "growth",
      "subscriptionStatus": "Trial",
      "workspaceUrl": "https://app.hratlas.com/workspace/acme"
    }
  }
}
```

---

### 2.2 Send Verification OTP
- **Method**: `POST`
- **Path**: `/api/user/sendOtp`
- **Access**: Public
- **Request Body**:
```json
{
  "email": "sarah@acme.com",
  "purpose": "TenantRegistration" // or "PasswordReset"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "A 6-digit verification code has been dispatched to sarah@acme.com. Valid for 10 minutes.",
  "data": "548291"
}
```

---

### 2.3 Verify OTP
- **Method**: `POST`
- **Path**: `/api/user/verifyOtp`
- **Access**: Public
- **Request Body**:
```json
{
  "email": "sarah@acme.com",
  "otp": "548291",
  "purpose": "TenantRegistration"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Email verification successful.",
  "data": true
}
```

---

### 2.4 Forgot Password
- **Method**: `PUT`
- **Path**: `/api/user/forgotPassword`
- **Access**: Public
- **Request Body**:
```json
{
  "email": "sarah@acme.com",
  "otp": "548291",
  "newPassword": "NewSecurePassword123!"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Password has been successfully updated. You may now sign in.",
  "data": true
}
```

---

## 3. SaaS Organisation & Subscription APIs

### 3.1 Register SaaS Tenant
- **Method**: `POST`
- **Path**: `/api/saas/register-tenant`
- **Access**: Public
- **Request Body**:
```json
{
  "companyName": "Acme Global Technologies",
  "slug": "acme-corp",
  "industry": "Software & Information Technology",
  "companySize": "16 - 50 Employees",
  "country": "India",
  "currency": "INR",
  "timezone": "Asia/Kolkata",
  "adminFullName": "Sarah Jenkins",
  "adminEmail": "sarah@acme.com",
  "adminPhoneNumber": "+91 9876543210",
  "password": "Password123!",
  "jobTitle": "Head of People",
  "otpCode": "548291",
  "planId": "growth",
  "billingCycle": "Monthly"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Organisation successfully registered and 14-day full access trial activated.",
  "data": {
    "tenantId": "org_7f9a1b2c",
    "companyName": "Acme Global Technologies",
    "slug": "acme-corp",
    "adminEmail": "sarah@acme.com",
    "workspaceUrl": "https://app.hratlas.com/workspace/acme-corp",
    "token": "eyJhbGciOiJIUzI1Ni...",
    "subscription": {
      "planId": "growth",
      "planName": "Growth Plan",
      "status": "Trial",
      "maxSeats": 50,
      "currentSeats": 1,
      "trialEndsAt": "2026-12-02T12:00:00Z",
      "currentPeriodEnd": "2026-12-02T12:00:00Z"
    }
  }
}
```

---

### 3.2 Check Subdomain Availability
- **Method**: `GET`
- **Path**: `/api/saas/check-slug/{slug}`
- **Access**: Public
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Workspace domain is available.",
  "data": true
}
```

---

### 3.3 Contact & Demo Request
- **Method**: `POST`
- **Path**: `/api/saas/contact-inquiry`
- **Access**: Public
- **Request Body**:
```json
{
  "fullName": "David Miller",
  "email": "david@acme.com",
  "phoneNumber": "+1 415 555 2671",
  "companyName": "Acme Corp",
  "companySize": "51-200",
  "modulesOfInterest": ["Payroll", "Attendance", "Payslips"],
  "message": "Interested in automated auto clock-out and PDF payslips.",
  "preferredDate": "2026-11-25T10:00:00Z",
  "type": "DemoRequest" // or "ContactEnquiry"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Your tailored demo request has been received! Our product specialist will confirm your session shortly.",
  "data": true
}
```

---

### 3.4 Retrieve Subscription Plans
- **Method**: `GET`
- **Path**: `/api/saas/subscription/plans`
- **Access**: Public
- **Response `200 OK`**: Returns array of Starter, Growth, and Enterprise plan definitions with monthly/annual rates in INR and USD.

---

### 3.5 Create Checkout Session
- **Method**: `POST`
- **Path**: `/api/saas/subscription/create-checkout`
- **Access**: Authenticated (`TenantAdmin`)
- **Headers**: `Authorization: Bearer <token>`
- **Request Body**:
```json
{
  "planId": "growth",
  "billingCycle": "Annual"
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Checkout session successfully created.",
  "data": {
    "sessionId": "cs_9f284b1a7d",
    "checkoutUrl": "https://checkout.hratlas.com/pay/cs_9f284b1a7d?tenant=acme-corp&plan=growth&cycle=Annual",
    "planId": "growth",
    "amount": 38388,
    "currency": "INR"
  }
}
```

---

### 3.6 Payment Webhook
- **Method**: `POST`
- **Path**: `/api/saas/subscription/webhook`
- **Access**: Public (Cryptographically validated via HMAC-SHA256 signature)
- **Request Body**:
```json
{
  "eventType": "payment.succeeded",
  "tenantId": "org_7f9a1b2c",
  "planId": "growth",
  "billingCycle": "Annual",
  "signature": "a8f3b2c9...",
  "timestamp": 1794998400
}
```
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Webhook processed successfully.",
  "data": true
}
```

---

### 3.7 Retrieve Live Organisation Usage & Seat Limits
- **Method**: `GET`
- **Path**: `/api/saas/organisation/usage`
- **Access**: Authenticated (Resolves TenantId securely from validated JWT token claims)
- **Headers**: `Authorization: Bearer <token>`
- **Response `200 OK`**:
```json
{
  "success": true,
  "message": "Success",
  "data": {
    "tenantId": "org_7f9a1b2c",
    "companyName": "Acme Global Technologies",
    "planId": "growth",
    "planName": "Growth Plan",
    "status": "Trial",
    "currentSeats": 1,
    "maxSeats": 50,
    "seatsRemaining": 49,
    "seatUtilizationPercent": 2.0,
    "isTrialActive": true,
    "trialDaysRemaining": 14,
    "subscriptionPeriodEnd": "2026-12-02T12:00:00Z",
    "activeModules": [
      "Employee Lifecycle Management",
      "Attendance & Auto Clock-Out",
      "Leave & Comp-Off Management",
      "Corporate Broadcasts",
      "Real-Time Notifications",
      "Timesheets & Project Tracking",
      "Salary & Payroll Engine",
      "Automated PDF Payslips",
      "Inventory & Asset Tracking",
      "Automated Employee Onboarding Workflows"
    ]
  }
}
```

