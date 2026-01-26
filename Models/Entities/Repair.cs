using System.Text.Json.Serialization;

public class Repair
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int RepairCategoryId { get; set; }
    public RepairCategory RepairCategory { get; set; } = null!;
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public bool IsActive { get; set; }
    
    [JsonIgnore]
    public ICollection<VehicleRepair> VehicleRepairs = new List<VehicleRepair>();
}