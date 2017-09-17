using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using System.Drawing;
using AircraftForSale.PCL;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class Page3RegistrationViewController : UITableViewController, IMultiStepProcessStep
	{
		void HoursTextField_EditingDidEnd(object sender, EventArgs e)
		{
			int hours = 0;
			bool isInteger = int.TryParse(HoursTextField.Text, out hours);
			if (isInteger)
			{
				Settings.Hours = hours;
			}
			else {
				if (HoursTextField.Text != string.Empty)
				{
					HoursTextField.Text = string.Empty;
					HelperMethods.SendBasicAlert("Validation", "Must input an integer (i.e. 115)");
				}
			}
		}

		async void CompleteRegistrationButton_TouchUpInside(object sender, EventArgs e)
		{
			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
             LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, "Loading ...");
			this.View.AddSubview(loadingIndicator);
				//Attempt to register in the API
				var response = await MagAppRegisterResponse.RegisterAsync();

				if (response.Status == "Success")
				{

					Settings.IsRegistered = true;

					var alert = UIAlertController.Create("Congratulations!", "You have successfully registered.", UIAlertControllerStyle.Alert);

                     alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
					{


					}));

					PresentViewController(alert, animated: true, completionHandler: () =>
					 {
						 RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;

                        var loginVC = registrationVC.PresentingViewController as LoginViewController;
                        registrationVC.DismissViewController(true, null);
						 if (loginVC != null)
						 {
							 loginVC.PerformSegue("LoadTabBarControllerSeque", loginVC);
						 }
					 });



				}
				else {
					loadingIndicator.Hide();
					HelperMethods.SendBasicAlert("Oops.", "There was a problem registering. Please try again.");
				}


			}
			else {
				HelperMethods.SendBasicAlert("Please connect to the internet", "Internet access is required.");
			}
		}

		public Page3RegistrationViewController(IntPtr handle) : base(handle)
		{
		}

		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;

		//Hide keyboard when touch anywhere
		public UITapGestureRecognizer HideKeyboardGesture
		{
			get;
			set;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

       UIImageView background = new UIImageView();
//assign our image file to the UIImageView
       background.Image = UIImage.FromBundle("new_home_bg1");

	//assign the UIImageView as the background view of the table
			this.TableView.BackgroundView = background;
 
    //set the textbox background to light gray
    this.TableView.BackgroundColor = UIColor.LightGray;
 
    //set the background of each table cell to clear
    foreach (var cell in this.TableView.VisibleCells)

	{
	cell.BackgroundColor = UIColor.Clear;
	cell.SelectionStyle = UITableViewCellSelectionStyle.None;

}

			var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;

			MultiStepProcessHorizontalViewController parentViewController = this.ParentViewController as MultiStepProcessHorizontalViewController;
			var navigationController = parentViewController.ContainerViewController.NavigationController;
			var navBarHeight = navigationController.NavigationBar.Bounds.Height;
			TableView.ContentInset = new UIEdgeInsets(statusBarHeight + navBarHeight, 0, 50, 0);

			ManufacturePicker.Alpha = 0f;
			DesigPickerView.Alpha = 0f;

			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				//ClassWidthConstraint.Constant = 150f;
				//ManufactureWidthConstraint.Constant = 150f;
				//DesigWidthConstraint.Constant = 150f;
			}

			ClassPicker.Model = new ClassModel(this);
			TimeframePicker.Model = new TimeframeModel();
			PurposePicker.Model = new PurposeModel();
			SortOptionsPicker.Model = new SortOptionsModel();

			if (Settings.ClassificationId != 0)
			{
				int classIndex = Settings.ClassificationList.IndexOf(Settings.ClassificationList.FirstOrDefault(row => row.ClassificationId == Settings.ClassificationId));
				ClassPicker.Select(classIndex, 0, true);
				ClassPicker.Model.Selected(ClassPicker, classIndex, 0);
			}

			if (Settings.PurchaseTimeFrame != 0)
			{
				int classIndex = 0;
				if(Settings.PurchaseTimeFrame == 3)
				{
					classIndex = 1;
				}
				if (Settings.PurchaseTimeFrame == 12)
				{
					classIndex = 2;
				}
				TimeframePicker.Select(classIndex, 0, true);
				TimeframePicker.Model.Selected(TimeframePicker, classIndex, 0);
			}

			if (Settings.PurposeId != 0)
			{
				//Settings.LocationResponse.PurposeForFlying
				int purposeIndex = Settings.LocationResponse.PurposeForFlying.IndexOf(Settings.LocationResponse.PurposeForFlying.FirstOrDefault(row => row.FlyingPurposeId == Settings.PurposeId));
				PurposePicker.Select(purposeIndex, 0, true);
				PurposePicker.Model.Selected(PurposePicker, purposeIndex, 0);
			}

			HoursTextField.Text = Settings.Hours.ToString();

			if (Settings.SortOptions != string.Empty)
			{
				//SortOptionsList.Add("No Preference");
				//SortOptionsList.Add("Recently Updated");
				//SortOptionsList.Add("Price");
				//SortOptionsList.Add("Total Time");
				int sortIndex = 0;
				if (Settings.SortOptions == "No Preference")
				{
					sortIndex = 0;
				}

				if (Settings.SortOptions == "Recently Updated")
				{
					sortIndex = 1;
				}

				if (Settings.SortOptions == "Price")
				{
					sortIndex = 2;
				}

				if (Settings.SortOptions == "Total Time")
				{
					sortIndex = 3;
				}
				SortOptionsPicker.Select(sortIndex, 0, true);
				SortOptionsPicker.Model.Selected(SortOptionsPicker, sortIndex, 0);

			}

			//hide keyboard when touch anywhere
			HideKeyboardGesture = new UITapGestureRecognizer(() =>
			{
				View.EndEditing(true);

			});

		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			StepActivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });

			//Register events
			View.AddGestureRecognizer(HideKeyboardGesture);
			CompleteRegistrationButton.TouchUpInside += CompleteRegistrationButton_TouchUpInside;
			HoursTextField.EditingDidEnd += HoursTextField_EditingDidEnd;
		}

