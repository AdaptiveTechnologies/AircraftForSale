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
	[Register ("EmailInquiryViewController")]
	partial class EmailInquiryViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton CheckButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton CloseButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextView CommentsTextView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UILabel ContactBrokerLabel { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField EmailAddressTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIView EmailInquiryView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UITextField NameTextField { get; set; }

		[Outlet]
		UIKit.UITextField PhoneTextField { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIKit.UIButton SubmitButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PhoneTextField != null) {
				PhoneTextField.Dispose ();
				PhoneTextField = null;
			}

			if (CheckButton != null) {
				CheckButton.Dispose ();
				CheckButton = null;
			}

			if (CloseButton != null) {
				CloseButton.Dispose ();
				CloseButton = null;
			}

			if (CommentsTextView != null) {
				CommentsTextView.Dispose ();
				CommentsTextView = null;
			}

			if (ContactBrokerLabel != null) {
				ContactBrokerLabel.Dispose ();
				ContactBrokerLabel = null;
			}

			if (EmailAddressTextField != null) {
				EmailAddressTextField.Dispose ();
				EmailAddressTextField = null;
			}

			if (EmailInquiryView != null) {
				EmailInquiryView.Dispose ();
				EmailInquiryView = null;
			}

			if (NameTextField != null) {
				NameTextField.Dispose ();
				NameTextField = null;
			}

			if (SubmitButton != null) {
				SubmitButton.Dispose ();
				SubmitButton = null;
			}
		}
	}
}
