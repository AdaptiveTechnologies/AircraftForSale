using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;

namespace AircraftForSale
{
    public partial class SpecCell1 : UITableViewCell
    {
        public SpecCell1 (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			// Called when loaded from xib or storyboard.
			var device = UIDevice.CurrentDevice;
			if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				//var font = UIFont.SystemFontOfSize(17);
				//this.SpecNameLabel.Font = UIFont.BoldSystemFontOfSize(17);
				//this.SpecValueLabel.Font = UIFont.SystemFontOfSize(17);
				//NameValueSpacingConstraint.Constant = 10;
			}
		}

        public nfloat rowheight
        {
	       get;
			set;
		}

		public nfloat UpdateCell(string item)
		{;
			this.SpecValueLabel.Text = item;

            this.SpecValueLabel.TextColor = UIColor.White;
            this.SpecValueLabel.Lines = 0;
            this.SpecValueLabel.LineBreakMode = UILineBreakMode.WordWrap;

            var font = UIFont.SystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 20 : 17);

this.SpecValueLabel.Font = font;


			//var section = keys[indexPath.Section];

			if (item!=null && item.Length > 0)
			{

				float width = (float)this.SpecValueLabel.Frame.Width;
				CoreGraphics.CGPoint points = new CoreGraphics.CGPoint();
				points.X = this.SpecValueLabel.Frame.X;
				points.Y = this.SpecValueLabel.Frame.Y;

				var labelFrame = new CoreGraphics.CGRect(points, ((NSString)this.SpecValueLabel.Text).GetBoundingRect(
new CoreGraphics.CGSize(width, nfloat.MaxValue),
NSStringDrawingOptions.UsesLineFragmentOrigin,
new UIStringAttributes { Font = this.SpecValueLabel.Font }, null).Size);

				labelFrame.Width = width;
this.SpecValueLabel.Frame = labelFrame;
				return labelFrame.Height;
			}

			return 0f;
		}



		public void UpdateCell(SpecField item)
		{
			
			this.SpecValueLabel.Text = item.FieldName +": "+item.FieldValue;
		}
    }
}