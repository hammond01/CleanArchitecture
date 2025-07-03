namespace ProductManager.Shared.DTOs.TerritoryDto;

public class GetTerritoryDto
{
    public string Id { get; set; } = default!;
    public string TerritoryDescription { get; set; } = default!;
    public string RegionId { get; set; } = default!;
    public string Name { get; set; } = default!;
    public DateTimeOffset CreatedDateTime { get; set; }
    public DateTimeOffset? UpdatedDateTime { get; set; }
}
