using Foundation;
using System;
using UIKit;
using System.Linq;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using System.Threading.Tasks;
using AircraftForSale.PCL;
using System.Collections.Generic;
using Google.Analytics;
using System.Drawing;

namespace AircraftForSale
{
	public partial class RegistrationProfileViewController : UIViewController, IMultiStepProcessStep
	{
		//partial void UIButton27947_TouchUpInside(UIButton sender)
		//{
		//	Settings.IsRegistered = true;

		//	//RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;

		//	//var loginVC = registrationVC.PresentingViewController as LoginViewController;
		//	////if loginVS is null, than started registration from the GlobalAir Magazine navigation controller or tabbarcontroller main home view
		//	//if (loginVC == null)
		//	//{
		//	//	//presenting viewcontroller will be tabbarcontroller if did registration from tab nav controller
		//	//	var tabBarController = registrationVC.PresentingViewController as UITabBarController;
		//	//	if (tabBarController != null)
		//	//	{


		//	//		registrationVC.DismissViewController(true, null);

		//	//		var viewController = tabBarController.ViewControllers.FirstOrDefault(row => row is MagazineNavigationViewController);
		//	//		tabBarController.SelectedViewController = viewController;

		//	//		//need to set the register tab bar item badge value to null now that we are registered
		//	//		var registrationTabBarItem = tabBarController.TabBar.Items[2];
		//	//		registrationTabBarItem.BadgeValue = null;


		//	//	}
		//	//	else {
		//	//		//if wasn't registering from login page or tabbarviewcontroller than registered from the Globalair magazinae nav controller
		//	//		registrationVC.NavigationController.PopViewController(true);
		//	//	}
		//	//}
		//	//else {
		//	//	registrationVC.DismissViewController(true, null);
		//	//	loginVC.PerformSegue("LoadTabBarControllerSeque", loginVC);


		//	//}
		//}

		void CloseButton_TouchUpInside(object sender, EventArgs e)
		{
			this.DismissViewController(true, null);
		}

