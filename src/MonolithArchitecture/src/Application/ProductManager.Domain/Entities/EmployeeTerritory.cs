namespace ProductManager.Domain.Entities;

public class EmployeeTerritory
{
    public string EmployeeId { get; set; } = null!;
    public Employees Employees { get; set; } = null!;
    public string TerritoryId { get; set; } = null!;
    public Territory Territory { get; set; } = null!;
}
