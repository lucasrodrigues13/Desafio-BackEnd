# MotorRental Web API

This is a .NET 8 Web API project for managing motor rentals. The project uses PostgreSQL as the database, RabbitMQ for messaging, and includes a worker service that processes messages from RabbitMQ.

## Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Requirements](#requirements)
- [Installation](#installation)
- [Usage](#usage)
- [Project Structure](#project-structure)
- [Environment Variables](#environment-variables)
- [Running the Application](#running-the-application)
- [Contact](#contact)

## Project Overview

Application to manage motorbike rentals and delivery drivers.

## Features

- **RESTful API** for managing motor rentals.
- **PostgreSQL** for data persistence.
- **RabbitMQ** for messaging between services.
- **Worker service** for background processing of rental tasks.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Installation

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/motorrental.git
cd motorrental
```

### 2. Build and Run with Docker Compose

Use Docker Compose to build and run the services. This will start the Web API, PostgreSQL database, RabbitMQ, and the worker service.

```bash
docker-compose up --build
```

### 3. Apply Migrations (if needed)

If your project uses EF Core migrations, apply them after starting the services:

```bash
docker-compose exec webapi dotnet ef database update
```

## Usage

### Access the API

Once the application is running, you can access the API at:

- **Base URL:** `http://localhost:8080`
- **Swagger UI:** `http://localhost:8080/swagger`

### Access RabbitMQ Management Interface

- **URL:** `http://localhost:15672`
- **Username:** `guest`
- **Password:** `guest`

## Project Structure

```plaintext
├── src
│   ├── MotorRental.Application          # Application layer for business logic and use cases
│   │   ├── Common                       # General classes for example a standard response for endpoints
│   │   ├── Extensions                   # Extension class to inject the interfaces
│   │   ├── Interfaces                   # Interfaces for application services
│   │   └── Services                     # Implementation of application services
│   ├── MotorRental.Domain               # Domain layer for core entities and logic
│   │   ├── Constants                    # Constant classes 
│   │   ├── Dtos                         # Data Transfer Objects
│   │   ├── Entities                     # Domain entities and aggregates
│   │   ├── Enums                        # Custom Enums exceptions
│   │   └── Interfaces                   # Domain service interfaces
│   ├── MotorRental.Infrastructure       # Infrastructure layer for data access, repositories, etc.
│   │   ├── Data                         # Database context and migrations
│   │   ├── Repositories                 # Repository implementations
│   │   ├── Messaging                    # Integration with RabbitMQ or other messaging systems
│   │   ├── Extensions                   # Extension class to inject the interfaces
│   │   ├── Configurations               # Infrastructure-related configurations
│   │   ├── Migrations                   # Migrations from Entity framework
│   │   └── ExternalServices             # External service implementations
│   ├── MotorRental.WebApi               # Main Web API project
│   │   ├── Controllers                  # API Controllers
│   │   ├── Filters                      # API filters (e.g., exception handling, validation)
│   │   ├── Startup.cs                   # ASP.NET Core startup configuration
│   │   └── Dockerfile                   # Dockerfile for Web API
│   ├── MotorRental.Workers              # Worker service project
│   │   └── Dockerfile                   # Dockerfile for Worker service
├── tests
│   ├── MotorRental.Api.Tests            # Unit tests for Web API
│   ├── MotorRental.Service.Tests        # Unit tests for Application services
├── docker-compose.yml                   # Docker Compose configuration for the entire stack
└── README.md                            # Project README file

```

## Environment Variables

The following environment variables need to be configured in the `.env` file:

```plaintext
# PostgreSQL
POSTGRES_USER=postgres
POSTGRES_PASSWORD=yourpassword
POSTGRES_DB=motorrentaldb

# RabbitMQ
RabbitMQ__HostName=rabbitmq

# ASP.NET Core
ASPNETCORE_ENVIRONMENT=Development
```

## Running the Application

### Run with Docker Compose

To start the entire application stack, use Docker Compose:

```bash
docker-compose up --build
```

### Stop the Application

To stop the services, use:

```bash
docker-compose down
```

### View Logs

You can view logs for any service using Docker Compose:

```bash
docker-compose logs -f webapi
```

## Contact

For any inquiries or issues, please contact flucasrodrigues@hotmail.com or open an issue on the GitHub repository.
