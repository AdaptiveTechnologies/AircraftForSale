using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using AircraftForSale.PCL.Helpers;
using System.Drawing;
using CoreAnimation;
using CoreGraphics;
using System.Linq;

namespace AircraftForSale
{
	public partial class RegistrationStepThree : UITableViewController
	{
		public RegistrationStepThree(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{

			NavigationItem.Title = "Registration";

			NavigationItem.BackBarButtonItem = new UIBarButtonItem("Back", UIBarButtonItemStyle.Plain, null);

			var finishButton = new UIBarButtonItem("Finish", UIBarButtonItemStyle.Plain, (sender, args) =>
			{
				this.DismissViewController(true, null);
			});
			UITextAttributes icoFontAttribute = new UITextAttributes();
			icoFontAttribute.Font = UIFont.BoldSystemFontOfSize(20);
			icoFontAttribute.TextColor = UIColor.White;

			finishButton.SetTitleTextAttributes(icoFontAttribute, UIControlState.Normal);


			this.NavigationItem.SetRightBarButtonItem(finishButton, true);

			//TimeframePicker.Model = new TimeframeViewModel();
			//PurposePicker.Model = new PurposeViewModel();
			// OrderByPicker.Model = new SortOptionsViewModel();

			var orderByPicker = new UIPickerView();
			orderByPicker.Model = new SortOptionsViewModel();
			orderByPicker.ShowSelectionIndicator = true;

			UIToolbar orderByToolbar = new UIToolbar();
			orderByToolbar.BarStyle = UIBarStyle.Black;
			orderByToolbar.Translucent = true;
			orderByToolbar.SizeToFit();

			UIBarButtonItem orderByDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				UITextField textview = OrderByTextField;

                textview.Text = Settings.SortOptions;
				textview.ResignFirstResponder();
			});
			orderByToolbar.SetItems(new UIBarButtonItem[] { orderByDoneButton }, true);


			OrderByTextField.InputView = orderByPicker;
			OrderByTextField.InputAccessoryView = orderByToolbar;

			var timeframePicker = new UIPickerView();
			timeframePicker.Model = new TimeframeViewModel();
			timeframePicker.ShowSelectionIndicator = true;

			UIToolbar timeframeToolbar = new UIToolbar();
			timeframeToolbar.BarStyle = UIBarStyle.Black;
			timeframeToolbar.Translucent = true;
			timeframeToolbar.SizeToFit();

			UIBarButtonItem timeframeDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				UITextField textview = TimeFrameTextField;

