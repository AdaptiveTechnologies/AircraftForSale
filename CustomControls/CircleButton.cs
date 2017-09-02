using System;
using System.ComponentModel;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	//[Register("CircleButton"), DesignTimeVisible(true)]
	public class CircleButton : UIButton
	{

		//[Export("BackgroundColorProperty"), Browsable(true)]
		public UIColor BackgroundColorProperty { get; set; }

		public CircleButton(IntPtr handle) : base(handle) { }

		public CircleButton()
		{
			// Called when created from code.
			Initialize();
		}

		public override void AwakeFromNib()
		{
			// Called when loaded from xib or storyboard.
			Initialize();
		}

		void Initialize()
		{
			// Common initialization code here.
			//BackgroundColorProperty = UIColor.FromRGB(255, 140, 0);
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();


			// define coordinates and size of the circular view

			// corner radius needs to be one half of the size of the view
			nfloat cornerRadius = this.Bounds.Width / 2;
			//CoreGraphics.CGRect frame = new CoreGraphics.CGRect(0, 0, this.Bounds.Width, this.Bounds.Height);
			//CircleButton.Frame = frame;
			// set corner radius
			this.Layer.CornerRadius = cornerRadius;
			// set background color, border color and width to see the circular view
			//CircleButton.BackgroundColor = UIColor.White;
			this.BackgroundColor = BackgroundColorProperty;
			this.Layer.CornerRadius = cornerRadius;
			this.Layer.BorderColor = UIColor.LightGray.CGColor;
			this.Layer.BorderWidth = 1;

		}
	}
}


