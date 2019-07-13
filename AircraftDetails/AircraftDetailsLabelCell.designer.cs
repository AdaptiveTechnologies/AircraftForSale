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
    [Register ("AircraftDetailsLabelCell")]
    partial class AircraftDetailsLabelCell
    {
        [Outlet]
        UIKit.UILabel aircraftDetails { get; set; }


        [Outlet]
        UIKit.UILabel aircraftLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel aircraftDescription { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (aircraftDescription != null) {
                aircraftDescription.Dispose ();
                aircraftDescription = null;
            }
        }
    }
}