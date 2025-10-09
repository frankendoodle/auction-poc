namespace AuctionPoc.Services;

using AuctionPoc.Models;

public class AuctionService
{
    private readonly List<AuctionProperty> _properties;

    public AuctionService()
    {
        _properties = GenerateSampleProperties();
    }

    public List<AuctionProperty> GetAllProperties()
    {
        return _properties;
    }

    public List<AuctionProperty> FilterProperties(string? propertyType, decimal? minPrice, decimal? maxPrice)
    {
        var filtered = _properties.AsEnumerable();

        if (!string.IsNullOrEmpty(propertyType) && propertyType != "All Types")
        {
            filtered = filtered.Where(p => p.PropertyType == propertyType);
        }

        if (minPrice.HasValue)
        {
            filtered = filtered.Where(p => p.StartingBid >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            filtered = filtered.Where(p => p.StartingBid <= maxPrice.Value);
        }

        return filtered.ToList();
    }

    public List<string> GetPropertyTypes()
    {
        return _properties.Select(p => p.PropertyType).Distinct().OrderBy(t => t).ToList();
    }

    private List<AuctionProperty> GenerateSampleProperties()
    {
        return new List<AuctionProperty>
        {
            new AuctionProperty
            {
                Id = 1,
                Title = "Former School",
                Address = "44,000 SF",
                City = "Oscoda",
                State = "MI",
                ZipCode = "48750",
                StartingBid = 1,
                PropertyType = "School",
                SquareFeet = 44000,
                Description = "Excellent opportunity for affordable housing",
                BiddingCloses = DateTime.Now.AddDays(5),
                ImageUrl = "/images/13167348_1.jpg",
                IsLive = true,
                HasVideo = true,
                Zoning = "Residential"
            },
            new AuctionProperty
            {
                Id = 2,
                Title = "Vacant Multi-Use Former Grocery",
                Address = "24,500 SF Building",
                City = "Prudenville",
                State = "MI",
                ZipCode = "48651",
                StartingBid = 150000,
                PropertyType = "Retail",
                SquareFeet = 24500,
                Acres = 2.45,
                Description = "Former grocery store with parking",
                BiddingCloses = DateTime.Now.AddDays(3),
                ImageUrl = "/images/MGM1.jfif",
                IsLive = true,
                HasVideo = true,
                Zoning = "Commercial"
            },
            new AuctionProperty
            {
                Id = 3,
                Title = "Taylor Road Industrial",
                Address = "L-1, Light Industrial Zoning",
                City = "Auburn Hills",
                State = "MI",
                ZipCode = "48326",
                StartingBid = 25000,
                PropertyType = "Industrial",
                SquareFeet = 0,
                Acres = 19.84,
                Description = "19.84 Acres",
                BiddingCloses = DateTime.Now.AddDays(7),
                ImageUrl = "/images/03d989b5.jpg",
                IsLive = true,
                HasVideo = false,
                Zoning = "L-1, Light Industrial"
            },
            new AuctionProperty
            {
                Id = 4,
                Title = "The Vegas Motel",
                Address = "170 Rooms",
                City = "Minot",
                State = "ND",
                ZipCode = "58703",
                StartingBid = 600000,
                PropertyType = "Hotel",
                SquareFeet = 335385,
                Description = "335,385 SF Building • 15,000+ VPD • 2315 N Broadway",
                BiddingCloses = DateTime.Now.AddDays(4),
                ImageUrl = "/images/Capture.JPG",
                IsLive = true,
                HasVideo = true,
                Zoning = "Commercial"
            },
            new AuctionProperty
            {
                Id = 5,
                Title = "5.3 Acres of Commercially Zoned Freeway Frontage",
                Address = "5.3 Acres",
                City = "Grand Prairie",
                State = "TX",
                ZipCode = "75062",
                StartingBid = 750000,
                PropertyType = "Land",
                Acres = 5.3,
                Description = "1100 I-20",
                BiddingCloses = DateTime.Now.AddDays(2),
                ImageUrl = "/images/Capture2.JPG",
                IsLive = true,
                HasVideo = true,
                IsOpportunityZone = true,
                Zoning = "Commercial"
            },
            new AuctionProperty
            {
                Id = 6,
                Title = "Former Markham Animal Clinic",
                Address = "3441-3451 W 159th St",
                City = "Markham",
                State = "IL",
                ZipCode = "60428",
                StartingBid = 50000,
                PropertyType = "Medical",
                SquareFeet = 0,
                Description = "High visibility in a desirable area",
                BiddingCloses = DateTime.Now.AddDays(6),
                ImageUrl = "/images/1.JPG",
                IsLive = true,
                HasVideo = true,
                IsOpportunityZone = true,
                Zoning = "Commercial"
            },
            new AuctionProperty
            {
                Id = 7,
                Title = "73 +/- Acres | Crowley, TX",
                Address = "Over 220 Lots",
                City = "Crowley",
                State = "TX",
                ZipCode = "76036",
                StartingBid = 500000,
                PropertyType = "Land",
                Acres = 73,
                Description = "N Trail St",
                BiddingCloses = DateTime.Now.AddDays(8),
                ImageUrl = "/images/2.JPG",
                IsLive = true,
                HasVideo = false,
                Zoning = "Residential"
            },
            new AuctionProperty
            {
                Id = 8,
                Title = "113 Carnegie St",
                Address = "13.2 Acres",
                City = "Greenwood",
                State = "MS",
                ZipCode = "38930",
                StartingBid = 300000,
                PropertyType = "Industrial",
                Acres = 13.2,
                Description = "3 Parcels",
                BiddingCloses = DateTime.Now.AddDays(5),
                ImageUrl = "/images/T2JPG.JPG",
                IsLive = true,
                HasVideo = true,
                Zoning = "Industrial"
            }
        };
    }
}
