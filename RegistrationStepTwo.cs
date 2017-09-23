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

            //this.TableView.BackgroundView = new UIImageView(UIImage.FromBundle("new_home_bg1"));

            NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);

            var nextBarButtonItem = new UIBarButtonItem("Next", UIBarButtonItemStyle.Plain, (sender, args) =>
            {
                //StepTwoSeque
                PerformSegue("StepThreeSeque", this);
            });
            UITextAttributes icoFontAttribute = new UITextAttributes();
            icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(20);
            icoFontAttribute.TextColor = UIColor.White;

            nextBarButtonItem.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);


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

            var classificationPickerViewModel = new ClassificationPickerViewModel();
            ClassificationPicker.Model = classificationPickerViewModel;
            //ClassificationPicker.key


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
                            manufacturerPickerViewModel = new ManufacturerPickerViewModel(cmmdResponse.MfgLst);
                            ManufacturerPicker.Model = manufacturerPickerViewModel;

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
                                            modelPickerViewModel = new ModelPickerViewModel(modelList.ToList());
                                            ModelPicker.Model = modelPickerViewModel;
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

            UIToolbar toolbar = new UIToolbar();
            toolbar.BarStyle = UIBarStyle.Black;
            toolbar.Translucent = true;
            toolbar.SizeToFit();

            UIBarButtonItem doneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
            {
                UITextField textview = PilotTypeTextField;
                //textview.Text = Settings.LocationResponse.PilotRating.FirstOrDefault(row => row.PilotTypeId == Settings.PilotTypeId).Title;
                textview.Text = "Selected Value";
                textview.ResignFirstResponder();
            });
            toolbar.SetItems(new UIBarButtonItem[] { doneButton }, true);


            PilotTypeTextField.InputView = pilotTypePicker;
            PilotTypeTextField.InputAccessoryView = toolbar;

            RatingPicker.Model = new RatingPickerViewModel();

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

            var bottomBorderLabel = new CALayer();
            bottomBorderLabel.Frame = new CGRect(0.0f, LocationLabel.Frame.Height - 1, LocationLabel.Frame.Width, 1.0f);
            bottomBorderLabel.BackgroundColor = borderBackgroundColor;

			PilotTypeTextField.Layer.AddSublayer(bottomBorder10);
			PilotTypeTextField.Layer.MasksToBounds = true;
			PilotTypeTextField.AttributedPlaceholder = new NSAttributedString(
				"Select a Type",
				font: UIFont.FromName("System-Regular", 22.0f),
				foregroundColor: UIColor.DarkGray
			//strokeWidth: 4
			);

            // add to UITextField
            UsernameTextView.Layer.AddSublayer(bottomBorder1);
            UsernameTextView.Layer.MasksToBounds = true;
            UsernameTextView.AttributedPlaceholder = new NSAttributedString(
                "Username (Your Email Address)",
                font: UIFont.FromName("System-Regular", 22.0f),
                foregroundColor: UIColor.DarkGray
            //strokeWidth: 4
            );

            ReEnterUsernameTextView.Layer.AddSublayer(bottomBorder2);
            ReEnterUsernameTextView.Layer.MasksToBounds = true;
            ReEnterUsernameTextView.AttributedPlaceholder = new NSAttributedString(
              "Re-Enter (Your Email Address)",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            PasswordTextView.Layer.AddSublayer(bottomBorder3);
            PasswordTextView.Layer.MasksToBounds = true;
            PasswordTextView.AttributedPlaceholder = new NSAttributedString(
              "Password",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            ReEnterPasswordTextView.Layer.AddSublayer(bottomBorder4);
            ReEnterPasswordTextView.Layer.MasksToBounds = true;
            ReEnterPasswordTextView.AttributedPlaceholder = new NSAttributedString(
              "Re-Enter Password",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            FirstNameTextView.Layer.AddSublayer(bottomBorder5);
            FirstNameTextView.Layer.MasksToBounds = true;
            FirstNameTextView.AttributedPlaceholder = new NSAttributedString(
              "First Name",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            LastNameTextView.Layer.AddSublayer(bottomBorder6);
            LastNameTextView.Layer.MasksToBounds = true;
            LastNameTextView.AttributedPlaceholder = new NSAttributedString(
              "Last Name",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            MobilePhoneTextView.Layer.AddSublayer(bottomBorder7);
            MobilePhoneTextView.Layer.MasksToBounds = true;
            MobilePhoneTextView.AttributedPlaceholder = new NSAttributedString(
              "Mobile Phone",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );






            CompanyTextView.Layer.AddSublayer(bottomBorder8);
            CompanyTextView.Layer.MasksToBounds = true;
            CompanyTextView.AttributedPlaceholder = new NSAttributedString(
                "Company (optional)",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            HomeAirportTextView.Layer.AddSublayer(bottomBorder9);
            HomeAirportTextView.Layer.MasksToBounds = true;
            HomeAirportTextView.AttributedPlaceholder = new NSAttributedString(
                "Home Airpot ID ex SDF (optional)",
              font: UIFont.FromName("System-Regular", 22.0f),
              foregroundColor: UIColor.DarkGray
          //strokeWidth: 4
          );

            LocationLabel.Layer.AddSublayer(bottomBorderLabel);
            LocationLabel.Layer.MasksToBounds = true;




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
            //UILabel lbl = new UILabel();

            //lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

            //lbl.Font = UIFont.SystemFontOfSize(17f);

            //lbl.TextColor = UIColor.DarkGray;

            //lbl.TextAlignment = UITextAlignment.Center;

            //lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
            //return lbl;

            UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
            containerView.BackgroundColor = UIColor.Gray;

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

            lbl.Font = UIFont.SystemFontOfSize(17f);

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Left;

            //if (row == 0)
            //{
            //	lbl.Text = "Select from list";
            //}
            //else
            //{

            lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
            //}

            lbl.BackgroundColor = UIColor.White;

            containerView.AddSubview(lbl);
            return containerView;
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
            //UILabel lbl = new UILabel();

            //lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

            //lbl.Font = UIFont.SystemFontOfSize(17f);

            //lbl.TextColor = UIColor.DarkGray;

            //lbl.TextAlignment = UITextAlignment.Center;

            ////lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
            //lbl.Text = Settings.LocationResponse.PilotRating[(int)row].Title;
            //return lbl;

            UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
            containerView.BackgroundColor = UIColor.Gray;

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

            lbl.Font = UIFont.SystemFontOfSize(17f);

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Left;

            //if (row == 0)
            //{
            //	lbl.Text = "Select from list";
            //}
            //else
            //{

            lbl.Text = Settings.LocationResponse.PilotRating[(int)row].Title;
            //}

            lbl.BackgroundColor = UIColor.White;

            containerView.AddSubview(lbl);
            return containerView;
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
            //UILabel lbl = new UILabel();

            //lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

            //lbl.Font = UIFont.SystemFontOfSize(17f);

            //lbl.TextColor = UIColor.DarkGray;

            //lbl.TextAlignment = UITextAlignment.Center;

            ////lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
            //lbl.Text = Settings.ClassificationList[(int)row].ClassificationName;
            //return lbl;

            UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
            containerView.BackgroundColor = UIColor.Gray;

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

            lbl.Font = UIFont.SystemFontOfSize(17f);

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Left;

            //if (row == 0)
            //{
            //	lbl.Text = "Select from list";
            //}
            //else
            //{

            lbl.Text = Settings.ClassificationList[(int)row].ClassificationName;
            //}

            lbl.BackgroundColor = UIColor.White;

            containerView.AddSubview(lbl);
            return containerView;
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
        }
        //public ManufacturerPickerViewModel()
        //{
        //	manufacturerList = new List<string>();
        //	manufacturerList.Add("Select a Manufacturer");
        //}
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
            //UILabel lbl = new UILabel();

            //lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

            //lbl.Font = UIFont.SystemFontOfSize(17f);

            //lbl.TextColor = UIColor.DarkGray;

            //lbl.TextAlignment = UITextAlignment.Center;

            ////lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
            //lbl.Text = manufacturerList[(int)row].Manufacturer;
            //return lbl;

            UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
            containerView.BackgroundColor = UIColor.Gray;

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

            lbl.Font = UIFont.SystemFontOfSize(17f);

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Left;

            //if (row == 0)
            //{
            //	lbl.Text = "Select from list";
            //}
            //else
            //{

            lbl.Text = manufacturerList[(int)row].Manufacturer;
            //}

            lbl.BackgroundColor = UIColor.White;

            containerView.AddSubview(lbl);
            return containerView;
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

        public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
        {
            //UILabel lbl = new UILabel();

            //lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);

            //lbl.Font = UIFont.SystemFontOfSize(17f);

            //lbl.TextColor = UIColor.DarkGray;

            //lbl.TextAlignment = UITextAlignment.Center;

            ////lbl.Text = Settings.LocationResponse.AreYouAPilot[(int)row].Title;
            //lbl.Text = modelList[(int)row].Designation;
            //return lbl;

            UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
            containerView.BackgroundColor = UIColor.Gray;

            UILabel lbl = new UILabel();

            lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

            lbl.Font = UIFont.SystemFontOfSize(17f);

            lbl.TextColor = UIColor.DarkGray;

            lbl.TextAlignment = UITextAlignment.Left;

            //if (row == 0)
            //{
            //	lbl.Text = "Select from list";
            //}
            //else
            //{

            lbl.Text = modelList[(int)row].Designation;
            //}

            lbl.BackgroundColor = UIColor.White;

            containerView.AddSubview(lbl);
            return containerView;
        }
    }
}