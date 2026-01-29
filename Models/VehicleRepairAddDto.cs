public class VehicleRepairAddDto
{
    public int VehicleId {get; set;}
    public uint SpeedoMeter {get; set;}
    public ICollection<VehicleRepairInfoDto> RepairsInfos {get; set;} = new List<VehicleRepairInfoDto>();

}