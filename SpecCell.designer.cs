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
    [Register ("SpecCell")]
    partial class SpecCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SpecNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SpecValueLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SpecNameLabel != null) {
                SpecNameLabel.Dispose ();
                SpecNameLabel = null;
            }

            if (SpecValueLabel != null) {
                SpecValueLabel.Dispose ();
                SpecValueLabel = null;
            }
        }
    }
}