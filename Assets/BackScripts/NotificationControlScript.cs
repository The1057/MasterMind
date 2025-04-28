using UnityEngine;
using Unity.Notifications.Android;
using UnityEngine.Android;

public class NotificationControlScript : MonoBehaviour
{
    public void requestAuthorization()
    {
        if (!Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS"))
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }
    public void registerNotifChanngel()
    {
        var c = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "MasterMind",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(c);
    }
    public void sendNotification(string title, string text, int timeInMinutes)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = text;
        notification.FireTime = System.DateTime.Now.AddMinutes(timeInMinutes);
        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notification, "default_channel");
    }
    public void Start()
    {
        requestAuthorization();
        registerNotifChanngel();
    }

    [ContextMenu("Send Debug Notification")]
    public void sendDebugNotif()
    {
        sendNotification("Title","This is debug notification",0);
    }
}
