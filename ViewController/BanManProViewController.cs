using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using SDWebImage;
using Google.Analytics;

namespace AircraftForSale
{
    public partial class BanManProViewController : UIViewController, IAdLayoutInterface
    {
        public BanManProViewController (IntPtr handle) : base (handle)
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
			else {
				HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
			}
		}

public override void ViewDidAppear(bool animated)
{
	base.ViewDidAppear(animated);

	// This screen name value will remain set on the tracker and sent with
	// hits until it is set to a new value or to null.
	Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "BanMan View");

	Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());
}

		UITapGestureRecognizer tapGesture1;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			//if (Reachability.IsHostReachable(Settings._baseDomain))
			//{
			//LoadingOverlay loadingOverlay = new LoadingOverlay(View.Frame);

			//BanManPropWebView.LoadFinished += (sender, e) =>
			//{
			//	//CGSize contentSize = BanManPropWebView.ScrollView.ContentSize;
			//	//CGSize viewSize = BanManPropWebView.Bounds.Size;

			//	////float rw = (float)viewSize.Width / (float)contentSize.Width;
			//	//float rw = .5f;

			//	// BanManPropWebView.ScrollView.MinimumZoomScale = rw;
			//	//BanManPropWebView.ScrollView.MaximumZoomScale = rw;
			//	//BanManPropWebView.ScrollView.ZoomScale = rw;

			//	//BanManPropWebView.ScalesPageToFit = true;
			//	//string html2 = BanManPropWebView.EvaluateJavascript("document.body.innerHTML");
			//	loadingOverlay.Hide();
			//};

			//var banManProURL = "https://www.globalair.com/banmanpro/ad.aspx?ZoneID=94&Task=Get&Mode=HTML&SiteID=1&PageID=78751";
			//var banManProHREF = banManProURL.Replace("Task=Get", "Task=Click");

			//BanManProImageView.SetImage(
			//	url: new NSUrl(banManProURL),
			//	placeholder: UIImage.FromBundle("ad_placeholder.jpg")
			//);

			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				BanManProImageView.Image = HelperMethods.FromUrl(Settings.BanManProURL);
			}
			else {
				BanManProImageView.Image = UIImage.FromBundle("ad_placeholder.jpg");
			}

				tapGesture1 = new UITapGestureRecognizer(TapImageAction1);

				//BanManPropWebView.ScalesPageToFit = true;
				//BanManPropWebView.ScrollView.ScrollEnabled = false;
				//BanManPropWebView.ContentMode = UIViewContentMode.ScaleAspectFit;

				////BanManPropWebView.LoadHtmlString(html, null);
				//BanManPropWebView.LoadRequest(new NSUrlRequest(new NSUrl(banManProURL)));



				//UITapGestureRecognizer tapGesture = null;

				//Action action = () =>
				//{
				//	BanManPropWebView.LoadRequest(new NSUrlRequest(new NSUrl(banManProHREF)));
				//};

				//tapGesture = new UITapGestureRecognizer(action);
				//tapGesture.NumberOfTapsRequired = 1;
				//// t_wbview.AddGestureRecognizer(tapGesture);
				//BanManPropWebView.ScrollView.contentAddGestureRecognizer(tapGesture);
				//View.AddSubview(t_wbview); 

		
				//View.AddSubview(loadingOverlay);
			//}
			//else {
			//	HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
			//}

			int currentIndex = this.DataObject.MagazinePageIndex;
			int totalPages = this.DataObject.TotalPages;

			//if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			//{
			//	LeadingPageIndocatorConstraint.Constant = 50;
			//	TrailingPageIndicatorConstraint.Constant = 50;
			//}
			//else {
			//	LeadingPageIndocatorConstraint.Constant = 10;
			//	TrailingPageIndicatorConstraint.Constant = 10;
			//}

			PageIndicator.SetIndicatorState(currentIndex, totalPages);
			//View.BringSubviewToFront(PageIndicator);
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