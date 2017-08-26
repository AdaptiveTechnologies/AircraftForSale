using System;

using UIKit;
using Google.Analytics;
using VideoSplash;


namespace AircraftForSale
{
	public partial class SplashViewController : VideoViewController
	{
		protected SplashViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}



		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "View Controller");

			Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());

			var bundle =  Foundation.NSBundle.MainBundle;
			//var resource = bundle.PathForResource("splashipad", "mp4");

			var fileName = (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)? "globalairphone" : "globalairtablet";

			var resource = bundle.PathForResource(fileName, "mov");

// set the video
			VideoUrl = new Foundation.NSUrl(resource, false);

			FillMode = ScalingMode.ResizeAspectFill;

		}

		public override void OnVideoReady()
		{
			base.OnVideoReady();
			PlayVideo();
		}

		public override void OnVideoComplete()
		{
			base.OnVideoComplete();

            this.PerformSegue("loginSegue", this);
		}
	}
}

