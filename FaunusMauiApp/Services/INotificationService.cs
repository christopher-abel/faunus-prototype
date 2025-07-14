namespace FaunusMauiApp.Services;

public interface INotificationService
{
    bool Show(string title, string message);
}