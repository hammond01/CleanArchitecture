using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using ProductManager.Blazor.Data;
namespace ProductManager.Blazor.Components.Pages;

/// <summary>
/// </summary>
public partial class Users
{

    private static readonly ConcurrentDictionary<Type, Func<IEnumerable<Foo>, string, SortOrder, IEnumerable<Foo>>> SortLambdaCache =
        new ConcurrentDictionary<Type, Func<IEnumerable<Foo>, string, SortOrder, IEnumerable<Foo>>>();
    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? Localizer { get; set; }

    /// <summary>
    ///     Get/Set pagination configuration data source
    /// </summary>
    private static IEnumerable<int> PageItemsSource => new[]
    {
        10, 20, 40
    };

    [NotNull]
    private IEnumerable<Foo>? Items { get; set; }

    private static string GetAvatarUrl(int id) => $"images/avatars/150-{id}.jpg";

    private static Color GetProgressColor(int count) => count switch
    {
        >= 0 and < 10 => Color.Secondary,
        >= 10 and < 20 => Color.Danger,
        >= 20 and < 40 => Color.Warning,
        >= 40 and < 50 => Color.Info,
        >= 50 and < 70 => Color.Primary,
        _ => Color.Success
    };

    private Task<QueryData<Foo>> OnQueryAsync(QueryPageOptions options)
    {
        // This code is not usable in production, written only for demonstration to prevent all data from being deleted
        if (Items == null || !Items.Any())
        {
            Items = Foo.GenerateFoo(Localizer, 23).ToList();
        }

        var items = Items;
        var isSearched = false;
        // Handle advanced queries
        if (options.SearchModel is Foo model)
        {
            if (!string.IsNullOrEmpty(model.Name))
            {
                items = items.Where(item => item.Name?.Contains(model.Name, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            if (!string.IsNullOrEmpty(model.Address))
            {
                items = items.Where(item => item.Address?.Contains(model.Address, StringComparison.OrdinalIgnoreCase) ?? false);
            }

            isSearched = !string.IsNullOrEmpty(model.Name) || !string.IsNullOrEmpty(model.Address);
        }

        if (options.Searches.Any())
        {
            // Perform fuzzy query based on SearchText
            items = items.Where(options.Searches.GetFilterFunc<Foo>(FilterLogic.Or));
        }

        // Filter
        var isFiltered = false;
        if (options.Filters.Any())
        {
            items = items.Where(options.Filters.GetFilterFunc<Foo>());
            isFiltered = true;
        }

        // Sort
        var isSorted = false;
        if (!string.IsNullOrEmpty(options.SortName))
        {
            // External sorting not performed, internal automatic sorting handling
            var invoker = SortLambdaCache.GetOrAdd(typeof(Foo), valueFactory: key => LambdaExtensions.GetSortLambda<Foo>().Compile());
            items = invoker(items, options.SortName, options.SortOrder);
            isSorted = true;
        }

        var total = items.Count();

        return Task.FromResult(new QueryData<Foo>
        {
            Items = items.Skip((options.PageIndex - 1) * options.PageItems).Take(options.PageItems).ToList(),
            TotalCount = total,
            IsFiltered = isFiltered,
            IsSorted = isSorted,
            IsSearch = isSearched
        });
    }
}
