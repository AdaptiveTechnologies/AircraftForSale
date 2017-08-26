using Foundation;
using System;
using UIKit;
using System.Linq;
using AircraftForSale.PCL.Helpers;

namespace AircraftForSale
{
    public partial class MyProfileViewController : UIViewController, IMultiStepProcessStep
    {
		partial void UIButton27947_TouchUpInside(UIButton sender)
		{
			Settings.IsRegistered = true;

			RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;

			var loginVC = registrationVC.PresentingViewController as LoginViewController;
			//if loginVS is null, than started registration from the GlobalAir Magazine navigation controller or tabbarcontroller main home view
			if (loginVC == null)
			{
				//presenting viewcontroller will be tabbarcontroller if did registration from tab nav controller
				var tabBarController = registrationVC.PresentingViewController as UITabBarController;
				if (tabBarController != null)
				{
					registrationVC.DismissViewController(true, null);
					//MagazineViewController myMagazineVC = this.Storyboard.InstantiateViewController("MagazineViewController") as MagazineViewController;
					var viewController = tabBarController.ViewControllers.FirstOrDefault(row => row is MagazineViewController);
					tabBarController.SelectedViewController = viewController;
				}
				else {
					//if wasn't registering from login page or tabbarviewcontroller than registered from the Globalair magazinae nav controller
					registrationVC.NavigationController.PopViewController(true);
				}
			}
			else {
				registrationVC.DismissViewController(true, null);
				loginVC.PerformSegue("MagazineSegue", loginVC);
			}
		}


		public MyProfileViewController (IntPtr handle) : base (handle)
        {
			
        }
		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			StepActivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });

		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			PageHeaderLabel.BackgroundColor = UIColor.Gray;

			ContainerViewWidthConstraint.Constant = View.Bounds.Width - 40;
			ContainerView.BackgroundColor = UIColor.Clear;
			ContainerView.Alpha = .8f;
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });
		}

		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;
    }
}