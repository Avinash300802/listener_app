using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;
using Windows.UI.Notifications.Management;
using Windows.UI.Xaml.Controls;

namespace listener
{
    public sealed partial class MainPage : Page
    {
        // private string logFilePath = "notification_log1.txt"; // Path to the log file

        private string logFilePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "notification_log1.txt");

        public MainPage()
        {
            InitializeComponent(); 

            // Check for Listener support and request access when the MainPage is initialized
            CheckForListenerSupportAndRequestAccess();

            // Retrieve and display toast notifications
            RetrieveAndDisplayToastNotifications();
        }

        // Retrieve and display toast notifications
        private async void RetrieveAndDisplayToastNotifications()
        {
            // Check for support
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                // Get the listener
                UserNotificationListener listener = UserNotificationListener.Current;

                // Request access to the user's notifications (must be called from UI thread)
                UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();

                // Handle access status
                switch (accessStatus)
                {
                    case UserNotificationListenerAccessStatus.Allowed:
                        // Access granted. Retrieve toast notifications
                        IReadOnlyList<UserNotification> notifs = await listener.GetNotificationsAsync(NotificationKinds.Toast);

                        // Write notifications to the log file
                        // Write notifications to the log file


                        using (StreamWriter writer = File.AppendText(logFilePath))
                        {
                            foreach (UserNotification notification in notifs)
                            {
                                var (appName, creationTime) = DisplayNotification(notification);

                                // Define fixed-width columns for each piece of information
                                string formattedOutput = string.Format(
                                        "App Name: {0,-20}| Notification created Time: {1}",
                                                appName, creationTime);


                                writer.WriteLine(formattedOutput);
                            }
                        }

                        break;
                    case UserNotificationListenerAccessStatus.Denied:
                        // Access denied. Show UI explaining that listener features will not work until user allows access.
                        break;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        // Access status unspecified. Show UI that allows the user to bring up the prompt again.
                        break;
                }
            }
            else
            {
                // Older version of Windows, no Listener
            }
        }

        // Display the notification information
        // Display the notification information
        private (string appName, DateTime creationTime) DisplayNotification(UserNotification notification)
        {
            // Get the app's display name
            string appDisplayName = notification.AppInfo.DisplayInfo.DisplayName;

            // Get the creation time of the notification
            DateTime creationTime = notification.CreationTime.DateTime;

            return (appDisplayName, creationTime);
        }


        // Request access to the UserNotificationListener
        private async void CheckForListenerSupportAndRequestAccess()
        {
            // Check for support
            if (ApiInformation.IsTypePresent("Windows.UI.Notifications.Management.UserNotificationListener"))
            {
                // Get the listener
                UserNotificationListener listener = UserNotificationListener.Current;

                // Request access to the user's notifications (must be called from UI thread)
                UserNotificationListenerAccessStatus accessStatus = await listener.RequestAccessAsync();


                // Handle access status
                switch (accessStatus)
                {
                    case UserNotificationListenerAccessStatus.Allowed:
                        // Access granted. Proceed as normal.
                        break;
                    case UserNotificationListenerAccessStatus.Denied:
                        // Access denied. Show UI explaining that listener features will not work until user allows access.
                        break;
                    case UserNotificationListenerAccessStatus.Unspecified:
                        // Access status unspecified. Show UI that allows the user to bring up the prompt again.
                        break;
                }
            }
            else
            {
                // Older version of Windows, no Listener
            }
        }
    }
}
