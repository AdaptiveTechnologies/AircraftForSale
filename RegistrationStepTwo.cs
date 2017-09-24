using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.Drawing;
using AircraftForSale.PCL.Helpers;
using System.Threading.Tasks;
using AircraftForSale.PCL;
using System.Linq;
using CoreAnimation;
using CoreGraphics;

namespace AircraftForSale
{
    public partial class RegistrationStepTwo : UITableViewController
    {
        public RegistrationStepTwo(IntPtr handle) : base(handle)
        {
        }

        bool hidePilotRows = true;
        bool hideManufacturerAndModelRow = true;
        bool hideModelRow = true;
        ManufacturerPickerViewModel manufacturerPickerViewModel;
        ModelPickerViewModel modelPickerViewModel;

        public override void ViewDidAppear(bool animated)
        {
            if (Settings.LocationPickerSelectedId == 0)
            {
                LocationLabel.Text = "Touch to Add Location";
            }
            else
            {
                LocationLabel.Text = "Location: " + Settings.LocationPickerSelectedName;
            }
        }
        public async override void ViewDidLoad()
        {
            NavigationItem.Title = "Registration";

            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);

            var nextBarButtonItem = new UIBarButtonItem("Next", UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                PerformSegue("StepThreeSeque", this);
            });
            UITextAttributes icoFontAttribute = new UITextAttributes();
            icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(20);
            icoFontAttribute.TextColor = UIColor.White;

            nextBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);

            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);
            NavigationItem.BackBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);
            NavigationItem.BackBarButtonItem.TintColor = UIColor.White;

            this.NavigationItem.SetRightBarButtonItem(nextBarButtonItem, true);

            await Task.Run(async () =>
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
                        InvokeOnMainThread(() =>
                        {
                            //Save to settings file to use object during the rest of the registration process
                            Settings.LocationResponse = location;




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


            var classificationPicker = new UIPickerView();
            var classificationPickerViewModel = new ClassificationPickerViewModel();
            classificationPicker.Model = classificationPickerViewModel;
            classificationPicker.ShowSelectionIndicator = true;

            UIToolbar classificationToolbar = new UIToolbar();
            classificationToolbar.BarStyle = UIBarStyle.Black;
            classificationToolbar.Translucent = true;
            classificationToolbar.SizeToFit();

            UIBarButtonItem classificationDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
            {
                UITextField textview = ClassificationTextField;
                textview.Text = classificationPickerViewModel.SelectedItem.ClassificationName;
                textview.ResignFirstResponder();
            });
            classificationToolbar.SetItems(new UIBarButtonItem[] { classificationDoneButton }, true);


            ClassificationTextField.InputView = classificationPicker;
            ClassificationTextField.InputAccessoryView = classificationToolbar;



            classificationPickerViewModel.ValueChanged += async (sender, e) =>
            {
                if (classificationPickerViewModel.selectedIndex == 0)
                {
                    hideManufacturerAndModelRow = true;
                    hideModelRow = true;
                }
                else
                {
                    hideManufacturerAndModelRow = false;
                    hideModelRow = true;

                    var cmmdResponse = await CMMDResponse.GetCMMDResponseAsync(classificationPickerViewModel.SelectedItem.ClassificationId, 0);
                    if (cmmdResponse.Status == "Success")
                    {
                        InvokeOnMainThread(() =>
                        {

                            var manufacturerPicker = new UIPickerView();
                            manufacturerPickerViewModel = new ManufacturerPickerViewModel(cmmdResponse.MfgLst);
                            manufacturerPicker.Model = manufacturerPickerViewModel;
                            manufacturerPicker.ShowSelectionIndicator = true;

                            UIToolbar manufacturerToolbar = new UIToolbar();
                            manufacturerToolbar.BarStyle = UIBarStyle.Black;
                            manufacturerToolbar.Translucent = true;
                            manufacturerToolbar.SizeToFit();

                            UIBarButtonItem manufacturerDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s2, e2) =>
                            {
                                UITextField textview = ManufacturerTextField;
                                //textview.Text = Settings.LocationResponse.PilotRating.FirstOrDefault(row => row.PilotTypeId == Settings.PilotTypeId).Title;
                                textview.Text = manufacturerPickerViewModel.SelectedItem.Manufacturer;
                                textview.ResignFirstResponder();
                            });
                            manufacturerToolbar.SetItems(new UIBarButtonItem[] { manufacturerDoneButton }, true);


                            ManufacturerTextField.InputView = manufacturerPicker;
                            ManufacturerTextField.InputAccessoryView = manufacturerToolbar;

                            manufacturerPickerViewModel.ValueChanged += async (sender2, e2) =>
                            {
                                if (manufacturerPickerViewModel.selectedIndex == 0)
                                {
                                    hideModelRow = true;
                                }
                                else
                                {
                                    hideModelRow = false;

                                    cmmdResponse = await CMMDResponse.GetCMMDResponseAsync(classificationPickerViewModel.SelectedItem.ClassificationId, manufacturerPickerViewModel.SelectedItem.ManufacturerId);
                                    if (cmmdResponse.Status == "Success")
                                    {
                                        InvokeOnMainThread(() =>
                                        {
                                            var modelList = cmmdResponse.ModDesLst.Where(row => row.ManufacturerId == manufacturerPickerViewModel.SelectedItem.ManufacturerId);
                                            //modelPickerViewModel = new ModelPickerViewModel(modelList.ToList());
                                            //ModelPicker.Model = modelPickerViewModel;

                                            var modelPicker = new UIPickerView();
                                            modelPickerViewModel = new ModelPickerViewModel(modelList.ToList());
                                            modelPicker.Model = modelPickerViewModel;
                                            modelPicker.ShowSelectionIndicator = true;

                                            UIToolbar modelToolbar = new UIToolbar();
                                            modelToolbar.BarStyle = UIBarStyle.Black;
                                            modelToolbar.Translucent = true;
                                            modelToolbar.SizeToFit();

                                            UIBarButtonItem modelDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s3, e3) =>
                                            {
                                                UITextField textview = ModelTextField;
                                                //textview.Text = Settings.LocationResponse.PilotRating.FirstOrDefault(row => row.PilotTypeId == Settings.PilotTypeId).Title;
                                                textview.Text = modelPickerViewModel.SelectedItem.Designation;
                                                textview.ResignFirstResponder();
                                            });
                                            modelToolbar.SetItems(new UIBarButtonItem[] { modelDoneButton }, true);


                                            ModelTextField.InputView = modelPicker;
                                            ModelTextField.InputAccessoryView = modelToolbar;
                                        });
                                    }

                                }

                                TableView.BeginUpdates();
                                TableView.EndUpdates();
                            };

                        });
                    }
                }

                TableView.BeginUpdates();
                TableView.EndUpdates();
            };

            var pilotTypePicker = new UIPickerView();
            pilotTypePicker.Model = new PilotTypePickerViewModel();
            pilotTypePicker.ShowSelectionIndicator = true;

            UIToolbar pilotTypeToolbar = new UIToolbar();
            pilotTypeToolbar.BarStyle = UIBarStyle.Black;
            pilotTypeToolbar.Translucent = true;
            pilotTypeToolbar.SizeToFit();

            UIBarButtonItem pilotTypeDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
            {
                UITextField textview = PilotTypeTextField;
                //textview.Text = Settings.LocationResponse.PilotRating.FirstOrDefault(row => row.PilotTypeId == Settings.PilotTypeId).Title;
                textview.Text = Settings.PilotStatusString;
                textview.ResignFirstResponder();
            });
            pilotTypeToolbar.SetItems(new UIBarButtonItem[] { pilotTypeDoneButton }, true);


            PilotTypeTextField.InputView = pilotTypePicker;
            PilotTypeTextField.InputAccessoryView = pilotTypeToolbar;

            var ratingPicker = new UIPickerView();
            ratingPicker.Model = new RatingPickerViewModel();
            ratingPicker.ShowSelectionIndicator = true;

            UIToolbar pilotRatingToolbar = new UIToolbar();
            pilotRatingToolbar.BarStyle = UIBarStyle.Black;
            pilotRatingToolbar.Translucent = true;
            pilotRatingToolbar.SizeToFit();

            UIBarButtonItem pilotRatingDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
            {
                UITextField textview = PilotRatingTextField;
                textview.Text = Settings.LocationResponse.PilotRating.FirstOrDefault(row => row.PilotTypeId == Settings.PilotTypeId).Title;
                textview.ResignFirstResponder();
            });
            pilotRatingToolbar.SetItems(new UIBarButtonItem[] { pilotRatingDoneButton }, true);


            PilotRatingTextField.InputView = ratingPicker;
            PilotRatingTextField.InputAccessoryView = pilotRatingToolbar;



            PilotSwitch.On = false;

            PilotSwitch.ValueChanged += (sender, e) =>
            {
                if (PilotSwitch.On)
                {
                    hidePilotRows = false;
                    Settings.IsPilot = true;
                }
                else
                {
                    hidePilotRows = true;
                    Settings.IsPilot = false;
                }
                TableView.BeginUpdates();
                TableView.EndUpdates();
            };

            // textFieldEmail is your UITextField

            var borderFrameHeight = UsernameTextView.Frame.Size.Height - 1;
            var borderFrameWidth = UsernameTextView.Frame.Size.Width;
            var borderBackgroundColor = UIColor.Gray.CGColor;

            // create CALayer
            var bottomBorder1 = new CALayer();
            bottomBorder1.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder1.BackgroundColor = borderBackgroundColor;

            var bottomBorder2 = new CALayer();
            bottomBorder2.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder2.BackgroundColor = borderBackgroundColor;

            var bottomBorder3 = new CALayer();
            bottomBorder3.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder3.BackgroundColor = borderBackgroundColor;

            var bottomBorder4 = new CALayer();
            bottomBorder4.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder4.BackgroundColor = borderBackgroundColor;

            var bottomBorder5 = new CALayer();
            bottomBorder5.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder5.BackgroundColor = borderBackgroundColor;

            var bottomBorder6 = new CALayer();
            bottomBorder6.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder6.BackgroundColor = borderBackgroundColor;

            var bottomBorder7 = new CALayer();
            bottomBorder7.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder7.BackgroundColor = borderBackgroundColor;

            var bottomBorder8 = new CALayer();
            bottomBorder8.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder8.BackgroundColor = borderBackgroundColor;

            var bottomBorder9 = new CALayer();
            bottomBorder9.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder9.BackgroundColor = borderBackgroundColor;

            var bottomBorder10 = new CALayer();
            bottomBorder10.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder10.BackgroundColor = borderBackgroundColor;

            var bottomBorder11 = new CALayer();
            bottomBorder11.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder11.BackgroundColor = borderBackgroundColor;

            var bottomBorder12 = new CALayer();
            bottomBorder12.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder12.BackgroundColor = borderBackgroundColor;

            var bottomBorder13 = new CALayer();
            bottomBorder13.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder13.BackgroundColor = borderBackgroundColor;

            var bottomBorder14 = new CALayer();
            bottomBorder14.Frame = new CGRect(0.0f, borderFrameHeight, borderFrameWidth, 1.0f);
            bottomBorder14.BackgroundColor = borderBackgroundColor;

            var bottomBorderLabel = new CALayer();
            bottomBorderLabel.Frame = new CGRect(0.0f, LocationLabel.Frame.Height - 1, LocationLabel.Frame.Width, 1.0f);
            bottomBorderLabel.BackgroundColor = borderBackgroundColor;


            var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 22.0f : 16.0f;
            var fontObject = UIFont.SystemFontOfSize(fontSize);

            PilotTypeTextField.Layer.AddSublayer(bottomBorder10);
            PilotTypeTextField.Layer.MasksToBounds = true;
            PilotTypeTextField.AttributedPlaceholder = new NSAttributedString(
                "Select a Type",
                font: fontObject,
                foregroundColor: UIColor.DarkGray
            //strokeWidth: 4
            );
            PilotTypeTextField.Font = fontObject;

            PilotRatingTextField.Layer.AddSublayer(bottomBorder11);
            PilotRatingTextField.Layer.MasksToBounds = true;
            PilotRatingTextField.AttributedPlaceholder = new NSAttributedString(
                "Select a Rating",
                font: fontObject,
                foregroundColor: UIColor.DarkGray
            //strokeWidth: 4
            );
            PilotRatingTextField.Font = fontObject;

            ClassificationTextField.Layer.AddSublayer(bottomBorder12);
            ClassificationTextField.Layer.MasksToBounds = true;
            ClassificationTextField.AttributedPlaceholder = new NSAttributedString(
                "Select a Classification",
                font: fontObject,
                foregroundColor: UIColor.DarkGray
            //strokeWidth: 4
            );
            ClassificationTextField.Font = fontObject;

            ManufacturerTextField.Layer.AddSublayer(bottomBorder13);
            ManufacturerTextField.Layer.MasksToBounds = true;
            ManufacturerTextField.AttributedPlaceholder = new NSAttributedString(
                "Select a Manufacturer",
                font: fontObject,
                foregroundColor: UIColor.DarkGray
            //strokeWidth: 4
            );
            ManufacturerTextField.Font = fontObject;

            ModelTextField.Layer.AddSublayer(bottomBorder14);
            ModelTextField.Layer.MasksToBounds = true;
            ModelTextField.AttributedPlaceholder = new NSAttributedString(
                "Select a Model",
                font: fontObject,
                foregroundColor: UIColor.DarkGray
            //strokeWidth: 4
            );
            ModelTextField.Font = fontObject;

            UsernameTextView.Layer.AddSublayer(bottomBorder1);
            UsernameTextView.Layer.MasksToBounds = true;
            UsernameTextView.AttributedPlaceholder = new NSAttributedString(
                "Username (Your Email Address)",
                font: fontObject,
                foregroundColor: UIColor.DarkGray
            );
            UsernameTextView.Font = fontObject;

            UsernameTextView.KeyboardType = UIKeyboardType.EmailAddress;
            UsernameTextView.EditingDidEnd += (sender, e) => {
                var emailAddress = UsernameTextView.Text;
				//validate email address
				if (!HelperMethods.IsValidEmail(emailAddress, UsernameTextView))
				{
					HelperMethods.SendBasicAlert("Validation", "Please input a valid email address");
				}
            };


            ReEnterUsernameTextView.Layer.AddSublayer(bottomBorder2);
            ReEnterUsernameTextView.Layer.MasksToBounds = true;
            ReEnterUsernameTextView.AttributedPlaceholder = new NSAttributedString(
              "Re-Enter (Your Email Address)",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          );
            ReEnterUsernameTextView.Font = fontObject;
            ReEnterUsernameTextView.EditingDidEnd += (sender, e) => {
                if(!HelperMethods.ReEnterEmail(ReEnterUsernameTextView.Text, ReEnterUsernameTextView))
                {
                    HelperMethods.SendBasicAlert("Validation", "Usernames must match");
                }
            };

            PasswordTextView.Layer.AddSublayer(bottomBorder3);
            PasswordTextView.Layer.MasksToBounds = true;
            PasswordTextView.AttributedPlaceholder = new NSAttributedString(
              "Password",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          );
            PasswordTextView.Font = fontObject;
            PasswordTextView.EditingDidEnd += (sender, e) => {
                if(!HelperMethods.IsValidPassword(PasswordTextView.Text, PasswordTextView))
                {
                    HelperMethods.SendBasicAlert("Validation", "Passwords should contain letters and numbers and be less than 15 characters.");
                }
            };

            ReEnterPasswordTextView.Layer.AddSublayer(bottomBorder4);
            ReEnterPasswordTextView.Layer.MasksToBounds = true;
            ReEnterPasswordTextView.AttributedPlaceholder = new NSAttributedString(
              "Re-Enter Password",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          );
            ReEnterPasswordTextView.Font = fontObject;
			ReEnterPasswordTextView.EditingDidEnd += (sender, e) => {
				if (!HelperMethods.ReEnterPassword(ReEnterPasswordTextView.Text, ReEnterPasswordTextView))
				{
					HelperMethods.SendBasicAlert("Validation", "Passwords must match");
				}
			};
            FirstNameTextView.Layer.AddSublayer(bottomBorder5);
            FirstNameTextView.Layer.MasksToBounds = true;
            FirstNameTextView.AttributedPlaceholder = new NSAttributedString(
             "First Name",
               font: fontObject,
             foregroundColor: UIColor.DarkGray
         );
            FirstNameTextView.Font = fontObject;
			FirstNameTextView.EditingDidEnd += (sender, e) => {
				if (FirstNameTextView.Text == string.Empty)
				{
                    HelperMethods.AnimateValidationBorder(FirstNameTextView);
					HelperMethods.SendBasicAlert("Validation", "First name is required");
                    Settings.FirstName = string.Empty;
                }else{
                    HelperMethods.RemoveValidationBorder(FirstNameTextView);
                    Settings.FirstName = FirstNameTextView.Text;
                }
			};

            LastNameTextView.Layer.AddSublayer(bottomBorder6);
            LastNameTextView.Layer.MasksToBounds = true;
            LastNameTextView.AttributedPlaceholder = new NSAttributedString(
              "Last Name",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          );
            LastNameTextView.Font = fontObject;
			LastNameTextView.EditingDidEnd += (sender, e) => {
				if (LastNameTextView.Text == string.Empty)
				{
					HelperMethods.AnimateValidationBorder(LastNameTextView);
					HelperMethods.SendBasicAlert("Validation", "Last name is required");
                    Settings.LastName = string.Empty;
				}
				else
				{
					HelperMethods.RemoveValidationBorder(LastNameTextView);
                    Settings.LastName = LastNameTextView.Text;
				}
			};

            MobilePhoneTextView.Layer.AddSublayer(bottomBorder7);
            MobilePhoneTextView.Layer.MasksToBounds = true;
            MobilePhoneTextView.AttributedPlaceholder = new NSAttributedString(
              "Mobile Phone",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          );

            MobilePhoneTextView.Font = fontObject;
            MobilePhoneTextView.KeyboardType = UIKeyboardType.PhonePad;
            MobilePhoneTextView.EditingDidEnd += (sender, e) => {
                if(!HelperMethods.IsValidPhoneNumber(MobilePhoneTextView.Text, MobilePhoneTextView)){
                    HelperMethods.SendBasicAlert("Validation", "Phone number is required");
                }
            };

            CompanyTextView.Layer.AddSublayer(bottomBorder8);
            CompanyTextView.Layer.MasksToBounds = true;
            CompanyTextView.AttributedPlaceholder = new NSAttributedString(
                "Company (optional)",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );
            CompanyTextView.Font = fontObject;
            CompanyTextView.EditingDidEnd += (sender, e) => {
                if(CompanyTextView.Text != string.Empty){
                    Settings.Company = CompanyTextView.Text;
                }
            };

            HomeAirportTextView.Layer.AddSublayer(bottomBorder9);
            HomeAirportTextView.Layer.MasksToBounds = true;
            HomeAirportTextView.AttributedPlaceholder = new NSAttributedString(
                "Home Airpot ID ex SDF (optional)",
              font: fontObject,
              foregroundColor: UIColor.DarkGray
          );
            HomeAirportTextView.Font = fontObject;
            HomeAirportTextView.EditingDidEnd += (sender, e) => {
                if(!HelperMethods.ValidateHomeAirport(HomeAirportTextView.Text, HomeAirportTextView))
                {
					HelperMethods.SendBasicAlert("Validation", "Home airport must be 5 characters or less");
                };
                HomeAirportTextView.Text = HomeAirportTextView.Text.ToUpper();
            };

            LocationLabel.Layer.AddSublayer(bottomBorderLabel);
            LocationLabel.Layer.MasksToBounds = true;
            LocationLabel.Font = fontObject;
            //Start here


            PilotLabel.Font = fontObject;
         
        }

        public override UIView GetViewForHeader(UITableView tableView, nint section)
        {
            var viewHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 55.0f : 40.0f;

            UIView view = new UIView(new System.Drawing.RectangleF(0, 0, (float)this.View.Frame.Width, viewHeight));
            view.BackgroundColor = UIColor.White;

            UILabel label = new UILabel();
            label.Opaque = false;
            label.TextColor = HelperMethods.GetLime();
            var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 35.0f : 25.0f;
            label.Font = UIFont.BoldSystemFontOfSize(fontSize);

            label.Frame = new System.Drawing.RectangleF(0, 0, (float)view.Frame.Width - 30, viewHeight);
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
            var rowHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 80.0f : 50.0f;
            if (hidePilotRows)
            {
                if (indexPath.Section == 1 && (indexPath.Row == 7 || indexPath.Row == 8))
                {
                    return 0;
                }
            }
            if (hideManufacturerAndModelRow)
            {
                if (indexPath.Section == 2 && (indexPath.Row == 1 || indexPath.Row == 2))
                {
                    return 0;
                }
            }
            if (hideModelRow)
            {
                if (indexPath.Section == 2 && indexPath.Row == 2)
                {
                    return 0;
                }
            }
            return rowHeight;
        }

    }

    public class PilotTypePickerViewModel : UIPickerViewModel
    {
        public PilotTypePickerViewModel()
        {
        }
        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return Settings.LocationResponse.AreYouAPilot.Count;
        }

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
            Settings.PilotStatusId = Settings.LocationResponse.AreYouAPilot[(int)row].PilotStatusId;
            Settings.PilotStatusString = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
		}



		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {

            var viewHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 60.0f : 40.0f;

			var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 22.0f : 16.0f;
			var fontObject = UIFont.SystemFontOfSize(fontSize);

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, viewHeight);

            lbl.Font = UIFont.SystemFontOfSize(fontSize);

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Center;

            lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;

            lbl.BackgroundColor = UIColor.White;

            return lbl;
        }
    }

    public class RatingPickerViewModel : UIPickerViewModel
    {
        public RatingPickerViewModel()
        {
        }
        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return Settings.LocationResponse.PilotRating.Count;
        }

		public override void Selected(UIPickerView pickerView, nint row, nint component)
		{
            Settings.PilotTypeId = Settings.LocationResponse.PilotRating[(int)row].PilotTypeId;
		}

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {

			var viewHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 60.0f : 40.0f;

			var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 22.0f : 16.0f;
			var fontObject = UIFont.SystemFontOfSize(fontSize);

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, viewHeight);

            lbl.Font = fontObject;

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Center;

            lbl.Text = Settings.LocationResponse.PilotRating[(int)row].Title;

            lbl.BackgroundColor = UIColor.White;

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
            Settings.ClassificationId = Settings.ClassificationList[(int)row].ClassificationId;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return Settings.ClassificationList.Count();
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
			var viewHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 60.0f : 40.0f;

			var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 22.0f : 16.0f;
			var fontObject = UIFont.SystemFontOfSize(fontSize);

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, viewHeight);

            lbl.Font = fontObject;

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Center;

            lbl.Text = Settings.ClassificationList[(int)row].ClassificationName;


            lbl.BackgroundColor = UIColor.White;

            return lbl;
        }
    }

    public class ManufacturerPickerViewModel : UIPickerViewModel
    {
        List<MfgLst> manufacturerList;

        public ManufacturerPickerViewModel(List<MfgLst> manList)
        {
            this.manufacturerList = manList;
        }

        public event EventHandler<EventArgs> ValueChanged;

        public MfgLst SelectedItem
        {
            get { return manufacturerList[selectedIndex]; }
        }

        public int selectedIndex = 0;


        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            selectedIndex = (int)row;
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());

            }
            Settings.ManufacturerId = manufacturerList[(int)row].ManufacturerId;
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

			var viewHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 60.0f : 40.0f;

			var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 22.0f : 16.0f;
			var fontObject = UIFont.SystemFontOfSize(fontSize);

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, viewHeight);

            lbl.Font = fontObject;

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Center;

            lbl.Text = manufacturerList[(int)row].Manufacturer;

            lbl.BackgroundColor = UIColor.White;

            return lbl;
        }
    }

    public class ModelPickerViewModel : UIPickerViewModel
    {
        List<ModDesLst> modelList;

        public ModelPickerViewModel(List<ModDesLst> modList)
        {
            this.modelList = modList;
        }

        public event EventHandler<EventArgs> ValueChanged;

        public ModDesLst SelectedItem
        {
            get { return modelList[selectedIndex]; }
        }

        public int selectedIndex = 0;
        public override nint GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public override nint GetRowsInComponent(UIPickerView picker, nint component)
        {
            return modelList.Count;
        }

        public override void Selected(UIPickerView pickerView, nint row, nint component)
        {
            selectedIndex = (int)row;
            if (ValueChanged != null)
            {
                ValueChanged(this, new EventArgs());
               

            }
			Settings.DesignationId = modelList[(int)row].DesignationId;
        }

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
			var viewHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 60.0f : 40.0f;

			var fontSize = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 22.0f : 16.0f;
			var fontObject = UIFont.SystemFontOfSize(fontSize);

           
            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, viewHeight);

            lbl.Font = fontObject;

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Center;

            lbl.Text = modelList[(int)row].Designation;

            lbl.BackgroundColor = UIColor.White;
            return lbl;
        }
    }
}