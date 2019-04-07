using System;
using UserNotifications;
namespace AircraftForSale
{

    public class UserNotificationCenterDelegate : UNUserNotificationCenterDelegate
    {

        #region Override Methods
        public override void DidReceiveNotificationResponse(UNUserNotificationCenter center, UNNotificationResponse response, Action completionHandler)
        {
            // Take action based on Action ID
            switch (response.ActionIdentifier)
            {
                case "reply":
                    // Do something
                    break;
                default:
                    // Take action based on identifier
                    if (response.IsDefaultAction)
                    {
                        // Handle default action...
                        var bPoint = "";
                    }
                    else if (response.IsDismissAction)
                    {
                        // Handle dismiss action
                    }
                    break;
            }

            // Inform caller it has been handled
            completionHandler();
        }
        #endregion
    }

}
