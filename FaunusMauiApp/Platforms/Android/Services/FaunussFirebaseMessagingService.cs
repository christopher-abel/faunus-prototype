using Android.App;
using Firebase.Messaging;
using Android.Content;
using AndroidX.Core.App;

[Service(Exported = true)]
[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
public class FaunussFirebaseMessagingService : FirebaseMessagingService
{
    public override void OnMessageReceived(RemoteMessage message)
    {
        var notification = message.GetNotification();
        if (notification != null)
        {
            var builder = new NotificationCompat.Builder(this, "default")
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetSmallIcon(Resource.Drawable.appicon);

            var notificationManager = NotificationManagerCompat.From(this);
            notificationManager.Notify(0, builder.Build());
        }
    }
}