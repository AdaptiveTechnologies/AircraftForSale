using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;

namespace AircraftForSale
{
    public partial class SpecCell : UITableViewCell
    {
        public SpecCell (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			// Called when loaded from xib or storyboard.
			var device = UIDevice.CurrentDevice;

			if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				//var font = UIFont.SystemFontOfSize(12);
				//this.SpecNameLabel.Font = UIFont.BoldSystemFontOfSize(17);
				//this.SpecValueLabel.Font = UIFont.SystemFontOfSize(17);
				//NameValueSpacingConstraint.Constant = 10;
			}
		}

		public void UpdateCell(string item)
		{;
			this.SpecValueLabel.Text = item;


		}





		public void UpdateCell(SpecField item)
		{
			this.SpecNameLabel.Text = item.FieldName;
			this.SpecValueLabel.Text = item.FieldValue;
		}
    }
}