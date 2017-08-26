using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	public class CustomAdCell : UITableViewCell
	{
		UILabel headingLabel, subheadingLabel;
		UIImageView imageView;
		public CustomAdCell(NSString cellId) : base(UITableViewCellStyle.Default, cellId)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Gray;
			ContentView.BackgroundColor = UIColor.FromRGBA(152, 251, 152, 50);
			imageView = new UIImageView();
			headingLabel = new UILabel()
			{
				Font = UIFont.FromName("Helvetica-Bold", 22f),
				TextColor = UIColor.Black,
				BackgroundColor = UIColor.Clear
			};
			subheadingLabel = new UILabel()
			{
				Font = UIFont.FromName("Helvetica-Bold", 12f),
				TextColor = UIColor.Gray,
				BackgroundColor = UIColor.Clear
			};
			ContentView.AddSubviews(new UIView[] { headingLabel, subheadingLabel, imageView });

		}
		public void UpdateCell(string caption, string subtitle, UIImage image)
		{
			imageView.Image = image;
			headingLabel.Text = caption;
			subheadingLabel.Text = subtitle;
		}
		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			var rightLeftMargin = 10;
			var imageWidth = 100;
			var imageHeight = 50;
			imageView.Frame = new CGRect(ContentView.Bounds.Width - (imageWidth + rightLeftMargin), rightLeftMargin, imageWidth, imageHeight);

			var labelWidth = (ContentView.Bounds.Width - imageWidth - (rightLeftMargin * 3)) / 2;
			headingLabel.Frame = new CGRect(rightLeftMargin, rightLeftMargin, labelWidth - rightLeftMargin, imageHeight);
			subheadingLabel.Frame = new CGRect(labelWidth + rightLeftMargin, rightLeftMargin, labelWidth, imageHeight);
		}
	}
}
