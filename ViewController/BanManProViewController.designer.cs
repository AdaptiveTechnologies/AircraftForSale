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
    [Register ("BanManProViewController")]
    partial class BanManProViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView BanManProImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        AircraftForSale.PageIndicatorClass PageIndicator { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (BanManProImageView != null) {
                BanManProImageView.Dispose ();
                BanManProImageView = null;
            }

            if (PageIndicator != null) {
                PageIndicator.Dispose ();
                PageIndicator = null;
            }
        }
    }
}