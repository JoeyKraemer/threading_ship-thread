namespace ShipAndThread.BlackBox;

using System;

public class DataGenerator
{
    private Random random = new Random();

    public (double latitude, double longitude) GenerateCoordinates()
    {
        double latitude = random.NextDouble() * 180 - 90;
        double longitude = random.NextDouble() * 360 - 180;
        return (latitude, longitude);
    }

    public DateTime GenerateTimestamp()
    {
        return DateTime.Now;
    }

    public string GenerateStatus()
    {
        string[] statuses = { "ON ROUTE", "DELIVERED" };
        return statuses[random.Next(statuses.Length)];
    }
}