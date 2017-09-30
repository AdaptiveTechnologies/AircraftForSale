using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;
using System.Collections.Generic;
using System.Linq;
using AircraftForSale.PCL.Helpers;

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



		public override void ViewDidLoad()
		{
			//filteredLocations = null;
				
			locations = new List<Location>(Settings.LocationResponse.Locations);
			locations.RemoveAt(0);
			locations.AddRange(Settings.LocationResponse.StatesLst);
			locations.AddRange(Settings.LocationResponse.ProvinceLst);
			locations.Remove(locations.First(row => row.LocName == "Canada"));
			locations.Remove(locations.First(row => row.LocName == "USA"));
			locations = locations.OrderBy(row => row.DisplayOrder).ToList();

			
			searchController = new UISearchController((UIViewController)null);

			searchController.DimsBackgroundDuringPresentation = false;

			TableView.TableHeaderView = searchController.SearchBar;
			searchController.SearchBar.SizeToFit();

			searchController.SearchBar.ScopeButtonTitles = new[]{
				"USA", "Canada", "World"
			};

            //searchController.SearchBar.CancelButtonClicked += (sender, e) => {
            //    TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
            //};

			//searchController.SearchBar.ShowsScopeBar = true;

			//DefinesPresentationContext = true;

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
                CancelClicked = () => {
                    TableView.ContentInset = new UIEdgeInsets(0, 0, 0, 0);
                }
			};

			//TableView.ReloadData();

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
                TableView.ContentInset = new UIEdgeInsets(30, 0, 0, 0);
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

			if (searchController.Active)
			{
				searchController.DismissViewController(true, null);
			}

			this.NavigationController.PopViewController(true);
		}
	}
	class SearchDelegate : UISearchBarDelegate
	{
		public Action<string, string> DoFilter { get; set; }
        public Action CancelClicked { get; set; }

		public override void SelectedScopeButtonIndexChanged(UISearchBar searchBar, nint selectedScope)
		{
			string scope = searchBar.ScopeButtonTitles[selectedScope];
			string text = searchBar.Text;

			DoFilter(text, scope);
		}

        public override void CancelButtonClicked(UISearchBar searchBar)
        {
            if(CancelClicked != null){
                CancelClicked();
            }
        }

	}
}