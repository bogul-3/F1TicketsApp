# 🏎️ F1Tickets App

A full-stack Formula 1 ticket booking and management platform built with modern web technologies and a modular, decoupled architecture.

---

## 🛠️ Tech Stack

### Backend
* **Language & Framework:** C# / .NET (ASP.NET Core Web API)
* **Architecture:** Microservices / Modular Solution (`F1Tickets.Api`, `F1Tickets.Reporting.Api`, `F1Tickets.Worker`)
* **Message Broker:** RabbitMQ (`rabbitmq:3-management`)
* **Caching:** Redis Cache (`redis:7`)
* **Database:** PostgreSQL 16 (Containerized)

### Frontend
* **Library:** React
* **Language:** TypeScript
* **Styling/Build:** Modern component-driven structure

---

## 🏗️ Project Architecture

The solution consists of three distinct backend services and a React frontend:

1. **`F1Tickets.Api`**: Core API handling user requests, race browsing, ticket reservations, and cancellations.
2. **`F1Tickets.Worker`**: Background worker service that processes asynchronous tasks and messaging via RabbitMQ.
3. **`F1Tickets.Reporting.Api`**: Dedicated reporting service with its own reporting database, restricted exclusively to users with the **Admin** role.
4. **`frontend`**: React + TypeScript client application communicating with the backend APIs.

---

## 🚀 Getting Started

### Prerequisites
* [.NET SDK](https://dotnet.microsoft.com/)
* [Node.js & npm](https://nodejs.org/)
* [Docker & Docker Compose](https://www.docker.com/)
* [Visual Studio](https://visualstudio.microsoft.com/) or your preferred C# IDE

### 1. Clone the Repository
```bash
git clone https://github.com/your-username/f1tickets.git
cd f1tickets
```

### 2. Start Infrastructure (Docker)
In the root directory of the project, run the following command to start PostgreSQL, RabbitMQ, and Redis:

```bash
docker compose up -d
```

This spins up the following services:
* **PostgreSQL:** Port `5433` (mapped to container port `5432`, DB: `f1tickets`)
* **RabbitMQ Management UI:** Port `5672` (AMQP) & `15672` (Web UI)
* **Redis Cache:** Port `6379`

### 3. Run the Backend (Visual Studio)
1. Open the solution file (`.sln`) in **Visual Studio**.
2. Configure multiple startup projects (`F1Tickets.Api`, `F1Tickets.Worker`, `F1Tickets.Reporting.Api`).
3. Press **F5** or click **Start** to run the backend services.

### 4. Run the Frontend
Navigate to the frontend directory, install dependencies, and start the development server:

```bash
cd frontend
npm install
npm run dev
```
