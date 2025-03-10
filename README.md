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

Out of the list of threading concepts, these are four that are applied to a cargo distribution and tracking system:

1. **Thread Pool** – Useful for handling multiple background tasks efficiently, such as updating GPS locations for multiple trucks. Instead of creating new threads for each update, a thread pool manages reusable worker threads, reducing overhead.

2. **Mutex** – Ideal for ensuring data integrity when multiple threads access shared resources, such as updating cargo inventory. If multiple processes attempt to modify the same cargo record (e.g., arrival confirmation), a mutex ensures only one thread modifies it at a time.

3. **Async & Await** (Task Parallel Library) – Helps with non-blocking operations, such as fetching data from remote servers or handling user requests in the UI. For instance, when querying truck statuses, async/await ensures the UI remains responsive while waiting for data.

4. **Asynchronous I/O** - Handling real-time communication with trucks over the network without blocking threads. For example, the server uses asynchronous I/O to listen and respond to updates without blocking other operations, allowing trucks to send location updates every few seconds.

## Core Functionalities
- Truck communication handling
- Real-time dashboard updates
- Cargo assignment management
- Event and Incident handling*


*niceToHaves
