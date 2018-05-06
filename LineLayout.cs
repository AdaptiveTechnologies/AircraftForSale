using System;
using CoreAnimation;
using CoreGraphics;
using UIKit;

namespace AircraftForSale
{

	public partial class LineLayout : UICollectionViewFlowLayout
	{
		//public static float ITEM_SIZE = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200.0f : 100.0f;
		public static int ACTIVE_DISTANCE = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200 : 100;
		public const float ZOOM_FACTOR = 0.3f;

		public LineLayout()
		{
			//var itemSize = ((float)UIScreen.MainScreen.Bounds.Height * .2f);
			var itemSize = ((float)UIScreen.MainScreen.Bounds.Width / 4f);
			var remainingScreenHeight = ((float)UIScreen.MainScreen.Bounds.Height - itemSize) * .9f;

			ACTIVE_DISTANCE = (int)itemSize;

			ItemSize = new CGSize(itemSize, itemSize);
			ScrollDirection = UICollectionViewScrollDirection.Horizontal;

			var topInset = remainingScreenHeight * .89f;
			var bottomInset = remainingScreenHeight * .11f;

			var leftRightInset = (UIScreen.MainScreen.Bounds.Width - (itemSize * 3f)) / 4f;
			        
			SectionInset = new UIEdgeInsets(topInset, leftRightInset, bottomInset, leftRightInset);

			MinimumLineSpacing = leftRightInset;
		}

		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			var array = base.LayoutAttributesForElementsInRect(rect);
			var visibleRect = new CGRect(CollectionView.ContentOffset, CollectionView.Bounds.Size);

			int collectionViewCount = (int)this.CollectionView.NumberOfItemsInSection(0);

			foreach (var attributes in array)
			{
				if (collectionViewCount == 1)
				{
					float zoom = 1 + ZOOM_FACTOR;
					attributes.Transform3D = CATransform3D.MakeScale(zoom, zoom, 1.0f);
					attributes.ZIndex = 1;
				}
				if (attributes.Frame.IntersectsWith(rect) && (collectionViewCount != 2 && collectionViewCount != 3))
				{
					float distance = (float)(visibleRect.GetMidX() - attributes.Center.X);
					float normalizedDistance = distance / ACTIVE_DISTANCE;
					if (Math.Abs(distance) < ACTIVE_DISTANCE)
					{
						float zoom = 1 + ZOOM_FACTOR * (1 - Math.Abs(normalizedDistance));
						attributes.Transform3D = CATransform3D.MakeScale(zoom, zoom, 1.0f);
						attributes.ZIndex = 1;
					}
				}
			}
			return array;
		}

		public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
		{
			float offSetAdjustment = float.MaxValue;
			float horizontalCenter = (float)(proposedContentOffset.X + (this.CollectionView.Bounds.Size.Width / 2.0));
			CGRect targetRect = new CGRect(proposedContentOffset.X, 0.0f, this.CollectionView.Bounds.Size.Width, this.CollectionView.Bounds.Size.Height);
			var array = base.LayoutAttributesForElementsInRect(targetRect);
			foreach (var layoutAttributes in array)
			{
				float itemHorizontalCenter = (float)layoutAttributes.Center.X;
				if (Math.Abs(itemHorizontalCenter - horizontalCenter) < Math.Abs(offSetAdjustment))
				{
					offSetAdjustment = itemHorizontalCenter - horizontalCenter;
				}
			}
			return new CGPoint(proposedContentOffset.X + offSetAdjustment, proposedContentOffset.Y);
		}

	}
}


