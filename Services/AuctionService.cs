namespace AuctionPoc.Services;

using AuctionPoc.Data;
using AuctionPoc.Models;
using Microsoft.EntityFrameworkCore;

public class AuctionService
{
    private readonly AuctionDbContext _context;

    public AuctionService(AuctionDbContext context)
    {
        _context = context;
    }

    public async Task<List<AuctionProperty>> GetAllPropertiesAsync()
    {
        return await _context.AuctionProperties.ToListAsync();
    }

    public async Task<List<AuctionProperty>> FilterPropertiesAsync(string? propertyType, decimal? minPrice, decimal? maxPrice)
    {
        var query = _context.AuctionProperties.AsQueryable();

        if (!string.IsNullOrEmpty(propertyType) && propertyType != "All Types")
        {
            query = query.Where(p => p.PropertyType == propertyType);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.StartingBid >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.StartingBid <= maxPrice.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<string>> GetPropertyTypesAsync()
    {
        return await _context.AuctionProperties
            .Select(p => p.PropertyType)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync();
    }
}
