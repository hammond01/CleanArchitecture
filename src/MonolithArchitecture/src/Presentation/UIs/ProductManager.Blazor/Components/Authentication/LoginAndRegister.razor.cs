using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using ProductManager.Shared.DTOs.UserDto;
namespace ProductManager.Blazor.Components.Authentication;

public partial class LoginAndRegister : ComponentBase
{
    private LoginRequest LoginRequest
    {
        get;
    } = new LoginRequest
    {
        UserName = "",
        Password = ""
    };

    private RegisterRequest RegisterRequest
    {
        get;
    } = new RegisterRequest
    {
        UserName = "",
        Password = "",
        PhoneNumber = "84",
        FirstName = "User",
        LastName = "Account"
    };
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;
    private async Task HandleLogin()
    {
        var a = LoginRequest;
        var b = RegisterRequest;
        await LocalStorage.SetItemAsync("userName", LoginRequest.UserName.ToUpper());
        NavigationManager.NavigateTo("/", true);
    }

    private async Task HandleRegister()
    {
        var a = LoginRequest;
        var b = RegisterRequest;
        await LocalStorage.SetItemAsync("userName", RegisterRequest.UserName.ToUpper());
        NavigationManager.NavigateTo("/", true);
    }
}
