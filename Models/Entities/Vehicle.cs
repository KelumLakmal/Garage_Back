using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public required string PlateNumber { get; set; }
    public string? Model { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    public string? ImagePath { get; set; }
    // foreign key
    public int BrandId { get; set; }
    // navigation property
    public Brand Brand { get; set; } = null!;
    public int CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    [JsonIgnore]
    public ICollection<VehicleRepair> VehicleRepairs = new List<VehicleRepair>();

}