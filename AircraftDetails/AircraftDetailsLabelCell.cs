using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class AircraftDetailsLabelCell : UITableViewCell
    {
        public AircraftDetailsLabelCell (IntPtr handle) : base (handle)
        {
            string bPoint = "";
        }

        public void UpdateCell(AircraftDetails item)
        {
            this.aircraftLabel.Text = item.Label;
            this.aircraftDetails.Text = item.Description;
        }
    }
}