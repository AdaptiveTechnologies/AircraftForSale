using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class PageIndicatorClass : UIView
    {
		public float Percentage
		{
			get;
			set;
		}

		public int CurrentIndex
		{
			get;
			set;
		}

		public int TotalPages
		{
			get;
			set;
		}

		public void SetIndicatorState(int currentIndex, int totalPages)
		{
			CurrentIndex = currentIndex;
			TotalPages = totalPages;

			if (currentIndex == 0 || ((totalPages - 1) < 1))
				Percentage = 0f;
			else {
				Percentage = (float)currentIndex / ((float)totalPages - 1);
			}

			SetNeedsDisplay();
		}

        public PageIndicatorClass (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			BackgroundColor = UIColor.Clear;
		}

		public override void Draw(CoreGraphics.CGRect rect)
		{
			MagazinePageIndicator.DrawCanvas1(rect, Percentage, CurrentIndex, TotalPages);
		}
    }
}