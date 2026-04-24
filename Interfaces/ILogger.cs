namespace CsEdgeSimulator.Interfaces;

using CsEdgeSimulator.Models;

public interface ILogger
{
    void WriteData(TelemetryData data);
}