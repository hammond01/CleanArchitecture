﻿namespace ProductManager.Domain.Entities;

[Index("CategoryName", Name = "CategoryName")]
public class Categories : Entity<string>
{
    [StringLength(15)]
    public string CategoryName { get; set; } = null!;

    [StringLength(250)]
    public string? Description { get; set; }

    [Column(TypeName = "image")]
    public byte[]? Picture { get; set; }

    [StringLength(100)]
    public string? PictureLink { get; set; }

    [InverseProperty("Category")]
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
