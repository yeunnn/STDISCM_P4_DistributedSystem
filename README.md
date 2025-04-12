# STDISCM_P4_DistributedSystem

Distributed systems allow for fault tolerance.  For this exercise you are tasked to create an online enrollment system with different services distributed across multiple nodes.

The system should have the following bare-minimum features:

1) Login/logout. Track sessions across different nodes. (Use OAuth / JWT)
2) View available courses.
3) Students to enroll to open courses.
4) Students to view previous grades.
5) Faculty to be able to upload grades.

The application should be web based using MVC.  The view will be a node on its own.  While the rest of the features / API / controllers will also be on a separate node.
Use of networked virtual machines/bare-metal computers is recommended.

When a node is down, on the features supported by that node should stop working, but the rest of the application should still work.

## Slides
https://docs.google.com/presentation/d/1jRAXIWoUqALTKpbHmSpHNAFz2he88n4_/edit?usp=sharing&ouid=101928301120544085464&rtpof=true&sd=true

## Dependencies and Prerequisites

- **.NET 6 or higher**
  - Install the .NET 6 SDK and Runtime on both the host machine and the VM

- **SQLite (included with EF Core)**
  - No external setup needed; database is created automatically

- **Visual Studio 2022**
  - With workloads for ".NET desktop development" and ".NET and web development"

- **Oracle VirtualBox (latest version)**
  - Setup a Windows 11 virtual machine
  - On the VM, install Visual Studio with the same configuration

- **Network Configuration**
  - Configure the VM’s network as a "Bridged Adapter" to obtain an IP address accessible from the host

## Repository Structure

The repository is organized into individual Visual Studio projects:

distributed-enrollment-dotnet\  
├── Frontend\        → MVC UI (host machine, port 8080)  
├── Broker\          → API Gateway middleware (host machine, port 3000)  
├── AuthService\     → Handles authentication/login (VM, port 4000)  
└── CourseService\   → Manages courses, enrollments, grades (VM, port 5000)

## Installing

1. **Clone the Repository**

   Clone the repository to both your host and virtual machine environments.

2. **Restore NuGet Packages**

   Open the solution in Visual Studio on each machine (host and VM) and allow NuGet to restore packages.

3. **Ensure Launch Ports are Correct**

   Check each project’s `launchSettings.json` to verify the ports:
   - Frontend → 8080
   - Broker → 3000
   - AuthService → 4000
   - CourseService → 5000

## Executing the Program

### Overview

- **Host Machine:**
  - Run `Frontend` (port 8080)
  - Run `Broker` (port 3000)

- **Virtual Machine (VM):**
  - Run `AuthService` (port 4000)
  - Run `CourseService` (port 5000)

### Instructions

1. **Set Up the Virtual Machine**

   - Install Oracle VirtualBox and set up a Windows 11 VM
   - Install Visual Studio 2022 with required workloads
   - Clone the repository

   - Open the solution and go to the `launchSettings.json` of each service (Auth and Course) then change the applicationUrl under the "http" section with the IP of the Virtual Machine:
     - For Auth-service
     - `applicationUrl` → `http://0.0.0.0:4000`
     - For Course-service
     - `applicationUrl` → `http://0.0.0.0:5000`

2. **Set Up the Host Machine**

   - On your host, open the solution in Visual Studio
   - In `Frontend/appsettings.json`, configure the Broker URL:
     ```json
     "BrokerUrl": "http://localhost:3000/api"
     ```

   - In `Broker/appsettings.json`, configure the backend endpoints using the VM's IP:
     ```json
     "ServiceEndpoints": {
         "AuthService": "http://192.168.100.64:4000",
         "CourseService": "http://192.168.100.64:5000"
     }
     ```
     Replace with your actual VM IP (use `ipconfig` inside VM to find it).

3. **Run the System**

   - To initialize in both your Visual Studio in the host and VM, set multiple startup projects:
     - Frontend
     - Broker
     - Auth Service
     - Course Service
   - Set the action to 'Start' and Debug target to 'http'
   - Press `F5` or click **Start Debugging**

   - Then, run each service manually using the command line to better control and monitor the processes.
     - On the host machine open two terminals for both the broker and frontend:
     ```
     cd distributed-enrollment-dotnet\Frontend
     dotnet run

     cd distributed-enrollment-dotnet\CourseService
     dotnet run
     ```
     - On the virtual machine open two terminals for both the auth and course:
     ```
     cd distributed-enrollment-dotnet\AuthService
     dotnet run

     cd distributed-enrollment-dotnet\CourseService
     dotnet run
     ```
   - Watch the console output to confirm that each service is listening on its designated port (Frontend: 8080, Broker: 3000, AuthService: 4000, CourseService: 5000).

4. **Using the Application**

   - Open your browser on the host and go to `http://localhost:8080`
   - Use the login page:
     - **Student:** `student1` / `password`
     - **Faculty:** `faculty1` / `password`

   - Features:
     - Students:
       - View available courses
       - Enroll in open courses
       - View grades
     - Faculty:
       - Upload grades
       - View detailed student info

   - Dummy courses:
     - `CSE101`: Introduction to Computer Science
     - `CSE102`: Data Structures
     - `CSE103`: Distributed Systems

5. **Simulating Fault Tolerance**

   - To simulate failure, stop a service in the VM (e.g., close AuthService)
   - On the host, test related functionality (e.g., try logging in → fails)
   - Unaffected features (e.g., dashboard navigation, course views) continue working

   - Example Error Handling (Frontend):
     ```html
     <p style="color:red;">Error: Course service is down</p>
     ```

   - Restart the service to restore functionality

## Notes

- SQLite database files (`*.db`) are created automatically on first run.
- You may delete them to reset to the initial dummy data state.
- Each project handles its own database and seeding logic via Entity Framework Core.
