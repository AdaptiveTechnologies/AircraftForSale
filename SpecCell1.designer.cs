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
    [Register ("SpecCell1")]
    partial class SpecCell1
    {

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SpecValueLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
          

            if (SpecValueLabel != null) {
                SpecValueLabel.Dispose ();
                SpecValueLabel = null;
            }
        }
    }
}