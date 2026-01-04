public class VehicleAddDto
{
    public required string PlateNumber { get; set; }
    public string? Model { get; set; }
    public required int BrandId { get; set; }
    public required int CustomerId { get; set; }
    public IFormFile? Image {get; set;} 

}