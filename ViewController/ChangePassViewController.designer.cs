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
    [Register ("ChangePassViewController")]
    partial class ChangePassViewController
    {
        [Outlet]
        UIKit.UITextField confirmpassEdt { get; set; }


        [Outlet]
        UIKit.UITextField nPassEdt { get; set; }


        [Outlet]
        UIKit.UITextField oldPassEdt { get; set; }


        [Outlet]
        UIKit.UIButton submitBtn { get; set; }


        [Action ("closeAction:")]
        partial void closeAction (Foundation.NSObject sender);


        [Action ("submitpassword:")]
        partial void submitpassword (Foundation.NSObject sender);

        void ReleaseDesignerOutlets ()
        {
        }
    }
}