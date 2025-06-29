// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

using System.ComponentModel.DataAnnotations;
using BootstrapBlazor.Components;
using Microsoft.Extensions.Localization;
namespace ProductManager.Blazor.Data;

/// <summary>
/// </summary>
public class Foo
{

    private static readonly Random random = new Random();
    // Column header information supports both Display and DisplayName tags

    /// <summary>
    /// </summary>
    [Display(Name = "Primary Key")]
    [AutoGenerateColumn(Ignore = true)]
    public int Id { get; set; }

    /// <summary>
    /// </summary>
    [Required(ErrorMessage = "{0} cannot be empty")]
    [AutoGenerateColumn(Order = 10, Filterable = true, Searchable = true)]
    [Display(Name = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// </summary>
    [AutoGenerateColumn(Order = 1, FormatString = "yyyy-MM-dd", Width = 180)]
    [Display(Name = "Date")]
    public DateTime DateTime { get; set; }

    /// <summary>
    /// </summary>
    [Display(Name = "Address")]
    [Required(ErrorMessage = "{0} cannot be empty")]
    [AutoGenerateColumn(Order = 20, Filterable = true, Searchable = true)]
    public string? Address { get; set; }

    /// <summary>
    /// </summary>
    [Display(Name = "Quantity")]
    [Required]
    [AutoGenerateColumn(Order = 40, Sortable = true)]
    public int Count { get; set; }

    /// <summary>
    /// </summary>
    [Display(Name = "Yes/No")]
    [AutoGenerateColumn(Order = 50, ComponentType = typeof(Switch))]
    public bool Complete { get; set; }

    /// <summary>
    /// </summary>
    [Required(ErrorMessage = "Please select education level")]
    [Display(Name = "Education")]
    [AutoGenerateColumn(Order = 60)]
    public EnumEducation? Education { get; set; }

    /// <summary>
    /// </summary>
    [Required(ErrorMessage = "Please select a {0}")]
    [Display(Name = "Hobby")]
    [AutoGenerateColumn(Order = 70)]
    public IEnumerable<string> Hobby { get; set; } = new List<string>();

    /// <summary>
    /// </summary>
    /// <param name="localizer"></param>
    /// <returns></returns>
    public static Foo Generate(IStringLocalizer<Foo> localizer)
        => new Foo
        {
            Id = 1,
            Name = localizer["Foo.Name", "1000"],
            DateTime = DateTime.Now,
            Address = localizer["Foo.Address", $"{random.Next(1000, 2000)}"],
            Count = random.Next(1, 100),
            Complete = random.Next(1, 100) > 50,
            Education = random.Next(1, 100) > 50 ? EnumEducation.Primary : EnumEducation.Middel
        };

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static List<Foo> GenerateFoo(IStringLocalizer<Foo> localizer, int count = 80) => Enumerable.Range(1, count).Select(i
        => new Foo
        {
            Id = i,
            Name = localizer["Foo.Name", $"{i:d4}"],
            DateTime = DateTime.Now.AddDays(i - 1),
            Address = localizer["Foo.Address", $"{random.Next(1000, 2000)}"],
            Count = random.Next(1, 100),
            Complete = random.Next(1, 100) > 50,
            Education = random.Next(1, 100) > 50 ? EnumEducation.Primary : EnumEducation.Middel
        }).ToList();

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SelectedItem> GenerateHobbys(IStringLocalizer<Foo> localizer)
        => localizer["Hobbys"].Value.Split(",").Select(i => new SelectedItem(i, i)).ToList();
}
/// <summary>
/// </summary>
public enum EnumEducation
{
    /// <summary>
    /// </summary>
    [Display(Name = "Primary School")]
    Primary,

    /// <summary>
    /// </summary>
    [Display(Name = "Middle School")]
    Middel
}
