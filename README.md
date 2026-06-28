# Library Management System — .NET 10 Web API

A production-grade RESTful API for managing a library's books, members, borrowing operations, and staff users. Built with Clean Architecture, Domain-Driven Design, JWT authentication, and MySQL.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| Framework | ASP.NET Core 10 Web API |
| ORM | Entity Framework Core 9 (Pomelo MySQL provider) |
| Database | MySQL 8 |
| Auth | JWT Bearer (HS256) |
| Password Hashing | BCrypt.Net-Next (work factor 12) |
| Validation | FluentValidation 11 |
| Logging | Serilog (Console + File) |
| API Docs | Scalar UI (native OpenAPI, replaces Swagger) |

---

## Architecture

The solution follows **Clean Architecture** with a strict inward dependency rule:

```
WebAPI → Application → Domain
Infrastructure → Domain
Infrastructure → Application (implements Application interfaces)
```

```
src/
├── LibraryManagement.Domain/          # Enterprise business rules
│   ├── Common/                        # BaseEntity, AuditableEntity
│   ├── Entities/                      # Book, Member, User, Role, ...
│   ├── Enums/                         # BookStatus, MembershipStatus, ...
│   └── Interfaces/Repositories/       # IUnitOfWork, IBookRepository, ...
│
├── LibraryManagement.Application/     # Application business rules
│   ├── Common/
│   │   ├── Exceptions/                # NotFoundException, ConflictException, ...
│   │   ├── Interfaces/                # ICurrentUserService, IJwtService, ...
│   │   ├── Models/                    # ApiResponse<T>
│   │   └── Validators/                # FluentValidation validators
│   ├── DTOs/                          # Request/Response records
│   └── Services/                      # AuthService, BookService, ...
│
├── LibraryManagement.Infrastructure/  # Frameworks & Drivers
│   ├── Persistence/
│   │   ├── LibraryDbContext.cs
│   │   └── Configurations/            # Fluent API entity configs
│   ├── Repositories/                  # EF Core repository implementations
│   ├── Services/                      # JwtService, PasswordService
│   ├── Settings/                      # JwtSettings
│   └── Seed/                          # DatabaseSeeder
│
└── LibraryManagement.WebAPI/          # Entry point
    ├── Controllers/                   # Auth, Books, Members, Users, Borrowing
    ├── Extensions/                    # JwtExtensions
    ├── Middleware/                     # GlobalExceptionHandler, ActivityLogger
    └── Services/                      # CurrentUserService
```

---

## Design Choices

### 1. Clean Architecture
Dependencies only flow inward. The Domain has zero external dependencies — no NuGet packages, no framework references. This makes the business logic fully testable in isolation.

### 2. Domain-Driven Design (DDD)
- **Rich domain entities** with behavior (`Book.TryBorrow()`, `Member.CanBorrow()`, `BorrowingTransaction.Return()`) instead of anemic models.
- **Aggregate roots** manage their child entities — `Book` owns `BookAuthor`, so authors are added via `book.AddAuthor()` not directly.
- **Factory methods** (`Book.Create(...)`) enforce invariants at construction time — an invalid Book cannot be created.
- **Value-based enums** (`BookStatus`, `MembershipStatus`) stored as strings in the DB for readability.

### 3. Repository Pattern + Unit of Work
- `IGenericRepository<T>` provides base CRUD.
- Specialized repositories (`IBookRepository`, `IMemberRepository`) extend it with domain-specific queries (e.g., `SearchAsync`, `GetByCategoryAsync`).
- `IUnitOfWork` coordinates all repositories under a single DB transaction, ensuring atomicity for operations like borrow (update book + create transaction together).

### 4. JWT Authentication + RBAC
Three roles with strict authorization policies:
- **Admin**: Full access — user management, delete operations, all admin tasks.
- **Librarian**: Member management, book management, borrowing operations.
- **Staff**: Borrowing and returning only — cannot access member or user management.

Policies are defined in `JwtExtensions.cs` and applied at the controller/action level via `[Authorize(Policy = "AdminOnly")]` etc.

### 5. Atomic Borrow Operation
The borrow flow uses a **database transaction** to prevent race conditions:
1. Check member eligibility
2. Check book availability
3. Call `book.TryBorrow()` — decrements `AvailableCopies` in memory
4. Save both the book update and the new transaction inside a single DB transaction
5. `TrySaveChangesAsync()` on `IUnitOfWork` catches any concurrency conflict and returns `false` instead of throwing — keeping EF Core details inside Infrastructure

