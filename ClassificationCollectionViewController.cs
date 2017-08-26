using System;
using System.Collections.Generic;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace AircraftForSale
{
	public class SimpleCollectionViewController : UICollectionViewController
	{
		static NSString classificationCellID = new NSString("ClassificationCell");
		static NSString headerId = new NSString("Header");
		List<IClassification> classifications;

		public SimpleCollectionViewController(UICollectionViewLayout layout) : base(layout)
		{
			classifications = new List<IClassification>();
			for (int i = 0; i < 12; i++)
			{
				classifications.Add(new Amphibian());
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			CollectionView.RegisterClassForCell(typeof(ClassificationCell), classificationCellID);
			CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, headerId);

			//UIMenuController.SharedMenuController.MenuItems = new UIMenuItem[] {
			//	new UIMenuItem ("Custom", new Selector ("custom"))
			//};
		}

		public override nint NumberOfSections(UICollectionView collectionView)
		{
			return 1;
		}

		public override nint GetItemsCount(UICollectionView collectionView, nint section)
		{
			return classifications.Count;
		}

		public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var classificationCell = (ClassificationCell)collectionView.DequeueReusableCell(classificationCellID, indexPath);

			var classification = classifications[indexPath.Row];

			classificationCell.Image = classification.Image;

			return classificationCell;
		}

		public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
		{
			var headerView = (Header)collectionView.DequeueReusableSupplementaryView(elementKind, headerId, indexPath);
			headerView.Text = "Select All Classifictions That Interest You";
			return headerView;
		}

		public override void ItemHighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = UIColor.Yellow;
		}

		public override void ItemUnhighlighted(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var cell = collectionView.CellForItem(indexPath);
			cell.ContentView.BackgroundColor = UIColor.White;
		}

		public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		//      public override bool ShouldSelectItem (UICollectionView collectionView, NSIndexPath indexPath)
		//      {
		//          return false;
		//      }

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

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate(toInterfaceOrientation, duration);

			var lineLayout = CollectionView.CollectionViewLayout as LineLayout;
			if (lineLayout != null)
			{
				if ((toInterfaceOrientation == UIInterfaceOrientation.Portrait) || (toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown))
					lineLayout.SectionInset = new UIEdgeInsets(400, 0, 400, 0);
				else
					lineLayout.SectionInset = new UIEdgeInsets(220, 0.0f, 200, 0.0f);
			}
		}

	}

	public class ClassificationCell : UICollectionViewCell
	{
		UIImageView imageView;

		[Export("initWithFrame:")]
		public ClassificationCell(CGRect frame) : base(frame)
		{
			BackgroundView = new UIView { BackgroundColor = UIColor.Orange };

			SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green };

			ContentView.Layer.BorderColor = UIColor.LightGray.CGColor;
			ContentView.Layer.BorderWidth = 2.0f;
			ContentView.BackgroundColor = UIColor.White;
			ContentView.Transform = CGAffineTransform.MakeScale(0.8f, 0.8f);
			//ContentView.Transform = CGAffineTransform.MakeScale(2f, 2f);

			imageView = new UIImageView(UIImage.FromBundle("airplane.png"));
			imageView.Center = ContentView.Center;
			imageView.Transform = CGAffineTransform.MakeScale(0.7f, 0.7f);
			//imageView.Transform = CGAffineTransform.MakeScale(1.8f, 1.8f);

			ContentView.AddSubview(imageView);
		}

		public UIImage Image
		{
			set
			{
				imageView.Image = value;
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

	public class Header : UICollectionReusableView
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
		public Header(CGRect frame) : base(frame)
		{
			label = new UILabel() { Frame = new CGRect(0, 0, 300, 50), BackgroundColor = UIColor.Yellow };
			AddSubview(label);
		}
	}
}

