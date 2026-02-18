namespace GeoAsset.Api.Models;

public class Asset
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? LastUpdated { get; set; }

    public ICollection<InspectionLog> InspectionLogs { get; set; } = new List<InspectionLog>();
}
