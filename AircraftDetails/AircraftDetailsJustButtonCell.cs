using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class AircraftDetailsJustButtonCell : UITableViewCell
    {
        public AircraftDetailsJustButtonCell (IntPtr handle) : base (handle)
        {
        }

		public void UpdateCell(AircraftDetails item)
        {

            this.button.SetTitle(item.Description, UIControlState.Normal);
        }
    }
}