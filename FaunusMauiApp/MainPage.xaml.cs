
namespace FaunusMauiApp;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

    private async Task<Location?> GetLocationAsync()
    {
        var location = await Geolocation.GetLastKnownLocationAsync()
            ?? await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(10)
            });
        return location;
    }

    private async void NotifyLocationButton_ClickedAsync(object sender, EventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;
            try
            {
                var location = await GetLocationAsync();

                if (location != null)
                {
                    await DisplayAlert("Location",
                        $"Latitude: {location.Latitude}\nLongitude: {location.Longitude}", "OK");
                }
                else
                {
                    await DisplayAlert("Location", "Unable to get location.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to get location: {ex.Message}", "OK");
            }
            finally
            {
                button.IsEnabled = true;
            }
        }
    }

}
