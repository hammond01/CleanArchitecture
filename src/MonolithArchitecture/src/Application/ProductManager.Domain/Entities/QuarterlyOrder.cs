namespace ProductManager.Domain.Entities;

[Keyless]
public class QuarterlyOrder
{
    [Column("CustomerID")]
    [StringLength(5)]
    public string? CustomerId { get; set; }

    [StringLength(40)]
    public string? CompanyName { get; set; }

    [StringLength(15)]
    public string? City { get; set; }

    [StringLength(15)]
    public string? Country { get; set; }
}
