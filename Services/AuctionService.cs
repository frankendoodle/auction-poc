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

    public async Task<List<AuctionProperty>> FilterPropertiesAsync(string? propertyType, decimal? minPrice, decimal? maxPrice, string? searchTerm = null)
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

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(p =>
                (p.Title != null && p.Title.ToLower().Contains(term)) ||
                (p.Address != null && p.Address.ToLower().Contains(term)) ||
                (p.City != null && p.City.ToLower().Contains(term)) ||
                (p.State != null && p.State.ToLower().Contains(term)) ||
                (p.ZipCode != null && p.ZipCode.ToLower().Contains(term)) ||
                (p.Description != null && p.Description.ToLower().Contains(term)) ||
                (p.PropertyType != null && p.PropertyType.ToLower().Contains(term))
            );
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
