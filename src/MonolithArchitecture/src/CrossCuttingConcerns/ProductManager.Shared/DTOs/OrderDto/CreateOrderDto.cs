namespace ProductManager.Shared.DTOs.OrderDto;

public class CreateOrderDto
{
    public string? CustomerId { get; set; }

    public string? EmployeeId { get; set; }

    public DateTimeOffset? OrderDate { get; set; }

    public DateTimeOffset? RequiredDate { get; set; }

    public DateTimeOffset? ShippedDate { get; set; }

    public string? ShipVia { get; set; }

    public decimal? Freight { get; set; }

    public string? ShipName { get; set; }

    public string? ShipAddress { get; set; }

    public string? ShipCity { get; set; }

    public string? ShipRegion { get; set; }

    public string? ShipPostalCode { get; set; }

    public string? ShipCountry { get; set; }
}
