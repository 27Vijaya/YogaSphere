# YogaSphere – Yoga Studio Management System

> **Project Status: In Development**
>
> YogaSphere is an actively developed full-stack
> yoga studio management application and portfolio project.
> Features and documentation are being improved continuously.

A full-stack yoga studio management application built using **ASP.NET Core Web API, C#, Entity Framework Core, SQL Server, and React**.

YogaSphere is designed to manage yoga programs, classes, schedules, customer bookings, payments, and trainer activities through a role-based application.

## Project Overview

YogaSphere provides separate functionality for customers, trainers, and administrators.

The application follows a layered backend architecture to maintain separation of concerns, improve maintainability, and support future enhancements.

## Technology Stack

| Area | Technologies |
|---|---|
| Backend | C#, ASP.NET Core Web API, .NET 10 |
| Architecture | Layered Architecture |
| ORM | Entity Framework Core |
| Database | Microsoft SQL Server |
| Frontend | React, JavaScript, CSS |
| Authentication | JWT Authentication |
| Authorization | Role-Based Authorization |
| API Documentation | Swagger / OpenAPI |
| Payments | Razorpay Integration |
| Version Control | Git and GitHub |
| Testing | xUnit |
| Package Management | NuGet, npm |

## Architecture

The backend follows a layered architecture.

```text
                    React Frontend
                          |
                    HTTP / JSON
                          |
                   ASP.NET Core API
                          |
                  Application Layer
                          |
                    Domain Layer
                          |
                 Infrastructure Layer
                          |
                  Entity Framework Core
                          |
                    SQL Server
```

### Backend Projects

| Project | Responsibility |
|---|---|
| YogaSphere.API | API controllers, authentication configuration, middleware and Swagger |
| YogaSphere.Application | Interfaces, DTOs and application contracts |
| YogaSphere.Domain | Domain entities and business enums |
| YogaSphere.Infrastructure | EF Core, repositories, services and database operations |
| YogaSphere.Tests | Automated tests |

## Features

### Authentication and Authorization
- User registration and login.
- JWT-based authentication.
- Role-based authorization.
- Customer, Trainer and Admin roles.
- Protected frontend routes.

### Customer Features
- Browse available yoga programs and classes.
- View class details.
- Manage bookings.
- View booking information.
- Access customer dashboard and profile.

### Trainer Features
- Trainer dashboard.
- View trainer-owned yoga programs.
- Manage classes.
- View schedules.
- View trainer-related information.

### Program and Class Management
- Yoga program management.
- Yoga class management.
- Trainer-specific program and class access.
- Active and inactive program/class handling.

### Booking and Payment
- Yoga class booking management.
- Booking status management.
- Booking cancellation.
- Razorpay payment integration.

## Repository Structure

```text
YogaSphere/
│
├── backend/
│   ├── YogaSphere.API/
│   └── YogaSphere/
│       ├── YogaSphere.slnx
│       ├── YogaSphere.Application/
│       ├── YogaSphere.Domain/
│       ├── YogaSphere.Infrastructure/
│       └── YogaSphere.Tests/
│
├── frontend/
│   ├── public/
│   ├── src/
│   │   ├── components/
│   │   ├── context/
│   │   ├── dashboard/
│   │   ├── pages/
│   │   └── services/
│   ├── package.json
│   └── vite.config.js
│
├── docs/
│
├── README.md
├── CHANGELOG.md
└── .gitignore
```

## Getting Started

### Prerequisites

Install the following:

- .NET 10 SDK
- Node.js and npm
- Microsoft SQL Server
- SQL Server Management Studio (SSMS), recommended
- Visual Studio or Visual Studio Code
- Git

### 1. Clone the Repository

```bash
git clone https://github.com/27vijaya/YogaSphere.git
```

```bash
cd YogaSphere
```

### 2. Configure the Backend

Restore NuGet packages:

```bash
dotnet restore backend/YogaSphere/YogaSphere.slnx
```

Configure the database connection string and application secrets using ASP.NET Core User Secrets or environment variables.

Do not commit database credentials, JWT signing keys, or Razorpay secret keys.

For local development, initialize User Secrets for the API project:

```bash
dotnet user-secrets init --project backend/YogaSphere.API
```

Configure the required settings:

- SQL Server connection string.
- JWT issuer, audience and signing key.
- Razorpay API credentials, if using payment functionality.

Use the configuration keys expected by the application.

### 3. Apply Database Migrations

After configuring the SQL Server connection:

```bash
dotnet ef database update --project backend/YogaSphere/YogaSphere.Infrastructure --startup-project backend/YogaSphere.API
```

### 4. Run the Backend

```bash
dotnet run --project backend/YogaSphere.API
```

Open the Swagger URL shown in the application startup output to explore the API endpoints.

### 5. Run the Frontend

Open another terminal:

```bash
cd frontend
```

Install dependencies:

```bash
npm install
```

Start the React development server:

```bash
npm run dev
```

Open the local URL displayed by Vite in your browser.

Make sure the frontend API base URL is configured to point to your running backend.

## Build and Validation

### Backend

```bash
dotnet build backend/YogaSphere/YogaSphere.slnx
```

Run automated tests:

```bash
dotnet test backend/YogaSphere/YogaSphere.slnx
```

### Frontend

```bash
cd frontend
npm run lint
npm run build
```

## Security Practices

- JWT-based authentication.
- Role-based endpoint authorization.
- Sensitive configuration stored outside source control.
- ASP.NET Core User Secrets for local development.
- Development-specific configuration excluded from Git.
- `.gitignore` configured to exclude local settings and generated files.

## Future Enhancements

- Expand automated unit and integration test coverage.
- Improve administrative management features.
- Enhance payment and booking workflows.
- Add notifications and reporting.
- Improve deployment automation using CI/CD.
- Add Docker-based development and deployment support.

## Project Status

**Active Development**

YogaSphere is a portfolio and learning project demonstrating full-stack development using the Microsoft .NET ecosystem and React.

## Developer

**Vijaya**

GitHub: [github.com/27vijaya](https://github.com/27vijaya)

---

*Built to demonstrate practical skills in ASP.NET Core, C#, SQL Server, EF Core, React, authentication, authorization, API development, and full-stack application architecture.*