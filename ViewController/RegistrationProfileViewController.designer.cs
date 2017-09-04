// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace AircraftForSale
{
    [Register ("RegistrationProfileViewController")]
    partial class RegistrationProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField CompanyTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView ContainerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton dismissButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton finishButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField FirstNameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField LastNameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView LocationsPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView MapCCodePickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField MobilePhoneTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PageHeaderLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISegmentedControl PilotSegmentControl { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIScrollView ProfileScrollView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (CompanyTextField != null) {
                CompanyTextField.Dispose ();
                CompanyTextField = null;
            }

            if (ContainerView != null) {
                ContainerView.Dispose ();
                ContainerView = null;
            }

            if (dismissButton != null) {
                dismissButton.Dispose ();
                dismissButton = null;
            }

            if (finishButton != null) {
                finishButton.Dispose ();
                finishButton = null;
            }

            if (FirstNameTextField != null) {
                FirstNameTextField.Dispose ();
                FirstNameTextField = null;
            }

            if (LastNameTextField != null) {
                LastNameTextField.Dispose ();
                LastNameTextField = null;
            }

            if (LocationsPicker != null) {
                LocationsPicker.Dispose ();
                LocationsPicker = null;
            }

            if (MapCCodePickerView != null) {
                MapCCodePickerView.Dispose ();
                MapCCodePickerView = null;
            }

            if (MobilePhoneTextField != null) {
                MobilePhoneTextField.Dispose ();
                MobilePhoneTextField = null;
            }

            if (PageHeaderLabel != null) {
                PageHeaderLabel.Dispose ();
                PageHeaderLabel = null;
            }

            if (PilotSegmentControl != null) {
                PilotSegmentControl.Dispose ();
                PilotSegmentControl = null;
            }

            if (ProfileScrollView != null) {
                ProfileScrollView.Dispose ();
                ProfileScrollView = null;
            }
        }
    }
}