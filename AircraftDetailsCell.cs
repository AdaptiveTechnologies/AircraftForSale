using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;

namespace AircraftForSale
{
    public partial class AircraftDetailsCell : UITableViewCell
    {
        public AircraftDetailsCell (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			// Called when loaded from xib or storyboard.
			var device = UIDevice.CurrentDevice;
			if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{

			}
		}

        public nfloat rowheight
        {
	       get;
			set;
		}

		

		public void UpdateCell(AircraftDetails item)
		{
			

		}
    }
}