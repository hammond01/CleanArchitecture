namespace ProductManager.Blazor.Components.Pages;

/// <summary>
/// </summary>
public partial class TableDemo : ComponentBase
{

    private readonly ConcurrentDictionary<Foo, IEnumerable<SelectedItem>> _cache =
        new ConcurrentDictionary<Foo, IEnumerable<SelectedItem>>();
    [Inject]
    [NotNull]
    private IStringLocalizer<Foo>? Localizer { get; set; }

    /// <summary>
    /// </summary>
    private static IEnumerable<int> PageItemsSource => new[]
    {
        20, 40
    };

    private IEnumerable<SelectedItem> GetHobbys(Foo item) => _cache.GetOrAdd(item, valueFactory: f => Foo.GenerateHobbys(Localizer));
}
