# Learning Center Platform

## Project Overview

The `Learning Center Platform` is an scalable backend application designed
 to manage educational content, user profiles, and authentication. Built with .NET, it adheres to Domain-Driven Design (DDD) principles and Clean Architecture, promoting modularity, maintainability, and testability.

This platform is structured around distinct Bounded Contexts, ensuring clear separation of concerns and enabling independent development and
 deployment of core functionalities.

## Table of Contents

1.  Architecture Overview
2.  Domain-Driven Design (DDD) Concepts
3.  Key Features & Best Practices Implemented
4.  Bounded Contexts
    *   IAM (Identity and Access Management)
    *   Profiles
    *   Publishing
5.  Technologies Used
6.  Getting Started
    *   Prerequisites
    *   Setup Instructions
7.  Project Structure
8.  License

## Architecture Overview

The project follows a Domain-Driven Design approach, organizing code into distinct layers with clear dependencies. This ensures that the domain model remains independent of external concerns like UI, databases, or external services.

*   **Domain Layer**: Contains the core business logic, entities, aggregates, value objects, and domain services. It is the heart of the application and has no dependencies on other layers.
*   **Application Layer**: Orchestrates domain objects to fulfill use cases (commands and queries). It defines application services, command handlers, and query handlers. It depends on the Domain layer.
*   **Infrastructure Layer**: Provides implementations for interfaces defined in the Domain and Application layers (e.g., database repositories, external service clients, authentication providers). It depends on the Application and Domain layers.
*   **Interfaces Layer (Presentation)**: Handles external communication, typically REST APIs. It defines controllers, API
 resources, and assemblers for transforming between API models and application/domain models. It depends on the Application layer.

## Domain-Driven Design (DDD) Concepts

The project embraces DDD to manage complexity in its core business domains:

*   **Bounded Contexts**: The application is explicitly divided into `IAM`, `Profiles`,
 and `Publishing` bounded contexts, each with its own ubiquitous language and domain model.
*   **Aggregates**: Key domain objects like `Tutorial` (in Publishing) and `User` (in IAM) are defined as aggregate roots. They encapsulate a cluster of domain objects (entities and value objects) and
 ensure transactional consistency within their boundaries.
*   **Entities**: Objects with a distinct identity that runs through time and different representations (e.g., `Category`, `Asset`).
*   **Value Objects**: Objects that measure, quantify, or describe a thing in the domain and are immutable (e.g.,
 `FullName`, `EmailAddress`, `StreetAddress` in Profiles). They are compared by their values, not their identity.
*   **Domain Services**: Operations that don't naturally fit within an entity or value object (e.g., `ITokenService`, `IHashingService` in IAM).

## Key Features & Best Practices Implemented

*   **Cancellation Tokens**: Integrated across all asynchronous operations (application services, repositories, controllers, middleware) to enable graceful cancellation of long-running tasks, improving responsiveness and resource management.
*   **Refined Error Management**:
    *   **`Result<T>` Pattern**: Used consistently in application services to
 explicitly represent success or failure
 of an operation, avoiding exceptions for control flow.
    *   **Domain-Specific Error Enums**: Strongly-typed enums (e.g., `IamError`, `ProfilesError`, `PublishingError`) provide clear, categorized error codes
 for business rule violations.
    *   **Localized Error Messages**: Error messages are externalized into `.resx` files and retrieved via `IStringLocalizer`, supporting multiple languages.
    *   **RFC 7807 Problem Details**: All API error responses adhere to this standard, providing machine-readable and
 consistent error information to
 clients.
    *   **Global Exception Handling Middleware**: Catches unhandled exceptions, logs them, and returns standardized `ProblemDetails` responses, preventing sensitive information leakage.
*   **Internationalization (i18n)**:
    *   **Decentralized Resources**: `.resx
` files are
 organized within the `Resources` folder of each
 bounded context, aligning with modularity principles.
    *   **`IStringLocalizer`**: Used throughout the application for dynamic retrieval of localized strings.
    *   **Culture Negotiation**: Configured to determine the user's preferred language from query strings
, cookies, or `Accept-Language` headers
.
*   **Clean API Design with `ActionResultAssemblers`**:
    *   **Thin Controllers**: Controllers are kept lean, focusing on request handling and delegating response formatting.
    *   **Static Assemblers**: Dedicated static assembler classes (e
.g., `IamActionResultAssembler`,
`PublishingActionResultAssembler`) encapsulate the logic for transforming `Result<T>` objects into `IActionResult`s, including mapping error enums to HTTP status codes and generating localized `ProblemDetails`. This centralizes response logic and ensures consistency.
*   **Persistence**: Util
izes Entity Framework Core for data access, supporting
 MySQL databases.
