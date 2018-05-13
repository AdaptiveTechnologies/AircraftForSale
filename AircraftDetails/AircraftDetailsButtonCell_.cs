using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class AircraftDetailsButtonCell : UITableViewCell
    {
        public UIButton AircraftDescriptionButton
        {
            get
            {
                return this.aircraftDescription;
            }
        }
          
        public AircraftDetailsButtonCell (IntPtr handle) : base (handle)
        {
            
        }

        public void UpdateCell(AircraftDetails item)
        {

            this.aircraftLabel.Text = item.Label;
            this.aircraftDescription.SetTitle(item.Description, UIControlState.Normal);
        }
    }
}