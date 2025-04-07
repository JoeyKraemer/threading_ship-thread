using ShipAndThread.Domain.Enums;

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
}