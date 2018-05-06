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

        public override UIWindow Window
        {
            get;
            set;
        }


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

            //var storyboard = UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "Main_ipad" : "Main", NSBundle.MainBundle);
            var storyboard = UIStoryboard.FromName("Main_ipad", NSBundle.MainBundle);

            bool skipFirstStep = Settings.IsRegistered;

            UIViewController rootViewController;
            if (skipFirstStep)
                rootViewController = storyboard.InstantiateViewController("MainTabBarController") as MainTabBarController;
            //rootViewController = (UIViewController)storyboard.InstantiateViewController("YourViewControllerId");
            else
                rootViewController = storyboard.InstantiateInitialViewController();



            //Wait until need to update content in the background... once implemented let the OS decide how often to fetch new content
            UIApplication.SharedApplication.SetMinimumBackgroundFetchInterval(UIApplication.BackgroundFetchIntervalNever);


			//changed for testing
			Window.RootViewController = rootViewController;
			//var testStoryBoard = UIStoryboard.FromName("Registration_New", NSBundle.MainBundle);
			//Window.RootViewController = testStoryBoard.InstantiateInitialViewController();
            Window.MakeKeyAndVisible();

            UINavigationBar.Appearance.BarTintColor = UIColor.Black;
            //UINavigationBar.Appearance.TintColor = UIColor.White; 


            //if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            //{
                //var pushSettings = UIUserNotificationSettings.GetSettingsForTypes(
                //                   UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                //                   new NSSet());

                //UIApplication.SharedApplication.RegisterUserNotificationSettings(pushSettings);
                //UIApplication.SharedApplication.RegisterForRemoteNotifications();
            //}
            //else
            //{
                //UIRemoteNotificationType notificationTypes = UIRemoteNotificationType.Alert | UIRemoteNotificationType.Badge | UIRemoteNotificationType.Sound;
                //UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(notificationTypes);
            //}

            if (!Reachability.IsHostReachable(Settings._baseDomain))
            {
                var alert = UIAlertController.Create("Connect to a Network", Settings._networkProblemMessage, UIAlertControllerStyle.Alert);

                alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
                //alert.AddAction(UIAlertAction.Create("Snooze", UIAlertActionStyle.Default, action => Snooze()));
                if (alert.PopoverPresentationController != null)
                {
                    alert.PopoverPresentationController.SourceView = rootViewController.View;
                    alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
                }
                rootViewController.PresentViewController(alert, animated: true, completionHandler: null);
            }
            else
            {
                //LoadingOverlay loadingOverlay = new LoadingOverlay(rootViewController.View.Frame, "Loading Aircraft...");
                //rootViewController.View.AddSubview(loadingOverlay);


                List<Task> taskList = new List<Task>();
                if (Settings.IsRegistered)
                {
                    var favoredClassificationsList = Settings.GetFavoredClassifications();
                    foreach (var fClass in favoredClassificationsList)
                    {
                        //remove current data in the database
                        var task = Task.Run(async () =>
                    {
                        Ad.DeleteAdsByClassification(fClass);

                        await ProactivelyDownloadImages(await Ad.GetAdsByClassificationAsync(fClass));
                    });

                        taskList.Add(task);
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
                        var task = Task.Run(async () =>
                        {
                            Ad.DeleteAdsByClassification(fClass);
                            await ProactivelyDownloadImages(await Ad.GetAdsByClassificationAsync(fClass));
                        });
                        taskList.Add(task);
                    }
                }

                Task.WhenAny(taskList.ToArray()).ContinueWith((arg) =>
               {


                   InvokeOnMainThread(() =>
                   {
                        //var url = "https://www.globalair.com/banmanpro/ad.aspx?ZoneID=94&Task=Get&Mode=HTML&SiteID=1&PageID=78751";
                        //var width = UIScreen.MainScreen.Bounds.Width;
                        //var screenHeight = UIScreen.MainScreen.Bounds.Height;

                        //var frame = new CGRect(0, 0, width, screenHeight);
                        //BanManProWebView = new UIWebView(frame);
                        //BanManProWebView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

                        //No need to hide overlay if it is never rendered
                        //loadingOverlay.Hide();

                        //var window = UIApplication.SharedApplication.KeyWindow;
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
                               }
                           }
                       }

                   });
               });

            }



            return true;
        }

        public static Task ProactivelyDownloadImages(IEnumerable<Ad> ads)
        {
            //if (FirstAd == null)
            //{
            //    FirstAd = ads.First();
            //}

            //Proactively download images
            return Task.Run(() =>
            {
                List<Task> taskList = new List<Task>();
                int counter = 0;
                foreach (var adListing in ads)
                {
                    try
                    {
                        //var imageObject = SDImageCache.SharedImageCache.ValueForKeyPath(new NSString(adListing.ImageURL));
                        SDImageCache.SharedImageCache.QueryDiskCache(adListing.ImageURL, (isInCache) =>
                        {
                            if (!isInCache)
                            {

                                if (counter < 4)
                                {
                                    var task = Task.Run(async () =>
                                    {
                                        var webClient = new WebClient();
                                        try
                                        {
                                            var url = new Uri(adListing.ImageURL);
                                            //Task.Run(async () =>
                                            //{

                                            var bytes = await webClient.DownloadDataTaskAsync(url);
                                            var data = NSData.FromArray(bytes);
                                            var uiimage = UIImage.LoadFromData(data);
                                            SDImageCache.SharedImageCache.StoreImage(image: uiimage, key: adListing.ImageURL);
                                        }
                                        catch (Exception e)
                                        {
                                            string breakPoint = e.Message;
                                        }
                                    });
                                    taskList.Add(task);
                                    //}).Wait();
                                }
                                else
                                {
                                    try
                                    {
                                        SDWebImageManager.SharedManager.Download(
                                        url: new NSUrl(adListing.ImageURL),
                                        options: SDWebImageOptions.ContinueInBackground | SDWebImageOptions.HighPriority,
                                        progressBlock: (recievedSize, expectedSize) =>
                                        {
                                            // Track progress...
                                        },
                                        completedBlock: (image, error, cacheType, finished, imageUrl) =>
                                        {
                                            //if (error != null)
                                            //{
                                            //	string breakPoint = "";
                                            //}
                                        });
                                    }
                                    catch (Exception e)
                                    {
                                        string breakPoint = e.Message;
                                    }

                                }

                            }
                            counter++;
                        });
                    }
                    catch (Exception e)
                    {
                        string breakPoint = e.Message;
                    }

                }
                if (taskList.Count > 0)
                {
                    //Why are we "waitall" here? need to test not waiting.
                    //Task.WaitAll(taskList.ToArray());
                    Task.WaitAny(taskList.ToArray());
                }
            });
        }

        //public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        //{
        //    // Get current device token
        //    var DeviceToken = deviceToken.Description;
        //    if (!string.IsNullOrWhiteSpace(DeviceToken))
        //    {
        //        DeviceToken = DeviceToken.Trim('<').Trim('>');
        //    }

        //    // Get previous device token
        //    var oldDeviceToken = NSUserDefaults.StandardUserDefaults.StringForKey("PushDeviceToken");

        //    // Has the token changed?
        //    if (string.IsNullOrEmpty(oldDeviceToken) || !oldDeviceToken.Equals(DeviceToken))
        //    {
        //        //TODO: Put your own logic here to notify your server that the device token has changed/been created!
        //    }

        //    // Save new device token 
        //    NSUserDefaults.StandardUserDefaults.SetString(DeviceToken, "PushDeviceToken");
        //}

        //perform data fetch here for the ads
        public override void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            // Check for new data, and display it


            // Inform system of fetch results... inform if data has been refreshed or an error has occurred 
            completionHandler(UIBackgroundFetchResult.NoData);
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

            //var classifications = AdList.Select(row => row.Classification).Distinct();
            //if (classifications.Count() > 0)
            //{
            //	var ad = new Ad();
            //	foreach (var classification in classifications)
            //	{
            //		ad.SaveAdsByClassification(AdList.Where(row => row.Classification == classification), classification);
            //	}
            //}
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


