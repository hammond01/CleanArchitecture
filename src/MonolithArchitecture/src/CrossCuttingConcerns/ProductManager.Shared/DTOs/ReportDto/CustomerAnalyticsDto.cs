namespace ProductManager.Shared.DTOs.ReportDto;

public class CustomerAnalyticsDto
{
    public DateTime PeriodFrom { get; set; }
    public DateTime PeriodTo { get; set; }
    public int TotalCustomers { get; set; }
    public int ActiveCustomers { get; set; }
    public List<TopCustomerDto> TopCustomers { get; set; } = new();
    public List<CustomerCountryDto> CustomersByCountry { get; set; } = new();
}

public class TopCustomerDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSpent { get; set; }
}

public class CustomerCountryDto
{
    public string Country { get; set; } = string.Empty;
    public int CustomerCount { get; set; }
}
