public class VehicleDto
{
    public int Id { get; set; }
    public required string PlateNumber { get; set; }
    public string? Model { get; set; }
    public string? ImageUrl { get; set; }
    public BrandDto? Brand { get; set; }
    public CustomerDto? Customer { get; set; }

}