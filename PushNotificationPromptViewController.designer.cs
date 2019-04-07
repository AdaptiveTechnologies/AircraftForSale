// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AircraftForSale
{
	[Register ("PushNotificationPromptViewController")]
	partial class PushNotificationPromptViewController
	{
		[Outlet]
		UIKit.UIView backgroundView { get; set; }

		[Outlet]
		UIKit.UIButton noButton { get; set; }

		[Outlet]
		UIKit.UIButton yesButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (yesButton != null) {
				yesButton.Dispose ();
				yesButton = null;
			}

			if (noButton != null) {
				noButton.Dispose ();
				noButton = null;
			}

			if (backgroundView != null) {
				backgroundView.Dispose ();
				backgroundView = null;
			}
		}
	}
}
