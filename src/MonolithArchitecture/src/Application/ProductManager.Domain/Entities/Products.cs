using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
namespace ProductManager.Domain.Entities;

[Index("CategoryId", Name = "CategoriesProducts")]
[Index("CategoryId", Name = "CategoryID")]
[Index("ProductName", Name = "ProductName")]
public class Products : Entity<string>
{
    [StringLength(40)]
    public string ProductName { get; set; } = null!;

    [Column("CategoryID")]
    public string CategoryId { get; set; } = null!;

    [StringLength(20)]
    public string? QuantityPerUnit { get; set; }

    [Column(TypeName = "money")]
    public decimal? UnitPrice { get; set; }

    public short? UnitsInStock { get; set; }

    public short? UnitsOnOrder { get; set; }

    public short? ReorderLevel { get; set; }

    public bool Discontinued { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public Categories Category { get; set; } = null!;
}
