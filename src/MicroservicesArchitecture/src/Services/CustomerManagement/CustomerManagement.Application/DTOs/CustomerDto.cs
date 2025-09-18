namespace CustomerManagement.Application.DTOs;

public class CustomerDto
{
  public string Id { get; set; } = null!;
  public string CompanyName { get; set; } = null!;
  public string? ContactName { get; set; }
  public string? ContactTitle { get; set; }
  public string? Address { get; set; }
  public string? City { get; set; }
  public string? Region { get; set; }
  public string? PostalCode { get; set; }
  public string? Country { get; set; }
  public string? Phone { get; set; }
  public string? Fax { get; set; }
  public string? Email { get; set; }
  public DateTimeOffset CreatedDateTime { get; set; }
  public DateTimeOffset? UpdatedDateTime { get; set; }
}
