using Foundation;
using System;
using UIKit;
using CoreGraphics;
using AircraftForSale.PCL;
using Google.Analytics;

namespace AircraftForSale
{
    public partial class EmailInquiryViewController : UIViewController
    {
		async void SubmitButton_TouchUpInside (object sender, EventArgs e)
		{
			if (!string.IsNullOrEmpty(EmailAddressTextField.Text) && !string.IsNullOrEmpty(CommentsTextView.Text) && !string.IsNullOrEmpty(NameTextField.Text)&& !string.IsNullOrEmpty(PhoneTextField.Text))
			{
				EmailAddressTextField.Layer.BorderColor = UIColor.Clear.CGColor;
				EmailAddressTextField.Layer.BorderWidth = 0f;

				CommentsTextView.Layer.BorderColor = UIColor.Clear.CGColor;
				CommentsTextView.Layer.BorderWidth = 0f;

				NameTextField.Layer.BorderColor = UIColor.Clear.CGColor;
                NameTextField.Layer.BorderWidth = 0f;

                //Send Inquiry
                //Clay Martin 1/1/18: Change app name to BuyPlane
                var response = await AdInquiryResponse.AdInquiry(int.Parse(AdProperty.ID), NameTextField.Text, EmailAddressTextField.Text, string.Empty, CommentsTextView.Text
                , AdProperty.BrokerId, AdInquirySource.Email, "Inquiry about " + AdProperty.Name + " from GlobalAir.com BuyPlane Magazine");

                //AdInquiryResponse response = new AdInquiryResponse();
                //response.Status = "Success";

				if (response.Status != "Success")
				{
					//var alert = UIAlertController.Create("Oops", "There was a problem sending your email to the aircraft broker. Please try again", UIAlertControllerStyle.Alert);

					HelperMethods.SendBasicAlert("Oops", "There was a problem sending your email to the aircraft broker. Please try again");

					//alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
					////alert.AddAction(UIAlertAction.Create("Snooze", UIAlertActionStyle.Default, action => Snooze()));
					//if (alert.PopoverPresentationController != null)
					//{
					//	alert.PopoverPresentationController.SourceView = this.View;
					//	alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
					//}
					//PresentViewController(alert, animated: true, completionHandler: null);
				}
				else
				{
					var alert = UIAlertController.Create("Congratulations!", "Email sent successfully.", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
					{
						this.PresentingViewController.DismissViewController(true, null);

					}));

					PresentViewController(alert, animated: true, completionHandler: () =>
					{

					});
				}

			}
			else {
				//Update UI to reflect invalid username or password

				if (string.IsNullOrEmpty(EmailAddressTextField.Text))
				{
					EmailAddressTextField.Layer.BorderColor = UIColor.Red.CGColor;
					EmailAddressTextField.Layer.BorderWidth = 1f;
				}
				else {
					EmailAddressTextField.Layer.BorderColor = UIColor.Clear.CGColor;
					EmailAddressTextField.Layer.BorderWidth = 0f;
				}

				if (string.IsNullOrEmpty(PhoneTextField.Text))
				{
					PhoneTextField.Layer.BorderColor = UIColor.Red.CGColor;
                    PhoneTextField.Layer.BorderWidth = 1f;
				}
				else {
                     PhoneTextField.Layer.BorderColor = UIColor.Clear.CGColor;
                     PhoneTextField.Layer.BorderWidth = 0f;
				}

				if (string.IsNullOrEmpty(CommentsTextView.Text))
				{
					CommentsTextView.Layer.BorderColor = UIColor.Red.CGColor;
					CommentsTextView.Layer.BorderWidth = 1f;
				}
				else {
					CommentsTextView.Layer.BorderColor = UIColor.Clear.CGColor;
					CommentsTextView.Layer.BorderWidth = 0f;
				}

				if (string.IsNullOrEmpty(NameTextField.Text))
				{
					NameTextField.Layer.BorderColor = UIColor.Red.CGColor;
					NameTextField.Layer.BorderWidth = 1f;
				}
				else {
					NameTextField.Layer.BorderColor = UIColor.Clear.CGColor;
					NameTextField.Layer.BorderWidth = 0f;
				}
			}
		}

public UITapGestureRecognizer HideKeyboardGesture
{
	get;
	set;	}

		bool EmailAddressTextField_ShouldReturn (UITextField textField)
		{
			textField.ResignFirstResponder();
			return true;
		}

		void CheckButton_TouchUpInside (object sender, EventArgs e)
		{
			CheckButton.Selected = !CheckButton.Selected;
		}


