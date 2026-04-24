namespace CsEdgeSimulator.Services;

using CsEdgeSimulator.Interfaces;
using CsEdgeSimulator.Models;

public class FileLogger : ILogger
{
    public void WriteData(TelemetryData data)
    {
        string text = $"[{data.Timestamp}] {data.WarningLevel}: {data.Temperature}ºC";

        // Dispose after usage
        using (StreamWriter writer = new StreamWriter("log.txt", true))
        {
            writer.WriteLine(text);
        }
    }
}