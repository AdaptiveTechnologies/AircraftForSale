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

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
		public static UIImage MaxResizeImage(this UIImage sourceImage, float maxWidth, float maxHeight)
		{
            //if(sourceImage == null){
            //    return new UIImage();
            //}
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Min(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new CGSize(width, height));
			sourceImage.Draw(new CGRect(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}
	}
}
