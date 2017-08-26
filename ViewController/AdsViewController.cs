using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using AircraftForSale.PCL;

namespace AircraftForSale
{
	public partial class AdsViewController : UICollectionViewController
	{
		public string DataObject
		{
			get; set;
		}
		public AdsViewController(UICollectionViewLayout layout) : base (layout)
        {
			
			ads = new List<Ad>();

			Ad ad1 = new Ad();
			ad1.Name = "1945 G-44A Widgeon Aircraft Details";
			ad1.Price = "$325,000";
			ad1.Classification = new Amphibian();
			ad1.Manufacturer = new Grumman();
			ad1.Image = UIImage.FromBundle("gruman_ad.jpg");

			Ad ad2 = new Ad();
			ad2.Name = "1987 Scoopers CL215 Aircraft Details";
			ad2.Price = "$3,000,000";
			ad2.Classification = new Amphibian();
			ad2.Manufacturer = new Bombardier();
			ad2.Image = UIImage.FromBundle("bombardier_ad.jpg");

			Ad ad3 = new Ad();
			ad3.Name = "1969 Scoopers CL215 Aircraft Details";
			ad3.Price = "$2,500,000";
			ad3.Classification = new Amphibian();
			ad3.Manufacturer = new Bombardier();
			ad3.Image = UIImage.FromBundle("bombardier_ad2.jpg");

			Ad ad4 = new Ad();
			ad4.Name = "1988 Scoopers CL215 Aircraft Details.jpg";
			ad4.Price = "$3,000,000";
			ad4.Classification = new Amphibian();
			ad4.Manufacturer = new Bombardier();
			ad4.Image = UIImage.FromBundle("bombardier_ad3.jpg");

			ads.Add(ad1);
			ads.Add(ad2);
			ads.Add(ad3);
			ads.Add(ad4);

		}



		public override void LoadView()
		{
			base.LoadView();


		}


		static NSString adCellID = new NSString("AdCell");
		//static NSString adHeaderID = new NSString("AdHeader");
		List<Ad> ads;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			CollectionView.RegisterClassForCell(typeof(AircraftAdCell), adCellID);
			//CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, adHeaderID);

			//this.View.BackgroundColor = UIColor.Green;
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return ads.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var adCell = (AircraftAdCell)collectionView.DequeueReusableCell(adCellID, indexPath);

			var ad = ads[indexPath.Row];

			adCell.Image = ad.Image;
			adCell.AdName = ad.Name;

			return adCell;
		}

		//public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		//{
		//	var headerView = (Header)collectionView.DequeueReusableSupplementaryView(elementKind, adHeaderID, indexPath);
		//	headerView.Text = "Select Your Favorite Classifications";
		//	return headerView;
		//}

		//public override ite



		public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = UIColor.Orange;
		}

		public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = null;
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			var ad = ads[(int)indexPath.Item];

			//if (classification.IsSelected)
			//{
			//	cell.Selected = false;
			//	classification.IsSelected = false;
			//}
			//else {
			//	cell.Selected = true;
			//	classification.IsSelected = true;
			//}


		}

		// public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		//{
		//	var cell = collectionView.CellForItem(indexPath);
		//	cell.ContentView.BackgroundColor = UIColor.White;
		//}

		public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		      public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
		      {
		          return true;
		      }
		public override bool ShouldDeselectItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		// for edit menu
		public override bool ShouldShowMenu(UICollectionView collectionView, NSIndexPath indexPath)
		{
			//return true;
			return false;
		}


	}

	public partial class AircraftAdCell : UICollectionViewCell
	{
		UIImageView imageView;
		UILabel adLabel;

		[Export("initWithFrame:")]
		public AircraftAdCell(CGRect frame) : base(frame)
		{
			BackgroundView = new UIView { BackgroundColor = UIColor.White, Alpha = (nfloat).5 };
			BackgroundView.Layer.CornerRadius = 6.0f;


			SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green, Alpha = (nfloat).5 };
			SelectedBackgroundView.Layer.CornerRadius = 6.0f;

			ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
			ContentView.Layer.BorderWidth = 3.0f;
			ContentView.Layer.CornerRadius = 6.0f;
			//ContentView.BackgroundColor = UIColor.White;

			ContentView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			ContentView.Layer.ShadowOpacity = 1.0f;
			ContentView.Layer.ShadowRadius = 6.0f;
			ContentView.Layer.ShadowOffset = new System.Drawing.SizeF(0f, 3f);
			//ContentView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
			//ContentView.Transform = CGAffineTransform.MakeScale(2f, 2f);

			//imageView = new UIImageView(new CGRect(0, 0, 200, 134));

			imageView = new UIImageView(new CGRect(0, frame.Height / 4, frame.Width - 10, frame.Height / 2));
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.Image = UIImage.FromBundle("airplane.png");
			//imageView = new UIImageView(UIImage.FromBundle("airplane.png"));
			imageView.Center = ContentView.Center;
			//imageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
			//imageView.Transform = CGAffineTransform.MakeScale(1.8f, 1.8f);

			adLabel = new UILabel(new CGRect(0, 5, frame.Width, frame.Height / 5));
			adLabel.TextAlignment = UITextAlignment.Center;

			ContentView.AddSubview(imageView);
			ContentView.AddSubview(adLabel);
		}

		public UIImage Image
		{
			set
			{
				imageView.Image = value;
			}
		}

		public string AdName
		{
			set
			{
				adLabel.Text = value;
			}
		}

	}

	//public class AdHeader : UICollectionReusableView
	//{
	//	UILabel label;

	//	public string Text
	//	{
	//		get
	//		{
	//			return label.Text;
	//		}
	//		set
	//		{
	//			label.Text = value;
	//			SetNeedsDisplay();
	//		}
	//	}

	//	[Export("initWithFrame:")]
	//	public AdHeader(CGRect frame) : base(frame)
	//	{
	//		//var xPosition = (UIScreen.MainScreen.Bounds.Width / 2) - 200;
	//		var headerFrame = new CGRect(0, 20, UIScreen.MainScreen.Bounds.Width, 50);

	//		UIFont font = UIFont.BoldSystemFontOfSize(20);

	//		label = new UILabel() 
	//		{ Frame = headerFrame,
	//			Font = font,
	//			TextAlignment = UITextAlignment.Center,
	//			TextColor = UIColor.White,
	//			BackgroundColor = UIColor.Gray
	//		};

	//		label.Font = font;

	//		AddSubview(label);
	//	}

	//}
}