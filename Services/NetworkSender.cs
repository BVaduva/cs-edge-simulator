namespace CsEdgeSimulator.Services;

using CsEdgeSimulator.Interfaces;

class NetworkSender : ITelemetrySender
{
    private static readonly Random _random = new();
    public void Send(string data)
    {
        if (_random.Next(1, 11) == 1)
        {
            throw new IOException("[SOCKET ERROR]");
        }

        Console.WriteLine($"Send via VPN: {data}");
    }
}