// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace AircraftForSale
{
    [Register ("RegistrationStepThree")]
    partial class RegistrationStepThree
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField HoursTextView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView OrderByPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView PurposePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView TimeframePicker { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (HoursTextView != null) {
                HoursTextView.Dispose ();
                HoursTextView = null;
            }

            if (OrderByPicker != null) {
                OrderByPicker.Dispose ();
                OrderByPicker = null;
            }

            if (PurposePicker != null) {
                PurposePicker.Dispose ();
                PurposePicker = null;
            }

            if (TimeframePicker != null) {
                TimeframePicker.Dispose ();
                TimeframePicker = null;
            }
        }
    }
}