// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AircraftForSale
{
	[Register ("ChangePassViewController")]
	partial class ChangePassViewController
	{
		[Outlet]
		UIKit.UITextField confirmpassEdt { get; set; }

		[Outlet]
		UIKit.UITextField nPassEdt { get; set; }

		[Outlet]
		UIKit.UITextField oldPassEdt { get; set; }

		[Outlet]
		UIKit.UIButton submitBtn { get; set; }

		[Action ("closeAction:")]
		partial void closeAction (Foundation.NSObject sender);

		[Action ("submitpassword:")]
		partial void submitpassword (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (confirmpassEdt != null) {
				confirmpassEdt.Dispose ();
				confirmpassEdt = null;
			}

			if (nPassEdt != null) {
				nPassEdt.Dispose ();
				nPassEdt = null;
			}

			if (oldPassEdt != null) {
				oldPassEdt.Dispose ();
				oldPassEdt = null;
			}

			if (submitBtn != null) {
				submitBtn.Dispose ();
				submitBtn = null;
			}
		}
	}
}
