namespace CsEdgeSimulator;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CsEdgeSimulator.Interfaces;
using CsEdgeSimulator.Models;
using CsEdgeSimulator.Services;

class Program
{
    // OLD: private static Random _random = new Random();
    // NEW (Clean Code): 'readonly' prevents the variable from being accidentally overwritten later. 
    // Target-typed 'new()' (from C# 9) makes it more concise since the type is already declared on the left.
    private static readonly Random _random = new();

    static async Task Main(string[] args)
    {
        Console.WriteLine("Starting Main.");
        ITelemetrySender telemetrySender = new NetworkSender();
        
        // OLD: FileLogger fileLogger = new FileLogger();
        // OLD: List<TelemetryData> buffer = new List<TelemetryData>();
        // NEW (Clean Code): Again using 'new()' to reduce visual noise.
        FileLogger fileLogger = new();
        // Queue instead of List for FIFO clearing.
        Queue<TelemetryData> buffer = new();
        
        // OLD: int lostConnections = 0;
        // NEW (Clean Code): Semantics. An integer suggests we want to count and calculate. 
        // But we only need to know a state (Yes/No). Therefore, a bool is used.
        bool wasDisconnected = false; 

        // _ Discard variable
        _ = Task.Run(ListenForCloudCommands);

        while (true)
        {
            TelemetryData data = GenerateTelemetryData();
            
            // OLD: bool isConnected = ConnectionStatus();
            // NEW (Clean Code): Intention-Revealing Name. A Yes/No variable should sound like a question.
            bool isConnected = IsConnected();
            
            // ------------------------------------------------------------------
            // CLEAN CODE CORE: Early Exit / Guard Clause instead of nesting
            // ------------------------------------------------------------------
            // We check the edge case (offline) first and break out early.
            if (!isConnected)
            {
                Console.WriteLine($"Connection lost: [STORING] {data}");
                buffer.Enqueue(data);
                fileLogger.WriteData(data);
                
                // OLD: lostConnections++;
                wasDisconnected = true;
                
                await Task.Delay(2000);
                
                // NEW (Clean Code): 'continue' instantly jumps back to the beginning of the while loop.
                // This protects the entire code below and saves us from wrapping it in an "if (isConnected)".
                continue; 
            }

            // --- From here on, thanks to the 'continue' above, it is 100% guaranteed 
            // that we are ONLINE. No huge if-blocks needed anymore! ---

            // OLD: if (lostConnections != 0)
            if (wasDisconnected)
            {
                Console.WriteLine("Connected!");
                // OLD: lostConnections = 0;
                wasDisconnected = false;
            }

            // Clear buffer if it contains any data
            if (buffer.Any())
            {
                while (buffer.Count > 0)
                {
                    // OLD: telemetrySender.Send("[STORED]" + line.ToString());
                    // NEW (Clean Code): String Interpolation ($"...") is often more readable than string concatenation using the plus sign.
                    try
                    {
                        TelemetryData storedData = buffer.Dequeue();
                        telemetrySender.Send($"[STORED] {storedData}");
                    }
                    catch (IOException e)
                    {
                        
                        Console.WriteLine($"{new string('#', 20)} \nCouldn't return stored data."); 
                        Console.WriteLine(e.Message);
                        Console.WriteLine($"Will be added into next batch.\n{new string('#', 20)}");
                        break;
                    }
                }
            }

            // Send current live data
            fileLogger.WriteData(data);
            try
            {
                telemetrySender.Send($"{data.WarningLevel}: {data.Temperature}");
            }
            catch (IOException e)
            {
                Console.WriteLine($"{new string('#', 20)} \nCouldn't send via VPN.");
                Console.WriteLine(e.Message); 
                Console.WriteLine($"Data is stored and will be added into next batch.\n{new string('#', 20)}");
            }
            
            await Task.Delay(2000);
        }
    }
    
    private static TelemetryData GenerateTelemetryData()
    {
        int currentTemp = _random.Next(20, 110);
        
        // OLD: 
        // TelemetryData data = new TelemetryData() {Timestamp = DateTime.Now};
        // data.Temperature = currentTemp;
        // if (data.Temperature <= 60) { data.WarningLevel = "Temperature stable"; }
        // else if (data.Temperature >= 60 && data.Temperature <= 95) { ... }
        // return data;
        
        // NEW (Clean Code): Single Responsibility Principle (SRP). 
        // This method ONLY constructs the object. The logic for evaluating the 
        // temperature warning has been extracted into its own method (EvaluateTemperatureWarning).
        return new TelemetryData() 
        { 
            Timestamp = DateTime.Now,
            Temperature = currentTemp,
            WarningLevel = EvaluateTemperatureWarning(currentTemp)
        };
    }

    // NEW (Clean Code): The extracted logic.
    // A bug fix: The old "else if (>= 60)" overlapped with "<= 60". 
    // By using early returns, we don't need any "else" blocks at all.
    private static string EvaluateTemperatureWarning(int temperature)
    {
        if (temperature < -50) return "Temperature can't be this low.";
        if (temperature < 60) return "Temperature stable";
        if (temperature <= 95) return "Temperature HIGH";
        
        return "[WARNING] Temperature critical";
    }

    // OLD: private static bool ConnectionStatus()
    // NEW (Clean Code): The name is now a clear question.
    private static bool IsConnected()
    {
        // OLD: 
        // int status = _random.Next(1, 6);
        // return status != 5;
        // NEW (Clean Code): We can return the expression directly without an intermediate variable.
        return _random.Next(1, 6) != 5;
    }

    private static async Task ListenForCloudCommands()
    {
        while (true)
        {
            if (_random.Next(1, 300) == 1)
            {
                Console.WriteLine("\n[RECEIVED NEW COMMAND.]");
            }
            await Task.Delay(100);
        }
    }
}