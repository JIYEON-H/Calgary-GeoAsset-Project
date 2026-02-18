using System.Text.Json.Serialization;
namespace GeoAsset.Api.Models;

public class InspectionLog
{
    public int Id { get; set; }
    public int AssetId { get; set; }
    public DateTime InspectionDate { get; set; }
    public string InspectorName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    [JsonIgnore]
    public Asset? Asset { get; set; }
}
