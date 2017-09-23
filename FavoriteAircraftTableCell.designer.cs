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
    [Register ("FavoriteAircraftTableCell")]
    partial class FavoriteAircraftTableCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AircraftDetailsLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView AircraftImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AircraftNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton NotesButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel NotesLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AircraftDetailsLabel != null) {
                AircraftDetailsLabel.Dispose ();
                AircraftDetailsLabel = null;
            }

            if (AircraftImage != null) {
                AircraftImage.Dispose ();
                AircraftImage = null;
            }

            if (AircraftNameLabel != null) {
                AircraftNameLabel.Dispose ();
                AircraftNameLabel = null;
            }

            if (NotesButton != null) {
                NotesButton.Dispose ();
                NotesButton = null;
            }

            if (NotesLabel != null) {
                NotesLabel.Dispose ();
                NotesLabel = null;
            }
        }
    }
}