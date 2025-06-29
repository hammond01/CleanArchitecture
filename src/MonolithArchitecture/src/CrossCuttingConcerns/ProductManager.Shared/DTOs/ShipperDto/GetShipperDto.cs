namespace ProductManager.Shared.DTOs.ShipperDto;

public class GetShipperDto
{
    public string Id { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? Phone { get; set; }
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
