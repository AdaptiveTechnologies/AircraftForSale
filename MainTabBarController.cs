using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL.Helpers;

namespace AircraftForSale
{
    public partial class MainTabBarController : UITabBarController
    {
        public MainTabBarController (IntPtr handle) : base (handle)
        {
			
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//need to set the register tab bar item badge value based on the registration status
			var registrationTabBarItem = TabBar.Items[2];
			//if (Settings.IsRegistered)
			//{

			//	registrationTabBarItem.BadgeValue = null;
			//}
			//else {
			//	registrationTabBarItem.BadgeValue = "1";
			//}


			TabBar.TintColor = UIKit.UIColor.Green;   
			TabBar.BarTintColor = UIKit.UIColor.Black;         
			TabBar.BackgroundColor = UIKit.UIColor.White;

		}
    }
}