using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using Google.Analytics;
using AircraftForSale.PCL;
using System.Threading.Tasks;

namespace AircraftForSale
{
	public partial class LoginViewController : UIViewController
	{


		[Action("UnwindToLoginViewController:")]
		public void UnwindToLoginViewController(UIStoryboardSegue segue)
		{
			Console.WriteLine("We've unwinded to LoginViewController!");
		}
		public LoginViewController(IntPtr handle) : base(handle)
		{


		}

		public UITabBarItem TabBarItemProperty
		{
			get
			{
				return RegisterTabBarButton;
			}
		}



public UITapGestureRecognizer HideKeyboardGesture
{
	get;
	set;	}




		async void SubmitButton_TouchUpInside(object sender, EventArgs e) { 
		        UsernameTextField.Enabled = false;
				PasswordTextField.Enabled = false;

				string username = UsernameTextField.Text;
                 string password = PasswordTextField.Text;

				//TODO: Perform username/password validation

				if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
				{
					//Save username/password to settings

					Settings.Username = username;
					Settings.Password = password;


				//if (Settings.AuthToken == null || Settings.AuthToken.Length == 0)
				//{
             LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, "Loading ...");
			this.View.AddSubview(loadingIndicator);

				//if(Settings.AuthToken==null)
				{
				    AuthResponse response = await AuthResponse.GetAuthResponseAsync(0, Settings.Username, Settings.Password);

					Settings.AppID = response.AppId;
					Settings.AuthToken = response.AuthToken;
				}   
					APIManager manager = new APIManager();

					{
					LoginResponse authResponse = await manager.loginUser(Settings.AppID, Settings.Username, Settings.Password, Settings.AuthToken);

					if (authResponse.AuthToken != null)
					{

						Settings.AppID = authResponse.AppId;
						Settings.UserID = authResponse.UserId;
						Settings.AuthToken = authResponse.AuthToken;
					}
					else { 
						loadingIndicator.Hide();
					HelperMethods.SendBasicAlert("Login", "Login Failed");
						return;
					}
				
                            //AuthResponse auResponse = await AuthResponse.GetAuthResponseAsync(Settings.AppID, Settings.Username, Settings.Password);
						try
						{
							var reponseProfile = await manager.getUserProfile(Settings.AppID, Settings.Username, Settings.AuthToken, Settings.Password);

							Settings.IsAmphibian = reponseProfile.C1 == 1;
							Settings.IsCommercial = reponseProfile.C2 == 1;
							Settings.IsExperimental = reponseProfile.C3 == 1;
							Settings.IsHelicopter = reponseProfile.C4 == 1;
							Settings.IsJets = reponseProfile.C5 == 1;
							Settings.IsSingles = reponseProfile.C7 == 1;
							Settings.IsSingleEngine = reponseProfile.C6 == 1;
							Settings.IsTwinPistons = reponseProfile.C8 == 1;
							Settings.IsTwinTurbines = reponseProfile.C9 == 1;
							Settings.IsVintage = reponseProfile.C10 == 1;
							Settings.IsWarbirds = reponseProfile.C11 == 1;
							Settings.IsLightSport = reponseProfile.C12 == 1;

							Settings.Email = Settings.Username;

                            Settings.FirstName = reponseProfile.FirstName;
                            Settings.LastName = reponseProfile.LastName;
                            Settings.Phone = reponseProfile.CellPhone;
						loadingIndicator.Hide();
                           this.PerformSegue("LoadTabBarControllerSeque", this);
						}
						catch (Exception exe) {
						loadingIndicator.Hide();
						HelperMethods.SendBasicAlert("Login", "Login Failed");
						}

							
                        }
						
					
					 
					
					PasswordTextField.Layer.BorderColor = UIColor.Clear.CGColor;
					PasswordTextField.Layer.BorderWidth = 0f;

					UsernameTextField.Layer.BorderColor = UIColor.Clear.CGColor;
					UsernameTextField.Layer.BorderWidth = 0f;
				}
				else {
					//Update UI to reflect invalid username or password

					if (string.IsNullOrEmpty(username))
					{
						UsernameTextField.Layer.BorderColor = UIColor.Red.CGColor;
						UsernameTextField.Layer.BorderWidth = 1f;
					}
					else {
						UsernameTextField.Layer.BorderColor = UIColor.Clear.CGColor;
						UsernameTextField.Layer.BorderWidth = 0f;
					}

					if (string.IsNullOrEmpty(password))
					{
						PasswordTextField.Layer.BorderColor = UIColor.Red.CGColor;
						PasswordTextField.Layer.BorderWidth = 1f;
					}
					else {
						PasswordTextField.Layer.BorderColor = UIColor.Clear.CGColor;
						PasswordTextField.Layer.BorderWidth = 0f;
					}
				}

