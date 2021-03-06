using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;
using System.Collections.Generic;
using System.Linq;
using AircraftForSale.PCL.Helpers;
using Google.Analytics;

namespace AircraftForSale
{
    public partial class SearchLocationTableViewController : UITableViewController, IUISearchResultsUpdating
    {
        //Location_CellID
        const string CellIdentifier = "Location_CellID";
        UISearchController searchController;
        public SearchLocationTableViewController(IntPtr handle) : base(handle)
        {

        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // This screen name value will remain set on the tracker and sent with
            // hits until it is set to a new value or to null.
            Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "SearchLocation View (Registration)");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
        }

        public override void ViewDidLoad()
        {

            locations = new List<Location>(Settings.LocationResponse.Locations);
            locations.RemoveAt(0);
            locations.AddRange(Settings.LocationResponse.StatesLst);
            locations.AddRange(Settings.LocationResponse.ProvinceLst);
            locations.Remove(locations.First(row => row.LocName == "Canada"));
            locations.Remove(locations.First(row => row.LocName == "USA"));
            locations = locations.OrderBy(row => row.DisplayOrder).ToList();


            searchController = new UISearchController((UIViewController)null);
            searchController.SearchBar.BackgroundColor = UIColor.White;

           
            searchController.DimsBackgroundDuringPresentation = false;

            this.NavigationItem.SearchController = searchController;
            this.NavigationItem.HidesSearchBarWhenScrolling = false;

            searchController.SearchBar.ScopeButtonTitles = new[]{
                "USA", "Canada", "World"
            };


            searchController.SearchResultsUpdater = this;

            searchController.SearchBar.WeakDelegate = new SearchDelegate()
            {
                DoFilter = (txt, scope) =>
                {
                    if (searchController.Active)
                        filteredLocations = new List<Location>();
                    else
                        filteredLocations = null;

                    string scopeShortForm = "";
                    if (scope == "World")
                    {
                        scopeShortForm = "WO";
                    }
                    if (scope == "Canada")
                    {
                        scopeShortForm = "CA";
                    }
                    if (scope == "USA")
                    {
                        scopeShortForm = "US";
                    }



                    FilterContentForSearchText(txt, scopeShortForm);
                },
                CancelClicked = () =>
                {
                    TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
                },

            };

        }

        public List<Location> locations { get; set; }

        public List<Location> filteredLocations { get; set; }
        public void UpdateSearchResultsForSearchController(UISearchController searchController)
        {
            if (searchController.Active)
                filteredLocations = new List<Location>();
            else
                filteredLocations = null;

            var textToSearchFor = searchController.SearchBar.Text;
            var index = searchController.SearchBar.SelectedScopeButtonIndex;
            var scope = searchController.SearchBar.ScopeButtonTitles[index];

            string scopeShortForm = "";
            if (scope == "World")
            {
                scopeShortForm = "WO";
            }
            if (scope == "Canada")
            {
                scopeShortForm = "CA";
            }
            if (scope == "USA")
            {
                scopeShortForm = "US";
            }


            FilterContentForSearchText(searchController.SearchBar.Text, scopeShortForm);
        }

        void FilterContentForSearchText(string text, string scope)
        {
            InvokeOnMainThread(() =>
            {
                if (filteredLocations != null)
                {
                    filteredLocations.Clear();
                    filteredLocations.AddRange(
                    locations.Where(e =>
                                    (string.IsNullOrWhiteSpace(text) || e.LocName.ToUpper().Contains(text.ToUpper())) &&
                                    (string.IsNullOrEmpty(scope) || e.MapCCode.ToUpper() == scope.ToUpper())
                     ));

                }

                TableView.ReloadData();





            });
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            if (filteredLocations != null)
                return filteredLocations.Count;

            return locations.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentifier, indexPath);

            Location location = (filteredLocations != null)
                ? filteredLocations[indexPath.Row]
                : locations[indexPath.Row];

            cell.TextLabel.Text = location.LocName;

            string mapCCodeFriendly = "";
            if (location.MapCCode == "WO")
            {
                mapCCodeFriendly = "World";
            }
            if (location.MapCCode == "CA")
            {
                mapCCodeFriendly = "Canada";
            }
            if (location.MapCCode == "US")
            {
                mapCCodeFriendly = "USA";
            }
            cell.DetailTextLabel.Text = mapCCodeFriendly;


            return cell;

        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {


            Location location = (filteredLocations != null)
                ? filteredLocations[indexPath.Row]
                : locations[indexPath.Row];

            Settings.LocationPickerSelectedId = location.LocationId;
            Settings.LocationPickerSelectedName = location.LocName;

            this.NavigationController.PopViewController(true);
        }
    }
    class SearchDelegate : UISearchBarDelegate
    {
        public Action<string, string> DoFilter { get; set; }
        public Action CancelClicked { get; set; }
        public Action BeginEditing { get; set; }

        public override void SelectedScopeButtonIndexChanged(UISearchBar searchBar, nint selectedScope)
        {
            string scope = searchBar.ScopeButtonTitles[selectedScope];
            string text = searchBar.Text;

            DoFilter(text, scope);
        }

        public override void CancelButtonClicked(UISearchBar searchBar)
        {
            if (CancelClicked != null)
            {
                CancelClicked();
            }
        }


    }
}