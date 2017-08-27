using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using AircraftForSale.PCL;
using System.Linq;
using System.Threading.Tasks;
using AircraftForSale.PCL.Helpers;
using System.Globalization;
using Google.Analytics;

using Google.Analytics;

namespace AircraftForSale
{
    public partial class ChooseClassificationCollectionViewController : UICollectionViewController
    {
        public ChooseClassificationCollectionViewController(IntPtr handle) : base(handle)
        {


            classifications = new List<IClassification>();
            var lineLayout = new LineLayout();

            this.CollectionView.CollectionViewLayout = lineLayout;

        }


        static NSString classificationCellID = new NSString("ClassificationCell");
        List<IClassification> classifications;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            CollectionView.RegisterClassForCell(typeof(ClassificationCell), classificationCellID);

        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            //CollectionView.DataSource = null;

            if (this.TabBarController != null)
            {
                this.TabBarController.TabBar.Hidden = false;
            }
            //Set background image to second image in the adlist property on the app delegate
            LoadBackgroundImages();

            var lineLayout = this.CollectionView.CollectionViewLayout as LineLayout;

            if (classifications.Count() < 4)
            {
                var currentInset = lineLayout.SectionInset;
                var currentItemSize = lineLayout.ItemSize;
                if (classifications.Count() == 1)
                {
                    var leftRightInset = (UIScreen.MainScreen.Bounds.Width - (currentItemSize.Width)) / 2;
                    lineLayout.SectionInset = new UIEdgeInsets(currentInset.Top, leftRightInset, currentInset.Bottom, leftRightInset);
                }
                else
                {
                    if (classifications.Count() == 2)
                    {
                        var leftRightInset = (UIScreen.MainScreen.Bounds.Width - ((currentItemSize.Width * 2) + (lineLayout.MinimumLineSpacing))) / 2;
                        lineLayout.SectionInset = new UIEdgeInsets(currentInset.Top, leftRightInset, currentInset.Bottom, leftRightInset);
                    }
                    else
                    {
                        var leftRightInset = (UIScreen.MainScreen.Bounds.Width - ((currentItemSize.Width * 3) + (lineLayout.MinimumLineSpacing * 2))) / 2;
                        lineLayout.SectionInset = new UIEdgeInsets(currentInset.Top, leftRightInset, currentInset.Bottom, leftRightInset);
                    }
                }
            }
            //this.CollectionView.DataSource.
            //CollectionView.DataSource = this;
            //this.CollectionView.ReloadData();

        }

        BetterExperienceOverlay loadingOverlay;

        public UIImageView BackgroundImageView
        {
            get;
            set;
        }

        public UIImageView LogoImageView
        {
            get;
            set;
        }

        public void LoadBackgroundImages()
        {

            if (BackgroundImageView != null)
            {
                BackgroundImageView.RemoveFromSuperview();
                BackgroundImageView.Dispose();
                BackgroundImageView = null;
            }
            if (LogoImageView != null)
            {
                LogoImageView.RemoveFromSuperview();
                LogoImageView.Dispose();
                LogoImageView = null;
            }


            Random rnd = new Random();
            var randomDouble = (int)(rnd.NextDouble() * 10) - 1;
            if (randomDouble < 0 || randomDouble > 9)
                randomDouble = 0;

            var imageName = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "tablet_back" : "background";
            //var sourceImage = HelperMethods.GetImageFromCacheOrDefault(AppDelegate.FirstAd.ImageURL);
            var sourceImage = UIImage.FromBundle(imageName + randomDouble + ".png");
            var resizedImage = sourceImage.ScaleImageToPreventHorizontalWhiteSpace(View.Frame.Size.Width);
            BackgroundImageView = new UIImageView(resizedImage);
            BackgroundImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var extraYForNewFrameBelowNavBar = UIApplication.SharedApplication.StatusBarFrame.Height + NavigationController.NavigationBar.Bounds.Height;

            var currentFrame = BackgroundImageView.Frame;
            BackgroundImageView.Frame = new CoreGraphics.CGRect(currentFrame.X, currentFrame.Y + extraYForNewFrameBelowNavBar, currentFrame.Width, currentFrame.Height);

            //this.CollectionView.Add(BackgroundImageView);

            this.CollectionView.BackgroundView = BackgroundImageView;
            var logoImage = UIImage.FromBundle("home_logo.png");
            var resizedLogoImage = logoImage.ScaleImageToPreventHorizontalWhiteSpace(View.Frame.Size.Width * .8f);
            LogoImageView = new UIImageView(resizedLogoImage);
            LogoImageView.ContentMode = UIViewContentMode.ScaleAspectFit;

            var currentLogoFrame = LogoImageView.Frame;
            LogoImageView.Frame = new CoreGraphics.CGRect((View.Bounds.Width - currentLogoFrame.Width) / 2, View.Bounds.Height * (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? .525 : .6), currentLogoFrame.Width, currentLogoFrame.Height);

            this.View.Add(LogoImageView);

            //View.SendSubviewToBack(LogoImageView);

            this.CollectionView.BackgroundColor = UIColor.White.ColorWithAlpha(.8f);

            //this.View.BringSubviewToFront(this.CollectionView);



            classifications = new List<IClassification>();

            if (!Settings.IsRegistered)
            {
                classifications.Add(new Amphibian());
                classifications.Add(new Commercial());

                classifications.Add(new Experimental());
                classifications.Add(new Helicopter());

                classifications.Add(new LightSport());
                classifications.Add(new Jet());
                classifications.Add(new SingleEngine());
                classifications.Add(new PCL.Single());
                classifications.Add(new TwinPiston());
                classifications.Add(new TwinTurbine());
                classifications.Add(new Vintage());
                classifications.Add(new Warbird());
            }
            else
            {

                if (Settings.IsAmphibian)
                {
                    classifications.Add(new Amphibian());
                }

                if (Settings.IsCommercial)
                {
                    classifications.Add(new Commercial());
                }




                if (Settings.IsExperimental)
                {
                    classifications.Add(new Experimental());
                }
                if (Settings.IsHelicopter)
                {
                    classifications.Add(new Helicopter());
                }



                if (Settings.IsLightSport)
                {
                    classifications.Add(new LightSport());
                }


                if (Settings.IsJets)
                {
                    classifications.Add(new Jet());
                }
                if (Settings.IsSingleEngine)
                {
                    classifications.Add(new SingleEngine());
                }
                if (Settings.IsSingles)
                {
                    classifications.Add(new PCL.Single());
                }
                if (Settings.IsTwinPistons)
                {
                    classifications.Add(new TwinPiston());
                }
                if (Settings.IsTwinTurbines)
                {
                    classifications.Add(new TwinTurbine());
                }
                if (Settings.IsVintage)
                {
                    classifications.Add(new Vintage());
                }
                if (Settings.IsWarbirds)
                {
                    classifications.Add(new Warbird());
                }

            }


        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);


