namespace CsEdgeSimulator.Models;

public class TelemetryData
{
    public int Temperature { get; set; }
    public string? WarningLevel { get; set; }
    public DateTime Timestamp { get; init; }

    public override string ToString()
    {
        return $"[{Timestamp}]{WarningLevel}: {Temperature}ºC";
    }
}