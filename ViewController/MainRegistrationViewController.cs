using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using System.Linq;
using AircraftForSale.PCL;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Drawing;
using CoreGraphics;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class MainRegistrationViewController : UITableViewController, IMultiStepProcessStep
	{
      partial void OnClick(UIButton sender)
		{

			EmailTextField.ResignFirstResponder();
			PasswordTextField.ResignFirstResponder();
			FirstNameTextField.ResignFirstResponder();
			LastNameTextField.ResignFirstResponder();

			//Ensure required fields are input
			if (Settings.Email == null || Settings.Email == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input a valid email address");
				return;
			}
			if (Settings.Password == null || Settings.Password == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input a valid password");
				return;
			}

			if (Settings.Email != ReEmailTextField.Text)
			{
				HelperMethods.SendBasicAlert("Validation", "Email doesn't match");
				return;
			}

			if (Settings.Password != RePasswordTextField.Text)
			{
				HelperMethods.SendBasicAlert("Validation", "Password doesn't match");
				return;
			}


			if (Settings.FirstName == null || Settings.FirstName == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input your first name");
				return;
			}
			if (Settings.LastName == null || Settings.LastName == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input your last name");
				return;
			}
			if (Settings.Phone == null || Settings.Phone == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input your phone number");
				return;
			}

			if (Settings.LocationPickerSelectedId == 0)
			{
				HelperMethods.SendBasicAlert("Validation", "Please select your location");
				return;
			}

			RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
var stepThree = registrationVC.Steps[2];
registrationVC._pageViewController.SetViewControllers(new[] { stepThree as UIViewController }, UIPageViewControllerNavigationDirection.Forward, true, (finished) =>
			{
				if (finished)
				{
					//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());
				}
			});
			//throw new NotImplementedException();
		}

		void PhoneTextField_EditingDidEnd (object sender, EventArgs e)
		{
			var phoneNumber = PhoneTextField.Text;
			//validate phone numberd
			if (HelperMethods.IsValidPhoneNumber(phoneNumber))
			{
				Settings.Phone = phoneNumber;
			}
			else {
				HelperMethods.SendBasicAlert("Validation", "Please input a valid mobile phone number");
				PhoneTextField.Text = string.Empty;
			}
		}

		void HomeAirportTextField_EditingDidEnd(object sender, EventArgs e)
		{
			var homeAirport = HomeAirportTextField.Text;
			Settings.HomeAirport = homeAirport;
		}

		void LastNameTextField_EditingDidEnd(object sender, EventArgs e)
		{
			var lastName = LastNameTextField.Text;
			Settings.LastName = lastName;
		}

		void FirstNameTextField_EditingDidEnd(object sender, EventArgs e)
		{
			var firstName = FirstNameTextField.Text;
			Settings.FirstName = firstName;
		}

		void CompanyTextField_EditingDidEnd(object sender, EventArgs e)
		{
			var companyName = CompanyTextField.Text;
			Settings.Company = companyName;
		}

		void PasswordTextField_EditingDidEnd(object sender, EventArgs e)
		{
			var password = PasswordTextField.Text;

			//validate password
			if (HelperMethods.IsValidPassword(password))
			{
				Settings.Password = password;
			}
			else {
				HelperMethods.SendBasicAlert("Validation", "Please input a valid email password (one number, one alpha character, and minimum length of 6)");
				PasswordTextField.Text = string.Empty;
			}

		}

		void EmailTextField_EditingDidEnd(object sender, EventArgs e)
		{
			var emailAddress = EmailTextField.Text;

			//validate email address
			if (HelperMethods.IsValidEmail(emailAddress))
			{
				Settings.Email = emailAddress;
			}
			else {
				HelperMethods.SendBasicAlert("Validation", "Please input a valid email address");
				EmailTextField.Text = string.Empty;
			}
		}

		void NextButton_TouchUpInside(object sender, EventArgs e)
		{
			//Dismiss keyboard

		}



		//bool routeToStepOne = false;
		public MainRegistrationViewController(IntPtr handle) : base(handle)
		{
		}

		public List<Location> MapCCodeList
		{
			get;
			set;
		}

		public Location SelectedMapCCode
		{
			get;
			set;
		}

		public List<Location> LocationList
		{
			get;
			set;
		}

		public List<Location> stateList
		{
			get;
			set;
		}

		public List<Location> provinceList
		{
			get;
			set;
		}

		public List<Location> FilteredLocationList
		{
			get;
			set;
		}

		public Location SelectedLocation
		{
			get;
			set;
		}

		public UIPickerView LocationsPickerViewProperty
		{
			get
			{
				return LocationPicker;
			}
		}

		//public NSLayoutConstraint LocationPickerWidthConstraintProperty
		//{
		//	get
		//	{
		//		return LocationPickerWidthConstraint;
		//	}
		//}

		//public NSLayoutConstraint MapCCodePickerWidthConstraintProperty
		//{
		//	get
		//	{
		//		return MapCCodePickerWidthConstraint;
		//	}
		//}

		public float LocationPickerWidth
		{
			get
			{
				var buffer = 90f;
				if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
				{
					buffer = 20f;
				}

				return (float)View.Frame.Width - (buffer + (float)LocationLabel.Frame.Width + (float)MapCCodePicker.Frame.Width);
			}
		}

		////variables to manage moving input elements on keyboard show
		//#region managing keyboard variables
		//private UIView activeview;             // Controller that activated the keyboard
		//private float scroll_amount = 0.0f;    // amount to scroll 
		//private float bottom = 0.0f;           // bottom point
		//private float offset = 10.0f;          // extra offset
		//									   //private bool moveViewUp = false;           // which direction are we moving
		//private bool keyboardIsShowing = false; //if keyboard is already showing, no need to move elements on page up even higher
		//#endregion

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

			MapCCodeList = new List<Location>();
			LocationList = new List<Location>();
			provinceList = new List<Location>();
			stateList = new List<Location>();
			FilteredLocationList = new List<Location>();
			SelectedMapCCode = new Location();
			SelectedLocation = new Location();

			LocationsPickerViewProperty.Alpha = 0f;
			//LocationPickerWidthConstraintProperty.Constant = 0f;
			//if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			//{
			//	MapCCodePickerWidthConstraint.Constant = 80f;
			//}
			//else {
			//	MapCCodePickerWidthConstraint.Constant = 150f;
			//}

			//Load values if already registered
			if (Settings.IsRegistered)
			{
				EmailTextField.Text = Settings.Email;
				PasswordTextField.Text = Settings.Password;
				FirstNameTextField.Text = Settings.FirstName;
				LastNameTextField.Text = Settings.LastName;
				PhoneTextField.Text = Settings.Phone;
				CompanyTextField.Text = Settings.Company;
				HomeAirportTextField.Text = Settings.HomeAirport;
			}

			//hide keyboard when touch anywhere
			HideKeyboardGesture = new UITapGestureRecognizer(() =>
			{
				View.EndEditing(true);

			});

			Task.Run(async () =>
			{
				LocationResponse location = new LocationResponse();
				if (Reachability.IsHostReachable(Settings._baseDomain))
				{
					location = await LocationResponse.GetLocations();
					if (location.Status != "Success")
					{
						InvokeOnMainThread(() =>
						{
							var alert = UIAlertController.Create("There was a problem loading registration data", "Please try again.", UIAlertControllerStyle.Alert);

							alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
							{
								RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
								//var firstStep = registrationVC.Steps.FirstOrDefault();
								//registrationVC._pageViewController.SetViewControllers(new[] { firstStep as UIViewController }, UIPageViewControllerNavigationDirection.Reverse, true, (finished) =>
								//{
								//	if (finished)
								//	{
								//		//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

								//	}
								//});
								registrationVC.DismissViewController(true, null);

							}));

							PresentViewController(alert, animated: true, completionHandler: () =>
							{

							});
						});
					}
					else {

						//Save to settings file to use object during the rest of the registration process
						Settings.LocationResponse = location;

						var distinctMapCodes = location.Locations.Select(row => row.MapCCode).Distinct().ToList();
						List<Location> filteredLocationListByFirstMapCode = new List<Location>();
						foreach (var mapCode in distinctMapCodes)
						{
							Location loc = location.Locations.FirstOrDefault(row => row.MapCCode == mapCode);
						
							if (loc != null)
							{
								if (loc.MapCCode == "WO")
								{
									Location placeholderLocation = new Location();
									placeholderLocation.LocationId = loc.LocationId;
									placeholderLocation.LocName = "World";
									placeholderLocation.Abbreviation = loc.Abbreviation;
									placeholderLocation.MapCCode = loc.MapCCode;
									placeholderLocation.DisplayOrder = loc.DisplayOrder;
									filteredLocationListByFirstMapCode.Add(placeholderLocation);
								}
								else {
									filteredLocationListByFirstMapCode.Add(loc);
								}
							}
						}
						MapCCodeList = filteredLocationListByFirstMapCode.OrderByDescending(row => row.DisplayOrder).ToList();

						Location sleLocation = new Location();
						sleLocation.LocationId = 0;
						sleLocation.LocName = "Select";
						sleLocation.Abbreviation = "Select";
						sleLocation.MapCCode = "SE";
						sleLocation.DisplayOrder = 0;
						MapCCodeList.Insert(0, sleLocation);

						LocationList = location.Locations.OrderBy(row => row.DisplayOrder).ToList();
						provinceList = location.ProvinceLst.OrderBy(row => row.DisplayOrder).ToList();
						stateList = location.StatesLst.OrderBy(row => row.DisplayOrder).ToList();
						FilteredLocationList = LocationList;

						if (MapCCodeList.Count > 1 && Settings.LocationPickerSelectedId == 0)
						{
							//Assign value to registration objec
							Settings.LocationPickerSelectedId = MapCCodeList[0].LocationId;
						}

						InvokeOnMainThread(() =>
						{
							MapCCodePicker.Model = new MapCCodeMod(this);
							LocationPicker.Model = new LocationsMod(this);

							if (Settings.IsRegistered)
							{
								var previouslSelectedLocation = MapCCodeList.FirstOrDefault(row => row.LocationId == Settings.LocationPickerSelectedId);

								if (previouslSelectedLocation != null)
								{
									int currentIndex = MapCCodeList.IndexOf(MapCCodeList.FirstOrDefault(row => row.LocationId == Settings.LocationPickerSelectedId));;
									MapCCodePicker.Select(currentIndex, 0, true);
								}
								else {
									if (Settings.LocationPickerSelectedId != 0)
									{
										int locationId = Settings.LocationPickerSelectedId;
										int mapCIndex = MapCCodeList.IndexOf(MapCCodeList.FirstOrDefault(row => row.LocationId == 0));
										MapCCodePicker.Select(mapCIndex, 0, true);
										MapCCodePicker.Model.Selected(MapCCodePicker, mapCIndex, 0);

										int locationCInddex = FilteredLocationList.IndexOf(FilteredLocationList.FirstOrDefault(row => row.LocationId == locationId));
										LocationPicker.Select(locationCInddex, 0, true);
										LocationPicker.Model.Selected(LocationPicker, locationCInddex, 0);
										//LocationPicker.Alpha = 1f;
									}
								}
							}
							else {
								//MapCCodePicker.Model.Selected(MapCCodePicker, 0, 0);
								MapCCodePicker.Select(0, 0, true);
							}
						});
					}



				}
				else
				{
					InvokeOnMainThread(() =>
					{
						var alert = UIAlertController.Create("Please connect to the internet", "Internet access is required.", UIAlertControllerStyle.Alert);

						alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
						{
							RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
							//var firstStep = registrationVC.Steps.FirstOrDefault();
							//registrationVC._pageViewController.SetViewControllers(new[] { firstStep as UIViewController }, UIPageViewControllerNavigationDirection.Reverse, true, (finished) =>
							//{
							//	if (finished)
							//	{
							//		//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

							//	}
							//});
							registrationVC.DismissViewController(true, null);

						}));

						PresentViewController(alert, animated: true, completionHandler: () =>
							{

							});
					});
				}
			});
		}

		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);



			StepActivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });


			if (!Settings.AnyClassificationChosen())
			{
				var alert = UIAlertController.Create("Please Choose a Classification", "A minimum of one classification is required.", UIAlertControllerStyle.Alert);

				alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
				{
					RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
					var firstStep = registrationVC.Steps.FirstOrDefault();
					registrationVC._pageViewController.SetViewControllers(new[] { firstStep as UIViewController }, UIPageViewControllerNavigationDirection.Reverse, true, (finished) =>
					{
						if (finished)
						{
							//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

						}
					});

				}));

				PresentViewController(alert, animated: true, completionHandler: () =>
				{

				});
			}

		//(UIKeyboard.WillHideNotification, KeyBoardDownNotification);



			//Register for events
			View.AddGestureRecognizer(HideKeyboardGesture);
			NextButton.TouchUpInside += NextButton_TouchUpInside;
			EmailTextField.EditingDidEnd += EmailTextField_EditingDidEnd;
			PasswordTextField.EditingDidEnd += PasswordTextField_EditingDidEnd;
			FirstNameTextField.EditingDidEnd += FirstNameTextField_EditingDidEnd;
			LastNameTextField.EditingDidEnd += LastNameTextField_EditingDidEnd;
			HomeAirportTextField.EditingDidEnd += HomeAirportTextField_EditingDidEnd;
			CompanyTextField.EditingDidEnd += CompanyTextField_EditingDidEnd;
			PhoneTextField.EditingDidEnd += PhoneTextField_EditingDidEnd;

