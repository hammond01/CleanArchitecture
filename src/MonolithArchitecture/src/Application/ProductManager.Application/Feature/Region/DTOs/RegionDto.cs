namespace ProductManager.Application.Feature.Region.DTOs;

public class RegionDto
{
    public string Id { get; set; } = string.Empty;
    public string RegionDescription { get; set; } = string.Empty;
    public string TestCode { get; set; } = string.Empty;
}

public class CreateRegionRequest
{
    public string RegionDescription { get; set; } = string.Empty;
    public string TestCode { get; set; } = string.Empty;
}

public class UpdateRegionRequest
{
    public string RegionDescription { get; set; } = string.Empty;
    public string TestCode { get; set; } = string.Empty;
}
