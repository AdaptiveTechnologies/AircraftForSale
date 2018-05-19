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
	[Register ("ContactWebViewController")]
	partial class ContactWebViewController
	{
		[Outlet]
		UIKit.UITextField addressEdt { get; set; }

		[Outlet]
		UIKit.UIButton checkbox { get; set; }

		[Outlet]
		UIKit.UITextView commentsEdt { get; set; }

		[Outlet]
		UIKit.UITextField nameEdt { get; set; }

		[Outlet]
		UIKit.UITextField phnoEdt { get; set; }

		[Outlet]
		UIKit.UIButton submitButton { get; set; }

		[Action ("setChecked:")]
		partial void setChecked (Foundation.NSObject sender);

		[Action ("submitAction:")]
		partial void submitAction (Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (addressEdt != null) {
				addressEdt.Dispose ();
				addressEdt = null;
			}

			if (checkbox != null) {
				checkbox.Dispose ();
				checkbox = null;
			}

			if (commentsEdt != null) {
				commentsEdt.Dispose ();
				commentsEdt = null;
			}

			if (nameEdt != null) {
				nameEdt.Dispose ();
				nameEdt = null;
			}

			if (phnoEdt != null) {
				phnoEdt.Dispose ();
				phnoEdt = null;
			}

			if (submitButton != null) {
				submitButton.Dispose ();
				submitButton = null;
			}
		}
	}
}
