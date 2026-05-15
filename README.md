# Film Rental Store

## Overview

Film Rental Store is a sprint project built with ASP.NET Core 8. It provides a role-based film rental management system with a REST API backend, an MVC frontend, SQL Server persistence, JWT authentication, and automated service-level tests.

The project is based on a Sakila-style film rental domain. It supports films, actors, categories, languages, inventory, stores, customers, staff, rentals, payments, and role-specific access for Admin, Manager, Staff, and Customer users.

## Table of Contents

- [Project Objectives](#project-objectives)
- [Technology Stack](#technology-stack)
- [Solution Structure](#solution-structure)
- [Application Architecture](#application-architecture)
- [Core Features](#core-features)
- [Role-Based Access](#role-based-access)
- [Database Overview](#database-overview)
- [Configuration](#configuration)
- [Setup and Execution](#setup-and-execution)
- [Testing](#testing)
- [API Overview](#api-overview)
- [MVC Frontend Overview](#mvc-frontend-overview)
- [Security Notes](#security-notes)
- [Development Notes](#development-notes)
- [Troubleshooting](#troubleshooting)
- [Sprint Summary](#sprint-summary)

## Project Objectives

The objective of this sprint project is to implement a maintainable film rental store system that demonstrates:

- ASP.NET Core Web API development.
- ASP.NET Core MVC frontend integration.
- SQL Server database integration using Entity Framework Core.
- Clean DTO-based request and response handling.
- Authentication and authorization using JWT and role-based access rules.
- Repository and service layer separation.
- Server-side validation.
- Store-scoped business rules for Manager, Staff, and Customer users.
- Unit testing for core service behavior.

## Technology Stack

| Area | Technology |
| --- | --- |
| Runtime | .NET 8 |
| Backend | ASP.NET Core Web API |
| Frontend | ASP.NET Core MVC, Razor Views |
| Database | SQL Server |
| ORM | Entity Framework Core 8 |
| Authentication | JWT Bearer Authentication |
| Authorization | Role-based authorization |
| Object Mapping | AutoMapper |
| Validation | FluentValidation |
| Password Hashing | BCrypt.Net-Next |
| API Documentation | Swagger / Swashbuckle |
| Testing | xUnit, Moq, coverlet.collector |
| UI Framework | Bootstrap |

## Solution Structure

```text
Film-Rental-System/
  FilmRentalStore.API/
    Controllers/
    Data/
    DTOs/
    Exceptions/
    Filters/
    Mappings/
    Middleware/
    Models/
    Repositories/
    Services/
    Validators/
    Program.cs
    appsettings.json

  FilmRentalStore.MVC/
    Controllers/
    DTOs/
    Filters/
    Helpers/
    Models/
    Services/
    ViewModels/
    Views/
    wwwroot/
    Program.cs
    appsettings.json

  FilmRentalStore.Tests/
    Services/
    FilmRentalStore.Tests.csproj

  FilmRentalStore.slnx
```

## Application Architecture

The solution is separated into three projects.

### FilmRentalStore.API

The API project contains the backend application logic. It exposes REST endpoints for authentication, films, actors, categories, languages, inventory, customers, staff, stores, rentals, payments, addresses, and dashboard statistics.

Primary responsibilities:

- Database access through Entity Framework Core.
- Repository and service layer implementation.
- DTO mapping with AutoMapper.
- Validation through FluentValidation.
- JWT token generation and validation.
- Role-based endpoint protection.
- Centralized exception handling middleware.
- Swagger documentation in development.

### FilmRentalStore.MVC

The MVC project is the frontend web application. It consumes the API through typed service classes and renders role-specific Razor views.

Primary responsibilities:

- Login and registration screens.
- Role-specific dashboards.
- Film catalogue browsing and management.
- Customer, staff, store, inventory, rental, and payment screens.
- Store-scoped frontend filtering.
- Session-based token storage.
- Role-aware navigation and action visibility.
- Fixed header/sidebar layout and paginated list views.

### FilmRentalStore.Tests

The test project contains automated service tests for the API layer.

Covered service areas include:

- Actor service
- Auth service
- Category service
- Customer service
- Film service
- Inventory service
- Language service
- Payment service
- Rental service
- Staff service
- Store service
- Token service

## Core Features

### Authentication and User Management

- Staff login.
- Customer login.
- Customer self-registration.
- Admin-only staff creation.
- JWT token generation with role, user, and store claims.
- Session-based authentication handling in MVC.

### Film Management

- Film listing, details, creation, and editing.
- Search support in the film catalogue.
- Film category mapping through the film-category relationship.
- Multiple actor assignment per film through the film-actor relationship.
- Multiple category assignment per film.
- Film details include actors and categories.

### Actor, Category, and Language Management

- Actor listing, details, creation, and editing.
- Category listing, details, creation, and editing.
- Language listing, details, creation, and editing.
- Details pages show related films where applicable.
- Related film lists respect store scope for non-admin users.

### Inventory Management

- Inventory listing and details.
- Inventory creation and editing for authorized roles.
- Inventory grouping by film and store for cleaner display.
- Available copy count and total copy count display.
- Store ID hidden from Customer, Staff, and Manager frontend views.

### Rental Management

- Rental listing, details, creation, and return workflow.
- Staff, Manager, and Admin users can rent out and return films.
- Customer users can view their own rentals.
- Rental sorting by rental date.
- Store-scoped access for Manager and Staff users.

### Payment Management

- Payment listing, details, and creation.
- Customer users can view their own payments.
- Staff, Manager, and Admin users can record payments.
- Store-scoped payment access for Manager and Staff users.

### Store and Staff Management

- Store listing and details.
- Admin-only store creation and editing.
- Admin-only staff creation, editing, and deactivation.
- Manager users can view Staff users assigned to their own store.
- Store identifiers are not exposed in Customer, Staff, and Manager frontend views.

### Dashboards

Each role has a simple dashboard with summary cards and quick actions.

- Admin dashboard: overall catalogue, store, inventory, and rental summary.
- Manager dashboard: store-specific film, copy, rental, and staff summary.
- Staff dashboard: store-specific operational summary.
- Customer dashboard: available films, current rentals, and payment summary.

Dashboard loading is optimized through a lightweight API statistics endpoint instead of loading large datasets into the MVC application.

## Role-Based Access

| Feature Area | Admin | Manager | Staff | Customer |
| --- | --- | --- | --- | --- |
| Staff login | Yes | Yes | Yes | No |
| Customer login | No | No | No | Yes |
| Customer registration | No | No | No | Yes |
| Staff registration/creation | Yes | No | No | No |
| Film catalogue | Yes | Store-scoped | Store-scoped | Store-scoped |
| Film create/edit | Yes | Yes | No | No |
| Inventory view | Yes | Store-scoped | Store-scoped | Store-scoped |
| Inventory create/edit | Yes | Yes | No | No |
| Rental view | Yes | Store-scoped | Store-scoped | Own rentals |
| Rental create/return | Yes | Yes | Yes | No |
| Payment view | Yes | Store-scoped | Store-scoped | Own payments |
| Payment create | Yes | Yes | Yes | No |
| Customer management | Yes | Store-scoped | Store-scoped | Own profile |
| Staff management | Yes | Store-scoped view only | No | No |
| Store management | Yes | Own store view | No | No |

## Database Overview

The application uses a Sakila-style relational model. Important entities include:

- `actor`
- `address`
- `category`
- `city`
- `country`
- `customer`
- `film`
- `film_actor`
- `film_category`
- `inventory`
- `language`
- `payment`
- `rental`
- `roles`
- `staff`
- `store`

Key relationships:

- A film can have multiple actors through `film_actor`.
- A film can have multiple categories through `film_category`.
- A store has inventory records.
- Inventory records connect films to stores.
- Rentals are created against inventory records.
- Payments are associated with customers, staff, and rentals.
- Staff and customers are assigned to stores.
- Role information controls authorization behavior.

## Configuration

### API Configuration

The API configuration is located in:

```text
FilmRentalStore.API/appsettings.json
```

Important settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=SERVER_NAME;Initial Catalog=Sakila;Integrated Security=True;TrustServerCertificate=True;Encrypt=False;"
  },
  "Jwt": {
    "Key": "REPLACE_WITH_A_SECURE_KEY",
    "Issuer": "FilmRentalStore.API",
    "Audience": "FilmRentalStore.Client",
    "ExpiresInMinutes": 60
  }
}
```

Update the connection string to match the local SQL Server instance and database name.

### MVC Configuration

The MVC configuration is located in:

```text
FilmRentalStore.MVC/appsettings.json
```

Important setting:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5058/"
  }
}
```

The `BaseUrl` must match the running API URL.

## Setup and Execution

### Prerequisites

Install or prepare the following:

- .NET 8 SDK
- SQL Server
- Sakila database schema and data
- Visual Studio 2022 or a compatible editor
- Git

### Restore Dependencies

From the solution root:

```bash
dotnet restore
```

### Build the Solution

```bash
dotnet build
```

### Run the API

```bash
dotnet run --project FilmRentalStore.API
```

Default launch URLs:

```text
HTTP:  http://localhost:5058
HTTPS: https://localhost:7269
Swagger: http://localhost:5058/swagger
```

### Run the MVC Application

Open a second terminal and run:

```bash
dotnet run --project FilmRentalStore.MVC
```

Default launch URLs:

```text
HTTP:  http://localhost:5127
HTTPS: https://localhost:7097
```

### Running Order

Run the API first, then run the MVC application. The MVC frontend depends on the API base URL configured in `FilmRentalStore.MVC/appsettings.json`.

## Testing

Run all tests from the solution root:

```bash
dotnet test
```

If the solution has already been built:

```bash
dotnet test --no-build
```

Current test coverage includes service-level tests for authentication, token generation, films, actors, categories, languages, inventory, customers, staff, stores, rentals, and payments.

## API Overview

The API uses REST-style endpoints under the `/api` route prefix.

| Area | Route Prefix | Purpose |
| --- | --- | --- |
| Authentication | `/api/Auth` | Staff login, customer login, customer registration, admin staff registration |
| Dashboard | `/api/Dashboard` | Role-specific aggregate dashboard statistics |
| Films | `/api/Films` | Film catalogue and film relationship management |
| Actors | `/api/Actors` | Actor management |
| Categories | `/api/Categories` | Category management |
| Languages | `/api/Languages` | Language management |
| Inventory | `/api/Inventory` | Film inventory management |
| Customers | `/api/Customers` | Customer management and customer profile data |
| Rentals | `/api/Rentals` | Rental creation, listing, detail, and return workflow |
| Payments | `/api/Payments` | Payment creation, listing, and detail workflow |
| Staff | `/api/Staff` | Staff management and store-scoped staff access |
| Stores | `/api/Stores` | Store management |
| Addresses | `/api/Addresses` | Address management |

Swagger is available in development mode at:

```text
http://localhost:5058/swagger
```

## MVC Frontend Overview

The MVC application provides role-aware UI flows:

- Login screens for staff and customers.
- Customer registration.
- Fixed header and sidebar layout.
- Role-specific sidebar navigation.
- Dashboard cards for each role.
- Paginated list pages.
- Film search.
- Rental date sorting.
- Store-scoped data visibility for Manager, Staff, and Customer users.
- Action buttons hidden when a role is not authorized to perform that operation.

## Security Notes

- Passwords are hashed using BCrypt.
- API endpoints are protected using JWT Bearer authentication.
- MVC stores the JWT token in session.
- Role-based authorization is enforced in both the API and MVC layers.
- Manager, Staff, and Customer users are scoped by `store_id`.
- Staff registration is restricted to Admin users.
- Customer registration is publicly available through the MVC frontend and API.
- Store IDs are not displayed to Customer, Staff, or Manager users in the frontend.

For production use:

- Move secrets out of `appsettings.json`.
- Use environment variables, user secrets, or a secure secret store.
- Replace the development JWT key.
- Use HTTPS-only deployment.
- Review database permissions for least privilege.

## Development Notes

### Important Design Decisions

- DTOs are used to avoid exposing EF entities directly.
- AutoMapper is used for mapping between entities and DTOs.
- FluentValidation is used for request validation.
- Repository classes isolate EF Core queries.
- Service classes contain business rules and orchestration.
- MVC services are responsible for API communication.
- Role-based UI visibility is handled in Razor views.
- Store-scoped authorization is enforced through API claims and MVC filtering.

### Dashboard Optimization

Dashboard pages use a dedicated statistics endpoint:

```text
GET /api/Dashboard
```

This avoids loading large datasets into the MVC application only to calculate counts. The endpoint returns aggregate values directly from the database according to the current user's role and store scope.

### Pagination and Layout

The MVC frontend includes:

- Server-side pagination where backend paging is already supported.
- Lightweight client-side pagination for small lookup-style lists.
- Fixed top header.
- Fixed sidebar on desktop.
- Scrollable content area.

## Troubleshooting

### MVC cannot reach the API

Confirm that the API is running and that the MVC base URL is correct:

```text
FilmRentalStore.MVC/appsettings.json
```

The default value should match the API launch URL:

```json
"BaseUrl": "http://localhost:5058/"
```

### Database connection fails

Verify:

- SQL Server is running.
- The Sakila database exists.
- The connection string points to the correct SQL Server instance.
- The configured user has database access.
- `TrustServerCertificate=True` is present for local development if needed.

### Unauthorized or missing data after login

If role or store-scoped data is not shown correctly, log out and log in again. The application stores token and store information in session at login time.

### Swagger is not available

Swagger is enabled only in development mode. Ensure the API is running with:

```text
ASPNETCORE_ENVIRONMENT=Development
```

### NuGet configuration permission issues

If `dotnet build` fails because it cannot access the user-level NuGet configuration, run the command from an environment that has access to the user profile, or configure a project-local NuGet source.

## Sprint Summary

This sprint project demonstrates an end-to-end film rental system with:

- API and MVC separation.
- SQL Server persistence.
- JWT authentication.
- Role-based authorization.
- Store-scoped data access.
- Customer self-registration.
- Admin-only staff creation.
- Film actors and categories through relational mapping tables.
- Rental and payment workflows.
- Inventory availability handling.
- Role-specific dashboards.
- Pagination and improved MVC layout.
- Automated service tests.

## Verification Commands

Recommended verification before submission:

```bash
dotnet build
dotnet test
```

Expected result:

```text
Build succeeded.
All tests passed.
```