            if (!Settings.IsRegistered)
            {
                var bounds = UIScreen.MainScreen.Bounds;
                // show the loading overlay on the UI thread using the correct orientation sizing
                loadingOverlay = new BetterExperienceOverlay(bounds);
                loadingOverlay.ParentViewController = this;
                View.Add(loadingOverlay);
            }
            //CollectionView.DataSource = null;
            this.CollectionView.ReloadData();
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            if (BackgroundImageView != null)
            {
                BackgroundImageView.RemoveFromSuperview();
                BackgroundImageView.Dispose();
                BackgroundImageView = null;
            }
            if (LogoImageView != null)
            {
                LogoImageView.RemoveFromSuperview();
                LogoImageView.Dispose();
                LogoImageView = null;
            }

            if (loadingOverlay != null)
            {
                loadingOverlay.Hide();
                loadingOverlay = null;
                //loadingOverlay.Dispose();
            }
            //classifications.Clear();
            //CollectionView.ReloadData();
            //CollectionView.DataSource = null;
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

            LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, "Loading Aircraft...");
            this.View.AddSubview(loadingIndicator);
            string magazineTitle = "GlobalAir.com Showcase (" + classification.Name + ")";
            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back to Showcase", UIBarButtonItemStyle.Plain, null);
            Task.Run(async () =>
            {
                List<Ad> ads;
                if (!Settings.IsRegistered)
                {
                    ads = (await Ad.GetAdsByClassificationAsync(classification.Name)).ToList();
                }
                else
                {
                    ;
                    IEnumerable<Ad> adListEnumerable = await Ad.GetAdsByClassificationAsync(classification.Name);
                    switch (Settings.SortOptions)
                    {
                        case "No Preference":
                            {
                                ads = adListEnumerable.ToList();
                                break;
                            }
                        case "Recently Updated":
                            {
                                ads = adListEnumerable.OrderByDescending(row => DateTime.Parse(row.LastUpdated)).ToList();
                                break;
                            }
                        case "Price":
                            {
                                ads = adListEnumerable.OrderBy(row => row.Price == null || row.Price == string.Empty || row.Price == "N/A" ? 999999999 : double.Parse(row.Price, NumberStyles.Currency)).ToList();
                                break;
                            }
                        case "Total Time":
                            {
                                ads = adListEnumerable.OrderBy(row => double.Parse(row.TotalTime)).ToList();
                                break;
                            }
                        default:
                            {
                                ads = adListEnumerable.ToList();
                                break;
                            }
                    }
                }
                await AppDelegate.ProactivelyDownloadImages(ads);

            }).ContinueWith((task) =>
            {

                InvokeOnMainThread(() =>
                    {
                        loadingIndicator.Hide();
                        MagazineFlipBoardViewController flipboardVC = this.Storyboard.InstantiateViewController("MagazineFlipBoardViewController") as MagazineFlipBoardViewController;
                        flipboardVC.Title = magazineTitle;
                        flipboardVC.SelectedClassification = classification.Name;
                        //flipboardVC.TabBarController.HidesBottomBarWhenPushed = true;
                        this.ShowViewController(flipboardVC, this);
                        //NavigationController.PushViewController(flipboardVC, true);
                    });
            });


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
            //return true;
            return false;
        }


    }

}
