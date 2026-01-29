public class VehicleRepairDto
{
    public int Id { get; set; }
    public VehicleDto? Vehicle {get; set;}
    public RepairDto? Repair {get; set;}
    public DateTime RepairedDate {get; set;}
    public uint? SpeedoMeter { get; set; }
    public string? Note {get; set;}

}