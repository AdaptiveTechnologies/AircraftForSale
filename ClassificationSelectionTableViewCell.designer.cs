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
    [Register ("SelectClassificationTableViewCell")]
    partial class SelectClassificationTableViewCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView ClassificationImageView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel ClassificationLabel { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel InstructionLabel { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (ClassificationImageView != null) {
                ClassificationImageView.Dispose ();
                ClassificationImageView = null;
            }

            if (ClassificationLabel != null) {
                ClassificationLabel.Dispose ();
                ClassificationLabel = null;
            }

            if (InstructionLabel != null) {
                InstructionLabel.Dispose ();
                InstructionLabel = null;
            }
        }
    }
}