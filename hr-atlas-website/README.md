# HR Atlas – SaaS Website & Multi-Tenant Platform

[![Platform](https://img.shields.io/badge/Platform-ASP.NET_Core_10-512bd4.svg)](https://dotnet.microsoft.com/)
[![Frontend](https://img.shields.io/badge/Frontend-Angular_16%2B-dd0031.svg)](https://angular.dev/)
[![Database](https://img.shields.io/badge/Database-MongoDB-47a248.svg)](https://www.mongodb.com/)
[![License](https://img.shields.io/badge/Security-SOC_2_%7C_ISO_27001-0284c7.svg)]()

HR Atlas is a modern, enterprise-grade multi-tenant B2B Human Resource Management System (HRMS) SaaS platform inspired by greytHR, Zoho People, and BambooHR.

---

## 1. Technology Stack

- **Frontend**:
  - Angular 16+ (Standalone Components, TypeScript, SCSS)
  - Angular Router with deep linking and title restoration
  - Reactive Forms with strong validation
  - RxJS Reactive State Management
  - Responsive CSS Grid / Flexbox with Google Inter typography
- **Backend**:
  - C#
  - ASP.NET Core Web API on .NET 10 (`net10.0`)
  - MongoDB via official `MongoDB.Driver`
  - JWT Bearer Authentication (Tenant-Scoped Claims)
  - BCrypt Password Hashing (`BCrypt.Net-Next`)
  - Swagger / OpenAPI 3.0 Documentation

---

## 2. Project Directory Structure

```text
hr-atlas-website/
├── docker-compose.yml              # Multi-container orchestration (Mongo, Backend, Frontend)
├── .env.example                    # Environment variable template
├── docs/
│   ├── API_DOCUMENTATION.md        # Comprehensive endpoint contracts & JSON schemas
│   └── ARCHITECTURE.md             # Security, multi-tenant isolation & data flows
├── backend/                        # ASP.NET Core .NET 10 Web API
│   ├── Controllers/
│   │   ├── AuthController.cs       # Login, OTP dispatch, OTP verify, Forgot password
│   │   └── SaasController.cs       # Tenant registration, checkout, usage, inquiries
│   ├── Models/
│   │   ├── Tenant.cs               # Multi-tenant organization entity & subscription
│   │   ├── User.cs                 # User account entity with BCrypt password hash
│   │   ├── OtpVerification.cs      # Cryptographic OTP records with TTL expiration
│   │   └── ContactInquiry.cs       # Demo bookings and contact leads
│   ├── DTOs/
│   │   ├── AuthDtos.cs             # Request/response contracts & ApiResponse<T>
│   │   └── SaasDtos.cs             # Registration, plans, checkout, usage DTOs
│   ├── Services/
│   │   ├── ITenantService.cs       # Provisioning, slug availability, usage tracking
│   │   ├── IAuthService.cs         # Credentials verification & password update
│   │   ├── ITokenService.cs        # JWT creation with TenantId & Role claims
│   │   ├── ITenantContext.cs       # Scoped verified tenant identity resolver
│   │   ├── IOtpService.cs          # 6-digit OTP generator, hasher & attempt limiter
│   │   ├── ISubscriptionService.cs # Plans catalog, checkout session, signed webhooks
│   │   ├── IInquiryService.cs      # Lead routing & notifications
│   │   └── PasswordHasher.cs       # BCrypt password hashing implementation
│   ├── Middleware/
│   │   ├── TenantContextMiddleware.cs # Extracts TenantId from validated JWT claims
│   │   └── ExceptionHandlingMiddleware.cs # Global standardized error handler
│   ├── Data/
│   │   ├── MongoDbContext.cs       # Mongo collections & compound index builder
│   │   └── DatabaseSettings.cs     # Configuration options bindings
│   ├── Program.cs                  # WebApplication builder, DI, CORS, Auth pipeline
│   ├── HrAtlas.SaaS.csproj         # TargetFramework net10.0 & dependencies
│   ├── appsettings.json            # Base configuration
│   ├── appsettings.Example.json    # Example configuration
│   └── Dockerfile                  # .NET 10 multi-stage build container
└── frontend/                       # Angular Application
    ├── src/
    │   ├── app/
    │   │   ├── core/               # Models, services, guards, interceptors
    │   │   ├── shared/             # Header, Footer, Toast Notification components
    │   │   ├── pages/              # Responsive SaaS pages
    │   │   │   ├── home/           # Hero, live preview tabs, ROI calculator, comparison
    │   │   │   ├── features/       # All 10 actual HR Atlas modules detailed
    │   │   │   ├── pricing/        # Starter, Growth, Enterprise with billing toggle
    │   │   │   ├── signup/         # 5-step multi-tenant onboarding wizard
    │   │   │   ├── login/          # Customer sign in with workspace routing
    │   │   │   ├── forgot-password/# 3-step OTP-based recovery
    │   │   │   ├── book-demo/      # Module-specific live demo request form
    │   │   │   ├── contact/        # Departmental enquiry routing & global offices
    │   │   │   └── about/          # Mission, architecture pillars & SOC 2 security
    │   │   ├── app.routes.ts       # Angular Router configuration
    │   │   ├── app.config.ts       # Application providers & HTTP interceptor
    │   │   ├── app.ts              # Root standalone component
    │   │   ├── app.html            # Main layout with header, router-outlet, footer
    │   │   └── app.scss            # Root layout styling
    │   ├── environments/           # Environment configuration (dev and prod)
    │   ├── styles.scss             # Global design system (Navy/Blue, Inter, buttons, cards)
    │   └── index.html              # HTML shell with Google Inter font & Font Awesome
    ├── angular.json                # Angular workspace configuration
    ├── package.json                # Frontend dependencies
    ├── nginx.conf                  # Production SPA routing configuration
    └── Dockerfile                  # Multi-stage Node + Nginx container
```

---

## 3. The 10 Actual HR Atlas Features

The website accurately showcases the 10 real capabilities of HR Atlas:
1. **Employee Lifecycle Management** – Centralized employee profiles, document vaults, probation milestones, promotion histories, and offboarding checklists.
2. **Attendance and Auto Clock-Out** – Single-click punches with configurable auto clock-out rules (e.g. after 10 hours of inactivity) to prevent inflated overtime.
3. **Leave and Comp-Off Management** – Custom leave policies, automatic compensatory-off credits for weekend shifts, multi-tier approvals, and absence calendars.
4. **Timesheets and Project Tracking** – Billable vs non-billable hours logged per client project, submission locks, and manager sign-off workflows.
5. **Salary and Payroll Engine** – Automated dual-tax regime calculation (Old vs New), statutory deductions (PF, ESI, PT, TDS), and bank disbursement files.
6. **Automated PDF Payslips** – Password-protected, branded PDF payslips generated in bulk and dispatched directly to employee inboxes.
7. **Inventory and Asset Management** – Complete tracking of physical hardware (laptops, monitors), serials, warranties, digital handover acknowledgments, and recovery.
8. **Employee Onboarding Emails** – Automated welcome packages, first-time portal credentials, manager prep checklists, and 30-day feedback surveys.
9. **Corporate Broadcasts** – Organization-wide and department announcements, emergency alerts, and mandatory policy read-receipt tracking.
10. **Real-Time Notifications** – Instant in-app alerts and notifications for approval requests, auto clock-out executions, and payslip releases.

---

## 4. Multi-Tenant Onboarding & Security

Customer Onboarding Journey:
`Company Profile` → `Admin Setup` → `Email OTP Verification` → `Tenant Creation` → `Trial/Subscription Selection` → `Workspace Provisioning` → `HR Atlas Dashboard`

### Security Rule: Zero-Trust Client TenantId
The client is **never** trusted to provide a `TenantId` in headers or body. The backend enforces:
1. Cryptographic issuance of `TenantId` inside the JWT ClaimsPrincipal during authenticated login or validated registration.
2. `TenantContextMiddleware` extracts and validates the claim from the verified token.
3. Scoped `ITenantContext` injects the verified identity into database queries.
4. Unique and compound database indexes in MongoDB prevent cross-tenant data collisions.

---

## 5. Execution & Setup Instructions

### Option A: Run Everything with Docker Compose (Recommended)

From `hr-atlas-website/`:
```bash
docker compose up --build
```
This automatically boots:
- **MongoDB**: `localhost:27017`
- **ASP.NET Core .NET 10 API**: `http://localhost:5000` (Swagger at `http://localhost:5000/swagger`)
- **Angular Frontend**: `http://localhost:4200`

### Option B: Run Services Independently

#### 1. Start MongoDB
Ensure MongoDB is running locally on port 27017:
```bash
mongod --dbpath /data/db
```

#### 2. Start ASP.NET Core Backend
From `hr-atlas-website/backend/`:
```bash
dotnet restore
dotnet run --configuration Development
```
The API will start listening on `http://localhost:5000`. You can inspect endpoints at `http://localhost:5000/swagger`.

#### 3. Start Angular Frontend
From `hr-atlas-website/frontend/`:
```bash
npm install
npm start
```
Open `http://localhost:4200` in your web browser.

---

## 6. API Route Reference

| Method | Endpoint | Description | Access |
|---|---|---|---|
| `POST` | `/api/login` | Authenticate user & issue tenant-scoped JWT | Public |
| `POST` | `/api/user/sendOtp` | Dispatch 6-digit OTP with 10-minute expiry | Public |
| `POST` | `/api/user/verifyOtp` | Verify OTP code against registered hash | Public |
| `PUT` | `/api/user/forgotPassword` | Reset account password with verified OTP | Public |
| `POST` | `/api/saas/register-tenant` | Register organisation, tenant, admin & 14-day trial | Public |
| `GET` | `/api/saas/check-slug/{slug}` | Real-time subdomain availability check | Public |
| `POST` | `/api/saas/contact-inquiry` | Capture demo requests and contact enquiries | Public |
| `GET` | `/api/saas/subscription/plans` | Public catalog of Starter, Growth & Enterprise plans | Public |
| `POST` | `/api/saas/subscription/create-checkout` | Create checkout session for subscription | Authenticated |
| `POST` | `/api/saas/subscription/webhook` | Process payment events with HMAC signature check | Webhook |
| `GET` | `/api/saas/organisation/usage` | Return live seat usage & active modules | Authenticated |
| `GET` | `/health` | API health and runtime status check | Public |

