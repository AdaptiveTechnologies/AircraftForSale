using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;

namespace AircraftForSale
{
    public partial class SearchResultsViewController : UIViewController
    {
        public List<Ad> SearchResultsAdList
        {
            get;
            set;
        }

        public UITableView SearchResultsTableViewProperty
        {
            get
            {
                return SearchResultsTableView;
            }
        }

        public SearchResultsViewController(IntPtr handle) : base(handle)
        {
        }

        //~SearchResultsViewController()
        //{
        //  Console.WriteLine("SearchResultsViewController is about to be garbage collected");
        //}

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;

            var closeButton = new UIButton(new CGRect(View.Bounds.Width - 110, statusBarHeight, 100, 25));
            closeButton.SetTitle("Close", UIControlState.Normal);
            closeButton.SetTitleColor(UIColor.White, UIControlState.Normal);

            View.Add(closeButton);

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
            SearchResultsTableCell cell = tableView.DequeueReusableCell(CellIdentifier) as SearchResultsTableCell;
            Ad item = Owner.SearchResultsAdList[indexPath.Row];

            cell.UpdateCell(item, this);
            return cell;
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 100;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.CellAt(indexPath);


            if (Reachability.IsHostReachable(Settings._baseDomain))
            {
                var ad = Owner.SearchResultsAdList[indexPath.Row];
                var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;
                var tabBarHeight = Owner.TabBarController.TabBar.Bounds.Height;

                var frame = new CGRect(0, statusBarHeight, Owner.View.Bounds.Width, Owner.View.Bounds.Height - (statusBarHeight + tabBarHeight));
                var webView = new UIWebView(frame);

                LoadingOverlay loadingOverlay = new LoadingOverlay(Owner.View.Frame);

                webView.LoadFinished += (sender, e) =>
                {
                    loadingOverlay.Hide();
                };


                var url = ad.AircraftForSaleURL;
                webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

                UIView.BeginAnimations("fadeflag");
                UIView.Animate(1, () =>
                {
                    cell.Alpha = .5f;
                }, () =>
                {

                    Owner.View.AddSubview(webView);
                    Owner.View.AddSubview(loadingOverlay);

                    UIButton closeButton = new UIButton(new CGRect(Owner.View.Bounds.Width - 50, 0, 50, 50));
                    closeButton.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
                    closeButton.BackgroundColor = UIColor.Black;
                    closeButton.TouchUpInside += (sender, e) =>
                    {
                        try
                        {
                            webView.RemoveFromSuperview();
                            closeButton.RemoveFromSuperview();
                        }
                        finally
                        {
                            webView.Dispose();
                        }
                    };
                    //Owner.View.AddSubview(closeButton);
                    webView.AddSubview(closeButton);

                    cell.Alpha = 1f;
                });

                UIView.CommitAnimations();
                //}
            }
            else
            {
                HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
            }




            tableView.DeselectRow(indexPath, true);
        }
    }
}