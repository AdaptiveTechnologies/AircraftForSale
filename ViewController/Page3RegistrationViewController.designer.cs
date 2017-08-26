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
    [Register ("Page3RegistrationViewController")]
    partial class Page3RegistrationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView ClassPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CompleteRegistrationButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel DesigLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView DesigPickerView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField HoursTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ManufactureLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView ManufacturePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView PurposePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView SortOptionsPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView TimeframePicker { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ClassPicker != null) {
                ClassPicker.Dispose ();
                ClassPicker = null;
            }

            if (CompleteRegistrationButton != null) {
                CompleteRegistrationButton.Dispose ();
                CompleteRegistrationButton = null;
            }

            if (DesigLabel != null) {
                DesigLabel.Dispose ();
                DesigLabel = null;
            }

            if (DesigPickerView != null) {
                DesigPickerView.Dispose ();
                DesigPickerView = null;
            }

            if (HoursTextField != null) {
                HoursTextField.Dispose ();
                HoursTextField = null;
            }

            if (ManufactureLabel != null) {
                ManufactureLabel.Dispose ();
                ManufactureLabel = null;
            }

            if (ManufacturePicker != null) {
                ManufacturePicker.Dispose ();
                ManufacturePicker = null;
            }

            if (PurposePicker != null) {
                PurposePicker.Dispose ();
                PurposePicker = null;
            }

            if (SortOptionsPicker != null) {
                SortOptionsPicker.Dispose ();
                SortOptionsPicker = null;
            }

            if (TimeframePicker != null) {
                TimeframePicker.Dispose ();
                TimeframePicker = null;
            }
        }
    }
}