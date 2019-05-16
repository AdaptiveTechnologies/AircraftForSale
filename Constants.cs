using System;
namespace AircraftForSale
{
    public class Constants
    {
        // Azure app-specific connection string and hub path
        public const string ListenConnectionString = "Endpoint=sb://buyplane.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=jkpAJZsfsow1QgiAj8C/VRUdrkm457l8s3fuVRB9Lk8=";
        //public const string ListenConnectionString = "Endpoint=sb://developmentbuyplane.servicebus.windows.net/;SharedAccessKeyName=DefaultListenSharedAccessSignature;SharedAccessKey=B6J07uLzqfUsIolQ096jg0FaDs9PAGb5l3FcPqTP3TQ=";
        public const string NotificationHubName = "NotificationHub";
        //public const string NotificationHubName = "DevNotificationHub";
    }
}
