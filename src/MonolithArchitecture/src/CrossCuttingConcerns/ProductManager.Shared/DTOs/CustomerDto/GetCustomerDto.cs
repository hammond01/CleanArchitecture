namespace ProductManager.Shared.DTOs.CustomerDto;

public class GetCustomerDto
{
    public string Id { get; set; } = default!;
    public string CompanyName { get; set; } = default!;
    public string? ContactName { get; set; }
    public string? ContactTitle { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; }
    public string? Phone { get; set; }
    public string? Fax { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
