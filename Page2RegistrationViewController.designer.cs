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
    [Register ("Page2RegistrationViewController")]
    partial class Page2RegistrationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NextButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch PilotSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel RatingLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView RatingPickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel TypeLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView TypePickerView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (NextButton != null) {
                NextButton.Dispose ();
                NextButton = null;
            }

            if (PilotSwitch != null) {
                PilotSwitch.Dispose ();
                PilotSwitch = null;
            }

            if (RatingLabel != null) {
                RatingLabel.Dispose ();
                RatingLabel = null;
            }

            if (RatingPickerView != null) {
                RatingPickerView.Dispose ();
                RatingPickerView = null;
            }

            if (TypeLabel != null) {
                TypeLabel.Dispose ();
                TypeLabel = null;
            }

            if (TypePickerView != null) {
                TypePickerView.Dispose ();
                TypePickerView = null;
            }
        }
    }
}