		async void CompleteRegistrationButton_TouchUpInside(object sender, EventArgs e)
		{


			Settings.FirstName = FirstNameTextField.Text;
			Settings.LastName = LastNameTextField.Text;
			Settings.Phone = MobilePhoneTextField.Text;
			Settings.Company = CompanyTextField.Text;




			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				var response = await MagAppRegisterResponse.updateAsync();

				if (response.Status == "Success")
				{

					Settings.IsRegistered = true;

					var alert = UIAlertController.Create("Congratulations!", "You have successfully updated.", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
					{
                       

					}));

                     PresentViewController(alert, animated: true, completionHandler: () =>
					 {

//						UIViewController registrationVC = (this.ParentViewController);

//registrationVC.DismissViewController(true, null);
						 
					 });

                        

				}
				else
				{
					HelperMethods.SendBasicAlert("Oops.", "There was a problem updating. Please try again.");
				}


			}
			else
			{
				HelperMethods.SendBasicAlert("Please connect to the internet", "Internet access is required.");
			}
		}




		public RegistrationProfileViewController(IntPtr handle) : base(handle)
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
				return LocationsPicker;
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
					var finalStep = registrationVC.Steps.FirstOrDefault();
					registrationVC._pageViewController.SetViewControllers(new[] { finalStep as UIViewController }, UIPageViewControllerNavigationDirection.Reverse, true, (finished) =>
					{
						if (finished)
						{
							//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

						}
					});

				}));
				//alert.AddAction(UIAlertAction.Create("Snooze", UIAlertActionStyle.Default, action => Snooze()));
				//if (alert.PopoverPresentationController != null)
				//	alert.PopoverPresentationController.BarButtonItem = myItem;
				PresentViewController(alert, animated: true, completionHandler: () =>
				{

				});
			}

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			PageHeaderLabel.BackgroundColor = UIColor.Gray;

			SizeF size = new SizeF(320, UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 600 : 800);
			ProfileScrollView.ContentSize = size;


			MapCCodeList = new List<Location>();
			LocationList = new List<Location>();
			provinceList = new List<Location>();
			stateList = new List<Location>();
			FilteredLocationList = new List<Location>();
			SelectedMapCCode = new Location();
			SelectedLocation = new Location();


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

			MobilePhoneTextField.AttributedPlaceholder = new NSAttributedString(
				"MOBILE PHONE",
				font: MobilePhoneTextField.Font,
				foregroundColor: UIColor.White
);



			CompanyTextField.AttributedPlaceholder = new NSAttributedString(
				"COMPANY",
				font: CompanyTextField.Font,
				foregroundColor: UIColor.White
);


			FirstNameTextField.Text = Settings.FirstName;
			MobilePhoneTextField.Text = Settings.Phone;
			LastNameTextField.Text = Settings.LastName;
			//Task.Run(async () => {
			//	try
			//	{
			//		AuthResponse response1 = await AuthResponse.GetAuthResponseAsync(Settings.AppID, Settings.Username, Settings.Password);

			//		APIManager manager = new APIManager();

			//		var reponseProfile = await manager.getUserProfile(Settings.AppID, Settings.Username, response1.AuthToken, Settings.Password);


			//		InvokeOnMainThread(() =>
			//				{
							
			//					//CompanyTextField.Text = reponseProfile.Company;
			//				});

			//	}
			//	catch (Exception e) {
					
			//	}

			//});

			//ContainerViewWidthConstraint.Constant = View.Bounds.Width - 40;
			ContainerView.BackgroundColor = UIColor.Clear;
			ContainerView.Alpha = .8f;

			//CGSize contentSize = new CGSize(View.Bounds.Width - 40, ContainerView.Bounds.Height);
			//ProfileScrollView.ContentSize = contentSize;
			//LocationResponseList = new List<LocationResponse>();


			MapCCodePickerView.Model = new MapCCodeModel(this);


			Task.Run(async () =>
			{
				try
				{
					LocationResponse location = new LocationResponse();
					if (Reachability.IsHostReachable(Settings._baseDomain))
					{
						location = await LocationResponse.GetLocations();
						if (location.Equals(null)) return;
						if (location.Status != "Success")
						{
							InvokeOnMainThread(() =>
							{
								var alert = UIAlertController.Create("There was a problem loading location data", "Please try again.", UIAlertControllerStyle.Alert);

								alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
															{
																this.DismissViewController(true, null);

															}));

								PresentViewController(alert, animated: true, completionHandler: () =>
								{

								});
							});
						}
						else
						{

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
										placeholderLocation.DisplayOrder = loc.DisplayOrder + 1;
										filteredLocationListByFirstMapCode.Add(placeholderLocation);
									}
									else
									{
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
							FilteredLocationList = LocationList;
							provinceList = location.ProvinceLst.OrderBy(row => row.DisplayOrder).ToList();
							stateList = location.StatesLst.OrderBy(row => row.DisplayOrder).ToList();

							if (MapCCodeList.Count > 1 && Settings.LocationPickerSelectedId == 0)
							{
								//Assign value to registration objec
								Settings.LocationPickerSelectedId = MapCCodeList[0].LocationId;
							}

							InvokeOnMainThread(() =>
							{
								MapCCodePickerView.Model = new MapCCodeModel(this);
								LocationsPicker.Model = new LocationsModel(this);

								if (Settings.IsRegistered)
								{
									var previouslSelectedLocation = MapCCodeList.FirstOrDefault(row => row.LocationId == Settings.LocationPickerSelectedId);

									if (previouslSelectedLocation != null)
									{
										int currentIndex = MapCCodeList.IndexOf(MapCCodeList.FirstOrDefault(row => row.LocationId == Settings.LocationPickerSelectedId)); ;
										MapCCodePickerView.Select(currentIndex, 0, true);
									}
									else
									{
										if (Settings.LocationPickerSelectedId != 0)
										{
											int locationId = Settings.LocationPickerSelectedId;
											int mapCIndex = MapCCodeList.IndexOf(MapCCodeList.FirstOrDefault(row => row.LocationId == 0));
											MapCCodePickerView.Select(mapCIndex, 0, true);
											MapCCodePickerView.Model.Selected(MapCCodePickerView, mapCIndex, 0);

											int locationCInddex = FilteredLocationList.IndexOf(FilteredLocationList.FirstOrDefault(row => row.LocationId == locationId));
											LocationsPicker.Select(locationCInddex, 0, true);
											LocationsPicker.Model.Selected(LocationsPicker, locationCInddex, 0);
											//LocationPicker.Alpha = 1f;
										}
									}
								}
								else
								{
									//MapCCodePicker.Model.Selected(MapCCodePicker, 0, 0);
									MapCCodePickerView.Select(0, 0, true);
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
				}
				catch (Exception exe) { 
				}
			}
			        );

		}


		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			//like, share, and email broker button event wiring
			finishButton.TouchUpInside += CompleteRegistrationButton_TouchUpInside;
			dismissButton.TouchUpInside += CloseButton_TouchUpInside;
		}
		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });

			finishButton.TouchUpInside -= CompleteRegistrationButton_TouchUpInside;
			dismissButton.TouchUpInside -= CloseButton_TouchUpInside;


		}

		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;

		public List<LocationResponse> OriginalResponseList;
		public List<LocationResponse> LocationResponseList;

		public LocationResponse SelectedLocationResponse
		{
			get;
			set;
		}


	}

	public class LocationsModel : UIPickerViewModel
	{


		WeakReference parent;
		public RegistrationProfileViewController Owner
		{
			get
			{
				return parent.Target as RegistrationProfileViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public LocationsModel(RegistrationProfileViewController owner)
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

		public override nfloat GetComponentWidth(UIPickerView picker, nint component)
		{
			if (component == 0)
				return 220f;
			else
				return 30f;
		}
	}

	public class MapCCodeModel : UIPickerViewModel
	{

		List<Tuple<string, string>> mapCodeNames;

		WeakReference parent;
		public RegistrationProfileViewController Owner
		{
			get
			{
				return parent.Target as RegistrationProfileViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public MapCCodeModel(RegistrationProfileViewController owner)
		{
			mapCodeNames = new List<Tuple<string, string>>();
			mapCodeNames.Add(new Tuple<string, string>("Select", "Select"));
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

		public override void Selected(UIPickerView picker, nint row, nint component)
		{
			//Owner.SelectedMapCCode = mapCodeNames[(int)picker.SelectedRowInComponent(0)];

			//Owner.LocationResponseList = Owner.OriginalResponseList.Where(r => r.Status == Owner.SelectedMapCCode.Item2 || r.Status == "Select a Location").ToList();

			//Owner.LocationsPickerViewProperty.ReloadAllComponents();

			try
			{

				Owner.SelectedMapCCode = Owner.MapCCodeList[(int)picker.SelectedRowInComponent(0)];

				//Owner.SelectedMapCCode = Owner.MapCCodeList[(int)row];

				if (Owner.SelectedMapCCode.MapCCode == "WO")
				{
					Owner.FilteredLocationList = Owner.LocationList.Where(r => r.MapCCode == Owner.SelectedMapCCode.MapCCode || r.LocName == "Select a Location").ToList();
				}
				else if (Owner.SelectedMapCCode.MapCCode == "US")
				{
					Owner.FilteredLocationList = Owner.stateList;//.Where(r => r.MapCCode == Owner.SelectedMapCCode.MapCCode).ToList();

				}
				else if (Owner.SelectedMapCCode.MapCCode == "CA")
				{
					Owner.FilteredLocationList = Owner.provinceList;

				}
				else
				{
					Owner.FilteredLocationList.Clear();
				}

				if (Owner.FilteredLocationList.Count > 2)
				{
					Owner.LocationsPickerViewProperty.Alpha = 1f;
					//Owner.LocationPickerWidthConstraintProperty.Constant = Owner.LocationPickerWidth;
				}
				else
				{
					Owner.LocationsPickerViewProperty.Alpha = 0f;
					//Owner.LocationPickerWidthConstraintProperty.Constant = 0f;
				}

				//Assign value to registration object
				Settings.LocationPickerSelectedId = Owner.SelectedMapCCode.LocationId;

				Owner.LocationsPickerViewProperty.ReloadAllComponents();
			}
			catch (Exception exe) { 
			}

		}

		public override nfloat GetComponentWidth(UIPickerView picker, nint component)
		{
			if (component == 0)
				return 220f;
			else
				return 30f;
		}
	}
}