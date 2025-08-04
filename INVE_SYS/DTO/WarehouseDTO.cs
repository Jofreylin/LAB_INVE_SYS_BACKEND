namespace INVE_SYS.DTO
{
    public class WarehouseDTO
    {
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
    }

    public class WarehouseResponse
    {
        public int? WarehouseId { get; set; }
        public DateTime? ExecutedAt { get; set; }
    }
}
