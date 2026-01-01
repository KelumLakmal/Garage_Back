public class VehicleUpdateDto
{
    public required string PlateNumber { get; set; }
    public string? Model { get; set; }
    public int BrandId { get; set; }
    public int CustomerId { get; set; }

}