EmailTextField.AttributedPlaceholder = new NSAttributedString(
				"EMAIL (Username)",
				font: EmailTextField.Font,
				foregroundColor: UIColor.White
);
PasswordTextField.AttributedPlaceholder = new NSAttributedString(
				"PASSWORD",
				font: PasswordTextField.Font,
				foregroundColor: UIColor.White
);

			ReEmailTextField.AttributedPlaceholder = new NSAttributedString(
				"RE-ENTER EMAIL",
				font: ReEmailTextField.Font,
				foregroundColor: UIColor.White
);
RePasswordTextField.AttributedPlaceholder = new NSAttributedString(
				"RE-ENTER PASSWORD",
				font: RePasswordTextField.Font,
				foregroundColor: UIColor.White
);

FirstNameTextField.AttributedPlaceholder = new NSAttributedString(
				"FIRST NAME",
				font: FirstNameTextField.Font,
				foregroundColor: UIColor.White
);
LastNameTextField.AttributedPlaceholder = new NSAttributedString(
				"LAST NAME",
				font: LastNameTextField.Font,
				foregroundColor: UIColor.White
);
HomeAirportTextField.AttributedPlaceholder = new NSAttributedString(
				"HOME AIRPORT (Optional)",
				font: HomeAirportTextField.Font,
				foregroundColor: UIColor.White
);
CompanyTextField.AttributedPlaceholder = new NSAttributedString(
				"COMPANY (Optional)",
				font: CompanyTextField.Font,
				foregroundColor: UIColor.White
);
PhoneTextField.AttributedPlaceholder = new NSAttributedString(
				"MOBILE PHONE",
				font: PhoneTextField.Font,
				foregroundColor: UIColor.White
);
		
		}



		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });

			//Unregister events
			NextButton.TouchUpInside -= NextButton_TouchUpInside;
			EmailTextField.EditingDidEnd -= EmailTextField_EditingDidEnd;
			PasswordTextField.EditingDidEnd -= PasswordTextField_EditingDidEnd;
			FirstNameTextField.EditingDidEnd -= FirstNameTextField_EditingDidEnd;
			LastNameTextField.EditingDidEnd -= LastNameTextField_EditingDidEnd;
			HomeAirportTextField.EditingDidEnd -= HomeAirportTextField_EditingDidEnd;
			CompanyTextField.EditingDidEnd -= CompanyTextField_EditingDidEnd;
			PhoneTextField.EditingDidEnd -= PhoneTextField_EditingDidEnd;
			View.RemoveGestureRecognizer(HideKeyboardGesture);


		}

       public override UIView GetViewForHeader(UITableView tableView, nint section)
{
	UILabel lblHeader = new UILabel(new CGRect(1, 1, tableView.SectionHeaderHeight, tableView.Frame.Width));
			if (section == 0) lblHeader.Text = "CREATE LOGIN";
			else lblHeader.Text = "BASIC INFORMATION";
			lblHeader.TextColor = UIColor.Green;
	return lblHeader;
}
	}

	public class MapCCodeMod : UIPickerViewModel
	{

		List<Tuple<string, string>> mapCodeNames;

		WeakReference parent;
		public MainRegistrationViewController Owner
		{
			get
			{
				return parent.Target as MainRegistrationViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public MapCCodeMod(MainRegistrationViewController owner)
		{
			mapCodeNames = new List<Tuple<string, string>>();
			mapCodeNames.Add(new Tuple<string, string>("Select", "SE"));
			mapCodeNames.Add(new Tuple<string, string>("USA", "US"));
			mapCodeNames.Add(new Tuple<string, string>("Canada", "CA"));
			mapCodeNames.Add(new Tuple<string, string>("World", "WO"));
			Owner = owner;

		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Owner.MapCCodeList.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return Owner.MapCCodeList[(int)row].LocName;

				default:
					throw new NotImplementedException();
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
			lbl.Text = Owner.MapCCodeList[(int)row].LocName;
			return lbl;
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			Owner.SelectedMapCCode = Owner.MapCCodeList[(int)picker.SelectedRowInComponent(0)];

			//Owner.SelectedMapCCode = Owner.MapCCodeList[(int)row];

			if (Owner.SelectedMapCCode.MapCCode == "WO")
			{
				Owner.FilteredLocationList = Owner.LocationList;//.Where(r => r.MapCCode == Owner.SelectedMapCCode.MapCCode || r.LocName == "Select a Location").ToList();
			}
			else if (Owner.SelectedMapCCode.MapCCode == "US")
			{
				Owner.FilteredLocationList = Owner.stateList;//.Where(r => r.MapCCode == Owner.SelectedMapCCode.MapCCode).ToList();

			}
			else if (Owner.SelectedMapCCode.MapCCode == "CA")
			{
				Owner.FilteredLocationList = Owner.provinceList;

			}
			else { 
				Owner.FilteredLocationList.Clear();
			}
			if (Owner.FilteredLocationList.Count > 2)
			{
				Owner.LocationsPickerViewProperty.Alpha = 1f;
				//Owner.LocationPickerWidthConstraintProperty.Constant = Owner.LocationPickerWidth;
			}
			else {
				Owner.LocationsPickerViewProperty.Alpha = 0f;
				//Owner.LocationPickerWidthConstraintProperty.Constant = 0f;
			}

			//Assign value to registration object
			Settings.LocationPickerSelectedId = Owner.SelectedMapCCode.LocationId;

			Owner.LocationsPickerViewProperty.ReloadAllComponents();
		}

	}

	public class LocationsMod : UIPickerViewModel
	{


		WeakReference parent;
		public MainRegistrationViewController Owner
		{
			get
			{
				return parent.Target as MainRegistrationViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public LocationsMod(MainRegistrationViewController owner)
		{
			Owner = owner;
		}

		public override nint GetComponentCount(UIPickerView v)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView pickerView, nint component)
		{
			return Owner.FilteredLocationList.Count;
		}

		public override string GetTitle(UIPickerView picker, nint row, nint component)
		{
			switch (component)
			{
				case 0:
					return Owner.FilteredLocationList[(int)row].LocName;
				default:
					throw new NotImplementedException();
			}
		}

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			Owner.SelectedLocation = Owner.FilteredLocationList[(int)picker.SelectedRowInComponent(0)];

			//Assign value to registration objec
			Settings.LocationPickerSelectedId = Owner.SelectedLocation.LocationId;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone) 			{
				lbl.Font = UIFont.SystemFontOfSize(12f);
			}
			else {

				lbl.Font = UIFont.SystemFontOfSize(22f);
			}

			lbl.TextColor = UIColor.White;

			lbl.TextAlignment = UITextAlignment.Center;
			lbl.Text = Owner.FilteredLocationList[(int)row].LocName;
			return lbl;
		}

	}



}