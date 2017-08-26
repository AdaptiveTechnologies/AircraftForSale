using System;
using System.Drawing;
using AircraftForSale.PCL;
using CoreGraphics;
using UIKit;

namespace AircraftForSale
{
	public static class ExtensionMethods
	{
		

		// resize the image (without trying to maintain aspect ratio)
		public static UIImage ResizeImage(this UIImage sourceImage, float width, float height)
		{
			//Images must be considerably wider than they are tall
			//if ((height * 1.2f) >= width)
			//{
			//	height = width * .8f;
			//}

			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		public static UIImage ScaleImageToPreventHorizontalWhiteSpace(this UIImage sourceImage, nfloat newWidth, NSLayoutConstraint heightConstraint = null)
		{
			//Images must be considerably wider than they are tall
			bool makeImageSmaller = false;
			var origHeight = sourceImage.Size.Height;
			var origWidth = sourceImage.Size.Width;

			nfloat percentage = 0f;
			if ((origHeight * 1.2f) >= origWidth)
			{
				//origHeight = origWidth * .8f;
				makeImageSmaller = true;

				if (origHeight <= origWidth)
				{
					percentage = .8f;
				}
				else {
					percentage = origHeight / origWidth;
					percentage += .2f;
				}
			}

			var heightWidthRatio = origHeight / origWidth;
			var newHeight = newWidth * heightWidthRatio;

			if (makeImageSmaller)
			{
				newHeight = newHeight * (nfloat)percentage;
				newWidth = newWidth * (nfloat)percentage;
			}

			if (heightConstraint != null)
			{
				heightConstraint.Constant = newHeight;
			}

			var newSize = new CGSize(newWidth, newHeight);
			return sourceImage.Scale(newSize);
		}
	}
}
