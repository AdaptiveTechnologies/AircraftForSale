using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Drawing;
using AircraftForSale.PCL.Helpers;
using System.Threading.Tasks;
using AircraftForSale.PCL;
using System.Linq;

namespace AircraftForSale
{
	public partial class RegistrationStepTwo : UITableViewController
	{
		public RegistrationStepTwo(IntPtr handle) : base(handle)
		{
		}

		bool hidePilotRows = true;
		bool hideManufacturerAndModelRow = true;
		public override void ViewDidLoad()
		{
			//this.NavigationController.NavigationBar
			NavigationItem.Title = "Registration";

			var classificationPickerViewModel = new ClassificationPickerViewModel();
			ClassificationPicker.Model = classificationPickerViewModel;

			classificationPickerViewModel.ValueChanged += (sender, e) =>
			{
				if (classificationPickerViewModel.selectedIndex == 0)
				{
					hideManufacturerAndModelRow = true;
				}
				else
				{
					hideManufacturerAndModelRow = false;
				}

				TableView.BeginUpdates();
				TableView.EndUpdates();
			};

			var manufacturerPickerViewModel = new ManufacturerPickerViewModel();
			ManufacturerPicker.Model = manufacturerPickerViewModel;

			//manufacturerPickerViewModel.ValueCha


			ModelPicker.Model = new ModelPickerViewModel();

			PilotSwitch.On = false;

			PilotSwitch.ValueChanged += (sender, e) =>
			{
				if (PilotSwitch.On)
				{
					hidePilotRows = false;
				}
				else
				{
					hidePilotRows = true;
				}
				TableView.BeginUpdates();
				TableView.EndUpdates();
			};

			//ClassificationPicker.chang



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

															DismissViewController(true, null);

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

						//						var distinctMapCodes = location.Locations.Select(row => row.MapCCode).Distinct().ToList();
						//						List<Location> filteredLocationListByFirstMapCode = new List<Location>();
						//						foreach (var mapCode in distinctMapCodes)
						//						{
						//							Location loc = location.Locations.FirstOrDefault(row => row.MapCCode == mapCode);
						//
						//							if (loc != null)
						//							{
						//								if (loc.MapCCode == "WO")
						//								{
						//									Location placeholderLocation = new Location();
						//									placeholderLocation.LocationId = loc.LocationId;
						//									placeholderLocation.LocName = "World";
						//									placeholderLocation.Abbreviation = loc.Abbreviation;
						//									placeholderLocation.MapCCode = loc.MapCCode;
						//									placeholderLocation.DisplayOrder = loc.DisplayOrder;
						//									filteredLocationListByFirstMapCode.Add(placeholderLocation);
						//								}
						//								else
						//								{
						//									filteredLocationListByFirstMapCode.Add(loc);
						//								}
						//							}
						//						}
						//						MapCCodeList = filteredLocationListByFirstMapCode.OrderByDescending(row => row.DisplayOrder).ToList();

						//						Location sleLocation = new Location();
						//						sleLocation.LocationId = 0;
						//						sleLocation.LocName = "Select";
						//						sleLocation.Abbreviation = "Select";
						//						sleLocation.MapCCode = "SE";
						//						sleLocation.DisplayOrder = 0;
						//						MapCCodeList.Insert(0, sleLocation);

						//						LocationList = location.Locations.OrderBy(row => row.DisplayOrder).ToList();
						//						provinceList = location.ProvinceLst.OrderBy(row => row.DisplayOrder).ToList();
						//						stateList = location.StatesLst.OrderBy(row => row.DisplayOrder).ToList();
						//						FilteredLocationList = LocationList;

						//						if (MapCCodeList.Count > 1 && Settings.LocationPickerSelectedId == 0)
						//						{
						//							//Assign value to registration objec
						//							Settings.LocationPickerSelectedId = MapCCodeList[0].LocationId;
						//						}

						InvokeOnMainThread(() =>
						{
							//	MapCCodePicker.Model = new MapCCodeMod(this);
							//	LocationPicker.Model = new LocationsMod(this);

							//	if (Settings.IsRegistered)
							//	{
							//		var previouslSelectedLocation = MapCCodeList.FirstOrDefault(row => row.LocationId == Settings.LocationPickerSelectedId);

							//		if (previouslSelectedLocation != null)
							//		{
							//			int currentIndex = MapCCodeList.IndexOf(MapCCodeList.FirstOrDefault(row => row.LocationId == Settings.LocationPickerSelectedId)); ;
							//			MapCCodePicker.Select(currentIndex, 0, true);
							//		}
							//		else
							//		{
							//			if (Settings.LocationPickerSelectedId != 0)
							//			{
							//				int locationId = Settings.LocationPickerSelectedId;
							//				int mapCIndex = MapCCodeList.IndexOf(MapCCodeList.FirstOrDefault(row => row.LocationId == 0));
							//				MapCCodePicker.Select(mapCIndex, 0, true);
							//				MapCCodePicker.Model.Selected(MapCCodePicker, mapCIndex, 0);

							//				int locationCInddex = FilteredLocationList.IndexOf(FilteredLocationList.FirstOrDefault(row => row.LocationId == locationId));
							//				LocationPicker.Select(locationCInddex, 0, true);
							//				LocationPicker.Model.Selected(LocationPicker, locationCInddex, 0);
							//				//LocationPicker.Alpha = 1f;
							//			}
							//		}
							//	}
							//	else
							//	{
							//		//MapCCodePicker.Model.Selected(MapCCodePicker, 0, 0);
							//		MapCCodePicker.Select(0, 0, true);
							//	}

							PilotTypePicker.Model = new PilotTypePickerViewModel();
							RatingPicker.Model = new RatingPickerViewModel();
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

													DismissViewController(true, null);

												}));

