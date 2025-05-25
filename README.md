**TheDailyLens Backend Documentation**

---

**Contents**

1. Project Overview
2. Technologies & Tools
3. Architecture
4. Setup & Installation
5. Authentication & Authorization
6. Contribution Guidelines

---

## 1. Project Overview

**TheDailyLens** is a news website built with ASP.NET (C#) on the server side and Microsoft SQL Server as the database. This documentation covers all aspects of the backend implementation: architecture, data model, API endpoints, setup, deployment, testing, security, and maintenance.

---

## 2. Technologies & Tools

* **Framework:** ASP.NET Core 8.0 (Web API)
* **Language:** C# 11.0
* **Database:** Microsoft SQL Server 2019
* **ORM:** Entity Framework Core 7.0
* **Dependency Injection:** Built-in ASP.NET Core DI container
* **Authentication & Authorization:** JWT (JSON Web Tokens) + ASP.NET Core Identity
* **Configuration:** appsettings.json / environment variables

---

## 3. Architecture

The backend follows a layered architecture:

1. **Presentation Layer** (Controllers): exposes RESTful API endpoints.
2. **Application Layer** (Services): business logic and orchestration.
3. **Domain Layer** (interfaces): interface for the Application Layer

---

## 4. Setup & Installation

### 4.1 Prerequisites

* .NET 8.0 SDK
* SQL Server 2019 (local or remote)
* Git

### 4.2 Configuration

1. Clone the repository:

   ```bash
    git clone https://github.com//TheDailyLens.git
    cd TheDailyLens
   ```

2. Set your database connection string:
   ```bash
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=TheDailyLens;Trusted_Connection=True;"
   } 
   ````
 
3. Configure JWT settings in `appsettings.json`:

   ```json
   "Jwt": {
     "Key": "<YourSecretKey>",
     "Issuer": "http://localhost:5110",
     "Audience": "http://localhost:5173",
   }
   ```

### 4.3 Database Migration

Run EF Core migrations:
  
  ```bash
  dotnet ef database update
  ```

---

## 5. Authentication & Authorization

* Users register and log in via JWT-based flow.

---

## 6. Contribution Guidelines

1. Fork the repository and create a feature branch.
2. Submit a pull request referencing an issue.

---




