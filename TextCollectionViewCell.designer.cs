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
    [Register ("TextCollectionViewCell")]
    partial class TextCollectionViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AdDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView AircraftImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel AircraftNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel BrokerNameLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ManufacturerLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel PriceLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (AdDescription != null) {
                AdDescription.Dispose ();
                AdDescription = null;
            }

            if (AircraftImage != null) {
                AircraftImage.Dispose ();
                AircraftImage = null;
            }

            if (AircraftNameLabel != null) {
                AircraftNameLabel.Dispose ();
                AircraftNameLabel = null;
            }

            if (BrokerNameLabel != null) {
                BrokerNameLabel.Dispose ();
                BrokerNameLabel = null;
            }

            if (ManufacturerLabel != null) {
                ManufacturerLabel.Dispose ();
                ManufacturerLabel = null;
            }

            if (PriceLabel != null) {
                PriceLabel.Dispose ();
                PriceLabel = null;
            }
        }
    }
}