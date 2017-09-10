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
    [Register ("RegistrationStepTwo")]
    partial class RegistrationStepTwo
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView ClassificationPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView ManufacturerPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView ModelPicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISwitch PilotSwitch { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView PilotTypePicker { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIPickerView RatingPicker { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ClassificationPicker != null) {
                ClassificationPicker.Dispose ();
                ClassificationPicker = null;
            }

            if (ManufacturerPicker != null) {
                ManufacturerPicker.Dispose ();
                ManufacturerPicker = null;
            }

            if (ModelPicker != null) {
                ModelPicker.Dispose ();
                ModelPicker = null;
            }

            if (PilotSwitch != null) {
                PilotSwitch.Dispose ();
                PilotSwitch = null;
            }

            if (PilotTypePicker != null) {
                PilotTypePicker.Dispose ();
                PilotTypePicker = null;
            }

            if (RatingPicker != null) {
                RatingPicker.Dispose ();
                RatingPicker = null;
            }
        }
    }
}