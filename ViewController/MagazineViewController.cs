using Foundation;
using System;
using UIKit;
using SDWebImage;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace AircraftForSale
{
    public partial class MagazineViewController : UIViewController
    {
        public MagazineViewController (IntPtr handle) : base (handle)
        {
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);


				if (AppDelegate.FirstAd != null)
				{

						BackgroundImage.SetImage(
							url: new NSUrl(AppDelegate.FirstAd.ImageURL),
							placeholder: UIImage.FromBundle("ad_placeholder.jpg")
						);

			
				}
		
		}
    }
}