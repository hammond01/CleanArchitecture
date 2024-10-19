namespace ProductManager.Domain.Entities;

public class EmployeeTerritory
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = null!;

    public string TerritoryId { get; set; } = null!;
    public Territory Territory { get; set; } = null!;
}
