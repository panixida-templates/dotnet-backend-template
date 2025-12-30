using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace Pl.Ui.Blazor.Admin.Layout;

public partial class MainLayout : IDisposable
{
    private bool _drawerOpen = true;

    private string _currentUrl = string.Empty;
    private string _apiBaseAddress = string.Empty;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IConfiguration Configuration { get; set; } = default!;

    protected override void OnInitialized()
    {
        _currentUrl = NavigationManager.Uri;
        _apiBaseAddress = Configuration["HttpClients:Api:BaseAddress"] ?? "(null)";

        NavigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        _currentUrl = e.Location;
        StateHasChanged();
    }

    private void ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
    }

    public void Dispose()
    {
        NavigationManager.LocationChanged -= OnLocationChanged;
    }
}
