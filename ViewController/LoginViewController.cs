using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using Google.Analytics;
using AircraftForSale.PCL;
using System.Threading.Tasks;
//using SVProgressHUDBinding;

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
            set;
        }




        async void SubmitButton_TouchUpInside(object sender, EventArgs e)
        {
            UsernameTextField.Enabled = false;
            PasswordTextField.Enabled = false;

            string username = UsernameTextField.Text;
            string password = PasswordTextField.Text;



            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {

                if(!HelperMethods.IsValidPassword(password)){
                    HelperMethods.SendBasicAlert("Validation", "Passwords should contain letters and numbers and be at least 6 characters.");
                    PasswordTextField.Layer.BorderColor = UIColor.Red.CGColor;
                    PasswordTextField.Layer.BorderWidth = 1f;
                    UsernameTextField.Enabled = true;
                    PasswordTextField.Enabled = true;
                    return;
                }
                //Save username/password to settings

                Settings.Username = username;
                Settings.Password = password;



                LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, "Loading ...");
                this.View.AddSubview(loadingIndicator);


                AuthResponse response = await AuthResponse.GetAuthResponseAsync(0, Settings.Username, Settings.Password);

                Settings.AppID = response.AppId;
                Settings.AuthToken = response.AuthToken;

                APIManager manager = new APIManager();

                {
                    LoginResponse authResponse = await manager.loginUser(Settings.AppID, Settings.Username, Settings.Password, Settings.AuthToken);

                    if (authResponse.AuthToken != null)
                    {

                        Settings.AppID = authResponse.AppId;
                        Settings.UserID = authResponse.UserId;
                        Settings.AuthToken = authResponse.AuthToken;
                    }
                    else
                    {
                        loadingIndicator.Hide();
                        String responseMessage = "";
                        if(authResponse.ResponseMsg != null && authResponse.ResponseMsg != string.Empty){
                            responseMessage = authResponse.ResponseMsg;
                        }else {
                            responseMessage = "Unable to authenticate user.";
                        }

                        if (responseMessage == "Incorrect Password")
                        {
                            MessageViewController messageViewController = (MessageViewController)Storyboard.InstantiateViewController("MessageViewController");
                            this.PresentViewController(messageViewController, true, null);
                        }
                        else
                        {
                            HelperMethods.SendBasicAlert("Login", responseMessage);
                        }
						UsernameTextField.Enabled = true;
						PasswordTextField.Enabled = true;
                        return;
                    }

                    //AuthResponse auResponse = await AuthResponse.GetAuthResponseAsync(Settings.AppID, Settings.Username, Settings.Password);
                    try
                    {
                        var responseProfile = await manager.getUserProfile(Settings.AppID, Settings.Username, Settings.AuthToken, Settings.Password);

                        Settings.IsAmphibian = responseProfile.C1 == 1;
                        Settings.IsCommercial = responseProfile.C2 == 1;
                        Settings.IsExperimental = responseProfile.C3 == 1;
                        Settings.IsHelicopter = responseProfile.C4 == 1;
                        Settings.IsJets = responseProfile.C5 == 1;
                        Settings.IsSingles = responseProfile.C7 == 1;
                        Settings.IsSingleEngine = responseProfile.C6 == 1;
                        Settings.IsTwinPistons = responseProfile.C8 == 1;
                        Settings.IsTwinTurbines = responseProfile.C9 == 1;
                        Settings.IsVintage = responseProfile.C10 == 1;
                        Settings.IsWarbirds = responseProfile.C11 == 1;
                        Settings.IsLightSport = responseProfile.C12 == 1;

                        Settings.Email = Settings.Username;

                        Settings.FirstName = responseProfile.FirstName;
                        Settings.LastName = responseProfile.LastName;
                        Settings.Phone = responseProfile.CellPhone;

                        Settings.Company = responseProfile.Company;
                        Settings.Hours = responseProfile.HourPerMonth;
                        Settings.ManufacturerId = responseProfile.DesignationId;
                        Settings.LocationPickerSelectedId = responseProfile.CountryId;
                        Settings.Phone = responseProfile.CellPhone;
                        Settings.DesignationId = responseProfile.DesignationId;
                        Settings.PurposeId = responseProfile.FlyingPurposeId;
                        Settings.HomeAirport = responseProfile.HomeAirport;
                        Settings.Password = responseProfile.Password;
                        Settings.PilotStatusId = responseProfile.PilotStatusId;
                        Settings.PilotTypeId = responseProfile.PilotTypeId;
                        Settings.PurchaseTimeFrame = responseProfile.PurchaseTimeFrame;

                        loadingIndicator.Hide();
                        this.PerformSegue("LoadTabBarControllerSeque", this);
                    }
                    catch (Exception exe)
                    {
                        loadingIndicator.Hide();
                        HelperMethods.SendBasicAlert("Login", "Login Failed");
                    }


                }




                PasswordTextField.Layer.BorderColor = UIColor.Clear.CGColor;
                PasswordTextField.Layer.BorderWidth = 0f;

                UsernameTextField.Layer.BorderColor = UIColor.Clear.CGColor;
                UsernameTextField.Layer.BorderWidth = 0f;
            }
            else
            {
                //Update UI to reflect invalid username or password

                if (string.IsNullOrEmpty(username))
                {
                    UsernameTextField.Layer.BorderColor = UIColor.Red.CGColor;
                    UsernameTextField.Layer.BorderWidth = 1f;
                }
                else
                {
                    UsernameTextField.Layer.BorderColor = UIColor.Clear.CGColor;
                    UsernameTextField.Layer.BorderWidth = 0f;
                }

                if (string.IsNullOrEmpty(password))
                {
                    PasswordTextField.Layer.BorderColor = UIColor.Red.CGColor;
                    PasswordTextField.Layer.BorderWidth = 1f;
                }
                else
                {
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

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());


            //if (!Settings.IsRegistered && !Settings.DisplayedMessage)
            //{
            //    //In this scenario, display a modal (just once) asking if user has had trouble registering.
            //    //ShowViewController(UIModalTransitionStyle.CoverVertical);

            //    MessageViewController messageViewController = (MessageViewController)Storyboard.InstantiateViewController("MessageViewController");
            //    //this.ShowViewController(messageViewController, this);
            //    this.PresentViewController(messageViewController, true, () => { Settings.DisplayedMessage = true; });
            //}
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();


            LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, "Loading aircraft data...");
            AppDelegate.OnDataUpdate += (source, e) =>
            {

                float progress = (float)e.GetComplete() / (float)e.GetTotal();

                if (progress < 1f)
                {
                    //SVProgressHUD.SetDefaultStyle(SVProgressHUDStyle.Dark);
                    //SVProgressHUD.ShowProgress(progress, "Loading aircraft data...", SVProgressHUDMaskType.Gradient);
                   
                    this.View.AddSubview(overlay);
                }

               

                if (progress >= 1)
                {
                    overlay.Hide();
                }

            };

            HideKeyboardGesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);

            });

            DataUpdateProgressView.Progress = 0f;
            DataUpdateProgressView.Hidden = true;



            int cornerRadius = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 25 : 20;

            UsernameTextField.Layer.CornerRadius = cornerRadius;
            UsernameTextField.Layer.BorderColor = UIColor.White.CGColor;
            UsernameTextField.Layer.BorderWidth = 1f;
            UsernameTextField.AttributedPlaceholder = new NSAttributedString(
                            "Email Address",
                            font: UsernameTextField.Font,
                            foregroundColor: UIColor.White
            );

            PasswordTextField.Layer.CornerRadius = cornerRadius;
            PasswordTextField.Layer.BorderColor = UIColor.White.CGColor;
            PasswordTextField.Layer.BorderWidth = 1f;
            PasswordTextField.AttributedPlaceholder = new NSAttributedString(
                            "Password",
                            font: PasswordTextField.Font,
                            foregroundColor: UIColor.White
            );

            LoginButton.Layer.CornerRadius = cornerRadius;
            LoginButton.Layer.BorderColor = UIColor.White.CGColor;
            LoginButton.Layer.BorderWidth = 0f;
            
            LaterButton.Layer.CornerRadius = cornerRadius;
            LaterButton.Layer.BorderColor = UIColor.White.CGColor;
            LaterButton.Layer.BorderWidth = 0f;
            
			changePassButton.Layer.CornerRadius = cornerRadius;
			changePassButton.Layer.BorderColor = UIColor.White.CGColor;
			changePassButton.Layer.BorderWidth = 0f;

            
            

            LoginButton.TouchUpInside += SubmitButton_TouchUpInside;



            UpdateMyProfileButton.Layer.CornerRadius = cornerRadius;
            UpdateMyProfileButton.TouchUpInside += (sender, e) =>
            {


                FavoriteClassificationsViewController favClassificationsVC = new FavoriteClassificationsViewController(new AircraftGridLayout(this));

                this.ShowViewController(new UINavigationController(favClassificationsVC), this);
            };

            LogoutButton.Layer.CornerRadius = cornerRadius;
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

            RegisterNowButton.TouchUpInside += (sender, e) =>
            {
            
                FavoriteClassificationsViewController favClassificationsVC = new FavoriteClassificationsViewController(new AircraftGridLayout(this));

                this.ShowViewController(new UINavigationController(favClassificationsVC), this);

            };

            ////TODO: Remove after testing: testing new check username api endpoint
            //Task.Run(async () =>
            //{
            //    APIManager apiManager = new APIManager();
            //    //var authResponseInner = await apiManager.CheckUserNameAsync(Settings.AppID, Settings.AuthToken, "clay12345@me.com", Settings.Password);

            //    var authResponseInner = await apiManager.ValidateLoginAsync(Settings.AppID, Settings.AuthToken, "Horseman@ganmail.com", "Showcase5");
            //});

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
            else
            {

                ///this.TabBarItem.BadgeValue = "1";

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