		void CloseButton_TouchUpInside (object sender, EventArgs e)
		{
			this.DismissViewController(true, null);
		}

		//variables to manage moving input elements on keyboard show
		#region managing keyboard variables
		private UIView activeview;             // Controller that activated the keyboard
		private float scroll_amount = 0.0f;    // amount to scroll 
		private float bottom = 0.0f;           // bottom point
		private float offset = 10.0f;          // extra offset
		//private bool moveViewUp = false;           // which direction are we moving
		private bool keyboardIsShowing = false; //if keyboard is already showing, no need to move elements on page up even higher
		#endregion

		public Ad AdProperty
		{
			get;
			set;
		}


        public EmailInquiryViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			ContactBrokerLabel.Text =  AdProperty.BrokerName;
            //Clay Martin 1/1/18: Change app name to BuyPlane
			CommentsTextView.Text = "Inquiry about " + AdProperty.Name + " Serial Number: "+AdProperty.SerialNumber+" from GlobalAir.com BuyPlane Magazine";

			//hide keyboard when touch anywhere
			HideKeyboardGesture = new UITapGestureRecognizer(() =>
			{
				View.EndEditing(true);

			});



		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			View.AddGestureRecognizer(HideKeyboardGesture);

// This screen name value will remain set on the tracker and sent with
// hits until it is set to a new value or to null.
Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "EmailEnquiry View");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());

			CloseButton.TouchUpInside += CloseButton_TouchUpInside;
			EmailAddressTextField.ShouldReturn += EmailAddressTextField_ShouldReturn;
			//CommentsTextView.shouldr += EmailAddressTextField_ShouldReturn;
			SubmitButton.TouchUpInside += SubmitButton_TouchUpInside;
			CheckButton.TouchUpInside += CheckButton_TouchUpInside;

			//keyboard observers
			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.DidShowNotification, KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver
						(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

			View.AddGestureRecognizer(HideKeyboardGesture);


		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			View.RemoveGestureRecognizer(HideKeyboardGesture);
			CloseButton.TouchUpInside -= CloseButton_TouchUpInside;
			EmailAddressTextField.ShouldReturn -= EmailAddressTextField_ShouldReturn;
			//CommentsTextView.ShouldReturn -= EmailAddressTextField_ShouldReturn;
			SubmitButton.TouchUpInside -= SubmitButton_TouchUpInside;
			CheckButton.TouchUpInside -= CheckButton_TouchUpInside;

				//keyboard observers
			// Keyboard popup
			NSNotificationCenter.DefaultCenter.RemoveObserver
			(UIKeyboard.DidShowNotification);

			// Keyboard Dow
			NSNotificationCenter.DefaultCenter.RemoveObserver
						(UIKeyboard.WillHideNotification);

			View.RemoveGestureRecognizer(HideKeyboardGesture);

		}

		#region keyboard management methods
		private void KeyBoardUpNotification(NSNotification notification)
		{
			if (keyboardIsShowing == false)
			{
				// get the keyboard size 
				var val = (NSValue)notification.UserInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey);
				CGRect r = val.CGRectValue;

				// Find what opened the keyboard
				//foreach (UIView view in this.View.Subviews)
				//{
				//	if (view.IsFirstResponder)
				//	{
				//		activeview = view;
				//		break;
				//	}
				//}

				//On this page don't want to cover the submission button, so making the submission button the active view

				activeview = SubmitButton;

				// Bottom of the controller = initial position + height + offset      
				bottom = (float)(activeview.Frame.Y + activeview.Frame.Height + offset);

				// Calculate how far we need to scroll
				scroll_amount = (float)(r.Height - (View.Frame.Size.Height - bottom));

				// Perform the scrolling
				if (scroll_amount > 0)
				{
					//moveViewUp = true;
					ScrollTheView();

				}
			//	else {
			//		//moveViewUp = false;
			//}
			}

		}

		private void KeyBoardDownNotification(NSNotification notification)
		{
			if (keyboardIsShowing)
			{
				//if (moveViewUp)
				//{
					ScrollTheView();

				//}
			}
		}

		private void ScrollTheView()
		{
			// scroll the view up or down
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			var frame = View.Frame;

			//frame.Y = 0;

			if (!keyboardIsShowing)
			{
				frame.Y -= scroll_amount;
				keyboardIsShowing = true;
			}
			else {
				frame.Y += scroll_amount;
				keyboardIsShowing = false;
				scroll_amount = 0;
				//UpdateViewConstraints();
			}

	

			View.Frame = frame;
			UIView.CommitAnimations();
		}

		#endregion
	}
}