using System;
using System.Drawing;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	public class CollectionViewFlowDelegate : UICollectionViewDelegateFlowLayout
	{
		public override  CGSize GetSizeForItem(UICollectionView collectionView, UICollectionViewLayout layout, NSIndexPath indexPath)
		{
			//if (IsOdd((int)indexPath.Item))
			//{
			//	return new CoreGraphics.CGSize(300, 300);
			//}
			//else {
			//	return new CoreGraphics.CGSize(300, 150);
			//}

			if (indexPath.Item % 2 == 0)
				return new CoreGraphics.CGSize(300, (UIScreen.MainScreen.Bounds.Width/3) - 20);

			return new CoreGraphics.CGSize(300, (UIScreen.MainScreen.Bounds.Width / 2) - 15);
		}

		public static bool IsOdd(int value)
		{
			return value % 2 != 0;
		}

		//public override nfloat GetMinimumLineSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		//{
		//	//return base.GetMinimumLineSpacingForSection(collectionView, layout, section);
		//	return (int)5;
		//}

		//public override nfloat GetMinimumInteritemSpacingForSection(UICollectionView collectionView, UICollectionViewLayout layout, nint section)
		//{
		//	//return base.GetMinimumInteritemSpacingForSection(collectionView, layout, section);
		//	return (int)5;
		//}
	}
}

