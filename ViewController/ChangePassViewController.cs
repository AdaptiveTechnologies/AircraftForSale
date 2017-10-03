using System;

using UIKit;
using Google.Analytics;
using AircraftForSale.PCL.Helpers;
using AircraftForSale.PCL;

namespace AircraftForSale
{
	public partial class ChangePassViewController : UIViewController
	{
		protected ChangePassViewController(IntPtr handle) : base(handle)
		{
			// Note: this .ctor should not contain any initialization logic.
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.
		}

		partial void closeAction(Foundation.NSObject sender)
		{
			this.DismissViewController(true, null);
		}

		async void SubmitButton_TouchUpInside(object sender, EventArgs e) { 

			//if (Settings.Password != oldPassEdt.Text)
			//{
			//	HelperMethods.SendBasicAlert("Validation", "Invalid Password");
			//	return;
			//}


			if (confirmpassEdt.Text != nPassEdt.Text)
			{
				HelperMethods.SendBasicAlert("Validation", "Password doesn't match");
				return;
			}



			try { 

           //AuthResponse response1 = await AuthResponse.GetAuthResponseAsync(0, Settings.Username, Settings.Password);

            APIManager manager = new APIManager();
			
				var response = await manager.ChangePassword(Settings.AppID, Settings.Username, Settings.AuthToken, oldPassEdt.Text, nPassEdt.Text);

			if (response == null)
			{

				HelperMethods.SendBasicAlert("Change Password", "Failed");
			}
			else {
				if (response.Status.Equals("Success"))
				{
					Settings.Password = nPassEdt.Text;
					HelperMethods.SendBasicAlert("Change Password", "Done");
				}
				else 
					HelperMethods.SendBasicAlert("Change Password", response.ResponseMsg);
			}
			}
			catch (Exception exc) { 
				HelperMethods.SendBasicAlert("Change Password", "Failed");
			}




		}




		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "View Controller");

			Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());

			//string url = "https://globalair.com/myaircraft/rates.aspx";
			//webView.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(url)));
			//webView.ScalesPageToFit = true;


			submitBtn.TouchUpInside += SubmitButton_TouchUpInside;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			submitBtn.TouchUpInside -= SubmitButton_TouchUpInside;
		}
	}
}

