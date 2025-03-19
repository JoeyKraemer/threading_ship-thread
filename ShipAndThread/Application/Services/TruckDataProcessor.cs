using ShipAndThread.Application.DTOs;
using ShipAndThread.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class TruckDataProcessor
{
    private readonly AppDbContext _dbContext;

    public TruckDataProcessor(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}