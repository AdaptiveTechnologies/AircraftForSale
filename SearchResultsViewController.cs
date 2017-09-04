using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using SDWebImage;

namespace AircraftForSale
{
    public partial class SearchResultsViewController : UIViewController
    {
        public List<Ad> SearchResultsAdList
        {
            get;
            set;
        }

       

        public UITableView SearchResultsTableView
        {
            get;
            set;
        }


        public SearchResultsViewController()
		{
            //this.AdSelectedAction = addSelectedAction;
		}

        public Ad SelectedAd
        {
            get;
            set;
        }

        public Action AdSelectedAction
        {
            get;
            set;
        }

        //~SearchResultsViewController()
        //{
        //  Console.WriteLine("SearchResultsViewController is about to be garbage collected");
        //}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //this.NavigationController.Title = "Search Results";

            this.NavigationItem.Title = "Search Results";

            View.BackgroundColor = UIColor.Black;

            var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;

            var closeButton = new UIButton(new CGRect(View.Bounds.Width - 110, statusBarHeight, 100, 25));
            closeButton.SetTitle("Close", UIControlState.Normal);
            closeButton.SetTitleColor(UIColor.White, UIControlState.Normal);

            View.Add(closeButton);


            //var titleView = new UITextView(titleRect);

           


			

            var tableViewYValue = statusBarHeight + (closeButton.Bounds.Height + 5);
            var tableViewHeight = this.View.Bounds.Height - tableViewYValue;


            SearchResultsTableView = new UITableView(new CGRect(0, tableViewYValue,this.View.Bounds.Width, tableViewHeight));

			var backgroundImage = UIImage.FromBundle("new_home_bg1.png").ResizeImage((float)SearchResultsTableView.Bounds.Width, (float)SearchResultsTableView.Bounds.Height);
			SearchResultsTableView.BackgroundColor = UIColor.FromPatternImage(backgroundImage);

            View.Add(SearchResultsTableView);

            var titleViewHeight = this.View.Bounds.Height - (this.SearchResultsTableView.Bounds.Height + statusBarHeight);
			var titleView = new UITextView(new CGRect(0, 0, this.View.Bounds.Width * .4f, titleViewHeight));

            titleView.Center = new CGPoint(View.Frame.Size.Width / 2, (titleViewHeight + statusBarHeight)/ 2f);

			titleView.Text = "Search Results";
			titleView.TextAlignment = UITextAlignment.Center;
			titleView.TextColor = UIColor.White;
			titleView.BackgroundColor = UIColor.Clear;
			titleView.Font = UIFont.BoldSystemFontOfSize(UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 15f : 20f);
			titleView.AdjustsFontForContentSizeCategory = true;


			this.View.Add(titleView);

            closeButton.TouchUpInside += (sender, e) =>
            {
                this.DismissViewController(true, null);
            };

            SearchResultsTableView.Source = new SearchResultsTableSource(this);

        }

    }



    public class SearchResultsTableSource : UITableViewSource
    {

        WeakReference parent;
        public SearchResultsViewController Owner
        {
            get
            {
                return parent.Target as SearchResultsViewController;
            }
            set
            {
                parent = new WeakReference(value);
            }
        }

        NSString CellIdentifier = (NSString)"searchResultsPrototypeCell";

        public SearchResultsTableSource(SearchResultsViewController owner)
        {
            Owner = owner;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return Owner.SearchResultsAdList.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);

            if (cell == null)
            {

                cell = new UITableViewCell(UITableViewCellStyle.Subtitle, CellIdentifier);
                cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
                var backgroundColor = UIColor.FromWhiteAlpha(1f, .3f);
                cell.BackgroundColor = backgroundColor;
            }
           


          
            Ad ad = Owner.SearchResultsAdList[indexPath.Row];

            //cell.UpdateCell(item, this);
            cell.TextLabel.Text = ad.Name;
            cell.DetailTextLabel.Text = ad.Teaser;

            var image = SDImageCache.SharedImageCache.ImageFromDiskCache(ad.ImageURL);
            var maxWidth = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 100f : 150f;
            image = image.MaxResizeImage(maxWidth, maxWidth);

            cell.ImageView.Image = image;

			//cell.ImageView.SetImage(
			//	url: new NSUrl(item.ImageURL),
			//	placeholder: UIImage.FromBundle("ad_placeholder.jpg")
			//);

            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var maxWidth = UIKit.UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 100f : 150f;
            return maxWidth;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath);


            Owner.SelectedAd = Owner.SearchResultsAdList[indexPath.Row];

            if(Owner.AdSelectedAction != null){
                Owner.AdSelectedAction.Invoke();
            }

            Owner.DismissViewController(true, null);


            tableView.DeselectRow(indexPath, true);
        }


    }

}