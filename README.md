
# Employee Management Microservices

## Codveda Technologies Internship – Level 3, Task 1

### Project Overview
This project demonstrates a microservices architecture using ASP.NET Core Web API and .NET 9.

The system is divided into independent services that communicate through an API Gateway and RabbitMQ messaging.

### Architecture

The solution contains three main applications:

- **EmployeeService:** Provides employee CRUD operations.
- **DepartmentService:** Provides department operations and consumes employee-created events.
- **ApiGateway:** Uses YARP Reverse Proxy to route incoming requests to the appropriate service.

### Technologies Used

- C# and .NET 9
- ASP.NET Core Web API
- YARP Reverse Proxy
- .NET Service Discovery
- RabbitMQ 4
- Docker and Docker Compose
- Kubernetes
- Swagger / OpenAPI

### Inter-Service Communication

RabbitMQ is used for asynchronous communication.

When a new employee is created:

1. EmployeeService saves the employee in its in-memory collection.
2. EmployeeService publishes an `EmployeeCreatedEvent` to RabbitMQ.
3. DepartmentService consumes the event.
4. DepartmentService displays the received employee information in its logs.

### API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/employees | Get all employees |
| GET | /api/employees/{id} | Get employee by ID |
| POST | /api/employees | Create employee |
| PUT | /api/employees/{id} | Update employee |
| DELETE | /api/employees/{id} | Delete employee |
| GET | /api/departments | Get all departments |
| GET | /api/departments/{id} | Get department by ID |
| POST | /api/departments | Create department |
| DELETE | /api/departments/{id} | Delete department |

### Running with Docker Compose

From the solution root directory, run:

```bash
docker compose up -d --build
```

Check running containers:

```bash
docker compose ps
```

Stop the containers:

```bash
docker compose down
```

### Local Docker URLs

- API Gateway: http://localhost:5151
- EmployeeService Swagger: http://localhost:5240/swagger
- DepartmentService Swagger: http://localhost:5009/swagger
- RabbitMQ Management: http://localhost:15672

RabbitMQ credentials:
- Username: guest
- Password: guest

### Running with Kubernetes

Apply the Kubernetes configuration:

```bash
kubectl apply -f Kubernetes/microservices.yaml
```

Check pods:

```bash
kubectl get pods
```

Check services:

```bash
kubectl get services
```

Access the API Gateway through port forwarding:

```bash
kubectl port-forward service/gateway 8081:8080
```

Then open:

http://localhost:8081/api/employees

### Important Implementation Notes

- Employee and department data are currently stored in static in-memory collections.
- RabbitMQ provides asynchronous event-based communication.
- YARP acts as the API Gateway.
- Kubernetes Deployments and Services manage the application containers and internal communication.
- This project is a learning demonstration and does not currently include persistent database storage.

### Internship
Codveda Technologies – .NET Development Internship

Level 3 – Task 1: Microservices Architecture with .NET