*   **Messaging/Mediation**: Employs Cortex.Mediator for handling commands and publishing domain events, promoting loose coupling between components.
*   **Authentication & Authorization**: Implements JWT-based authentication with custom middleware
 for request authorization.

## Bounded Contexts

### IAM (
Identity and Access Management)

Manages user authentication, registration, and authorization concerns.

*   **Aggregates**: `User`
*   **Services**: `ITokenService`, `IHashingService`
*   **API Endpoints**: `/api/v1/authentication`, `/api/v1/users`

### Profiles

Handles the management of user profiles, including personal details.

*   **Aggregates**: `Profile`
*   **Value Objects**: `FullName`, `EmailAddress`, `StreetAddress`
*   **API Endpoints**: `/api/v1/profiles`

### Publishing

Manages educational content, such as tutorials and categories.

*   **Aggregates**: `Tutorial`
*   **Entities**: `Category`, `Asset` (and its specializations `VideoAsset`, `ImageAsset
`, `ReadableContentAsset`)
*   **API Endpoints**: `/api/v1/categories`, `/api/v1/tutorials`

## Technologies Used

*   **.NET 10**: Core framework for the application.
*   **ASP.NET Core**: For building RESTful APIs.
*   **Entity Framework Core**: ORM for database interactions
 (
MySQL).
*   **MySQL**: Relational database.
*   **Cortex.Mediator**: For implementing the Mediator pattern.
*   **BCrypt.Net-Next**: For secure password hashing.
*   **System.IdentityModel.Tokens.Jwt**: For JWT generation and validation.
*   **Swashbuckle.AspNetCore**: For OpenAPI/Swagger documentation.
*   **Microsoft.Extensions.Localization**: For Internationalization (i18n).
*   **Humanizer**: For string manipulation (e.g., kebab-case routing).

## Getting Started

### Prerequisites

*   .NET 10 SDK
*   MySQL Server (or Docker for local development)
*   Git

### Setup Instructions

1.  **Clone the repository:**
    ```bash
    git clone https://github.com/upc-pre-202610-1asi0730-sandbox/learning-center-platform.git
    cd learning-center-platform
    ```

2.  **Navigate to the project directory:**
    ```bash
    cd Acme.Center.Platform
    ```

3.  **Restore NuGet packages:**
    ```bash
    dotnet restore
    ```

4.  **Configure Database:**
    *   Ensure your MySQL server is running.
    *   Update the `DefaultConnection` string in `appsettings.json` (and `appsettings.Development.json`) to point to your MySQL instance.
    *   The application is
 configured to automatically create the database if it doesn't exist on startup (`context.Database.EnsureCreated();`).

5.  **Run the application:**
    ```bash
    dotnet run
    ```
    The API will typically run on `https://localhost:7000` (or a
 similar port).

6.  **Access Swagger UI:**
    Open your browser and navigate to `https://localhost:7000/swagger` to explore the API endpoints.

## Project Structure

The project is organized by **Bounded Contexts** at the top level, with each
 context further decomposed into
 architectural layers:

```
Acme.Center.Platform/
├── Iam/                     # IAM Bounded Context
│   ├── Application/         # Command/Query Services, Handlers
│   ├── Domain/              # Aggregates, Entities, Value Objects, Interfaces
│   ├── Infrastructure/      # Repository Implementations, External Service Clients
│   ├── Interfaces/          # REST Controllers, Resources, Assemblers
│   └── Resources/           # Iam-specific localization files (e.g., IamMessages.resx)
├── Profiles/                # Profiles Bounded Context
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   ├── Interfaces/          # REST Controllers, Resources, Assemblers
│   └── Resources/           # Profiles-specific localization files (e.g., ProfilesMessages.resx)
├── Publishing/              # Publishing Bounded Context
│   ├── Application/
│   ├── Domain/
│   ├── Infrastructure/
│   ├── Interfaces/          # REST Controllers, Resources, Assemblers
│   └── Resources/           # Publishing-specific localization files (e.g., PublishingMessages.resx)
├── Shared/                  # Shared Bounded Context (cross-cutting concerns)
│   ├── Application/         # Generic Result<T>, common application models
│   ├── Domain/              # Base Repository, Unit of Work interfaces, common domain models
│   ├── Infrastructure/      # Base Repository implementation, AppDbContext
│   ├── Interfaces/          # ProblemDetailsFactory, common REST interfaces
│   └── Resources/           # Shared localization files (Commons.resx, Errors/ErrorMessages.resx)
├── Program.cs               # Application startup and DI configuration
├── appsettings.json         # Configuration files
└── Acme.Center.Platform.csproj # Project file
```

## License

This project is licensed
 under the Apache 2.0 License. See the LICENSE.md file for details.