### 6. Hierarchical Categories
Categories support a parent-child tree structure via self-referencing foreign key (`ParentCategoryId`). When searching books by category, a BFS traversal collects all descendant category IDs so "search in Fiction" also returns books in Mystery, Thriller, and Science Fiction sub-categories.

### 7. Activity Logging Middleware
Every non-GET request is logged to the `UserActivityLogs` table with: user identity, action, entity affected, IP address, HTTP status code, and timestamp. GET requests and Scalar/OpenAPI paths are skipped. Failures in the logging itself never crash the request — the middleware catches its own exceptions silently.

### 8. Global Exception Handler
A single middleware maps all custom application exceptions to appropriate HTTP status codes, returning a consistent `ApiResponse<T>` envelope:
- `NotFoundException` → 404
- `ValidationException` → 400 (with errors array)
- `ConflictException` → 409
- `UnauthorizedException` → 401
- `ForbiddenException` → 403
- Unhandled → 500

---

## API Response Format

All endpoints return a consistent envelope:

```json
{
  "success": true,
  "message": "Success",
  "data": { ... },
  "errors": null,
  "statusCode": 200
}
```

---

## Role Permissions Summary

| Endpoint | Admin | Librarian | Staff |
|---|---|---|---|
| POST /api/auth/login | ✅ | ✅ | ✅ |
| GET /api/books | ✅ | ✅ | ✅ |
| POST/PUT /api/books | ✅ | ✅ | ❌ |
| DELETE /api/books | ✅ | ❌ | ❌ |
| GET/POST/PUT /api/members | ✅ | ✅ | ❌ |
| DELETE /api/members | ✅ | ❌ | ❌ |
| PATCH /api/members/suspend | ✅ | ✅ | ❌ |
| ALL /api/users | ✅ | ❌ | ❌ |
| POST /api/borrowing/borrow | ✅ | ✅ | ✅ |
| POST /api/borrowing/return | ✅ | ✅ | ✅ |
| GET /api/borrowing/* | ✅ | ✅ | ✅ |

---

## Seed Credentials

| Username | Password | Role |
|---|---|---|
| admin | Admin@123! | Admin |
| librarian | Librarian@123! | Librarian |
| staff | Staff@123! | Staff |

---

## Running the Project

### Prerequisites
- .NET 10 SDK
- MySQL 8

### Setup

1. Update connection string in `src/LibraryManagement.WebAPI/appsettings.json`:
```json
"DefaultConnection": "Server=localhost;Port=3306;Database=LibraryManagementDb;Uid=root;Pwd=YOUR_PASSWORD;"
```

2. Run:
```bash
dotnet run --project src/LibraryManagement.WebAPI
```

The application will:
- Create the database automatically
- Create all tables
- Seed all reference data

3. Open API docs at: `https://localhost:{PORT}/scalar/v1`

---

## Endpoints Overview

| Method | Endpoint | Description |
|---|---|---|
| POST | /api/auth/login | Get JWT token |
| GET | /api/auth/me | Current user info |
| GET | /api/books | List books (search, filter) |
| GET | /api/books/{id} | Book detail |
| POST | /api/books | Create book |
| PUT | /api/books/{id} | Update book |
| DELETE | /api/books/{id} | Delete book |
| GET | /api/members | List members |
| GET | /api/members/{id} | Member detail |
| POST | /api/members | Create member |
| PUT | /api/members/{id} | Update member |
| DELETE | /api/members/{id} | Delete member |
| PATCH | /api/members/{id}/suspend | Suspend member |
| PATCH | /api/members/{id}/renew | Renew membership |
| GET | /api/users | List users (Admin) |
| POST | /api/users | Create user (Admin) |
| PUT | /api/users/{id} | Update user (Admin) |
| PATCH | /api/users/{id}/deactivate | Deactivate user |
| PATCH | /api/users/{id}/password | Change password |
| POST | /api/borrowing/borrow | Borrow a book |
| POST | /api/borrowing/{id}/return | Return a book |
| GET | /api/borrowing/active | Active borrowings |
| GET | /api/borrowing/overdue | Overdue borrowings |
| GET | /api/borrowing/member/{id} | Member borrowing history |
