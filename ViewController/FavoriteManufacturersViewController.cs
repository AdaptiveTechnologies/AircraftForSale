using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using AircraftForSale.PCL;

namespace AircraftForSale
{
	public partial class FavoriteManufactuersViewController : UICollectionViewController, IMultiStepProcessStep
	{
		public string DataObject
		{
			get; set;
		}
		public FavoriteManufactuersViewController(UICollectionViewLayout layout) : base (layout)
        {
			manufacturers = new List<IManufacturer>();
			manufacturers.Add(new Aerospatiale());
			manufacturers.Add(new Airbus());
			manufacturers.Add(new Bombardier());
			manufacturers.Add(new Grumman());
			manufacturers.Add(new Lake());
			manufacturers.Add(new Maule());
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			StepActivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });
		}

		public override void LoadView()
		{
			base.LoadView();

		}

		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;




		static NSString manufacturerCellID = new NSString("ManufacturerIDCell");
		static NSString headerId = new NSString("ManufacturerHeader");
		List<IManufacturer> manufacturers;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			CollectionView.RegisterClassForCell(typeof(ManufacturerCell), manufacturerCellID);
			CollectionView.RegisterClassForSupplementaryView(typeof(ManufacturerHeader), UICollectionElementKindSection.Header, headerId);

			//this.View.BackgroundColor = UIColor.Green;
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return manufacturers.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var manufacturerCell = (ManufacturerCell)collectionView.DequeueReusableCell(manufacturerCellID, indexPath);

			var manufacturer = manufacturers[indexPath.Row];

			//var imageView = new UIImageView(new CGRect(0, 0, manufacturer.Image.Size.Width, manufacturer.Image.Size.Height));
			//imageView.Image = manufacturer.Image;
			manufacturerCell.Image = UIImage.FromBundle(manufacturer.Image);

			manufacturerCell.ManufacturerName = manufacturer.Name;

			return manufacturerCell;
		}

		public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			var headerView = (ManufacturerHeader)collectionView.DequeueReusableSupplementaryView(elementKind, headerId, indexPath);
			headerView.Text = "Select Your Favorite Manufacturers";
			return headerView;
		}

		public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = UIColor.Orange;
			//cell.ContentView.Alpha = (nfloat).5;
		}

		public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = null;
			//cell.ContentView.Alpha = (nfloat).5;
		}

		public override void ItemSelected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			var manufacturer = manufacturers[(int)indexPath.Item];

			if (manufacturer.IsSelected)
			{
				cell.Selected = false;
				manufacturer.IsSelected = false;
			}
			else {
				cell.Selected = true;
				manufacturer.IsSelected = true;
			}
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

		//public override bool CanPerformAction(UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
		//{
		//	// Selector should be the same as what's in the custom UIMenuItem
		//	if (action == new Selector("custom"))
		//		return true;
		//	else
		//		return false;
		//}

		//public override void PerformAction(UICollectionView collectionView, Selector action, NSIndexPath indexPath, NSObject sender)
		//{
		//	System.Diagnostics.Debug.WriteLine("code to perform action");
		//}

		// CanBecomeFirstResponder and CanPerform are needed for a custom menu item to appear
		//public override bool CanBecomeFirstResponder
		//{
		//	get
		//	{
		//		return true;
		//	}
		//}

		/*public override bool CanPerform (Selector action, NSObject withSender)
		{
			if (action == new Selector ("custom"))
				return true;
			else
				return false;
		}*/

		//public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		//{
		//	base.WillRotate(toInterfaceOrientation, duration);

		//	var lineLayout = CollectionView.CollectionViewLayout as LineLayout;
		//	if (lineLayout != null)
		//	{
		//		if ((toInterfaceOrientation == UIInterfaceOrientation.Portrait) || (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown))
		//			lineLayout.SectionInset = new UIEdgeInsets(400, 0, 400, 0);
		//		else
		//			lineLayout.SectionInset = new UIEdgeInsets(220, 0.0f, 200, 0.0f);
		//	}
		//}

	}

	public partial class ManufacturerCell : UICollectionViewCell
	{
		UIImageView imageView;
		UILabel manufacturerLabel;

		[Export("initWithFrame:")]
		public ManufacturerCell(CGRect frame) : base(frame)
		{
			BackgroundView = new UIView { BackgroundColor = UIColor.White, Alpha = (nfloat).5 };
			BackgroundView.Layer.CornerRadius = 6.0f;


			SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green, Alpha = (nfloat).5 };
			SelectedBackgroundView.Layer.CornerRadius = 6.0f;


			ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
			ContentView.Layer.BorderWidth = 3.0f;
			ContentView.Layer.CornerRadius = 6.0f;
			//ContentView.BackgroundColor = UIColor.White;
			//ContentView.Alpha = (nfloat).5;

			ContentView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			ContentView.Layer.ShadowOpacity = 1.0f;
			ContentView.Layer.ShadowRadius = 6.0f;
			ContentView.Layer.ShadowOffset = new System.Drawing.SizeF(0f, 3f);
			//ContentView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
			//ContentView.Transform = CGAffineTransform.MakeScale(2f, 2f);

			imageView = new UIImageView(new CGRect(0, frame.Height / 4, frame.Width - 10, frame.Height / 2));
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.Image = UIImage.FromBundle("airplane.png");


			//imageView = new UIImageView(UIImage.FromBundle("airplane.png"));
			//imageView = new UIImageView(UIImage.FromBundle("airplane.png"));
			 imageView.Center = ContentView.Center;
			//imageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
			//imageView.Transform = CGAffineTransform.MakeScale(1.8f, 1.8f);

			manufacturerLabel = new UILabel(new CGRect(0, 5, frame.Width, frame.Height / 5));
			manufacturerLabel.TextAlignment = UITextAlignment.Center;

			ContentView.AddSubview(imageView);
			ContentView.AddSubview(manufacturerLabel);
		}

		public UIImage Image
		{
			set
			{
				imageView.Image = value;
			}
		}

		public string ManufacturerName
		{
			set
			{
				manufacturerLabel.Text = value;
			}
		}

		//[Export("custom")]
		//void Custom()
		//{
		//	// Put all your custom menu behavior code here
		//	Console.WriteLine("custom in the cell");
		//}


		//public override bool CanPerform(Selector action, NSObject withSender)
		//{
		//	if (action == new Selector("custom"))
		//		return true;
		//	else
		//		return false;
		//}
	}

	public class ManufacturerHeader : UICollectionReusableView
	{
		UILabel label;

		public string Text
		{
			get
			{
				return label.Text;
			}
			set
			{
				label.Text = value;
				SetNeedsDisplay();
			}
		}

		[Export("initWithFrame:")]
		public ManufacturerHeader(CGRect frame) : base(frame)
		{
			//var xPosition = (UIScreen.MainScreen.Bounds.Width / 2) - 200;
			var headerFrame = new CGRect(0, 20, UIScreen.MainScreen.Bounds.Width, 50);

			UIFont font = UIFont.BoldSystemFontOfSize(20);

			label = new UILabel()
			{
				Frame = headerFrame,
				Font = font,
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Gray
			};

			label.Font = font;

			AddSubview(label);
		}

	}
}