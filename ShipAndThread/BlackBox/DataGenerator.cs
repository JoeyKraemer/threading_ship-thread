using ShipAndThread.Domain.Enums;

namespace ShipAndThread.BlackBox;

using System;

public class DataGenerator
{
    private Random random = new Random();
    
    // Amsterdam bounding box coordinates (approximate)
    private readonly (double minLat, double maxLat, double minLon, double maxLon) amsterdamBounds = 
        (52.3000, 52.4300, 4.8000, 5.0000);

    public (double latitude, double longitude) GenerateCoordinates()
    {
        // Default to Amsterdam coordinates
        return GenerateCoordinatesInRegion(amsterdamBounds.minLat, amsterdamBounds.maxLat, 
                                          amsterdamBounds.minLon, amsterdamBounds.maxLon);
    }
    
    public (double latitude, double longitude) GenerateCoordinatesInRegion(
        double minLatitude, double maxLatitude, double minLongitude, double maxLongitude)
    {
        double latitude = minLatitude + (random.NextDouble() * (maxLatitude - minLatitude));
        double longitude = minLongitude + (random.NextDouble() * (maxLongitude - minLongitude));
        return (latitude, longitude);
    }

    public DateTime GenerateTimestamp()
    {
        return DateTime.Now;
    }
}