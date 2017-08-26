using System;

using UIKit;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class ViewController : UIViewController
	{
		protected ViewController(IntPtr handle) : base(handle)
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
}
	}
}

