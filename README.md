# IoT Edge Telemetry Simulator

![C#](https://img.shields.io/badge/C%23-Modern-239120?logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-Console_App-512BD4?logo=dotnet)
![Clean Code](https://img.shields.io/badge/Focus-Clean_Code-success)

> **Project Goal:** A lightweight .NET Console Application built to simulate an IoT Edge device. The primary focus of this project is to demonstrate robust handling of volatile network connections (offline buffering) and the practical application of **Clean Code principles** (Guard Clauses, Single Responsibility Principle, and meaningful naming).

## 🏗️ System Architecture & Features

The simulator runs an asynchronous, infinite loop generating telemetry data while dynamically simulating network drops and reconnects.

* **Network Resilience & Buffering:** Simulates an unstable VPN/Network connection. If the connection drops, telemetry data is securely stored in a local FIFO `Queue<T>`.
* **Batch Flushing:** Once the connection is restored, the simulator automatically flushes the stored offline buffer to the backend before resuming live data transmission.
* **Asynchronous Processing:** Utilizes C# `async/await` and `Task.Run()` to listen for incoming cloud commands in the background without blocking the main telemetry generation loop.
* **Clean Code Focus:** * Replaced deeply nested `if/else` statements with **Guard Clauses / Early Exits**.
  * Enforced the **Single Responsibility Principle (SRP)** by extracting warning-level evaluations into dedicated methods.
  * Use of modern C# features like target-typed `new()`, string interpolation, and read-only fields.

## 🛠️ Tech Stack

* **Language:** C# (Modern Syntax)
* **Framework:** .NET (Console Application)
* **Concepts:** Asynchronous Programming, FIFO Queues, Clean Code, Exception Handling

## 🚀 How to Run

Since this is a standard .NET Console Application, you only need the .NET SDK installed on your machine.

### 1. Clone & Run
Open your terminal in the project directory and execute:
```bash
dotnet run