				UsernameTextField.Enabled = true;
				PasswordTextField.Enabled = true;
		
		}


public override void ViewDidAppear(bool animated)
{
	base.ViewDidAppear(animated);

	// This screen name value will remain set on the tracker and sent with
	// hits until it is set to a new value or to null.
	Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "Login View");

	Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());
}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//Comment this if we decide it is important to support login
			//LoginButton.RemoveFromSuperview();
			//UsernameTextField.RemoveFromSuperview();
			//PasswordTextField.RemoveFromSuperview();

			HideKeyboardGesture = new UITapGestureRecognizer(() =>
			{
				View.EndEditing(true);

			});

            UsernameTextField.Layer.BorderColor = UIColor.White.CGColor;
            UsernameTextField.Layer.BorderWidth = 1f;
UsernameTextField.AttributedPlaceholder = new NSAttributedString(
				"Email Address",
				font: UsernameTextField.Font,
				foregroundColor: UIColor.White
);

            PasswordTextField.Layer.BorderColor = UIColor.White.CGColor;
            PasswordTextField.Layer.BorderWidth = 1f;
PasswordTextField.AttributedPlaceholder = new NSAttributedString(
				"Password",
				font: PasswordTextField.Font,
				foregroundColor: UIColor.White
);

            LoginButton.Layer.BorderColor = UIColor.White.CGColor;
            LoginButton.Layer.BorderWidth = 0f;

            LaterButton.Layer.BorderColor = UIColor.White.CGColor;
            LaterButton.Layer.BorderWidth = 0f;

			LoginButton.TouchUpInside += SubmitButton_TouchUpInside;
	



			UpdateMyProfileButton.TouchUpInside += (sender, e) =>
			{
				GridLayout classificationsGridLayout = new GridLayout();
				int classificationItemWidth = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200 : 100;
				int classificationItemHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 150 : 100;

				classificationsGridLayout.ItemSize = new CoreGraphics.CGSize(classificationItemWidth, classificationItemHeight);

				int insetTop = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 125 : 75;
				int insetLeftBottomRight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 50 : 5;

				classificationsGridLayout.SectionInset = new UIEdgeInsets(insetTop, insetLeftBottomRight, insetLeftBottomRight, insetLeftBottomRight);
				classificationsGridLayout.HeaderReferenceSize = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 100, 0);


				FavoriteClassificationsViewController favClassificationsVC = new FavoriteClassificationsViewController(classificationsGridLayout);
				favClassificationsVC.CollectionView.BackgroundView = new UIImageView(UIImage.FromBundle("new_home_bg1"));
				favClassificationsVC.CollectionView.AllowsMultipleSelection = true;

				//this.PresentViewController(favClassificationsVC, true, null);
				PresentModalViewController(favClassificationsVC, true);

				//this.PerformSegue("RegisterSegue", this);

              //RegistrationViewController registrationViewController = (RegistrationViewController)ParentViewController.Storyboard.InstantiateViewController("RegistrationViewController");
              //NavigationController.PushViewController(registrationViewController, true);
			};


			LogoutButton.TouchUpInside += (sender, e) =>
			{
				UIView.Animate(
					0.25,
					() =>
					{
						UsernameTextField.Alpha = 1f;
						PasswordTextField.Alpha = 1f;
						LoginButton.Alpha = 1f;
						LaterButton.Alpha = 1f;
						RegisterNowButton.Alpha = 1f;
					});

				LogoutButton.Alpha = 0f;
				UpdateMyProfileButton.Alpha = 0f;
				changePassButton.Alpha = 0f;

				Settings.Logout();
				//this.TabBarItem.BadgeValue = "1";
			};

			//View.AddSubviews(UpdateMyProfileButton, LogoutButton);

		}

public override void ViewWillDisappear(bool animated)
{
	base.ViewWillDisappear(animated);

	View.RemoveGestureRecognizer(HideKeyboardGesture);
			//LoginButton.TouchUpInside -= SubmitButton_TouchUpInside;
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			View.AddGestureRecognizer(HideKeyboardGesture);

			if (Settings.IsRegistered)
			{
				
				//this.TabBarItem.BadgeValue = null;

				UsernameTextField.Alpha = 0f;
				PasswordTextField.Alpha = 0f;
				LoginButton.Alpha = 0f;
				LaterButton.Alpha = 0f;
				RegisterNowButton.Alpha = 0f;

				UpdateMyProfileButton.Alpha = 1f;
				LogoutButton.Alpha = 1f;
				changePassButton.Alpha = 1f;
			}
			else {
				
				//this.TabBarItem.BadgeValue = "1";

				UsernameTextField.Alpha = 1f;
				PasswordTextField.Alpha = 1f;
				LoginButton.Alpha = 1f;
				LaterButton.Alpha = 1f;
				RegisterNowButton.Alpha = 1f;

				UpdateMyProfileButton.Alpha = 0f;
				LogoutButton.Alpha = 0f;
				changePassButton.Alpha = 0f;
			}
		}
	}
}