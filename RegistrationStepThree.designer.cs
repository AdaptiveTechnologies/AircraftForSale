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
        UIKit.UITextField OrderByTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField TimeFrameTextField { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField WhyFlyTextField { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (HoursTextView != null) {
                HoursTextView.Dispose ();
                HoursTextView = null;
            }

            if (OrderByTextField != null) {
                OrderByTextField.Dispose ();
                OrderByTextField = null;
            }

            if (TimeFrameTextField != null) {
                TimeFrameTextField.Dispose ();
                TimeFrameTextField = null;
            }

            if (WhyFlyTextField != null) {
                WhyFlyTextField.Dispose ();
                WhyFlyTextField = null;
            }
        }
    }
}