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
    [Register ("SpecViewController_")]
    partial class SpecViewController_
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AircraftLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel CategoryLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ClassificationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton CloseButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ConklinImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel SpecDescriptionLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView SpecImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SpecTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AircraftLabel != null) {
                AircraftLabel.Dispose ();
                AircraftLabel = null;
            }

            if (CategoryLabel != null) {
                CategoryLabel.Dispose ();
                CategoryLabel = null;
            }

            if (ClassificationLabel != null) {
                ClassificationLabel.Dispose ();
                ClassificationLabel = null;
            }

            if (CloseButton != null) {
                CloseButton.Dispose ();
                CloseButton = null;
            }

            if (ConklinImageView != null) {
                ConklinImageView.Dispose ();
                ConklinImageView = null;
            }

            if (SpecDescriptionLabel != null) {
                SpecDescriptionLabel.Dispose ();
                SpecDescriptionLabel = null;
            }

            if (SpecImage != null) {
                SpecImage.Dispose ();
                SpecImage = null;
            }

            if (SpecTableView != null) {
                SpecTableView.Dispose ();
                SpecTableView = null;
            }
        }
    }
}