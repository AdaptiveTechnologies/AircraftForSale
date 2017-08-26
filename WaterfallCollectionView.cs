using System;
using UIKit;
using System.Collections.Generic;
using Foundation;

namespace AircraftForSale
{
	[Register("WaterfallCollectionView")]
	public class WaterfallCollectionView : UICollectionView
	{
		public WaterfallCollectionSource Source
		{
			get { return (WaterfallCollectionSource)DataSource; }
		}
		#region Constructors
		public WaterfallCollectionView(IntPtr handle) : base(handle)
		{
		}
		#endregion

		#region Override Methods
		public override void AwakeFromNib()
		{
			base.AwakeFromNib();

			// Initialize
			DataSource = new WaterfallCollectionSource(this);
			Delegate = new WaterfallCollectionDelegate(this);

		}
		#endregion
	}
}

