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
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## Project Overview

The MotorRental Web API allows users to manage motor rentals, including viewing available vehicles, making reservations, and tracking rental history. The application is built using .NET 8, with PostgreSQL as the database and RabbitMQ as the message broker. Additionally, the project includes a worker service that consumes messages from RabbitMQ to process rental operations asynchronously.

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

### 2. Set Up the Environment Variables

Copy the `.env.example` file to `.env` and update the environment variables as needed.

```bash
cp .env.example .env
```

### 3. Build and Run with Docker Compose

Use Docker Compose to build and run the services. This will start the Web API, PostgreSQL database, RabbitMQ, and the worker service.

```bash
docker-compose up --build
```

### 4. Apply Migrations (if needed)

If your project uses EF Core migrations, apply them after starting the services:

```bash
docker-compose exec webapi dotnet ef database update
```

## Usage

### Access the API

Once the application is running, you can access the API at:

- **Base URL:** `http://localhost:8080`
- **Swagger UI:** `http://localhost:8080/swagger`

### Example API Endpoints

- **Get Available Vehicles:** `GET /api/vehicles`
- **Create a Rental:** `POST /api/rentals`
- **View Rental History:** `GET /api/rentals/history`

### Access RabbitMQ Management Interface

- **URL:** `http://localhost:15672`
- **Username:** `guest`
- **Password:** `guest`

## Project Structure

```plaintext
├── src
│   ├── MotorRental.WebApi             # Main Web API project
│   │   ├── Controllers                # API Controllers
│   │   ├── Models                     # Data models
│   │   ├── Services                   # Application services
│   │   └── Dockerfile                 # Dockerfile for Web API
│   ├── MotorRental.Workers            # Worker service project
│   │   ├── Consumers                  # RabbitMQ consumers
│   │   └── Dockerfile                 # Dockerfile for Worker service
├── docker-compose.yml                 # Docker Compose configuration
└── README.md                          # This README file
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

## Contributing

We welcome contributions! If you'd like to contribute, please fork the repository, create a new branch, and submit a pull request. Make sure to follow the coding standards and include tests where applicable.

### Steps to Contribute

1. Fork the repository.
2. Create a new branch (`git checkout -b feature/YourFeature`).
3. Make your changes and commit them (`git commit -m 'Add YourFeature'`).
4. Push to the branch (`git push origin feature/YourFeature`).
5. Open a pull request on GitHub.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contact

For any inquiries or issues, please contact [your email] or open an issue on the GitHub repository.