public override UIView GetViewForHeader(UITableView tableView, nint section)
{
	UILabel lblHeader = new UILabel(new CGRect(1, 1, tableView.SectionHeaderHeight, tableView.Frame.Width));
			if (section == 0) lblHeader.Text = "  AIRCRAFT INTEREST (Optional)";
			else if(section ==1) lblHeader.Text = "  WHEN ARE YOU GOING TO BUY? (Optional)";
			else if(section ==2) lblHeader.Text = "  WHY DO YOU FLY? (Optional)";
			else if(section ==3) lblHeader.Text = "  Flight Hours Per Month?(Optional)";
			else if (section == 4) lblHeader.Text = "  ORDER BY (Optional)";
	lblHeader.TextColor = UIColor.Green;
	return lblHeader;
}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });

			//Unregister events
			View.RemoveGestureRecognizer(HideKeyboardGesture);
			CompleteRegistrationButton.TouchUpInside -= CompleteRegistrationButton_TouchUpInside;
			HoursTextField.EditingDidEnd -= HoursTextField_EditingDidEnd;
		}

		public UIPickerView ClassPickerProperty
		{
			get
			{
				return ClassPicker;
			}
		}

		public UIPickerView ManufacturePickerProperty
		{
			get
			{
				return ManufacturePicker;
			}
		}

		public UIPickerView DesigPickerProperty
		{
			get
			{
				return DesigPickerView;
			}
		}

		public CMMDResponse CMMDResonseProperty
		{
			get;
			set;
		}
	}

	public class ClassModel : UIPickerViewModel
	{
		WeakReference parent;
		public Page3RegistrationViewController Owner
		{
			get
			{
				return parent.Target as Page3RegistrationViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}
		public ClassModel(Page3RegistrationViewController owner)
		{
			Owner = owner;
		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Settings.ClassificationList.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return Settings.ClassificationList[(int)row].ClassificationName;
				default:
					throw new NotImplementedException();
			}
		}

		public async override void Selected(UIPickerView picker, nint row, nint component)
		{
			var selected = Settings.ClassificationList[(int)picker.SelectedRowInComponent(0)];

			//Assign value to registration object
			Settings.ClassificationId = selected.ClassificationId;

			//if class value is selected, show designation)
			if (selected.ClassificationId != 0)
			{
				if (Reachability.IsHostReachable(Settings._baseDomain))
				{
					var manufactuerResponse = await CMMDResponse.GetCMMDResponseAsync(Settings.ClassificationId, 0);
					Owner.CMMDResonseProperty = manufactuerResponse;
					Owner.ManufacturePickerProperty.Alpha = 1f;
					Owner.ManufacturePickerProperty.Model = new ManufactureModel(Owner);

					if (Settings.ManufacturerId != 0)
					{
						int manufacturerIndex = manufactuerResponse.MfgLst.IndexOf(manufactuerResponse.MfgLst.FirstOrDefault(r => r.ManufacturerId == Settings.ManufacturerId));
						Owner.ManufacturePickerProperty.Select(manufacturerIndex, 0, true);
						Owner.ManufacturePickerProperty.Model.Selected(Owner.ManufacturePickerProperty, manufacturerIndex, 0);
					}
				}
				else
				{
					var alert = UIAlertController.Create("Please connect to the internet", "Internet access is required.", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
					{
						this.Selected(picker, 0, 0);

					}));

					Owner.PresentViewController(alert, animated: true, completionHandler: () =>
						{

						});
				}

			}
			else {
				Owner.ManufacturePickerProperty.Alpha = 0f;
				Owner.DesigPickerProperty.Alpha = 0f;
			}
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(10f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(18f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = Settings.ClassificationList[(int)row].ClassificationName;
			return lbl;
		}
	}

	public class ManufactureModel : UIPickerViewModel
	{
		WeakReference parent;
		public Page3RegistrationViewController Owner
		{
			get
			{
				return parent.Target as Page3RegistrationViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public List<MfgLst> MfgLst { get; set; }

		public ManufactureModel(Page3RegistrationViewController owner)
		{
			Owner = owner;

			MfgLst = Owner.CMMDResonseProperty.MfgLst;
		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return MfgLst.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return MfgLst[(int)row].Manufacturer;
				default:
					throw new NotImplementedException();
			}
		}

		public async override void Selected(UIPickerView picker, nint row, nint component)
		{


			var selected = MfgLst[(int)picker.SelectedRowInComponent(0)];

			//Assign value to registration obje
			Settings.ManufacturerId = selected.ManufacturerId;

			if (selected.ManufacturerId != 0)
			{
				if (Reachability.IsHostReachable(Settings._baseDomain))
				{
					var manufactuerResponse = await CMMDResponse.GetCMMDResponseAsync(Settings.ClassificationId, Settings.ManufacturerId);
					Owner.CMMDResonseProperty = manufactuerResponse;
					Owner.DesigPickerProperty.Alpha = 1f;
					Owner.DesigPickerProperty.Model = new DesigModel(Owner);

					if (Settings.DesignationId != 0)
					{
						int desigIndex = manufactuerResponse.ModDesLst.IndexOf(manufactuerResponse.ModDesLst.FirstOrDefault(r => r.DesignationId == Settings.DesignationId));
						//int desigIndex = 1;
						Owner.DesigPickerProperty.Select(desigIndex, 0, true);
						Owner.DesigPickerProperty.Model.Selected(Owner.DesigPickerProperty, desigIndex, 0);
					}
				}
				else
				{
					var alert = UIAlertController.Create("Please connect to the internet", "Internet access is required.", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
					{
						this.Selected(picker, 0, 0);

					}));

					Owner.PresentViewController(alert, animated: true, completionHandler: () =>
						{

						});
				}
			}
			else {
				Owner.DesigPickerProperty.Alpha = 0f;
			}
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(10f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(18f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = MfgLst[(int)row].Manufacturer;
			return lbl;
		}
	}

	public class DesigModel : UIPickerViewModel
	{
		WeakReference parent;
		public Page3RegistrationViewController Owner
		{
			get
			{
				return parent.Target as Page3RegistrationViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}
		public DesigModel(Page3RegistrationViewController owner)
		{
			Owner = owner;
		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Owner.CMMDResonseProperty.ModDesLst.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return Owner.CMMDResonseProperty.ModDesLst[(int)row].Designation;
				default:
					throw new NotImplementedException();
			}
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			var selected = Owner.CMMDResonseProperty.ModDesLst[(int)picker.SelectedRowInComponent(0)];

			//Assign value to registration obje
			Settings.DesignationId = selected.DesignationId;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(10f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(18f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			String designation = Owner.CMMDResonseProperty.ModDesLst[(int)row].Designation;
			lbl.Text = designation.Length == 0 ? "Select Model" : designation;
			return lbl;
		}
	}

	public class TimeframeModel : UIPickerViewModel
	{


		public List<Tuple<string, DateTime>> TimeFramePickerOptions
		{
			get;
			set;
		}
		public TimeframeModel()
		{

			TimeFramePickerOptions = new List<Tuple<string, DateTime>>();
			TimeFramePickerOptions.Add(new Tuple<string, DateTime>("No Specific", DateTime.Now));
			TimeFramePickerOptions.Add(new Tuple<string, DateTime>("0 - 3 months", DateTime.Now.AddMonths(3)));
			TimeFramePickerOptions.Add(new Tuple<string, DateTime>("4 - 12 months", DateTime.Now.AddMonths(12)));


		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return TimeFramePickerOptions.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return TimeFramePickerOptions[(int)row].Item1;
				default:
					throw new NotImplementedException();
			}
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			var selected = TimeFramePickerOptions[(int)picker.SelectedRowInComponent(0)];

			if (selected.Item2.Date != DateTime.Now.Date)
			{
				int months = ((selected.Item2.Date - DateTime.Now.Date).Days / 30);
				//Assign value to registration obje
				Settings.PurchaseTimeFrame = months;
			}
			else {
				Settings.PurchaseTimeFrame = 0;
			}
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(10f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(18f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = TimeFramePickerOptions[(int)row].Item1;
			return lbl;
		}
	}

	public class SortOptionsModel : UIPickerViewModel
	{

		List<string> SortOptionsList { get; set; }

		public SortOptionsModel()
		{
			SortOptionsList = new List<string>();
			SortOptionsList.Add("No Preference");
			SortOptionsList.Add("Recently Updated");
			SortOptionsList.Add("Price");
			SortOptionsList.Add("Total Time");

		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return SortOptionsList.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return SortOptionsList[(int)row];
				default:
					throw new NotImplementedException();
			}
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			var selected = SortOptionsList[(int)picker.SelectedRowInComponent(0)];

			Settings.SortOptions = selected;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(10f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(18f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = SortOptionsList[(int)row];
			return lbl;
		}
	}

	public class PurposeModel : UIPickerViewModel
	{


		public PurposeModel()
		{


		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Settings.LocationResponse.PurposeForFlying.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return Settings.LocationResponse.PurposeForFlying[(int)row].Purpose;
				default:
					throw new NotImplementedException();
			}
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			var selected = Settings.LocationResponse.PurposeForFlying[(int)picker.SelectedRowInComponent(0)];

			Settings.PurposeId = selected.FlyingPurposeId;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(10f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(18f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			if (row != 0)
			{
				lbl.Text = Settings.LocationResponse.PurposeForFlying[(int)row].Purpose;
			}
			else
			{
				lbl.Text = "Select from list";
			}
			return lbl;
		}
	}
}