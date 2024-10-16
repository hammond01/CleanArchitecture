namespace ProductManager.Domain.Entities;

[Index("CategoryName", Name = "CategoryName")]
public sealed class Category
{
    [Key]
    [Column("CategoryID")]
    public int CategoryId { get; set; }

    [StringLength(15)]
    public string CategoryName { get; set; } = null!;

    [Column(TypeName = "ntext")]
    public string? Description { get; set; }

    [Column(TypeName = "image")]
    public byte[]? Picture { get; set; }

    [StringLength(100)]
    public string? PictureLink { get; set; }

    [InverseProperty("Category")]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
