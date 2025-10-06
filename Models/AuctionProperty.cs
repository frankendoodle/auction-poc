namespace AuctionPoc.Models;

public class AuctionProperty
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public decimal StartingBid { get; set; }
    public string PropertyType { get; set; } = string.Empty;
    public double SquareFeet { get; set; }
    public double? Acres { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime BiddingCloses { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsLive { get; set; }
    public bool HasVideo { get; set; }
    public bool IsOpportunityZone { get; set; }
    public string Zoning { get; set; } = string.Empty;
}
