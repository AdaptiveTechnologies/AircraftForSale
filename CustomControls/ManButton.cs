using System;
using System.ComponentModel;
using CoreGraphics;
using Foundation;
using UIKit;
namespace AircraftForSale
{
	//[Register("ManufacturerButton"), DesignTimeVisible(true)]
	public class ManufacturerButton : UIButton
	{

		//[Export("BackgroundColorProperty"), Browsable(true)]
		//public UIColor BackgroundColorProperty { get; set; }

		public ManufacturerButton(IntPtr handle) : base(handle) { }

		public ManufacturerButton()
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
			Layer.BorderWidth = 1;
			Layer.CornerRadius = 2;
			Layer.BorderColor = UIColor.White.CGColor;
			//this.ContentEdgeInsets = new UIEdgeInsets(5, 0, 5, 0);
		}

		public override bool PointInside(CoreGraphics.CGPoint point, UIEvent uievent)
		{
			var margin = -10f;
			var area = this.Bounds;
			var expandedArea = area.Inset(margin, margin);
			return expandedArea.Contains(point);
		}

	}
}


