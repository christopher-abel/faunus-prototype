using Android.App;
using Android.Content;
using Android.OS;
using FaunusMauiApp.Services;

namespace FaunusMauiApp.Platforms.Android.Services;

public class NotificationService : INotificationService
{
    const string channelId = "location_channel";
    const string channelName = "Location Notifications";

    public void Show(string title, string message)
    {
        var context = Application.Context;

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Default);
            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }

        var builder = new Notification.Builder(context, channelId)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetSmallIcon(Resource.Drawable.ic_launcher);

        var notificationManagerCompat = NotificationManager.FromContext(context);
        notificationManagerCompat.Notify(1001, builder.Build());
    }
}