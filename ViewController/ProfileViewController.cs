using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class ProfileViewController : UIViewController
	{
		//partial void UIButton27_TouchUpInside(UIButton sender)
		//{
		//	// Launches a new instance of CallHistoryController
  // 			 RegistrationViewController regViewController = this.Storyboard.InstantiateViewController("RegistrationViewController") as RegistrationViewController;
		//	if (regViewController != null)
		//	{
		//		//callHistory.PhoneNumbers = PhoneNumbers;
		//		//this.NavigationController.PushViewController(regViewController, true);
		//		//this.TabBarController.NavigationController.PushViewController(regViewController, true);
		//	}
		//}

		partial void UIButton26_TouchUpInside(UIButton sender)
		{
			LaterButton.Hidden = true;
		}

		partial void LaterAction(UIButton sender)
		{
			LaterButton.Hidden = true;
		}

		public ProfileViewController (IntPtr handle) : base (handle)
        {

        }

		public override void ViewDidLoad()
		{
			if (ParentViewController != null)
			{
				LaterButton.Hidden = true;
			}
		}
    }
}