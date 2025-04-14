# Ship & Thread 

Ship & Thread is a simple program with a simple UI that deals with cargo distribution and tracking for trucks transporting goods.

## Background
This project is a NHL Stenden student project for the course 3.3 Threading in C# to learn the concepts of threading in C#.

### Project Team
- Caterina  Aresti
- Sebastian Güntzel
- Joey Krämer
- Nicanor Martinez

## Threading concepts applied in the project

The following threading concepts are implemented in this cargo distribution and tracking system:

1. **Thread Pool** – Used for efficient handling of multiple truck simulations in parallel. Instead of creating new threads for each truck, the system leverages the ThreadPool via `Task.Run()` to manage reusable worker threads, reducing overhead and improving performance. This is implemented in the `RunSimulationAsync` method of the `TruckDataGenerator` class.

2. **Semaphore** – Implemented using `SemaphoreSlim` to limit the number of concurrent truck simulations based on system capabilities. This prevents resource exhaustion by controlling how many threads from the ThreadPool can execute simultaneously. The semaphore is configured to use a maximum number of threads equal to the minimum of the truck count and twice the processor count.

3. **Async & Await** – Extensively used throughout the application for non-blocking operations, such as database access, network communications, and UI responsiveness. This pattern allows the application to perform I/O-bound work without blocking threads, making efficient use of system resources. All service methods use async/await to ensure the application remains responsive.

4. **Asynchronous I/O** – Implemented for handling real-time communication with trucks and updating the UI via SignalR. The application uses asynchronous I/O operations for database access, network communications, and file operations, allowing it to handle multiple operations concurrently without blocking the main thread.

## Core Functionalities
- Truck communication handling
- Real-time dashboard updates
- Cargo assignment management
- Event and Incident handling*

## How to Run

1. Clone the repository
2. Open the solution in IDE
3. Install Packages: 
```
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.InMemory
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```
4. Build the solution
5. Run the solution
6. Start application

### Troubleshooting
If the browser doesn't open automatically:
- Open your browser and navigate to `https://localhost:5001` or `http://localhost:5000`
- If these ports are already in use, check the console output for the actual port being used
- You may see a message like: `Now listening on: http://localhost:<port>` in the console
- For HTTPS connections, you might need to accept the self-signed certificate

*niceToHaves