                textview.Text = Settings.PurchaseTimeFrame + " months";
				textview.ResignFirstResponder();
			});
			timeframeToolbar.SetItems(new UIBarButtonItem[] { timeframeDoneButton }, true);


			TimeFrameTextField.InputView = timeframePicker;
			TimeFrameTextField.InputAccessoryView = timeframeToolbar;

            var whyFlyPicker = new UIPickerView();
			whyFlyPicker.Model = new PurposeViewModel();
			whyFlyPicker.ShowSelectionIndicator = true;

            UIToolbar whyFlyToolbar = new UIToolbar();
			whyFlyToolbar.BarStyle = UIBarStyle.Black;
			whyFlyToolbar.Translucent = true;
			whyFlyToolbar.SizeToFit();

            UIBarButtonItem whyFlyDoneButton = new UIBarButtonItem("Done", UIBarButtonItemStyle.Done, (s, e) =>
			{
				UITextField textview = WhyFlyTextField;

                var selected = Settings.LocationResponse.PurposeForFlying.FirstOrDefault(row => row.FlyingPurposeId == Settings.PurposeId);

                textview.Text = selected.Purpose;
				textview.ResignFirstResponder();
			});
			whyFlyToolbar.SetItems(new UIBarButtonItem[] { whyFlyDoneButton }, true);


			WhyFlyTextField.InputView = whyFlyPicker;
			WhyFlyTextField.InputAccessoryView = whyFlyToolbar;




			

			//TimeframePicker.Layer.BorderColor = UIColor.Gray.CGColor;
			//TimeframePicker.Layer.BorderWidth = 1;

			var borderFrameHeight = WhyFlyTextField.Frame.Size.Height - 1;
			var borderFrameWidth = WhyFlyTextField.Frame.Size.Width;
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


			WhyFlyTextField.Layer.AddSublayer(bottomBorder2);
			WhyFlyTextField.Layer.MasksToBounds = true;
			WhyFlyTextField.AttributedPlaceholder = new NSAttributedString(
				"Select from list",
				font: UIFont.FromName("System-Regular", 22.0f),
				foregroundColor: UIColor.DarkGray
			//strokeWidth: 4
			);

			TimeFrameTextField.Layer.AddSublayer(bottomBorder3);
			TimeFrameTextField.Layer.MasksToBounds = true;
			TimeFrameTextField.AttributedPlaceholder = new NSAttributedString(
				"Select from list",
				font: UIFont.FromName("System-Regular", 22.0f),
				foregroundColor: UIColor.DarkGray
			//strokeWidth: 4
			);

			OrderByTextField.Layer.AddSublayer(bottomBorder4);
			OrderByTextField.Layer.MasksToBounds = true;
			OrderByTextField.AttributedPlaceholder = new NSAttributedString(
				"Select from list",
				font: UIFont.FromName("System-Regular", 22.0f),
				foregroundColor: UIColor.DarkGray
			//strokeWidth: 4
			);



			// add to UITextField
			HoursTextView.Layer.AddSublayer(bottomBorder1);
			HoursTextView.Layer.MasksToBounds = true;
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
						label.Text = "Purchase Timeframe";
						break;
					}
				case 1:
					{
						label.Text = "Why do you fly?";
						break;
					}
				case 2:
					{
						label.Text = "Flight hours";
						break;
					}
				case 3:
					{
						label.Text = "Order By";
						break;
					}
			}
			return view;
		}
	}

	public class TimeframeViewModel : UIPickerViewModel
	{


		public List<Tuple<string, DateTime>> TimeFramePickerOptions
		{
			get;
			set;
		}
		public TimeframeViewModel()
		{

			TimeFramePickerOptions = new List<Tuple<string, DateTime>>();
			TimeFramePickerOptions.Add(new Tuple<string, DateTime>("Select from list", DateTime.Now));
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
			else
			{
				Settings.PurchaseTimeFrame = 0;
			}
		}

		public override UIView GetView(UIPickerView pickerView, nint row, nint component, UIView view)
		{
			//UILabel lbl = new UILabel();

			//lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			//if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			//{
			//	lbl.Font = UIFont.SystemFontOfSize(10f);
			//}
			//else
			//{

			//	lbl.Font = UIFont.SystemFontOfSize(18f);
			//}

			//lbl.TextColor = UIColor.White;

			//lbl.TextAlignment = UITextAlignment.Center;
			//lbl.Text = TimeFramePickerOptions[(int)row].Item1;

			UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
			containerView.BackgroundColor = UIColor.Gray;

			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Left;

			lbl.Text = TimeFramePickerOptions[(int)row].Item1;

			lbl.BackgroundColor = UIColor.White;

			containerView.AddSubview(lbl);
			return containerView;

		}
	}

	public class PurposeViewModel : UIPickerViewModel
	{


		public PurposeViewModel()
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
			//UILabel lbl = new UILabel();

			//lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			//if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			//{
			//	lbl.Font = UIFont.SystemFontOfSize(10f);
			//}
			//else
			//{

			//	lbl.Font = UIFont.SystemFontOfSize(18f);
			//}

			//lbl.TextColor = UIColor.White;

			//lbl.TextAlignment = UITextAlignment.Center;
			//lbl.Text = Settings.LocationResponse.PurposeForFlying[(int)row].Purpose;
			//return lbl;

			UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
			containerView.BackgroundColor = UIColor.Gray;

			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Left;

			if (row == 0)
			{
				lbl.Text = "Select from list";
			}
			else
			{

				lbl.Text = Settings.LocationResponse.PurposeForFlying[(int)row].Purpose;
			}

			lbl.BackgroundColor = UIColor.White;

			containerView.AddSubview(lbl);
			return containerView;
		}
	}

	public class SortOptionsViewModel : UIPickerViewModel
	{

		List<string> SortOptionsList { get; set; }

		public SortOptionsViewModel()
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
			//UILabel lbl = new UILabel();

			//lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 40f);
			//if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			//{
			//	lbl.Font = UIFont.SystemFontOfSize(10f);
			//}
			//else
			//{

			//	lbl.Font = UIFont.SystemFontOfSize(18f);
			//}

			//lbl.TextColor = UIColor.White;

			//lbl.TextAlignment = UITextAlignment.Center;
			//lbl.Text = SortOptionsList[(int)row];
			//return lbl;

			UIView containerView = new UIView(new System.Drawing.RectangleF(0, 0, (float)pickerView.Frame.Width, 31));
			containerView.BackgroundColor = UIColor.Gray;

			UILabel lbl = new UILabel();

			lbl.Frame = new RectangleF(0, 0, (float)pickerView.Frame.Width, 30f);

			lbl.Font = UIFont.SystemFontOfSize(17f);

			lbl.TextColor = UIColor.DarkGray;

			lbl.TextAlignment = UITextAlignment.Left;

			if (row == 0)
			{
				lbl.Text = "Select from list";
			}
			else
			{

				lbl.Text = SortOptionsList[(int)row];
			}

			lbl.BackgroundColor = UIColor.White;

			containerView.AddSubview(lbl);
			return containerView;
		}
	}

}