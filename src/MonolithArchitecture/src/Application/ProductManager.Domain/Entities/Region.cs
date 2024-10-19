﻿namespace ProductManager.Domain.Entities;

[Table("Region")]
public class Region : Entity<int>
{
    [StringLength(50)]
    public string RegionDescription { get; set; } = null!;

    [InverseProperty("Region")]
    public ICollection<Territory> Territories { get; set; } = new List<Territory>();
}
