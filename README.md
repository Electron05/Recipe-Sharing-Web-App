# RecipeBay

RecipeBay is a full-stack web application for managing and sharing recipes. It features user authentication, recipe creation with ingredient autosearch, and a PostgreSQL database. This project was developed as a learning experience in C# backend development, Angular frontend, and DevOps practices, but was ultimately abandoned due to frequent database schema changes, lack of personal interest, and time constraints during a challenging semester.

## Features

- **User Authentication**: JWT-based login and registration.
- **Recipe Management**: Create, display, and manage recipes with ingredient entries.
- **Ingredient Autosearch**: Reactive forms with autosuggestions and plural form generation checking.
- **Comments System**: Add comments to recipes.
- **Docker Integration**: Containerized setup with Docker Compose for easy deployment.
- **CI/CD**: Basic GitHub Actions workflow for build checks on push.

## Technologies Used

- **Backend**: C# ASP.NET Core, Entity Framework Core, Npgsql (PostgreSQL integration)
- **Frontend**: Angular (with reactive forms and services)
- **Database**: PostgreSQL
- **Authentication**: JWT (JSON Web Tokens)
- **Containerization**: Docker and Docker Compose
- **Environment Management**: .env files
- **CI/CD**: GitHub Actions

## Prerequisites

- Docker and Docker Compose installed
- Node.js and npm (for frontend development)
- .NET 8 SDK (for backend development)
- PostgreSQL (if running locally without Docker)

## Installation and Setup

1. **Clone the Repository**:
git clone https://github.com/Electron05/Recipe-Sharing-Web-App


2. **Environment Configuration**:
- Copy `.env.example` to `.env` (if provided) and set variables like `JWT_KEY`, `ConnectionStrings__DefaultConnection`, etc.
- For local development, update `backend/RecipeBay/appsettings.json` with your PostgreSQL connection string.

3. **Run with Docker** (Recommended):

docker-compose up --build

- This starts the PostgreSQL database, backend (on port 5000), and frontend (on port 4200).
- Migrations are applied automatically on backend startup.

4. **Local Development**:
- **Backend**: Navigate to `backend/RecipeBay`, run `dotnet ef database update`, then `dotnet run`.
- **Frontend**: Navigate to `frontend/RecipeBayFrontend`, run `npm install` and `ng serve`.

5. **Access the Application**:
- Frontend: http://localhost:4200
- Backend API: http://localhost:5000
- Swagger UI: http://localhost:5000/swagger/index.html

## Usage

- Register a new user or log in.
- Create recipes by adding ingredients (with autosearch and plural checking).
- View and comment on recipes.
- Explore user profiles.

## Project Structure

- `backend/RecipeBay/`: ASP.NET Core API with controllers, models, migrations, and services.
- `frontend/RecipeBayFrontend/`: Angular app with components, services, and types.
- `docs/`: DB ER diagram.
- `docker-compose.yml`: Docker setup.
- `.github/workflows/`: CI/CD pipeline for build checks.

## Lessons Learned

- **Database Schema Stability**: Frequent changes to the DB schema led to migration issues and wasted time. Plan schemas carefully upfront.
- **Project Engagement**: If a project isn't personally fascinating, motivation wanes—choose topics that excite you.
- **Time Management**: Balancing a hard semester with side projects requires realistic scoping.
- Overall, this was a valuable introduction to full-stack development, JWT auth, Docker, and CI/CD, but it highlighted the importance of focus and planning.

## Screenshots

<img width="844" height="635" alt="Zrzut ekranu 2026-01-12 204805" src="https://github.com/user-attachments/assets/2ce97536-4d11-4c6a-ada4-807bfd790c24" />
<img width="1466" height="646" alt="Zrzut ekranu 2026-01-12 204325" src="https://github.com/user-attachments/assets/3168a58f-0d7b-40ca-b449-874874561dc9" />
<img width="1240" height="631" alt="Zrzut ekranu 2026-01-12 204733" src="https://github.com/user-attachments/assets/e7b85564-6026-420d-97e2-4fbd796b8f43" />
<img width="1919" height="973" alt="Zrzut ekranu 2026-01-12 210900" src="https://github.com/user-attachments/assets/a66f776d-96a3-4f99-aea3-f7f2037ec0be" />


## Contributing

This project is archived and no longer maintained. Feel free to fork and experiment!

## License

MIT License (or specify if applicable).
