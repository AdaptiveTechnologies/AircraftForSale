using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;

namespace AircraftForSale
{
	public partial class SelectClassificationTableViewController : UITableViewController
	{
		public SelectClassificationTableViewController(IntPtr handle) : base(handle)
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
		}

		static NSString classificationCellID = new NSString("cell_id");
		static NSString headerId = new NSString("Header");
		List<IClassification> classifications;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//this.TableView.ContentInset = new UIEdgeInsets(20, 0, 0, 0);

			this.TableView.RowHeight = 110;

			this.NavigationController.NavigationBar.BarTintColor = HelperMethods.GetLime();

			var nextBarButtonItem = new UIBarButtonItem("Next", UIBarButtonItemStyle.Plain, (sender, args) =>
			{
//StepTwoSeque
                PerformSegue("StepTwoSeque", this);
			});
			UITextAttributes icoFontAttribute = new UITextAttributes();
			icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(20);
        	icoFontAttribute.TextColor = UIColor.White;

			nextBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);

            this.NavigationItem.SetRightBarButtonItem(nextBarButtonItem, true);

			var cancelBarButtonItem = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, (sender, args) =>
			{
							// button was clicked
			});

			cancelBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);

			this.NavigationItem.SetLeftBarButtonItem(cancelBarButtonItem, true);

			NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);

		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{

			var classificationCell = (SelectClassificationTableViewCell)TableView.DequeueReusableCell(classificationCellID, indexPath);

			var classification = classifications[indexPath.Row];


			if (classification.IsSelected)
			{
				classificationCell.Selected = true;

			}

			classificationCell.UpdateData(classification);

			return classificationCell;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return classifications.Count;


		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var classification = classifications[(int)indexPath.Item];
			classification.IsSelected = true;
            Settings.SaveClassification(classification.Name, true);
		}

		public override void RowDeselected(UITableView tableView, NSIndexPath indexPath)
		{
			var classification = classifications[(int)indexPath.Item];
			classification.IsSelected = false;
            Settings.SaveClassification(classification.Name, false);
		}
	}
}