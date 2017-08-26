using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;

namespace AircraftForSale
{
    public partial class WaterfallCollectionViewController : UICollectionViewController, IAdLayoutInterface
    {
		public string DataObject
		{
			get;
			set;
		}

		public List<Ad> AdList
		{
			get
			{
				throw new NotImplementedException();
			}

			set
			{
				throw new NotImplementedException();
			}
		}

		public WaterfallCollectionViewController (IntPtr handle) : base (handle)
        {
        }

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			var waterfallLayout = new WaterfallCollectionLayout();

			// Wireup events
			waterfallLayout.SizeForItem += (collectionView, layout, indexPath) =>
			{
				var collection = collectionView as WaterfallCollectionView;
				return new CGSize((View.Bounds.Width - 40) / 3, collection.Source.Heights[(int)indexPath.Item]);
			};

			// Attach the custom layout to the collection
			CollectionView.SetCollectionViewLayout(waterfallLayout, false);
		}
    }
}