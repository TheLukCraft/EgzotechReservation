# Egzotech Reservation API

REST API for managing rehabilitation robot reservations. The system handles concurrent booking requests, prevents double-booking, and manages reservation expiration automatically.

## Tech Stack

* **.NET 10** (ASP.NET Web API)
* **Entity Framework** (SQL Server)
* **Docker & Docker Compose**
* **FluentValidation**
* **xUnit & Testcontainers** (Integration Tests)

## Getting Started

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop) installed and running.

### How to Run

The project includes helper scripts to set up the environment variables and start the containers.

**Windows:**
```bat
./start.bat
```

**Linux / macOS:**
```bash
chmod +x start.sh
./start.sh
```

The script will automatically create a .env file from .env.example if it doesn't exist, build the image, and start the services.

### Access the API
Once running, the API documentation (Swagger UI) is available at: http://localhost:5000/swagger

## Running Tests

To run unit and integration tests (including race condition simulation):

```bash
dotnet test
```

**Note:** Integration tests use Testcontainers, so Docker must be running.

## Author

Łukasz Capała

