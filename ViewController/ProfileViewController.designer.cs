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
    [Register ("ProfileViewController")]
    partial class ProfileViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LaterButton { get; set; }

        [Action ("LaterAction:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LaterAction (UIKit.UIButton sender);

        [Action ("UIButton26_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void UIButton26_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (LaterButton != null) {
                LaterButton.Dispose ();
                LaterButton = null;
            }
        }
    }
}