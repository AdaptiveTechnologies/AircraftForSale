using Foundation;
using System;
using UIKit;
using Google.Analytics;

namespace AircraftForSale
{
    public partial class MagazineNavigationViewController : UINavigationController
    {
        public MagazineNavigationViewController (IntPtr handle) : base (handle)
        {
        }

public override void ViewDidAppear(bool animated)
{
	base.ViewDidAppear(animated);

	// This screen name value will remain set on the tracker and sent with
	// hits until it is set to a new value or to null.
	Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "Magazine View");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
}
    }
}