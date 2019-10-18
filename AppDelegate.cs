using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using Akavache;
using CoreGraphics;
using Foundation;
using SDWebImage;
using UIKit;
using Google.Analytics;
using AdSupport;
using SVProgressHUDBinding;
using WindowsAzure.Messaging;
using UserNotifications;
using AircraftForSale.Helpers;

namespace AircraftForSale
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the
    // User Interface of the application, as well as listening (and optionally responding) to application events from iOS.
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations

        public ITracker Tracker;

        public static readonly string TrackingId = "UA-101868397-1";//"UA-204701-5";

        private SBNotificationHub Hub { get; set; }

        public override UIWindow Window
        {
            get;
            set;
        }

        bool launchedFromPushNotification = false;


        public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
            // Override point for customization after application launch.
            Gai.SharedInstance.DispatchInterval = 20;
            Gai.SharedInstance.TrackUncaughtExceptions = true;

            Tracker = Gai.SharedInstance.GetTracker(TrackingId);

            //Clay Martin 10/21/17: Allowing IDFA collection for more sophisticated analytics
            Tracker.SetAllowIdfaCollection(true);

            // Code to start the Xamarin Test Cloud Agent
#if ENABLE_TEST_CLOUD
            Xamarin.Calabash.Start();
#endif
            // Make sure you set the application name before doing any inserts or gets
            BlobCache.ApplicationName = "AircraftForSaleAkavache";
            Window = new UIWindow(UIScreen.MainScreen.Bounds);

            //Wait until need to update content in the background... once implemented let the OS decide how often to fetch new content
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalMinimum);

            //Register for push notifications is user indicates wishes to receive
            if (Settings.IsRegisterForPushNotifications)
            {
                this.PromptForPushNotifications();
            }

            var storyboard = UIStoryboard.FromName("Main_ipad", NSBundle.MainBundle);
            bool skipFirstStep = Settings.IsRegistered;
            UIViewController rootViewController;
            if (skipFirstStep)
                rootViewController = storyboard.InstantiateViewController("MainTabBarController") as MainTabBarController;
            else
                rootViewController = storyboard.InstantiateInitialViewController();

            Window.RootViewController = rootViewController;
            Window.MakeKeyAndVisible();


            UINavigationBar.Appearance.BarTintColor = UIColor.Black;


            if (!Reachability.IsHostReachable(Settings._baseDomain))
            {
                var alert = UIAlertController.Create("Connect to a Network", Settings._networkProblemMessage, UIAlertControllerStyle.Alert);

                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));

                if (alert.PopoverPresentationController != null)
                {
                    alert.PopoverPresentationController.SourceView = rootViewController.View;
                    alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
                }
                rootViewController.PresentViewController(alert, animated: true, completionHandler: null);
            }
            else
            {
                //Determine if need to dump cache
                if (Settings.LastImageCacheDump != string.Empty)
                {
                    DateTime startDate = DateTime.Parse(Settings.LastImageCacheDump);
                    double days = (DateTime.Now - startDate).TotalDays;
                    if (days > 7)
                    {
                        SDImageCache.SharedImageCache.ClearMemory();
                        SDImageCache.SharedImageCache.ClearDisk();
                        Settings.LastImageCacheDump = string.Empty;
                    }

                }

                //Determine if app was launched from a push notification

                if (launchOptions != null && launchOptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
                {
                    launchedFromPushNotification = true;
                }
                else
                {
                    launchedFromPushNotification = false;
                }

                if (!launchedFromPushNotification)
                {

                    //Task.Run(async () =>
                    //{
                    //    await PerformBackgroundDataFetchFromBuyplaneAPI(() =>
                    //    {
                    //        InvokeOnMainThread(() =>
                    //        {

                                var vc = Window.RootViewController;
                                while (vc.PresentedViewController != null)
                                {
                                    vc = vc.PresentedViewController;
                                }

                                if (vc is MainTabBarController)
                                {
                                    var maintTabBarController = vc as MainTabBarController;
                                    var magNavVC = maintTabBarController.ViewControllers.FirstOrDefault(row => row is MagazineNavigationViewController);
                                    if (magNavVC != null)
                                    {
                                        var chooseClassVC = (magNavVC as MagazineNavigationViewController).ViewControllers.FirstOrDefault(row => row is ChooseClassificationCollectionViewController);
                                        if (chooseClassVC != null)
                                        {
                                            (chooseClassVC as ChooseClassificationCollectionViewController).LoadBackgroundImages();
                                            if (launchOptions != null && launchOptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
                                            {
                                                NSDictionary notificationDictionary = launchOptions[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                                                ReceivedRemoteNotification(application, notificationDictionary);
                                            }
                                        }
                                    }
                                }

                    //        });

                    //    });
                    //});
                   
                }
                else
                {
                    var vc = Window.RootViewController;
                    while (vc.PresentedViewController != null)
                    {
                        vc = vc.PresentedViewController;
                    }

                    if (vc is MainTabBarController)
                    {
                        var maintTabBarController = vc as MainTabBarController;
                        var magNavVC = maintTabBarController.ViewControllers.FirstOrDefault(row => row is MagazineNavigationViewController);
                        if (magNavVC != null)
                        {
                            var chooseClassVC = (magNavVC as MagazineNavigationViewController).ViewControllers.FirstOrDefault(row => row is ChooseClassificationCollectionViewController);
                            if (chooseClassVC != null)
                            {
                                (chooseClassVC as ChooseClassificationCollectionViewController).LoadBackgroundImages();
                                if (launchOptions != null && launchOptions.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey))
                                {
                                    NSDictionary notificationDictionary = launchOptions[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;
                                    ReceivedRemoteNotification(application, notificationDictionary);
                                }
                            }
                        }
                    }
                }


                //});

            }



            return true;
        }

        public void PromptForPushNotifications()
        {
            //Register for push notifications
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert | UNAuthorizationOptions.Sound | UNAuthorizationOptions.Sound,
                                                                        (granted, error) =>
                                                                        {
                                                                            if (granted)
                                                                                InvokeOnMainThread(UIApplication.SharedApplication.RegisterForRemoteNotifications);
                                                                        });
            }
            else if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                        UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                        new NSSet());

                UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                UIApplication.SharedApplication.RegisterForRemoteNotifications();
            }
            else
            {
                UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            }
        }


        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            Hub = new SBNotificationHub(Constants.ListenConnectionString, Constants.NotificationHubName);

            List<string> tagList = new List<string>();
            //add global identifier
            tagList.Add("buyplane");
            if (Settings.IsRegistered)
            {
                tagList.Add("userid:" + Settings.UserID.ToString());
            }

            Hub.UnregisterAllAsync(deviceToken, (error) =>
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine("Error calling Unregister: {0}", error.ToString());
                    return;
                }



                NSSet tags = new NSSet(tagList.ToArray());
                Hub.RegisterNativeAsync(deviceToken, tags, (errorCallback) =>
                {
                    if (errorCallback != null)
                        System.Diagnostics.Debug.WriteLine("RegisterNativeAsync error: " + errorCallback.ToString());
                });
            });
        }

        public override void ReceivedRemoteNotification(UIApplication application, NSDictionary userInfo)
        {
            bool appInForeground = UIApplication.SharedApplication.ApplicationState == UIApplicationState.Active;
            ProcessNotification(userInfo, appInForeground);
        }

        void ProcessNotification(NSDictionary options, bool appInForeground)
        {
            // Check to see if the dictionary has the aps key.  This is the notification payload you would have sent
            if (null != options && options.ContainsKey(new NSString("aps")))
            {
                //Get the aps dictionary
                NSDictionary aps = options.ObjectForKey(new NSString("aps")) as NSDictionary;

                string alert = string.Empty;
                string adId = string.Empty;
                string classification = string.Empty;


                if (aps.ContainsKey(new NSString("alert")))
                    alert = (aps[new NSString("alert")] as NSString).ToString();

                if (aps.ContainsKey(new NSString("adid")))
                    adId = (aps[new NSString("adid")] as NSString).ToString();

                if (aps.ContainsKey(new NSString("classification")))
                    classification = (aps[new NSString("classification")] as NSString).ToString();


                //Manually show an alert
                if (!string.IsNullOrEmpty(alert))
                {
                    //if adid != 0, navigate to ad
                    if (adId != string.Empty && classification != string.Empty)
                    {

                        //if logged in, retrieve highlighted ad and navigate to it
                        if (Settings.IsRegistered)
                        {
                            //SVProgressHUD.Show();


                            if (!appInForeground || launchedFromPushNotification)
                            {
                                if (launchedFromPushNotification)
                                {
                                    launchedFromPushNotification = false;
                                }
                                NavigateToAdd(classification, adId);
                            }
                            else
                            {
                                //Create Alert
                                var okAlertController = UIAlertController.Create("View Important Ad Update!", alert, UIAlertControllerStyle.Alert);

                                //Add Action
                                okAlertController.AddAction(UIAlertAction.Create("View Aircraft", UIAlertActionStyle.Default, async (obj) =>
                                {
                                    NavigateToAdd(classification, adId);
                                }));

                                okAlertController.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, async (obj) =>
                                {

                                }));

                                // Present Alert
                                Window.RootViewController.PresentViewController(okAlertController, true, null);
                            }
                        }
                    }
                }
            }
        }

        public async void NavigateToAdd(string classification, string adId)
        {
            var vc = Window.RootViewController;
            while (vc.PresentedViewController != null)
            {
                vc = vc.PresentedViewController;
            }

            LoadingOverlay loadingIndicator = new LoadingOverlay(vc.View.Frame, "Loading ...", false);


            if (vc is MainTabBarController)
            {

                vc.View.AddSubview(loadingIndicator);

                //remove cache
                //Ad.DeleteAdsByClassification(classification);
                //Retrieve highlighted ad
                var adsByClassification = await Ad.GetAdsByClassificationAsync(classification);
                var highlightedAd = adsByClassification.Where(row => row.ID == adId).First();


                var maintTabBarController = vc as MainTabBarController;
                var magNavVC = maintTabBarController.ViewControllers.FirstOrDefault(row => row is MagazineNavigationViewController);
                if (magNavVC != null)
                {
                    maintTabBarController.SelectedIndex = 0;
                    var chooseClassVC = (magNavVC as MagazineNavigationViewController).ViewControllers.FirstOrDefault(row => row is ChooseClassificationCollectionViewController);
                    if (chooseClassVC != null)
                    {
                        var chooseClassificationViewController = chooseClassVC as ChooseClassificationCollectionViewController;
                        var storyboard = UIStoryboard.FromName("Main_ipad", NSBundle.MainBundle);
                        MagazineFlipBoardViewController flipboardVC = storyboard.InstantiateViewController("MagazineFlipBoardViewController") as MagazineFlipBoardViewController;
                        flipboardVC.Title = classification;
                        flipboardVC.SelectedClassification = classification;
                        flipboardVC.NavigateDirectlyToAdId = adId;
                        chooseClassificationViewController.NavigationController.PopToRootViewController(false);
                        chooseClassificationViewController.ShowViewController(flipboardVC, this);


                    }
                }
            }
            loadingIndicator.Hide();
        }

        //perform data fetch here for the ads
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Check for new data, and display it
            Task.Run(async () =>
            {
                await PerformBackgroundDataFetchFromBuyplaneAPI(() =>
                {
                    // Inform system of fetch results... inform if data has been refreshed or an error has occurred 
                    completionHandler(UIBackgroundFetchResult.NewData);
                });
            }); 
        }

        public override void DidReceiveRemoteNotification(UIApplication application, NSDictionary userInfo, Action<UIBackgroundFetchResult> completionHandler)
        {
            Task.Run(async () =>
            {
                //Refresh content prior to calling completion handler
                await PerformBackgroundDataFetchFromBuyplaneAPI(() =>
                {
                    // fetch content
                    completionHandler(UIBackgroundFetchResult.NewData);
                });
            });
           
        }

        const string SessionId = "com.globalair.aircraftforsale.backgroundupdate";
        private NSUrlSession ConfigureBackgroundSession()
        {
            var configuration = NSUrlSessionConfiguration.BackgroundSessionConfiguration(SessionId);
            if (configuration == null)
            {
                configuration = NSUrlSessionConfiguration.CreateBackgroundSessionConfiguration(SessionId);
            }

            using (configuration)
            {
                return NSUrlSession.FromConfiguration(configuration, new BuyplaneAPISessionDelegate(), null);
            }
        }

        public async Task PerformBackgroundDataFetchFromBuyplaneAPI(System.Action completionHandler, List<Ad> adListOverride = null)
        {
            //NSUrlSession session = ConfigureBackgroundSession();



            List<Ad> adList = new List<Ad>();
            if (adListOverride == null)
            {

                if (Settings.IsRegistered)
                {
                    var favoredClassificationsList = Settings.GetFavoredClassifications();

                    foreach (var fClass in favoredClassificationsList)
                    {
                        //remove current data in the database
                        Ad.DeleteAdsByClassification(fClass);
                        adList.AddRange(await Ad.GetAdsByClassificationAsync(fClass));
                    }
                }
                else
                {

                    //preload Jets
                    Settings.IsJets = true;
                    Settings.IsSingleEngine = true;
                    Settings.IsTwinTurbines = true;

                    List<string> preloadJetsAndOtherClassificationsIfSetSettingsTrueAbove = Settings.GetFavoredClassifications();

                    foreach (var fClass in preloadJetsAndOtherClassificationsIfSetSettingsTrueAbove)
                    {
                        //remove current data in the database
                        Ad.DeleteAdsByClassification(fClass);
                        adList.AddRange(await Ad.GetAdsByClassificationAsync(fClass));
                    }
                }
            }
            else
            {
                adList = adListOverride;
            }

            //filter out previously cached
            int adsAlreadyInCacheCount = 0;
            int adCount = 0;
            foreach (var ad in adList)
            {
                SDImageCache.SharedImageCache.DiskImageExists(ad.ImageURL, (isInCache) =>
                {
                    adCount++;
                    if (!isInCache)
                    {
                        FetchImage(ad.ImageURL);

                    }
                    else
                    {
                        adsAlreadyInCacheCount++;
                    }
                    if (adsAlreadyInCacheCount == adList.Count)
                    {
                        completionHandler.Invoke();
                    }
                    if (adCount == adList.Count)
                    {
                        completionHandler.Invoke();
                    }
                });
            }




        }

        public UIBackgroundFetchResult FetchImage(string url)
        {
            var session = ConfigureBackgroundSession();

            var request = new NSMutableUrlRequest(NSUrl.FromString(url));
            request.HttpMethod = "GET";
            using (request)
            {
                var task = session.CreateDownloadTask(request);
                task.Resume();
            }

            return UIBackgroundFetchResult.NewData;
        }

        public System.Action BackgroundUrlCompletionHandler;

        public override void HandleEventsForBackgroundUrl(UIApplication application, string sessionIdentifier, System.Action completionHandler)
        {
            this.BackgroundUrlCompletionHandler = completionHandler;
        }

        public override void OnResignActivation(UIApplication application)
        {
            // Invoked when the application is about to move from active to inactive state.
            // This can occur for certain types of temporary interruptions (such as an incoming phone call or SMS message) 
            // or when the user quits the application and it begins the transition to the background state.
            // Games should use this method to pause the game.
        }

        public override void DidEnterBackground(UIApplication application)
        {
            // Use this method to release shared resources, save user data, invalidate timers and store the application state.
            // If your application supports background exection this method is called instead of WillTerminate when the user quits.
        }

        public override void WillEnterForeground(UIApplication application)
        {
            // Called as part of the transiton from background to active state.
            // Here you can undo many of the changes made on entering the background.
        }

        public override void OnActivated(UIApplication application)
        {
            // Restart any tasks that were paused (or not yet started) while the application was inactive. 
            // If the application was previously in the background, optionally refresh the user interface.
        }

        public override void WillTerminate(UIApplication application)
        {
            // Called when the application is about to terminate. Save data, if needed. See also DidEnterBackground.
            BlobCache.Shutdown().Wait();
        }
    }
}


