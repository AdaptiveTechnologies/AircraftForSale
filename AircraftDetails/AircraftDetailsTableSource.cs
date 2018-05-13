using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftForSale;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using Foundation;
using UIKit;

public class AircraftDetails
{
	public AircraftDetails(string label, string description, bool isButton, bool isSpecs = false, bool isRange = false)
	{
		Label = label;
		Description = description;
		IsDescriptionButton = isButton;
		IsSpecs = isSpecs;
		IsRange = isRange;
	}
	public String Label
	{
		get;
		set;
	}

	public String Description
	{
		get;
		set;
	}

	public bool IsDescriptionButton
	{
		get;
		set;
	}

	public bool IsSpecs
	{
		get;
		set;
	}

	public bool IsRange
	{
		get;
		set;
	}
}
public class AircraftDetailsTableSource : UITableViewSource
{

	AircraftDetails[] TableItems;
	public const string CellButtonIdentifier = "spec_cell_button";
	public const string CellLabelIdentifier = "spec_cell_label";
	public const string CellButtonOnlyIdentifier = "spec_cell_just_button";

	Ad ad;

	void AdSortButton_TouchUpInside(object sender, EventArgs e)
	{
		//Get current view controller
		var window = UIApplication.SharedApplication.KeyWindow;
		var vc = window.RootViewController;
		while (vc.PresentedViewController != null)
		{
			vc = vc.PresentedViewController;
		}

		if (!Settings.IsRegistered)
		{
			HelperMethods.SellerRegistrationRequiredSmallPrompt(vc, sender as UIButton);
			return;
		}

		bool isAdNameSort = false;



		LoadingOverlay loadingIndicator = new LoadingOverlay(vc.View.Frame, isAdNameSort ? "Loading Aircraft by Selected Type" : "Loading Aircraft by Selected Broker");
		vc.View.AddSubview(loadingIndicator);

		var pageViewController = vc.ParentViewController as UIPageViewController;
		var magFlipBoardViewController = pageViewController.ParentViewController as MagazineFlipBoardViewController;

		var modelController = magFlipBoardViewController.ModelController;
		List<Ad> adList = new List<Ad>();

		Task.Run(async () =>
		{

			adList = (await Ad.GetAdsByClassificationAsync(ad.Classification)).ToList();

			//get ads with this name and move them to the from of the list
			List<Ad> similarAdList = new List<Ad>();

			if (isAdNameSort)
			{
				similarAdList = adList.Where(row => row.Name == ad.Name).ToList();
			}
			else
			{
				similarAdList = adList.Where(row => row.BrokerName == ad.BrokerName).ToList();
			}


			for (int i = 0; i < similarAdList.Count(); i++)
			{
				adList.Remove(similarAdList[i]);
				adList.Insert(0, similarAdList[i]);
			}

			InvokeOnMainThread(() =>
			{
				modelController.LoadModalController(adList, ad.Classification);
				loadingIndicator.Hide();
				var startingViewController = modelController.GetViewController(0, false);
				var viewControllers = new UIViewController[] { startingViewController };
				pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

				HelperMethods.SendBasicAlert("", "Aircraft arranged by " + (isAdNameSort ? ad.Name : ad.BrokerName));
			});
		});
	}

	async void TechnicalSpecButton_TouchUpInside(object sender, EventArgs e)
	{
		//Get current view controller
		var window = UIApplication.SharedApplication.KeyWindow;
		var vc = window.RootViewController;
		while (vc.PresentedViewController != null)
		{
			vc = vc.PresentedViewController;
		}

		SpecResponse specResponse = new SpecResponse();

		var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

		if (specification.SpecId != 0)
		{
			var specTableViewController = vc.Storyboard.InstantiateViewController("SpecViewController_") as SpecViewController_;

			specTableViewController.Spec = specification;

			vc.PresentViewController(specTableViewController, true, null);
		}
		else
		{
			if (!Reachability.IsHostReachable(Settings._baseDomain))
			{
				var alert = UIAlertController.Create("Connect to a Network", "Please connect to a network to retrieve these aircraft specs", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
				if (alert.PopoverPresentationController != null)
				{
					alert.PopoverPresentationController.SourceView = vc.View;
					alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
				}
				vc.PresentViewController(alert, animated: true, completionHandler: null);
			}
		}
	}

	async void AdRGButton_TouchUpInside(object sender, EventArgs e)
	{
		//Get current view controller
		var window = UIApplication.SharedApplication.KeyWindow;
		var vc = window.RootViewController;
		while (vc.PresentedViewController != null)
		{
			vc = vc.PresentedViewController;
		}


		if (Reachability.IsHostReachable(Settings._baseDomain))
		{
			SpecResponse specResponse = new SpecResponse();

			var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

			if (specification.SpecId != 0)
			{

				MapViewController mapViewController = (MapViewController)vc.Storyboard.InstantiateViewController("MapViewController");

				mapViewController.SpecFieldList = specification.SpecFieldDictionary[SpecTableViewSource._rangeSection];
				vc.ShowDetailViewController(mapViewController, this);
			}



		}

		else
		{
			HelperMethods.SendBasicAlert("Connect to a Network", Settings._networkProblemMessage);
		}
	}

	public AircraftDetailsTableSource(AircraftDetails[] items, Ad ad)
	{
		TableItems = items;
		this.ad = ad;
	}

	public override nint RowsInSection(UITableView tableview, nint section)
	{
		return TableItems.Length;
	}

	public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
	{
		AircraftDetails item = TableItems[indexPath.Row];
		UITableViewCell cell;
		if (item.IsDescriptionButton)
		{
			cell = tableView.DequeueReusableCell(CellButtonIdentifier);
			var buttonCell = cell as AircraftDetailsButtonCell;
			buttonCell.UpdateCell(item);
			buttonCell.AircraftDescriptionButton.TouchUpInside -= AdSortButton_TouchUpInside;
			buttonCell.AircraftDescriptionButton.TouchUpInside += AdSortButton_TouchUpInside;
			return buttonCell;
		}
		else
		{
			if (item.IsSpecs || item.IsRange)
			{
				cell = tableView.DequeueReusableCell(CellButtonOnlyIdentifier);
				var justButtonCell = cell as AircraftDetailsJustButtonCell;

				justButtonCell.ButtonProperty.TouchUpInside -= TechnicalSpecButton_TouchUpInside;
				justButtonCell.ButtonProperty.TouchUpInside -= AdRGButton_TouchUpInside;

				if (item.IsRange)
				{
					justButtonCell.ButtonProperty.TouchUpInside += AdRGButton_TouchUpInside;
				}
				if (item.IsSpecs)
				{
					justButtonCell.ButtonProperty.TouchUpInside += TechnicalSpecButton_TouchUpInside;
				}

				justButtonCell.UpdateCell(item);

				return justButtonCell;
			}
			else
			{
				cell = tableView.DequeueReusableCell(CellLabelIdentifier);
				var labelCell = cell as AircraftDetailsLabelCell;
				labelCell.UpdateCell(item);
				return labelCell;
			}

		}
	}
}
