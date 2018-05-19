using System;

using UIKit;
using Google.Analytics;
using AircraftForSale.PCL;
using CoreGraphics;

namespace AircraftForSale
{
    public partial class ContactWebViewController : UIViewController
    {
        protected ContactWebViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public UITapGestureRecognizer HideKeyboardGesture
        {
            get;
            set;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            HideKeyboardGesture = new UITapGestureRecognizer(() =>
            {
                View.EndEditing(true);

            });
			// Perform any additional setup after loading the view, typically from a nib.
			submitButton.Layer.CornerRadius = 25;
        }


        async partial void submitAction(Foundation.NSObject sender)
        {
            if (!string.IsNullOrEmpty(addressEdt.Text) && !string.IsNullOrEmpty(commentsEdt.Text) && !string.IsNullOrEmpty(nameEdt.Text) && !string.IsNullOrEmpty(phnoEdt.Text))
            {
                addressEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                addressEdt.Layer.BorderWidth = 0f;

                commentsEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                commentsEdt.Layer.BorderWidth = 0f;

                nameEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                nameEdt.Layer.BorderWidth = 0f;


                //this.DismissViewController(true, null);

                ////Send Inquiry
                var response = await AdInquiryResponse.AdInquiry(0, nameEdt.Text, addressEdt.Text, string.Empty, commentsEdt.Text
                                                                 , 0, AdInquirySource.Email, "");
                if (response == null)
                {
                    //var alert = UIAlertController.Create("Oops", "There was a problem sending your email to the aircraft broker. Please try again", UIAlertControllerStyle.Alert);

                    HelperMethods.SendBasicAlert("Oops", "There was a problem sending your email. Please try again");

                }
                else
                {
					var alert = UIAlertController.Create("Congratulations!", "Email sent successfully.", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, (obj) =>
					{
                        MainTabBarController controller = this.ParentViewController as MainTabBarController;
                        //Navigate to the main magazine tab
                        controller.SelectedIndex = 0;

					}));

					PresentViewController(alert, animated: true, completionHandler: () =>
					{

					});
                }

            }
            else
            {
                //Update UI to reflect invalid username or password

                if (string.IsNullOrEmpty(addressEdt.Text))
                {
                    //UIView.Animate(1, () =>
                    //{
                    //    addressEdt.Layer.BorderColor = UIColor.Red.CGColor;
                    //    addressEdt.Layer.BorderWidth = 4f;
                    //    View.LayoutIfNeeded();
                    //}, () => {

                    //    UIView.Animate(1, () =>
                    //    {
                    //        addressEdt.Layer.BorderColor = UIColor.Red.CGColor;
                    //        addressEdt.Layer.BorderWidth = 2f;
                    //        View.LayoutIfNeeded();
                    //    });
                    //});
                    AnimateValidationBorder(addressEdt);
                   
                }
                else
                {
                    addressEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                    addressEdt.Layer.BorderWidth = 0f;
                }

                if (string.IsNullOrEmpty(phnoEdt.Text))
                {
                    //phnoEdt.Layer.BorderColor = UIColor.Red.CGColor;
                    //phnoEdt.Layer.BorderWidth = 1f;
                    AnimateValidationBorder(phnoEdt);
                }
                else
                {
                    phnoEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                    phnoEdt.Layer.BorderWidth = 0f;
                }

                if (string.IsNullOrEmpty(commentsEdt.Text))
                {
                    //commentsEdt.Layer.BorderColor = UIColor.Red.CGColor;
                    //commentsEdt.Layer.BorderWidth = 1f;

                    AnimateValidationBorder(commentsEdt);

                }
                else
                {
                    commentsEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                    commentsEdt.Layer.BorderWidth = 0f;
                }

                if (string.IsNullOrEmpty(nameEdt.Text))
                {
                    //nameEdt.Layer.BorderColor = UIColor.Red.CGColor;
                    //nameEdt.Layer.BorderWidth = 1f;
                    AnimateValidationBorder(nameEdt);
                }
                else
                {
                    nameEdt.Layer.BorderColor = UIColor.Clear.CGColor;
                    nameEdt.Layer.BorderWidth = 0f;
                }
            }
        }

        partial void setChecked(Foundation.NSObject sender)
        {
            checkbox.Selected = !checkbox.Selected;
        }

        private void AnimateValidationBorder(UIView view){
            view.Layer.BorderColor = UIColor.Red.CGColor;

            //UIView.Animate(2d, () =>
            //{
            //    view.Layer.BorderColor = UIColor.Red.CGColor;
            //    view.Layer.BorderWidth = 4f;
            //    view.LayoutIfNeeded();
            //});
            //                        , () => {

            //	UIView.Animate(1, () =>
            //	{
            //		view.Layer.BorderColor = UIColor.Red.CGColor;
            //		view.Layer.BorderWidth = 2f;
            //		View.LayoutIfNeeded();
            //	});
            //});
            var orignalCenter = view.Center;
			UIView.Animate(.1, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.Autoreverse,
				() => {
					view.Center =
						new CGPoint(orignalCenter.X + 3f, view.Center.Y);
                    
                    view.Layer.BorderWidth = 4f;
				},
				() => {
					view.Center = orignalCenter;
                    view.Layer.BorderWidth = 2f;
                    
				}
			);
        }



        public override void DidReceiveMemoryWarning()
        {
            base.DidReceiveMemoryWarning();
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            View.AddGestureRecognizer(HideKeyboardGesture);

            // This screen name value will remain set on the tracker and sent with
            // hits until it is set to a new value or to null.
            Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "View Controller");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());

            //string url = "https://globalair.com/myaircraft/rates.aspx";
            //webView.LoadRequest(new Foundation.NSUrlRequest(new Foundation.NSUrl(url)));
            //webView.ScalesPageToFit = true;

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            View.RemoveGestureRecognizer(HideKeyboardGesture);
        }
    }
}

