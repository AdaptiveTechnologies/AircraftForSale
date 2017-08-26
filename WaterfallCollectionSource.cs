using System;
using Foundation;
using UIKit;
using System.Collections.Generic;

namespace AircraftForSale
{
	public class WaterfallCollectionSource : UICollectionViewDataSource
	{
		#region Computed Properties
		public WaterfallCollectionView CollectionView { get; set; }
		public List<Ad> AdList { get; set; } = new List<Ad>();
		#endregion

		#region Constructors
		public WaterfallCollectionSource(WaterfallCollectionView collectionView)
		{
			// Initialize
			CollectionView = collectionView;

			Ad ad1 = new Ad();
			ad1.Name = "1945 G-44A Widgeon";
			ad1.Price = "$325,000";
			ad1.Classification = new Amphibian();
			ad1.Manufacturer = new Grumman();
			ad1.Image = UIImage.FromBundle("gruman_ad.jpg");
			ad1.Broker = "Prairie Aircraft Sales, Ltd.";
			ad1.Teaser = "Is pleased to announce the acquisition of the Cessna Piston Dealership for the provinces of Manitoba, Saskatchewan, Alberta and the North West Territories. We continue to represent Cessna for the full Caravan line for all of Western Northern Canada as well as all makes and models of Pre-owned aircraft from singles to Jets.";

			Ad ad2 = new Ad();
			ad2.Name = "1987 Scoopers CL215";
			ad2.Price = "$3,000,000";
			ad2.Classification = new Amphibian();
			ad2.Manufacturer = new Bombardier();
			ad2.Image = UIImage.FromBundle("bombardier_ad.jpg");
			ad2.Broker = "Lone Star Jet";
			ad2.Teaser = "We offer Fast, Accurate, reliable NAAA Certified Aircraft Appraisal Services, nationwide! Senior certified member of National Aircraft Appraisers Association. The Aviation Industry's leading source for Certified Appraisals since 1980. NAAA Certified Aircraft Appraisals";

			Ad ad3 = new Ad();
			ad3.Name = "1969 Scoopers CL215";
			ad3.Price = "$2,500,000";
			ad3.Classification = new Amphibian();
			ad3.Manufacturer = new Bombardier();
			ad3.Image = UIImage.FromBundle("bombardier_ad2.jpg");
			ad3.Broker = "The 206 Express";
			ad3.Teaser = "John Hopkinson & Associates was incorporated in 1979 as an international aircraft brokerage and consulting firm. JH&A specializes in corporate and commuter aircraft.";

			Ad ad4 = new Ad();
			ad4.Name = "1988 Scoopers CL215.jpg";
			ad4.Price = "$3,000,000";
			ad4.Classification = new Amphibian();
			ad4.Manufacturer = new Bombardier();
			ad4.Image = UIImage.FromBundle("bombardier_ad3.jpg");
			ad4.Broker = "Technical Aircraft Services,LLC";
			ad4.Teaser = "To offer professional services in the area's of airframe & powerplant maintenance, inspection, start up, record keeping, training, aircraft purchasing, leasing, financing, etc.";


			Ad ad5 = new Ad();
			ad5.Name = "1945 G-44A Widgeon";
			ad5.Price = "$325,000";
			ad5.Classification = new Amphibian();
			ad5.Manufacturer = new Grumman();
			ad5.Image = UIImage.FromBundle("gruman_ad.jpg");
			ad5.Broker = "Prairie Aircraft Sales, Ltd.";
			ad5.Teaser = "Is pleased to announce the acquisition of the Cessna Piston Dealership for the provinces of Manitoba, Saskatchewan, Alberta and the North West Territories. We continue to represent Cessna for the full Caravan line for all of Western Northern Canada as well as all makes and models of Pre-owned aircraft from singles to Jets.";

			//Ad ad6 = new Ad();
			//ad6.Name = "1987 Scoopers CL215 Aircraft Details";
			//ad6.Price = "$3,000,000";
			//ad6.Classification = new Amphibian();
			//ad6.Manufacturer = new Bombardier();
			//ad6.Image = UIImage.FromBundle("bombardier_ad.jpg");

			//Ad ad7 = new Ad();
			//ad7.Name = "1969 Scoopers CL215 Aircraft Details";
			//ad7.Price = "$2,500,000";
			//ad7.Classification = new Amphibian();
			//ad7.Manufacturer = new Bombardier();
			//ad7.Image = UIImage.FromBundle("bombardier_ad2.jpg");

			//Ad ad8 = new Ad();
			//ad8.Name = "1988 Scoopers CL215 Aircraft Details.jpg";
			//ad8.Price = "$3,000,000";
			//ad8.Classification = new Amphibian();
			//ad8.Manufacturer = new Bombardier();
			//ad8.Image = UIImage.FromBundle("bombardier_ad3.jpg");

			AdList.Add(ad1);
			AdList.Add(ad2);
			AdList.Add(ad3);
			AdList.Add(ad4);
			AdList.Add(ad5);
			//AdList.Add(ad6);
			//AdList.Add(ad7);
			//AdList.Add(ad8);

			// Init numbers collection
			for (int n = 0; n < 5; ++n)
			{
				int rndNumber = rnd.Next(1, 3);

				if (rndNumber == 2)
				{
					Heights.Add(rndNumber * 135.0f);
				}
				else {
					Heights.Add(rndNumber * 170.0f);
				}
			}
		}
		#endregion

		private Random rnd = new Random();
		public List<nfloat> Heights { get; set; } = new List<nfloat>();

		#region Override Methods
		public override nint NumberOfSections(UICollectionView collectionView)
		{
			// We only have one section
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			// Return the number of items
			return AdList.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			// Get a reusable cell and set {~~it's~>its~~} title from the item
			var cell = collectionView.DequeueReusableCell("Cell", indexPath) as TextCollectionViewCell;

			var aircraftAd = AdList[(int)indexPath.Item];
			cell.AircraftName = aircraftAd.Name;
			cell.AircraftImageProperty = aircraftAd.Image;
			cell.Manufacturer = aircraftAd.Manufacturer.Name;
			cell.BrokerName = aircraftAd.Broker;
			cell.AdDescriptionProperty = aircraftAd.Teaser;
			cell.Price = aircraftAd.Price;

			return cell;
		}

		public override bool CanMoveItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			// We can always move items
			return false;
		}

		//public override void MoveItem(UICollectionView collectionView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		//{
		//	// Reorder our list of items
		//	var item = Numbers[(int)sourceIndexPath.Item];
		//	Numbers.RemoveAt((int)sourceIndexPath.Item);
		//	Numbers.Insert((int)destinationIndexPath.Item, item);
		//}
		#endregion
	}
}

