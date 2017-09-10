using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;
using System.Collections.Generic;

namespace AircraftForSale
{
    public partial class SelectClassificationTableViewCell : UITableViewCell
    {
        public SelectClassificationTableViewCell (IntPtr handle) : base (handle)
        {
			UIView BGViewColor = new UIView();
			BGViewColor.BackgroundColor = UIColor.Yellow.ColorWithAlpha(.3f); //or whatever color you want.
			BGViewColor.Layer.MasksToBounds = true;
			SelectedBackgroundView = BGViewColor;
        }



        public void UpdateData(IClassification classification)
        {
			this.ClassificationLabel.Text = classification.Name;
			this.ClassificationImageView.Image = UIImage.FromBundle(classification.Image);
			this.InstructionLabel.Text = "Tap to include this category in your collection";
        }
    }
}