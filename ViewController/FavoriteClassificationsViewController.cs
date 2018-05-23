using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using AircraftForSale.PCL;
using System.Linq;
using AircraftForSale.PCL.Helpers;
using Google.Analytics;


namespace AircraftForSale
{
	public partial class FavoriteClassificationsViewController : UICollectionViewController
	{
		public FavoriteClassificationsViewController(AircraftGridLayout layout):base(layout)
		{


			classifications = new List<IClassification>();
			classifications.Add(new Amphibian());
			classifications.Add(new Commercial());
			classifications.Add(new Experimental());
			classifications.Add(new Helicopter());
			classifications.Add(new LightSport());
			classifications.Add(new Jet());
			classifications.Add(new SingleEngine());
			classifications.Add(new AircraftForSale.PCL.Single());
			classifications.Add(new TwinPiston());
			classifications.Add(new TwinTurbine());
			classifications.Add(new Vintage());
			classifications.Add(new Warbird());

			this.Title = "Registration";
                     
			UINavigationBar.Appearance.TitleTextAttributes = new UIStringAttributes
			{
				ForegroundColor = UIColor.White
			};

			UITextAttributes icoFontAttribute = new UITextAttributes();
			icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(20);
			icoFontAttribute.TextColor = UIColor.White;

			NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
			NavigationItem.BackBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
			NavigationItem.BackBarButtonItem.TintColor = UIColor.White;
         
		}
        
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "FavoriteClassifications View (Registration)");

			Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());         
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
		}

		public override void LoadView()
		{
			base.LoadView();
		}
              

		static NSString classificationCellID = new NSString("ClassificationCell");
		List<IClassification> classifications;
              
		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//Next toolbar button item
			var nextBarButtonItem = new UIBarButtonItem("Next", UIBarButtonItemStyle.Plain, (sender, args) =>
			{
				if (Settings.AnyClassificationChosen())
				{
					var testStoryBoard = UIStoryboard.FromName("Registration_New", NSBundle.MainBundle);
					var regViewController = testStoryBoard.InstantiateViewController("RegistrationStepTwo");
					this.ShowViewController(regViewController, this);
				}
				else
				{
					//Display message here
					HelperMethods.SendBasicAlert("Alert", "Please select at least one aircraft classification");
				}
			});
			UIFont font;
			int headerHight;
            var horizontalClass = this.TraitCollection.HorizontalSizeClass;
            switch (horizontalClass)
            {
                case UIUserInterfaceSizeClass.Compact:
                    {
                        font = UIFont.BoldSystemFontOfSize(13);
						headerHight = 25;
						break;
                    }

                case UIUserInterfaceSizeClass.Regular:
                    {
                        font = UIFont.BoldSystemFontOfSize(20);
						headerHight = 50;
						break;
                    }

                default:
                    {
                        font =  UIFont.BoldSystemFontOfSize(20);
						headerHight = 50;
						break;
                    }
            }

			UITextAttributes icoFontAttribute = new UITextAttributes();
			icoFontAttribute.Font = font;
			icoFontAttribute.TextColor = UIColor.White;

			nextBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);

			NavigationItem.SetRightBarButtonItem(nextBarButtonItem, true);

			//Cancel toolbar button item
			var cancelBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, args) =>
			{
				DismissViewController(true, null);
			});

			cancelBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);

			NavigationItem.SetLeftBarButtonItem(cancelBarButtonItem, true);

			CollectionView.RegisterClassForCell(typeof(ClassificationCell), classificationCellID);
         
			CollectionView.BackgroundColor = UIColor.White;
			CollectionView.AllowsMultipleSelection = true;


			var headerLabel = new UILabel()
			{
				Font = font,
				TextAlignment = UITextAlignment.Center,
				TextColor = UIColor.White,
				BackgroundColor = UIColor.Gray,
				TranslatesAutoresizingMaskIntoConstraints = false
			};
			headerLabel.Text = "Select Your Favorite Classifications";
			this.View.Add(headerLabel);

			headerLabel.TopAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.TopAnchor).Active = true;
			headerLabel.LeftAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.LeftAnchor).Active = true;
			headerLabel.RightAnchor.ConstraintEqualTo(this.View.SafeAreaLayoutGuide.RightAnchor).Active = true;
			headerLabel.HeightAnchor.ConstraintEqualTo((System.nfloat)headerHight).Active = true;
			headerLabel.AdjustsFontSizeToFitWidth = true;


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

			classificationCell.Image = UIImage.FromBundle(classification.Image);
			classificationCell.ClassificationName = classification.Name;

			if (classification.IsSelected)
			{
				classificationCell.Selected = true;
				collectionView.SelectItem(indexPath, false, UICollectionViewScrollPosition.None);
			}

			return classificationCell;
		}

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
			var classification = classifications[(int)indexPath.Item];
			classification.IsSelected = true;
			Settings.SaveClassification(classification.Name, true);
		}

		public override void ItemDeselected(UICollectionView collectionView, NSIndexPath indexPath)
		{
			var classification = classifications[(int)indexPath.Item];
			classification.IsSelected = false;
			Settings.SaveClassification(classification.Name, false);
		}

		public override bool ShouldHighlightItem(UICollectionView collectionView, NSIndexPath indexPath)
		{
			return true;
		}

		public override bool ShouldSelectItem(UICollectionView collectionView, NSIndexPath indexPath)
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
			return false;
		}



	}

	public partial class ClassificationCell : UICollectionViewCell
	{
		UIImageView imageView;
		UILabel classificationLabel;

		[Export("initWithFrame:")]
		public ClassificationCell(CGRect frame) : base(frame)
		{
			BackgroundView = new UIView { BackgroundColor = UIColor.LightGray, Alpha = (nfloat).5 };
			BackgroundView.Layer.CornerRadius = 6.0f;

			SelectedBackgroundView = new UIView { BackgroundColor = HelperMethods.GetLime(), Alpha = (nfloat).5 };
			SelectedBackgroundView.Layer.CornerRadius = 6.0f;

			ContentView.Layer.BorderColor = UIColor.Clear.CGColor;
			ContentView.Layer.BorderWidth = 0.0f;
			ContentView.Layer.CornerRadius = 6.0f;

			ContentView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
			ContentView.Layer.ShadowOpacity = 1.0f;
			ContentView.Layer.ShadowRadius = 6.0f;
			ContentView.Layer.ShadowOffset = new System.Drawing.SizeF(0f, 3f);

			imageView = new UIImageView(new CGRect(0, frame.Height / 4, frame.Width - 10, frame.Height / 2));
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			imageView.Image = UIImage.FromBundle("airplane.png");
			imageView.Center = ContentView.Center;

			classificationLabel = new UILabel(new CGRect(0, 5, frame.Width, frame.Height / 5));
			classificationLabel.Font = UIFont.SystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 18 : 8);
			classificationLabel.TextAlignment = UITextAlignment.Center;

			ContentView.AddSubview(imageView);
			ContentView.AddSubview(classificationLabel);
		}

		public UIImage Image
		{
			set
			{
				imageView.Image = value;
			}
		}

		public string ClassificationName
		{
			set
			{
				classificationLabel.Text = value;
			}
		}
	}
}