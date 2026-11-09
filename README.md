# RaceDay – Event Management System

## 1. Project Overview

RaceDay is an event management system developed using ASP.NET Core MVC, ASP.NET Core Web API, and Microsoft SQL Server.

The system provides a platform for users to interact with events. Organisers can create and manage events, while attendees can access event information.

The project demonstrates web application development, REST API integration, database management, authentication, and containerisation using Docker.

## 2. Technologies Used

- **Frontend:** ASP.NET Core MVC, HTML, CSS, JavaScript
- **Backend:** C#, ASP.NET Core Web API
- **Database:** Microsoft SQL Server
- **Authentication:** JWT (JSON Web Tokens)
- **API Documentation:** Swagger / OpenAPI
- **Containerisation:** Docker and Docker Compose
- **Version Control:** Git and GitHub
- **CI/CD:** GitHub Actions

## 3. Project Structure

```text
RaceDay/
│
├── .github/
│   └── workflows/
│
├── RaceDay/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Views/
│   ├── wwwroot/
│   ├── appsettings.json
│   └── RaceDay.Web.csproj
│
├── RaceDay.API/
│   ├── Controllers/
│   ├── Data/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   ├── Dockerfile
│   └── RaceDay.API.csproj
│
├── RaceDay.API.Tests/
│
├── .env
├── .gitignore
├── docker-compose.yml
└── RaceDay.API.sln
```

## 4. Main Features

### Organiser Features
- Create new events.
- View and manage existing events.
- Edit event information.
- Delete events.

### Attendee Features
- Access the event management platform.
- View available event information.

### API Features
- RESTful API endpoints.
- JWT-based authentication.
- Swagger API documentation.
- SQL Server database integration.

## 5. Database

RaceDay uses Microsoft SQL Server to store application data.

The API connects to the database using Entity Framework Core.

Database configuration is managed through environment variables.

## 6. Installation and Setup

### Requirements

Install the following before running the project:

- Visual Studio 2022
- .NET 8 SDK
- Docker Desktop
- Microsoft SQL Server
- Git

### Clone the Repository

```bash
git clone YOUR_GITHUB_REPOSITORY_URL
```

Navigate to the project folder:

```bash
cd RaceDay.API
```

### Configure Environment Variables

Create a `.env` file in the root project directory.

Add the following configuration:

```env
DB_PASSWORD=YOUR_DATABASE_PASSWORD
JWT_KEY=YOUR_SECURE_BASE64_JWT_KEY
```

Replace the placeholder values with your own configuration.

**Important:** Never commit your `.env` file, database password, or JWT secret to GitHub.

### Run Using Docker

Make sure Docker Desktop is running.

From the root project directory, execute:

```bash
docker compose up --build -d
```

This builds and starts the API and MVC containers.

### Access the Application

| Component | URL |
|---|---|
| RaceDay MVC Website | http://localhost:5089 |
| RaceDay API | http://localhost:5088 |
| Swagger Documentation | http://localhost:5088/swagger |

To stop the containers:

```bash
docker compose down
```

## 7. Running Through Visual Studio

1. Open `RaceDay.API.sln` in Visual Studio 2022.
2. Restore NuGet packages.
3. Ensure SQL Server is running and the database connection is configured.
4. Build the solution.
5. Start the API and MVC projects using the appropriate launch profiles.
6. Open the application in your browser.

## 8. Testing

The project includes a separate `RaceDay.API.Tests` project for API testing.

To run the tests using the .NET CLI:

```bash
dotnet test RaceDay.API.sln
```

## 9. Continuous Integration (GitHub Actions)

GitHub Actions is used to automate project validation.

The workflow files are stored in:

```text
.github/workflows/
```

The workflows can run automatically when changes are pushed to GitHub or pull requests are created, depending on their configured triggers.

A successful workflow run is indicated by a green check mark in the GitHub Actions tab.

**CI/CD Build Screenshot:**

[Insert screenshot of successful GitHub Actions run here.]

## 10. Demonstration Video

A demonstration video showing the RaceDay application, its main features, and API functionality is available below.

**YouTube Video:** [Insert YouTube demonstration link here.]

## 11. Security

- JWT authentication is used for protected API operations.
- Database credentials and JWT secrets are stored in environment variables.
- Sensitive configuration files are excluded from version control.
- API requests are handled through the ASP.NET Core Web API.

## 12. Author

Developed as part of a Software Development academic project.

**Project Name:** RaceDay  
**Language:** C#  
**Framework:** .NET 8  
**IDE:** Visual Studio 2022

## 13. License

This project was developed for educational purposes.
