using System;

using UIKit;

namespace AircraftForSale
{
	public partial class ContactViewController : UIViewController
	{
		public ContactViewController() : base("ContactViewController", null)
		{
		}
        
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
            

            
			// Perform any additional setup after loading the view, typically from a nib.
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
      string url = "https://globalair.com/myaircraft/rates.aspx";
      webView.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(url)));
			webView.ScalesPageToFit = true;

		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}
	}
}

