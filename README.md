# 🎓 University Survey Management System

An end-to-end full-stack survey and evaluation platform designed for higher education institutions, built with .NET / C#. The system features an interactive Blazor WebAssembly client and a scalable ASP.NET Core Web API backend architected with Clean Architecture principles. It enables administrators to create and target surveys to university segments (Students and Employees), tracks response lifecycles through draft and submission stages, handles complex question constraints, and connects the Blazor client to the backend via dedicated consumer services.

---

## 📑 Table of Contents

- [About the Project](#-about-the-project)
- [Architecture & Flow](#-architecture--flow)
- [Layer Responsibilities](#-layer-responsibilities)
- [Project Structure](#-project-structure)
- [Technologies & Libraries](#-technologies--libraries)
- [Getting Started](#-getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation & Setup](#installation--setup)
  - [Port & Localhost Configuration](#-port--localhost-configuration)
  - [Database Migrations](#-database-migrations)
  - [Running the Application](#-running-the-application)
- [Core Response Endpoints](#-core-response-endpoints)

---

## 🎓 About the Project

This platform is tailored specifically for university campus feedback and institutional research, offering:

- **Targeted Audience Distribution:** Restricts survey participation strictly based on user roles (`Student` vs. `Employee`), blocking unauthorized access and preventing administrative self-submission.
- **Dynamic Question Types:** Validates answers according to dynamic question criteria:
  - **Single & Multiple Choice:** Verifies option ownership and blocks duplicate selections.
  - **Text:** Validates non-empty string feedback.
  - **Yes/No:** Enforces boolean values (`"true"` / `"false"`).
  - **Rating:** Restricts evaluations to numeric scores between 1 and 5.
- **Draft & Lifecycle Management:** Supports saving progress incrementally (`InProgress`), resuming existing drafts, blocking submissions outside active date windows (`StartDate` to `EndDate`), and enforcing single-submission policies when `Allowmultiblesubmission` is disabled.
- **Consumer Pattern Integration:** Decouples Blazor WebAssembly UI components from raw HTTP networking through a dedicated `ResponseConsumer` layer.

---

## 🏛 Architecture & Flow

The application follows an end-to-end flow with strictly decoupled layers:

```text
┌─────────────────────────────────────────────────────────────┐
│             Frontend UI (Blazor WebAssembly Pages)          │
└──────────────────────────────┬──────────────────────────────┘
                               │ Calls Consumer Methods
                               ▼
┌─────────────────────────────────────────────────────────────┐
│               Consumer Layer (Front.Consumer)               │
│       *HttpClient wrappers calling API & handling DTOs*     │
└──────────────────────────────┬──────────────────────────────┘
                               │ HTTP Requests / JSON Payloads
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                 API Layer (Api.Controllers)                 │
│      *Extracts User Claims/Roles, Validates, Calls Service* │
└──────────────────────────────┬──────────────────────────────┘
                               │ Passes DTOs & Parameters
                               ▼
┌─────────────────────────────────────────────────────────────┐
│            Business Services (Infrastructure.Service)       │
│  *Validates Dates, Enforces Roles, Checks Types, Maps DTOs* │
└──────────────────────────────┬──────────────────────────────┘
                               │ EF Core Operations
                               ▼
┌─────────────────────────────────────────────────────────────┐
│           Data Context (Infrastructure.Data.AppDbContext)   │
│         *Direct Database Operations on Domain Entities*     │
└──────────────────────────────┬──────────────────────────────┘
                               │ Reads / Persists
                               ▼
┌─────────────────────────────────────────────────────────────┐
│                Database & Domain (SQL Server)               │
└──────────────────────────────┘
```

---

## 🔄 Layer Responsibilities

- **Domain (`Domain`):** Contains business entities (`Survey`, `Question`, `QuestionOption`, `Response`, `Answer`) and domain enums (`Role`, `Statues`, `Statue`, `QuestionType`, `TargetGroups`) with zero external dependencies.
- **Shared (`Shared`):** Contains shared request/response models (`CreateResponseDto`, `AnswerDto`, `ResponseIdResult`, `AnswerIdResult`) utilized across both the client and server.
- **Infrastructure (`Infrastructure`):**
  - **Context (`AppDbContext`):** Entity Framework Core mapping and database configurations.
  - **Services (`IService` / `Service`):** Houses business rules (verifying survey status, scheduling dates, role eligibility, draft resumption, duplicate answer guards, and submission publication).
- **API (`Api`):** Exposes secured RESTful endpoints (`ResponseController`) protected with `[Authorize(Roles = "Student,Employee")]`, extracts identity claims (`NameIdentifier`, `Role`), and delegates operations to the service layer.
- **Frontend (`Front` - Blazor WebAssembly):**
  - **Consumer Layer (`ResponseConsumer`):** Encapsulates `HttpClient` requests, serializes request bodies, deserializes JSON responses, and exposes strongly-typed asynchronous methods to Blazor `.razor` components.
  - **Components & Pages:** Interactive client views for viewing and answering dynamic surveys.

---

## 📁 Project Structure

```text
UniversitySurveySystem/
├── src/
│   ├── Front/                           # Blazor WebAssembly Client
│   │   ├── Consumer/                    # API Client consumers (e.g., ResponseConsumer.cs)
│   │   ├── Pages/                       # Blazor Razor views (.razor)
│   │   ├── Shared/                      # Reusable Razor components and layouts
│   │   ├── wwwroot/                     # Static styling, index.html, and assets
│   │   └── Program.cs                   # Blazor WASM startup & Consumer DI registration
│   │
│   ├── Api/
│   │   ├── Controllers/                 # REST Controllers (e.g., ResponseController.cs)
│   │   ├── Properties/
│   │   │   └── launchSettings.json      # Local port configurations
│   │   ├── appsettings.json             # DB connection strings & JWT settings
│   │   └── Program.cs                   # API middleware pipeline, CORS, & DI container
│   │
│   ├── Infrastructure/
│   │   ├── Context/                     # AppDbContext & entity configurations
│   │   ├── Migrations/                  # EF Core database migrations
│   │   ├── IService/                    # Service contracts (e.g., IResponseService.cs)
│   │   └── Service/                     # Core business logic (e.g., ResponseService.cs)
│   │
│   ├── Domain/
│   │   ├── Entities/                    # Database models (Survey, Question, Response, etc.)
│   │   └── Enums/                       # Domain enums (Role, Statue, QuestionType, etc.)
│   │
│   └── Shared/
│       ├── DTOs/                        # Transfer objects (CreateResponseDto, AnswerDto)
│       └── Results/                     # Consumer result contracts (ResponseIdResult, etc.)
│
└── UniversitySurveySystem.sln
```

---

## 🛠 Technologies & Libraries

- **Frontend:** Blazor WebAssembly (.NET), `System.Net.Http.Json`
- **Backend API:** ASP.NET Core Web API (.NET 8/9)
- **Database & ORM:** SQL Server & Entity Framework Core
- **Shared Layer:** .NET Class Library for contracts & DTOs
- **Authentication & Security:** ASP.NET Core Identity / JWT Bearer Authentication with Role-based authorization (`Student`, `Employee`)
- **API Documentation:** Swagger / OpenAPI

---

## ⚙️ Getting Started

### Prerequisites

- **.NET SDK (8.0 or 9.0)**
- **SQL Server / LocalDB**
- **Visual Studio 2022** (with *ASP.NET and web development* workload) or **VS Code** with C# Dev Kit

### Installation & Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/your-username/university-survey-system.git
   cd university-survey-system
   ```

2. **Restore NuGet packages:**
   ```bash
   dotnet restore
   ```

3. **Configure Database Connection:**
   Open `src/Api/appsettings.json` and adjust the connection details:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost;Database=UniversitySurveyDb;Trusted_Connection=True;TrustServerCertificate=True;"
     },
     "JwtSettings": {
       "Key": "YourSuperSecretKeyWithSufficientLengthForValidation123!",
       "Issuer": "UniversitySurveyApi",
       "Audience": "UniversitySurveyApp"
     }
   }
   ```

---

## 🔌 Port & Localhost Configuration

### 1. Verify API Port (Api)

Review the assigned HTTPS port in `src/Api/Properties/launchSettings.json`:
```json
"https": {
  "commandName": "Project",
  "applicationUrl": "https://localhost:7123;http://localhost:5123"
}
```

### 2. Configure Consumer HttpClient in Blazor WebAssembly (Front)

In `src/Front/Program.cs`, ensure the `HttpClient` base address points to the running API, then register the consumer:
```csharp
builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri("https://localhost:7123/") 
});

// Register Consumers
builder.Services.AddScoped<ResponseConsumer>();
```

### 3. Check CORS Policy (Api)

In `src/Api/Program.cs`, confirm that the Blazor WebAssembly client origin is allowed through CORS:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7001", "http://localhost:5001") // Blazor WebAssembly URL
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

app.UseCors("AllowBlazorClient");
```

---

## 🗄️ Database Migrations

Apply database schema changes:

**Package Manager Console (Visual Studio):**
```powershell
# Set Default project: Infrastructure, Startup project: Api
Update-Database
```

**.NET CLI:**
```bash
dotnet ef database update --project src/Infrastructure --startup-project src/Api
```

---

## ▶️ Running the Application

### Visual Studio (Multiple Startup Projects)

1. Right-click the Solution in Solution Explorer -> **Configure Startup Projects...**
2. Choose **Multiple startup projects**:
   - Set `Api` to **Start**.
   - Set `Front` (Blazor WebAssembly) to **Start**.
3. Press **F5** to run both applications concurrently.

---

## 📑 Core Response Endpoints

| Method | Endpoint | Description | Auth Required |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Response/response` | Creates a new draft or resumes an in-progress response | Yes (`Student`, `Employee`) |
| `POST` | `/api/Response/response/{responseId}/answer` | Adds or updates an answer to a question within a response | Yes (`Student`, `Employee`) |
| `POST` | `/api/Response/response/{responseId}/publish` | Finalizes the response and marks it as submitted | Yes (`Student`, `Employee`) |
