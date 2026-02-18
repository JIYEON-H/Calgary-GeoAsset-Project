# 📍 GeoAsset Calgary
**Energy Asset Management & Inspection Tracking System**

A full-stack web application designed to manage and visualize energy asset data in the Calgary area.  
This project features a robust .NET backend, a modern React frontend, and is fully containerized using Docker.

---

## 🚀 Tech Stack

### Backend
- **Framework:** .NET 8.0 (ASP.NET Core Web API)
- **Database:** SQL Server (via Entity Framework Core)
- **API Documentation:** Swagger (Swashbuckle)

### Frontend
- **Library:** React 18 (TypeScript)
- **Build Tool:** Vite
- **Styling:** CSS Modules

### Infrastructure
- **Containerization:** Docker & Docker Compose
- **Orchestration:** Multi-container setup (API, Client, DB)

---

## 📊 Key Features

### **1. Interactive Asset Mapping**
* **📍 Geospatial Visualization:** View and track energy assets across the Calgary area on an integrated map.
* **⚡ Instant Navigation:** Click on any asset icon to instantly pan the map to its location and display detailed metadata.

### **2. Inspection Management System**
* **📝 Inspection Logging:** Seamlessly add, update, and manage maintenance records for each asset to ensure safety standards.
* **🔄 Real-time Data Sync:** Experience high-speed data synchronization between the React frontend and SQL Server backend via the .NET API.

### **3. Professional Developer Workflow**
* **🐳 One-Click Setup:** Launch the entire full-stack environment (Frontend, Backend, MSSQL) instantly with Docker Compose.
* **🛡️ End-to-End Type Safety:** Robust development using TypeScript on the frontend and C# on the backend.

---

## 🖼️ Screenshots

### **Main Dashboard & Asset Map**
<img width="1528" height="833" alt="Screenshot 2026-02-17 at 11 38 21 PM" src="https://github.com/user-attachments/assets/1974174a-c4bc-486b-ac4c-93a528cba06e" />

*Calgary-based energy asset visualization with interactive markers.*

### **Inspection Management**
<img width="749" height="740" alt="Screenshot 2026-02-17 at 11 41 47 PM" src="https://github.com/user-attachments/assets/314545aa-67d4-404d-b6d8-f3dad5b784e6" />

*Adding and tracking maintenance logs for specific assets.*

---

## 🛠️ Getting Started

### Prerequisites
- Docker Desktop installed
- .NET 8 SDK (for local development)

### Installation & Execution

1. Clone the repository:
```bash
git clone https://github.com/JIYEON-H/Calgary-GeoAsset-Project.git
cd geoasset-project
```

2. Run with Docker Compose:
```bash
docker compose up --build
```

3. Access Endpoints

| Service | URL |
|---|---|
| Frontend (React) | http://localhost:3000 |
| Backend API | http://localhost:5140 |
| Swagger UI | http://localhost:5140/swagger |
