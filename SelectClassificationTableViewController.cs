using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using AircraftForSale.PCL;

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

			this.TableView.ContentInset = new UIEdgeInsets(20, 0, 0, 0);

			this.TableView.RowHeight = 110;



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
	}
}