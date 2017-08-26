using System;
using System.ComponentModel;
using CoreGraphics;
using CoreText;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	[Register("LikeManufacturerView"), DesignTimeVisible(true)]
	public class LikeManufacturerView : UIView
	{
		UIImage image;

		[Export("Image"), Browsable(true)]
		public UIImage Image
		{
			get { return image; }
			set
			{
				image = value;
				SetNeedsDisplay();
			}
		}


		public LikeManufacturerView()
		{
			Initialize();
		}
		public LikeManufacturerView(IntPtr p) : base(p)
		{
			Initialize();
		}

		void Initialize()
		{
			BackgroundColor = UIColor.White.ColorWithAlpha(.8f);
			Layer.CornerRadius = (float)(Bounds.Size.Height/3);
			ClipsToBounds = true;
			Opaque = false;

			SetNeedsDisplay();
		}

	

		public override void Draw(CGRect rect)
		{
			base.Draw(rect);

			using (var g = UIGraphics.GetCurrentContext())
			{
				// scale and translate the CTM so the image appears upright
				g.ScaleCTM(1, -1);
				g.TranslateCTM(0, -Bounds.Height);
				if (image != null)
				{
					nfloat imageWidth = image.Size.Width;
					nfloat imageHeight = image.Size.Height;
					var centerX = Bounds.Width / 2 - (imageWidth / 2);
					var centerY = (Bounds.Height / 2 - (imageHeight / 2));
					CGRect imageRect = new CGRect(centerX, centerY - (centerY/3), imageWidth, imageHeight);
					g.DrawImage(imageRect, image.CGImage);
				}

				g.TranslateCTM(10, .8f * Bounds.Height);
				g.SetFillColor(UIColor.Black.CGColor);
				string someText = "Like this manufacturer?";

				var attributedString = new NSAttributedString(someText,
				   new CTStringAttributes
				   {
					   ForegroundColorFromContext = true,
					   Font = new CTFont("Arial", 12)
				   });

				using (var textLine = new CTLine(attributedString))
				{
					textLine.Draw(g);
				}
			}
		}
	}
}
