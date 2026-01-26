📦 E-Commerce Platform

A full-stack e-commerce platform developed with ASP.NET Core MVC and Web API, designed to simulate a real-world production scenario with separated responsibilities, authentication strategies, and deployment considerations.

This project was developed as part of the KM106 Backend Masterclass and was deployed to a live environment for technical interview presentation.

🚀 Project Overview

The system consists of multiple applications working together:

E-Commerce MVC – customer-facing web application

Admin MVC – product, category and system management

Data API – handles business logic and database operations

File API – manages image uploads and static file access

The architecture was intentionally designed to reflect real enterprise patterns rather than a single monolithic application.

🧱 Architecture

Layered Architecture

Presentation Layer (MVC)

API Layer

Service Layer

Data Access Layer

DTO / ViewModel separation

API-based service communication

Separation of Concerns across all projects

🔐 Authentication & Authorization

The project uses a hybrid authentication model:

MVC applications

Cookie-based authentication

User session management

Web APIs

JWT-based authentication

Token validation via Authorization header

This structure reflects common enterprise systems where UI and APIs use different authentication mechanisms.

🖼️ File & Image Management

Dedicated File API for image uploads

Product images stored and served independently from business APIs

Seed images managed via File API root directory

⚙️ Configuration & Environments

Development and production environments are configured separately

Environment-specific settings (URLs, ports, SSL, IIS bindings) are not committed to source control

GitHub version represents the development-ready configuration

This approach follows real-world best practices for environment management.

🌐 Deployment

The project was deployed to a live server for technical interview presentation.

Deployment experience included:

IIS publish configuration

Site and application pool setup

HTTPS binding and SSL certificate configuration

Handling HTTP / HTTPS communication issues

Production-specific appsettings management

🧪 Technologies Used

ASP.NET Core MVC

ASP.NET Core Web API

Entity Framework Core

JWT Authentication

Cookie Authentication

SQL Server

IIS

RESTful APIs

📌 Notes

This project focuses on backend architecture, authentication flows, and deployment experience, rather than UI design perfection.

👤 Author

Mustafa Ümit Uysal
Backend-focused .NET Developer
