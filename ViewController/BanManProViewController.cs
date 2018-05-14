using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using SDWebImage;
using Google.Analytics;
using System.Net;
using System.Threading.Tasks;
using System.IO;

namespace AircraftForSale
{
    public partial class BanManProViewController : UIViewController, IAdLayoutInterface
    {
        public BanManProViewController(IntPtr handle) : base(handle)
        {
        }

        WeakReference parent;
        public MagazinePage DataObject
        {
            get
            {
                return parent.Target as MagazinePage;
            }
            set
            {
                parent = new WeakReference(value);
            }
        }

        void TapImageAction1(UITapGestureRecognizer tap)
        {
            if (Reachability.IsHostReachable(Settings._baseDomain))
            {
                LoadingOverlay loadingOverlay = new LoadingOverlay(View.Frame);
                var webView = new UIWebView(View.Frame);


                webView.LoadFinished += (sender, e) =>
                {
                    loadingOverlay.Hide();
                };

                var banManProHREF = Settings.BanManProURL.Replace("Task=Get", "Task=Click");
                ;
                webView.LoadRequest(new NSUrlRequest(new NSUrl(banManProHREF)));

                UIView.BeginAnimations("fadeflag");
                UIView.Animate(1, () =>
                {
                    tap.View.Alpha = .5f;
                }, () =>
                {

                    View.AddSubview(webView);
                    View.AddSubview(loadingOverlay);

                    UIButton closeButton = new UIButton(new CGRect(View.Bounds.Width - 75, 0, 75, 50));
                    closeButton.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
                    closeButton.BackgroundColor = UIColor.White.ColorWithAlpha(.5f);
                    closeButton.TouchUpInside += (sender, e) =>
                    {
                        try
                        {
                            webView.RemoveFromSuperview();
                            closeButton.RemoveFromSuperview();
                        }
                        finally
                        {
                            webView.Dispose();
                        }
                    };

                    UIImageView imageView = new UIImageView(new CGRect(View.Bounds.Width - 75, View.Bounds.Height / 2, 75, 50));
                    imageView.Image = UIImage.FromBundle("swipe_left");
                    imageView.Alpha = .5f;
                    webView.AddSubview(closeButton);
                    webView.AddSubview(imageView);

                    tap.View.Alpha = 1f;
                });

                UIView.CommitAnimations();
            }
            else
            {
                HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
            }
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // This screen name value will remain set on the tracker and sent with
            // hits until it is set to a new value or to null.
            Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "BanMan View");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
        }

        UITapGestureRecognizer tapGesture1;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
         
            if (Reachability.IsHostReachable(Settings._baseDomain))
            {
                LoadingOverlay loadingOverlay = new LoadingOverlay(View.Frame);
                View.Add(loadingOverlay);;
                Task.Run(() =>
               {
                   var image = HelperMethods.FromUrl(Settings.BanManProURL);
                   BeginInvokeOnMainThread(() =>
                   {
                        loadingOverlay.Hide();
                       BanManProImageView.Image = image;
                   });
                   
                });
            }
            else
            {
                BanManProImageView.Image = UIImage.FromBundle("ad_placeholder.jpg");
            }

            tapGesture1 = new UITapGestureRecognizer(TapImageAction1);

            int currentIndex = this.DataObject.MagazinePageIndex;
            int totalPages = this.DataObject.TotalPages;

            PageIndicator.SetIndicatorState(currentIndex, totalPages);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            BanManProImageView.AddGestureRecognizer(tapGesture1);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);

            BanManProImageView.RemoveGestureRecognizer(tapGesture1);
        }
    }
}