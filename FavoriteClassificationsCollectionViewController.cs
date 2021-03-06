using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using AircraftForSale.PCL;
using CoreGraphics;
using AircraftForSale.PCL.Helpers;

namespace AircraftForSale
{
    public partial class FavoriteClassificationsCollectionViewController : UICollectionViewController
    {
        public FavoriteClassificationsCollectionViewController(IntPtr handle) : base(handle)
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

            GridLayout classificationsGridLayout = new GridLayout();

            int classificationItemWidth = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200 : 100;
            int classificationItemHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 150 : 100;

            classificationsGridLayout.ItemSize = new CoreGraphics.CGSize(classificationItemWidth, classificationItemHeight);

            int insetTop = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 125 : 75;
            int insetLeftBottomRight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 50 : 5;

            classificationsGridLayout.SectionInset = new UIEdgeInsets(insetTop, insetLeftBottomRight, insetLeftBottomRight, insetLeftBottomRight);
            classificationsGridLayout.HeaderReferenceSize = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 100, 0);

            this.CollectionView.SetCollectionViewLayout(classificationsGridLayout, true);
            //this.CollectionView.data
        }
        static NSString classificationCellID = new NSString("ClassificationCell");
        static NSString headerId = new NSString("Header");
        List<IClassification> classifications;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            CollectionView.RegisterClassForCell(typeof(ClassificationCell), classificationCellID);
           // CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, headerId);

			CollectionView.RegisterClassForSupplementaryView(typeof(Header), UICollectionElementKindSection.Header, headerId);
			CollectionView.ShowsHorizontalScrollIndicator = false;

            UIButton nextButton = new UIButton();
            nextButton.SetTitle("Next", UIControlState.Normal);
            nextButton.BackgroundColor = UIColor.Green;

            UIButton cancelButton = new UIButton();
            cancelButton.SetTitle("Finish", UIControlState.Normal);
            cancelButton.BackgroundColor = UIColor.Green;

            CGPoint point2 = new CGPoint((UIScreen.MainScreen.Bounds.Width / 2) - 200, UIScreen.MainScreen.Bounds.Height - 120);
            var frame2 = new CGRect(point2, new CGSize(100, 50));
            cancelButton.Frame = frame2;
            cancelButton.Font = UIFont.BoldSystemFontOfSize(22);


            CGPoint point = new CGPoint((UIScreen.MainScreen.Bounds.Width / 2) + 100, UIScreen.MainScreen.Bounds.Height - 120);
            var frame = new CGRect(point, new CGSize(100, 50));
            nextButton.Frame = frame;
            nextButton.Font = UIFont.BoldSystemFontOfSize(22);

            nextButton.TouchUpInside += (sender, e) =>
            {
                if (Settings.IsRegistered)
                {
                    RegistrationProfileViewController myInterestsVC = (RegistrationProfileViewController)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "Main_ipad" : "Main", null).InstantiateViewController("MyProfileViewController");

                    //this.Storyboard.InstantiateViewController("MyProfileViewController") as RegistrationProfileViewController;

                    PresentModalViewController(myInterestsVC, true);

                    return;
                }

                RegistrationStepTwo stepTwoVC = (RegistrationStepTwo)this.Storyboard.InstantiateViewController("RegistrationStepTwo");
                this.ShowViewController(stepTwoVC, this);
            };

            cancelButton.TouchUpInside += (sender, e) =>
            {
                this.DismissViewController(true, null);
            };

            View.Add(nextButton);
            if (Settings.IsRegistered)
                View.Add(cancelButton);

            this.CollectionView.BackgroundColor = UIColor.Gray;
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

        public override UICollectionReusableView GetViewForSupplementaryElement(UICollectionView collectionView, NSString elementKind, NSIndexPath indexPath)
        {
            var headerView = (Header)collectionView.DequeueReusableSupplementaryView(elementKind, headerId, indexPath);
            headerView.Text = "Select Your Favorite Classifications";
            return headerView;
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
    //public partial class ClassificationCell : UICollectionViewCell
    //{
    //    UIImageView imageView;
    //    UILabel classificationLabel;

    //    [Export("initWithFrame:")]
    //    public ClassificationCell(CGRect frame) : base(frame)
    //    {
    //        //var semiTransparentColor = new Color(0, 0, 0, 0.5);

    //        BackgroundView = new UIView { BackgroundColor = UIColor.White, Alpha = (nfloat).5 };
    //        BackgroundView.Layer.CornerRadius = 6.0f;
    //        //BackgroundView.Alpha = 0.5f;


    //        SelectedBackgroundView = new UIView { BackgroundColor = UIColor.Green, Alpha = (nfloat).5 };
    //        SelectedBackgroundView.Layer.CornerRadius = 6.0f;
    //        //SelectedBackgroundView.Alpha = 0.5f;

    //        ContentView.Layer.BorderColor = UIColor.Clear.CGColor;
    //        ContentView.Layer.BorderWidth = 0.0f;
    //        ContentView.Layer.CornerRadius = 6.0f;




    //        ContentView.Layer.ShadowColor = UIColor.DarkGray.CGColor;
    //        ContentView.Layer.ShadowOpacity = 1.0f;
    //        ContentView.Layer.ShadowRadius = 6.0f;
    //        ContentView.Layer.ShadowOffset = new System.Drawing.SizeF(0f, 3f);

    //        imageView = new UIImageView(new CGRect(0, frame.Height / 4, frame.Width - 10, frame.Height / 2));
    //        imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
    //        imageView.Image = UIImage.FromBundle("airplane.png");
    //        imageView.Center = ContentView.Center;

    //        classificationLabel = new UILabel(new CGRect(0, 5, frame.Width, frame.Height / 5));
    //        classificationLabel.Font = UIFont.SystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 18 : 8);
    //        classificationLabel.TextAlignment = UITextAlignment.Center;

    //        ContentView.AddSubview(imageView);
    //        ContentView.AddSubview(classificationLabel);
    //    }

    //    public UIImage Image
    //    {
    //        set
    //        {
    //            imageView.Image = value;
    //        }
    //    }

    //    public string ClassificationName
    //    {
    //        set
    //        {
    //            classificationLabel.Text = value;
    //        }
    //    }
    //}

    //public class Header : UICollectionReusableView
    //{
    //    UILabel label;

    //    public string Text
    //    {
    //        get
    //        {
    //            return label.Text;
    //        }
    //        set
    //        {
    //            label.Text = value;
    //            SetNeedsDisplay();
    //        }
    //    }

    //    [Export("initWithFrame:")]
    //    public Header(CGRect frame) : base(frame)
    //    {
    //        var headerFrame = new CGRect(0, 60, UIScreen.MainScreen.Bounds.Width, 50);

    //        UIFont font = UIFont.BoldSystemFontOfSize(20);

    //        label = new UILabel()
    //        {
    //            Frame = headerFrame,
    //            Font = font,
    //            TextAlignment = UITextAlignment.Center,
    //            TextColor = UIColor.White,
    //            BackgroundColor = UIColor.Gray
    //        };

    //        label.Font = font;

    //        AddSubview(label);
    //    }

    //}

}