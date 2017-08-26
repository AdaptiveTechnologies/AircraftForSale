using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using System.Drawing;
using System.Linq;

using CoreGraphics;

using Google.Analytics;

namespace AircraftForSale
{
	public partial class Page2RegistrationViewController : UITableViewController, IMultiStepProcessStep
	{
		void NextButton_TouchUpInside(object sender, EventArgs e)
		{
			RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
			var stepFour = registrationVC.Steps[3];
			registrationVC._pageViewController.SetViewControllers(new[] { stepFour as UIViewController }, UIPageViewControllerNavigationDirection.Forward, true, (finished) =>
			{
				if (finished)
				{
					//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

				}
			});
		}

		void PilotSwitch_ValueChanged(object sender, EventArgs e)
		{
			if (PilotSwitch.On)
			{
				TypeLabel.Alpha = 1f;
				TypePickerView.Alpha = 1f;

				Settings.IsPilot = true;
			}
			else {
				TypeLabel.Alpha = 0f;
				RatingLabel.Alpha = 0f;

				TypePickerView.Alpha = 0f;
				RatingPickerView.Alpha = 0f;

				Settings.IsPilot = false;
				Settings.PilotTypeId = 0;
				Settings.PilotStatusId = 0;
			}
		}

		public UIPickerView RatingPickerViewProperty
		{
			get
			{
				return RatingPickerView;

			}
		}

		public UILabel RatingLabelProperty
		{
			get
			{
				return RatingLabel;
			}
		}


		public Page2RegistrationViewController(IntPtr handle) : base(handle)
		{
		}




		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;

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

			TypeLabel.Alpha = 0f;
			RatingLabel.Alpha = 0f;

			TypePickerView.Alpha = 0f;
			RatingPickerView.Alpha = 0f;

			TypePickerView.Model = new PilotTypeModel(this);
			RatingPickerView.Model = new RatingModel();

			if (Settings.IsPilot)
			{
				PilotSwitch.On = true;
				TypeLabel.Alpha = 1f;
				TypePickerView.Alpha = 1f;

				int statusIndex = Settings.LocationResponse.AreYouAPilot.IndexOf(Settings.LocationResponse.AreYouAPilot.FirstOrDefault(row => row.PilotStatusId == Settings.PilotStatusId));
				TypePickerView.Select(statusIndex, 0, true);
				TypePickerView.Model.Selected(TypePickerView, statusIndex, 0);



				int ratingIndex = Settings.LocationResponse.PilotRating.IndexOf(Settings.LocationResponse.PilotRating.FirstOrDefault(row => row.PilotTypeId == Settings.PilotTypeId));
				RatingPickerView.Select(ratingIndex, 0, true);
				RatingPickerView.Model.Selected(RatingPickerView, ratingIndex, 0);
			}
			else {
				PilotSwitch.On = false;
				TypeLabel.Alpha = 0f;
				TypePickerView.Alpha = 0f;
			}





		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

				base.ViewDidAppear(animated);

// This screen name value will remain set on the tracker and sent with
// hits until it is set to a new value or to null.
Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "Page View");

	Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());

			StepActivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });


			bool needsToReturnToPreviousStep = false;
			//Ensure required fields are input
			string validationMessage = string.Empty;
			if (Settings.Email == null || Settings.Email == string.Empty)
			{
				validationMessage = "Please input a valid email address";
				needsToReturnToPreviousStep = true;
			}
			if (Settings.Password == null || Settings.Password == string.Empty)
			{
				validationMessage = "Please input a valid password";
				needsToReturnToPreviousStep = true;
			}
			if (Settings.FirstName == null || Settings.FirstName == string.Empty)
			{
				validationMessage = "Please input your first name";
				needsToReturnToPreviousStep = true;
			}
			if (Settings.LastName == null || Settings.LastName == string.Empty)
			{
				validationMessage = "Please input your last name";
				needsToReturnToPreviousStep = true;
			}
			if (Settings.Phone == null || Settings.Phone == string.Empty)
			{
				validationMessage = "Please input your cell phone number";
				needsToReturnToPreviousStep = true;
			}
			if (Settings.LocationPickerSelectedId == 0)
			{
				validationMessage = "Please select your location";
				needsToReturnToPreviousStep = true;
			}
			if (needsToReturnToPreviousStep)
			{
				HelperMethods.SendBasicAlert("Validation", validationMessage);
				RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
				var secondStep = registrationVC.Steps[1];
				InvokeOnMainThread(() =>
				{
					registrationVC._pageViewController.SetViewControllers(new[] { secondStep as UIViewController }, UIPageViewControllerNavigationDirection.Reverse, true, (finished) =>
					{
						if (finished)
						{
							//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

						}
					});
				});
			}


			//Register events
			PilotSwitch.ValueChanged += PilotSwitch_ValueChanged;
			NextButton.TouchUpInside += NextButton_TouchUpInside;
		}

