# TechMove - Enterprise Service Management System

A full-stack ASP.NET Core MVC application for managing clients, contracts, and service requests with currency conversion and file handling.

## Features

- **Client & Contract Management** with full CRUD
- **Service Requests** with workflow validation
- **Real-time Currency Conversion** (USD → ZAR) using external API
- **PDF Signed Agreement Upload** with validation
- **Search & Advanced Filtering** using LINQ
- **Unit Testing** with xUnit
- **Responsive UI** with Bootstrap 5

## Technologies Used

- ASP.NET Core MVC
- Entity Framework Core + SQL Server
- xUnit + Moq (Unit Testing)
- HttpClient (External API)
- Bootstrap 5 + Bootstrap Icons

## Setup Instructions

1. Clone the repository
2. Update connection string in `appsettings.json`
3. Run the following commands:

```bash
dotnet ef database update
dotnet run
