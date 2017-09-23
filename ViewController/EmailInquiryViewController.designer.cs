// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;

namespace AircraftForSale
{
    [Register ("EmailInquiryViewController")]
    partial class EmailInquiryViewController
    {
        [Outlet]
        UIKit.UITextField PhoneTextField { get; set; }

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
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton SubmitButton { get; set; }

        void ReleaseDesignerOutlets ()
        {
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

            if (PhoneTextField != null) {
                PhoneTextField.Dispose ();
                PhoneTextField = null;
            }

            if (SubmitButton != null) {
                SubmitButton.Dispose ();
                SubmitButton = null;
            }
        }
    }
}