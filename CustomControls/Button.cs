using System;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	[Register("Button")]
	public class Button : UIButton
	{
		public Button(IntPtr handle) : base(handle)
		{
			AdjustsImageWhenHighlighted = false;
			//TintColor = UIColor.Clear;

		}

		protected void SetBackgroundColorForState(UIControlState state)
		{
			switch (state)
			{
				case UIControlState.Normal:
					{
						BackgroundColor = UIColor.Clear;
						break;
					}
				case UIControlState.Highlighted:
					{
						BackgroundColor = UIColor.Clear;

						break;
					}
				case UIControlState.Disabled:
					{
						BackgroundColor = UIColor.Gray;

						break;
					}
				case UIControlState.Selected:
					{
						BackgroundColor = UIColor.Clear;

						break;
					}
			}
		}
		public override bool Enabled
		{
			get
			{
				return base.Enabled;
			}
			set
			{
				base.Enabled = value;
				var state = value ? UIControlState.Normal : UIControlState.Disabled;
				SetBackgroundColorForState(state);
			}
		}
		public override bool Selected
		{
			get
			{
				return base.Selected;
			}
			set
			{
				base.Selected = value;
				var state = value ? UIControlState.Selected : UIControlState.Normal;
				SetBackgroundColorForState(state);
			}
		}
		public override bool Highlighted
		{
			get
			{
				return base.Highlighted;
			}
			set
			{
				base.Highlighted = value;
				var state = value ? UIControlState.Highlighted : UIControlState.Normal;
				SetBackgroundColorForState(state);
			}
		}
	}
}