public override UIView GetViewForHeader(UITableView tableView, nint section)
{
	UILabel lblHeader = new UILabel(new CGRect(1, 1, tableView.SectionHeaderHeight, tableView.Frame.Width));
			if (section == 0) lblHeader.Text = "PILOT INFORMATION (Optional)";
	else lblHeader.Text = "BASIC INFORMATION";
	lblHeader.TextColor = UIColor.Green;
	return lblHeader;
}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });

			//Unregister events
			PilotSwitch.ValueChanged -= PilotSwitch_ValueChanged;
			NextButton.TouchUpInside -= NextButton_TouchUpInside;
		}
	}

	public class PilotTypeModel : UIPickerViewModel
	{
		WeakReference parent;
		public Page2RegistrationViewController Owner
		{
			get
			{
				return parent.Target as Page2RegistrationViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}
		public PilotTypeModel(Page2RegistrationViewController owner)
		{
			Owner = owner;
		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			if (Settings.LocationResponse.AreYouAPilot != null)
				return Settings.LocationResponse.AreYouAPilot.Count;
			else return 0;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return Settings.LocationResponse.AreYouAPilot[(int)row].Title;
				default:
					throw new NotImplementedException();
			}
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			var selected = Settings.LocationResponse.AreYouAPilot[(int)picker.SelectedRowInComponent(0)];

			//Assign value to registration object
			Settings.PilotStatusId = selected.PilotStatusId;

			//if owner / pilor or “my flight deaprmartment - pilot (2 or 3)
			if (selected.PilotStatusId == 2 || selected.PilotStatusId == 3)
			{
				Owner.RatingLabelProperty.Alpha = 1f;
				Owner.RatingPickerViewProperty.Alpha = 1f;
			}
			else {
				Owner.RatingLabelProperty.Alpha = 0f;
				Owner.RatingPickerViewProperty.Alpha = 0f;
			}
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				lbl.Font = UIFont.SystemFontOfSize(12f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(22f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			return lbl;
		}
	}
}

public class RatingModel : UIPickerViewModel
{

	public RatingModel()
	{
	}

	public override nint GetComponentCount(UIPickerView v)
	{
		return 1;
	}

	public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
	{
		return Settings.LocationResponse.PilotRating.Count;
	}

	public override string GetTitle(UIPickerView picker, nint row, nint component)
	{
		switch (component)
		{
			case 0:
				return Settings.LocationResponse.PilotRating[(int)row].Title;
			default:
				throw new NotImplementedException();
		}
	}

	public override void Selected(UIPickerView picker, nint row, nint component)
	{
		var selected = Settings.LocationResponse.PilotRating[(int)picker.SelectedRowInComponent(0)];

		//Assign value to registration object
		Settings.PilotTypeId = selected.PilotTypeId;
	}

	public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
	{
		UILabel lbl = new UILabel();

		lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
		if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
		{
			lbl.Font = UIFont.SystemFontOfSize(12f);
		}
		else {

			lbl.Font = UIFont.SystemFontOfSize(22f);
		}

		lbl.TextColor = UIColor.White;

		lbl.TextAlignment = UITextAlignment.Center;
		lbl.Text = Settings.LocationResponse.PilotRating[(int)row].Title;
		return lbl;
	}
}