						PresentViewController(alert, animated: true, completionHandler: () =>
							{

							});
					});
				}
			});

		}

		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{


			UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)this.View.Frame.Width, 40));
			view.BackgroundColor = UIColor.White;

			UILabel label = new UILabel();
			label.Opaque = false;
			label.TextColor = HelperMethods.GetLime();
			label.Font = UIFont.BoldSystemFontOfSize(25);

			label.Frame = new System.Drawing.RectangleF(0, 0, (float)view.Frame.Width - 30, 30);
			label.Center = view.Center;

			view.AddSubview(label);

			switch (section)
			{
				case 0:
					{
						label.Text = "Create Login";
						break;
					}
				case 1:
					{
						label.Text = "Basic Information";
						break;
					}
				case 2:
					{
						label.Text = "Aircraft Interest (optional)";
						break;
					}
			}
			return view;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			if (hidePilotRows)
			{
				if (indexPath.Section == 1 && (indexPath.Row == 7 || indexPath.Row == 8))
				{
					return 0;
				}
			}
			if (hideManufacturerAndModelRow)
			{
				if (indexPath.Section == 2 && (indexPath.Row == 2 || indexPath.Row == 3))
				{
					return 0;
				}
			}
			return 50;
		}

	}

	public class PilotTypePickerViewModel : UIPickerViewModel
	{
		//List<string> pilotTypeList;

		public PilotTypePickerViewModel()
		{
			//pilotTypeList = new List<string>();
			//pilotTypeList.Add("Select a Type");
		}
		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			return Settings.LocationResponse.AreYouAPilot.Count;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Center;

			//lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			return lbl;
		}
	}

	public class RatingPickerViewModel : UIPickerViewModel
	{
		//List<string> ratingList;

		public RatingPickerViewModel()
		{
			//ratingList = new List<string>();
			//ratingList.Add("Select a Rating");
		}
		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			return Settings.LocationResponse.PilotRating.Count;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Center;

			//lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			lbl.Text = Settings.LocationResponse.PilotRating[(int)row].Title;
			return lbl;
		}
	}

	public class ClassificationPickerViewModel : UIPickerViewModel
	{
		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// The current selected item
		/// </summary>
		public ClassLst SelectedItem
		{
			get { return Settings.ClassificationList[selectedIndex]; }
		}

		public int selectedIndex = 0;

		public ClassificationPickerViewModel()
		{

		}
		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
			 selectedIndex = (int)row;
			 if (ValueChanged != null)
			 {
			 	ValueChanged(this, new EventArgs());
			 }
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			return Settings.ClassificationList.Count();
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Center;

			//lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			lbl.Text = Settings.ClassificationList[(int)row].ClassificationName;
			return lbl;
		}
	}

	public class ManufacturerPickerViewModel : UIPickerViewModel
	{
		List<string> manufacturerList;

		public ManufacturerPickerViewModel()
		{
			manufacturerList = new List<string>();
			manufacturerList.Add("Select a Manufacturer");
		}
		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			return manufacturerList.Count;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Center;

			//lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			lbl.Text = manufacturerList[(int)row];
			return lbl;
		}
	}

	public class ModelPickerViewModel : UIPickerViewModel
	{
		List<string> modelList;

		public ModelPickerViewModel()
		{
			modelList = new List<string>();
			modelList.Add("Select a Model");
		}
		public override nint GetComponentCount(UIPickerView picker)
		{
			return 1;
		}

		public override nint GetRowsInComponent(UIPickerView picker, nint component)
		{
			return modelList.Count;
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Center;

			//lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
			lbl.Text = modelList[(int)row];
			return lbl;
		}
	}
}