using Android;
using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using FaunusMauiApp.Services;

namespace FaunusMauiApp.Platforms.Android.Services;

public static class NotificationVersions
{
    public const int NotificationChannelsIntroduced = 26;  // Android 8.0 (API 26)
    public const int NotificationImportanceChanged = 28;   // Android 9.0 (API 28)
    public const int BubbleNotifications = 29;             // Android 10.0 (API 29)
    public const int NotificationPermissionRequired = 33;  // Android 13.0 (API 33)
}

public static class AndroidVersionHelper
{
    public static bool IsApi26OrHigher => Build.VERSION.SdkInt >= BuildVersionCodes.O;
    public static bool IsApi31OrHigher => Build.VERSION.SdkInt >= BuildVersionCodes.S;
    public static bool IsApi33OrHigher => Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu;

    // Or using OperatingSystem
    public static bool SupportsNotificationChannels => OperatingSystem.IsAndroidVersionAtLeast(NotificationVersions.NotificationChannelsIntroduced);
    public static bool RequiresNotificationPermission => OperatingSystem.IsAndroidVersionAtLeast(NotificationVersions.NotificationPermissionRequired);
}


public class NotificationService : INotificationService
{
    const string channelId = "location_channel";
    const string channelName = "Location Notifications";
    private const int locationNotificationId = 1001;
    private const int PermissionRequestCode = 1002;

    /// <summary>
    /// Initializes the notification channel if the Android version supports it.
    /// </summary>
    public void InitializeNotificationChannel(Context context)
    {
        try
        {
            // Check if notification channel is required (Android 8.0+)
            if (AndroidVersionHelper.SupportsNotificationChannels)
            {
                CreateNotificationChannel(context);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to create notification channel: {ex.Message}");
        }
    }

    private void CreateNotificationChannel(Context context)
    {
        var channel = new NotificationChannel(
            channelId,
            channelName,
            NotificationImportance.Default)
        {
            Description = "Notifications for location updates"
        };
        channel.EnableVibration(true);

        var notificationManager = context.GetSystemService(Context.NotificationService) as NotificationManager;
        notificationManager?.CreateNotificationChannel(channel);
    }

    public bool CheckNotificationPermissionAsync()
    {
        var context = global::Android.App.Application.Context;

        // Check if POST_NOTIFICATIONS is granted (Android 13+)
        if (AndroidVersionHelper.RequiresNotificationPermission)
        {
            // Check if permission is already granted
            if (ContextCompat.CheckSelfPermission(context, Manifest.Permission.PostNotifications)
                == global::Android.Content.PM.Permission.Granted)
            {
                return true;
            }

            // Request permission should already be handled in the MainActivity on startup
            return false;
        }

        // For older versions, check if notifications are enabled
        return AreNotificationsEnabled();
    }

    public bool AreNotificationsEnabled()
    {
        var context = global::Android.App.Application.Context;
        return NotificationManagerCompat.From(context).AreNotificationsEnabled();
    }

    public bool Show(string title, string message)
    {
        bool hasPermission = CheckNotificationPermissionAsync();
        if (!hasPermission)
            return false;

        var context = global::Android.App.Application.Context;
        if (context == null)
            return false;

        InitializeNotificationChannel(context);

        // Create an intent to launch the app when the notification is tapped
        var packageName = context.PackageName;
        if (packageName == null)
            return false;

        var intent = new Intent(context, typeof(MainActivity));
        intent.SetFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

        /*
        var intent = context.PackageManager?.GetLaunchIntentForPackage(packageName);
        if (intent == null)
            return false;
        */

        var pendingIntent = PendingIntent.GetActivity(
            context,
            0,
            intent,
            PendingIntentFlags.Immutable | PendingIntentFlags.UpdateCurrent
        );

        // Build the notification
        var builder = new Notification.Builder(context, channelId)
            .SetContentTitle(title)
            .SetContentText(message)
            .SetStyle(new Notification.BigTextStyle().BigText(message))
            .SetSmallIcon(Resource.Drawable.ic_launcher)
            .SetColor(ContextCompat.GetColor(context, Resource.Color.colorPrimary))
            .SetAutoCancel(true)
            .SetContentIntent(pendingIntent);

        // Show the notification
        var notificationManagerCompat = NotificationManager.FromContext(context);
        notificationManagerCompat?.Notify(locationNotificationId, builder.Build());

        return true;
    }
}