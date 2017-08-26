using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	public partial class GridLayout : UICollectionViewFlowLayout
	{
		public GridLayout()
		{
		}


		public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
		{
			return true;
		}

		public override UICollectionViewLayoutAttributes LayoutAttributesForItem(NSIndexPath path)
		{
			return base.LayoutAttributesForItem(path);
		}

		public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
		{
			return base.LayoutAttributesForElementsInRect(rect);
		}

		public override CGSize CollectionViewContentSize
		{
			get
			{
				return CollectionView.Bounds.Size;
			}
		}
		//public override CGSize ItemSize
		//{
		//	get
		//	{
		//		return new CoreGraphics.CGSize(200, 150);
		//	}
		//	set
		//	{
		//		base.ItemSize = value;
		//	}
		//}

	}
}

