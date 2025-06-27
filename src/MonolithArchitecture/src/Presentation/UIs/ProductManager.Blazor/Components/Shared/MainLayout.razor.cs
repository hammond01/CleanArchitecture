using BootstrapBlazor.Components;
using Microsoft.AspNetCore.Components.Routing;
namespace ProductManager.Blazor.Components.Shared;

/// <summary>
/// </summary>
public sealed partial class MainLayout
{
    private bool UseTabSet { get; set; } = true;

    private string Theme
    {
        get;
    } = "";

    private bool IsOpen { get; set; }

    private bool IsFixedHeader { get; set; } = true;

    private bool IsFixedFooter { get; set; } = true;

    private bool IsFullSide { get; set; } = true;

    private bool ShowFooter { get; set; } = true;

    private List<MenuItem>? Menus { get; set; }

    /// <summary>
    ///     OnInitialized method
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();

        Menus = GetIconSideMenuItems();
    }

    private static List<MenuItem> GetIconSideMenuItems()
    {
        var menus = new List<MenuItem>
        {
            new MenuItem
            {
                Text = "Dash board", Icon = "fa-solid fa-fw fa-home", Url = "/", Match = NavLinkMatch.All
            },
            new MenuItem
            {
                Text = "Counter", Icon = "fa-solid fa-fw fa-check-square", Url = "/counter"
            },
            new MenuItem
            {
                Text = "Weather", Icon = "fa-solid fa-fw fa-database", Url = "/weather"
            },
            new MenuItem
            {
                Text = "Table", Icon = "fa-solid fa-fw fa-table", Url = "/table"
            },
            new MenuItem
            {
                Text = "User Directory", Icon = "fa-solid fa-fw fa-users", Url = "/users"
            }
        };

        return menus;
    }
}
