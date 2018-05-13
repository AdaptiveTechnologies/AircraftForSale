using System;
using System.Linq;
using System.Text.RegularExpressions;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using Foundation;
using MessageUI;
using SDWebImage;
using UIKit;

namespace AircraftForSale
{
	public class HelperMethods
	{


		public static UIColor GetLime()
		{
			return UIColor.FromRGB(45, 185, 45);//lime
		}

		public async static void SwapLikeButtonState(UIButton senderButton, Ad ad)
		{
			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				int adID;
				int.TryParse(ad.ID, out adID);
				if (ad.IsLiked)
				{
					//attempt to record unlike in the API
					if (adID != 0)
					{
						ad.IsLiked = false;
						senderButton.SetImage(UIImage.FromBundle("like"), UIControlState.Normal);

						AdLikeResponse adLikeResponse = await new AdLikeResponse().RecordAdLike(adID, false);

						if (adLikeResponse.Status != "Success")
						{
							if (adLikeResponse.ResponseMsg.Equals("E1000"))
								SendBasicAlert("Oops", "Auth token invalid or expired");

							ad.IsLiked = true;
							senderButton.SetImage(UIImage.FromBundle("like_selected"), UIControlState.Normal);
						}
					}
				}
				else
				{
					//attempt to record like in the API

					if (adID != 0)
					{
						ad.IsLiked = true;
						senderButton.SetImage(UIImage.FromBundle("like_selected"), UIControlState.Normal);

						AdLikeResponse adLikeResponse = await new AdLikeResponse().RecordAdLike(adID, true);

						if (adLikeResponse.Status != "Success")
						{
							if (adLikeResponse.ResponseMsg.Equals("E1000"))
								SendBasicAlert("Oops", "Auth token invalid or expired");

							ad.IsLiked = false;
							senderButton.SetImage(UIImage.FromBundle("like"), UIControlState.Normal);
						}
					}

				}
			}
			else
			{
				SendBasicAlert("Connect to a Network", "Please connect to a network to like this ad");
			}
		}
		public static void SetInitialLikeButtonState(UIButton senderButton, Ad ad)
		{
			if (ad.IsLiked)
			{
				senderButton.SetImage(UIImage.FromBundle("like_selected"), UIControlState.Normal);
			}
			else
			{
				senderButton.SetImage(UIImage.FromBundle("like"), UIControlState.Normal);
			}
		}

		//public static void LoadWebBanManPro(UIView mainView)
		//{
		//	Random random = new Random();
		//	var randomInt = random.Next(1, 100);
		//	if (randomInt < 17)
		//	{

		//		if (Reachability.IsHostReachable(Settings._baseDomain))
		//		{
		//			LoadingOverlay loadingOverlay = new LoadingOverlay(mainView.Frame);


		//			var frame = new CGRect(20, 20, mainView.Bounds.Width - 40, mainView.Bounds.Height - 150);
		//			//var frame = new CGRect(0.0, 20.0, mainView.Bounds.Width, 60.0);
		//			var webView = new UIWebView(frame);


		//			webView.LoadFinished += (sender, e) =>
		//			{
		//				loadingOverlay.Hide();
		//			};

		//			var url = "https://www.globalair.com/banmanpro/ad.aspx?ZoneID=94&Task=Get&Mode=HTML&SiteID=1&PageID=78751";
		//			webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

		//			UIView.BeginAnimations("fadeflag");
		//			UIView.Animate(1, () =>
		//			{
		//				mainView.Alpha = .5f;
		//			}, () =>
		//			{

		//				mainView.AddSubview(webView);
		//				mainView.AddSubview(loadingOverlay);

		//				UIButton closeButton = new UIButton(new CGRect(mainView.Bounds.Width - 100, 10, 50, 50));
		//				closeButton.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
		//				closeButton.BackgroundColor = UIColor.White.ColorWithAlpha(.5f);
		//				closeButton.TouchUpInside += (sender, e) =>
		//				{
		//					try
		//					{
		//						webView.RemoveFromSuperview();
		//						closeButton.RemoveFromSuperview();
		//					}
		//					finally
		//					{
		//						webView.Dispose();
		//					}
		//				};

		//				UIImageView imageView = new UIImageView(new CGRect(mainView.Bounds.Width - 75, mainView.Bounds.Height / 2, 75, 50));
		//				imageView.Image = UIImage.FromBundle("swipe_left");
		//				imageView.Alpha = .5f;
		//				webView.AddSubview(closeButton);
		//				webView.AddSubview(imageView);

		//				mainView.Alpha = 1f;
		//			});

		//			UIView.CommitAnimations();
		//		}
		//		else {
		//			SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
		//	}
		//	}
		//}


		public static void LoadWebViewWithAd(UITapGestureRecognizer tap, Ad ad, UIView mainView)
		{
			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				LoadingOverlay loadingOverlay = new LoadingOverlay(mainView.Frame);

				var frame = new CGRect(0, 0, mainView.Bounds.Width, mainView.Bounds.Height);
				var webView = new UIWebView(frame);


				webView.LoadFinished += (sender, e) =>
				{
					loadingOverlay.Hide();
				};


				var url = ad.AircraftForSaleURL;
				//commenting to test banmanpro
				//var url = "https://www.globalair.com/banmanpro/ad.aspx?ZoneID=94&Task=Get&Mode=HTML&SiteID=1&PageID=78751";
				webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

				UIView.BeginAnimations("fadeflag");
				UIView.Animate(1, () =>
				{
					tap.View.Alpha = .5f;
				}, () =>
				{

					mainView.AddSubview(webView);
					mainView.AddSubview(loadingOverlay);

					UIButton closeButton = new UIButton(new CGRect(mainView.Bounds.Width - 75, 0, 75, 50));
					closeButton.SetImage(UIImage.FromBundle("close"), UIControlState.Normal);
					closeButton.BackgroundColor = UIColor.Black;
					closeButton.TouchUpInside += (sender, e) =>
					{
						try
						{
							webView.RemoveFromSuperview();
							closeButton.RemoveFromSuperview();
						}
						finally
						{
							webView.Dispose();
						}
					};

					UIImageView imageView = new UIImageView(new CGRect(mainView.Bounds.Width - 75, mainView.Bounds.Height / 2, 75, 50));
					imageView.Image = UIImage.FromBundle("swipe_left");
					imageView.Alpha = .5f;

					webView.AddSubview(imageView);
					webView.AddSubview(closeButton);

					tap.View.Alpha = 1f;
				});

				UIView.CommitAnimations();
			}
			else
			{
				SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
			}
		}

		public static UIImage GetImageFromCacheOrDefault(string imagePath)
		{
			UIImage image;
			//try
			//{
			image = SDImageCache.SharedImageCache.ImageFromMemoryCache(imagePath);
			if (image == null)
			{
				image = SDImageCache.SharedImageCache.ImageFromDiskCache(imagePath);
			}
			//}
			//catch (Exception e)
			//{
			//	image = UIImage.FromBundle("ad_placeholder.jpg");
			//	}
			if (image == null)
			{
				image = UIImage.FromBundle("garage.jpeg");
			}
			return image;
		}

		public static UIImage FromUrl(string uri)
		{
			using (var url = new NSUrl(uri))
			using (var data = NSData.FromUrl(url))
				return UIImage.LoadFromData(data);
		}

		public static String GetRegistrationString(Ad ad, UIStringAttributes labelAttribute)
		{
			string plainRegistrationNumberText = ad.RegistrationNumber == string.Empty ? "N/A" : "" + ad.RegistrationNumber;
			//var registrationNumberString = new NSMutableAttributedString(plainRegistrationNumberText);
			//registrationNumberString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 0));
			return plainRegistrationNumberText;
		}
		public static String GetSerialString(Ad ad, UIStringAttributes labelAttribute)
		{
			string plainSerialNumberText = ad.SerialNumber == string.Empty ? " N/A" : "" + ad.SerialNumber;
			//var serialNumberString = new NSMutableAttributedString(plainSerialNumberText);
			//serialNumberString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 0));
			return plainSerialNumberText;
		}
		public static String GetTotalTimeString(Ad ad, UIStringAttributes labelAttribute)
		{
			string plainTotalTimeText = ad.TotalTime == "0" ? "0 hours" : "" + ad.TotalTime;
			//var totalTimeString = new NSMutableAttributedString(plainTotalTimeText);
			//totalTimeString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 0));
			return plainTotalTimeText;
		}
		//public static NSAttributedString GetLocationAttributedString(Ad ad, UIStringAttributes labelAttribute)
		//{
		//	string simpleLocationText = "" + ad.Location;
		//	//var locationString = new NSMutableAttributedString(simpleLocationText);
		//	//locationString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 0));
		//          return simpleLocationText;
		//}



		//public static NSAttributedString GetBrokerAttributedString(Ad ad, UIStringAttributes labelAttribute, UIStringAttributes valueAttribute)
		//{
		//	string plainBrokerText = "" + ad.BrokerName;
		//	var brokerString = new NSMutableAttributedString(plainBrokerText);
		//	//brokerString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 0));
		//	//brokerString.SetAttributes(valueAttribute.Dictionary, new NSRange(0, ad.BrokerName.Count()));
		//	return brokerString;
		//}

		public static bool ShowPriceChangedLabel(string stringLastUpdated)
		{
			DateTime lastUpdated;

			if (DateTime.TryParse(stringLastUpdated, out lastUpdated))
			{
				var timeDifference = DateTime.UtcNow - lastUpdated;
				var days = timeDifference.TotalDays;
				if (days < 31)
				{
					return true;
				}
			}

			return false;

		}

		public static void SendBasicAlert(string title, string message)
		{
			var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}
			if (alert.PopoverPresentationController != null)
			{
				alert.PopoverPresentationController.SourceView = vc.View;
				alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}
			vc.PresentViewController(alert, animated: true, completionHandler: null);
		}

		public static void MakeModelRegistrationRequiredPrompt(UIViewController viewController, UIButton button)
		{
			var alert = UIAlertController.Create("The Order by feature requires registration", "This feature will order the Make/Model of Aircraft (example: All of the listings for King Air B200). Would you like to register now?", UIAlertControllerStyle.ActionSheet);

			alert.AddAction(UIAlertAction.Create("Not Yet", UIAlertActionStyle.Default, null));

			alert.AddAction(UIAlertAction.Create("Register Now", UIAlertActionStyle.Default,
												 (action) =>
												 {
													 //var returnedViewController = viewController.NavigationController.PopViewController(true);
													 //RegistrationViewController registrationViewController = (RegistrationViewController)viewController.Storyboard.InstantiateViewController("RegistrationViewController");
													 //returnedViewController.NavigationController.PushViewController(registrationViewController, true);

													 //var returnedViewController = viewController.NavigationController.PopViewController(true);
													 //RegistrationViewController registrationViewController = (RegistrationViewController)viewController.Storyboard.InstantiateViewController("RegistrationViewController");
													 //viewController.ShowViewController(registrationViewController, viewController);

													 GridLayout classificationsGridLayout = new GridLayout();
													 int classificationItemWidth = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200 : 100;
													 int classificationItemHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 150 : 100;

													 classificationsGridLayout.ItemSize = new CoreGraphics.CGSize(classificationItemWidth, classificationItemHeight);

													 int insetTop = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 125 : 75;
													 int insetLeftBottomRight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 50 : 5;

													 classificationsGridLayout.SectionInset = new UIEdgeInsets(insetTop, insetLeftBottomRight, insetLeftBottomRight, insetLeftBottomRight);
													 classificationsGridLayout.HeaderReferenceSize = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 100, 0);

													 FavoriteClassificationsViewController favClassificationsVC = new FavoriteClassificationsViewController(classificationsGridLayout);

													 viewController.ShowViewController(new UINavigationController(favClassificationsVC), viewController);
												 }));



			if (alert.PopoverPresentationController != null)
			{
				alert.PopoverPresentationController.SourceView = button;
				alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}
			viewController.PresentViewController(alert, animated: true, completionHandler: null);
		}

		public static void SellerRegistrationRequiredPrompt(UIViewController viewController, UIButton button)
		{
			var alert = UIAlertController.Create("The Order by feature requires registration", "This feature will order the inventory a seller has (example: All of the listings Jetcraft has). Would you like to register now?", UIAlertControllerStyle.ActionSheet);

			alert.AddAction(UIAlertAction.Create("Not Yet", UIAlertActionStyle.Default, null));

			alert.AddAction(UIAlertAction.Create("Register Now", UIAlertActionStyle.Default,
												 (action) =>
												 {
													 //var returnedViewController = viewController.NavigationController.PopViewController(true);
													 //RegistrationViewController registrationViewController = (RegistrationViewController)viewController.Storyboard.InstantiateViewController("RegistrationViewController");
													 //returnedViewController.NavigationController.PushViewController(registrationViewController, true);

													 //var returnedViewController = viewController.NavigationController.PopViewController(true);
													 //RegistrationViewController registrationViewController = (RegistrationViewController)viewController.Storyboard.InstantiateViewController("RegistrationViewController");
													 //viewController.ShowViewController(registrationViewController, viewController);

													 GridLayout classificationsGridLayout = new GridLayout();
													 int classificationItemWidth = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200 : 100;
													 int classificationItemHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 150 : 100;

													 classificationsGridLayout.ItemSize = new CoreGraphics.CGSize(classificationItemWidth, classificationItemHeight);

													 int insetTop = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 125 : 75;
													 int insetLeftBottomRight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 50 : 5;

													 classificationsGridLayout.SectionInset = new UIEdgeInsets(insetTop, insetLeftBottomRight, insetLeftBottomRight, insetLeftBottomRight);
													 classificationsGridLayout.HeaderReferenceSize = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 100, 0);

													 FavoriteClassificationsViewController favClassificationsVC = new FavoriteClassificationsViewController(classificationsGridLayout);

													 viewController.ShowViewController(new UINavigationController(favClassificationsVC), viewController);
												 }));



			if (alert.PopoverPresentationController != null)
			{
				alert.PopoverPresentationController.SourceView = button;
				alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}
			viewController.PresentViewController(alert, animated: true, completionHandler: null);
		}

		public static void SellerRegistrationRequiredSmallPrompt(UIViewController viewController, UIButton button)
		{
			var alert = UIAlertController.Create("The Order by feature requires registration", "This feature will order the inventory a seller has (example: All of the listings Jetcraft has). Would you like to register now?", UIAlertControllerStyle.ActionSheet);

			alert.AddAction(UIAlertAction.Create("Not Yet", UIAlertActionStyle.Default, null));

			alert.AddAction(UIAlertAction.Create("Register Now", UIAlertActionStyle.Default,
												 (action) =>
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

													 viewController.ShowViewController(new UINavigationController(favClassificationsVC), viewController);
												 }));



			if (alert.PopoverPresentationController != null)
			{
				alert.PopoverPresentationController.SourceView = button;
				alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Any;
			}
			viewController.PresentViewController(alert, animated: true, completionHandler: null);
		}

		public static void LikeRegistrationRequiredPrompt(UIViewController viewController, UIButton button)
		{
			var alert = UIAlertController.Create("The Like feature requires registration", "This feature will allow you to “Like” your favorite aircraft and write notes.\nWould you like to register now?", UIAlertControllerStyle.ActionSheet);

			alert.AddAction(UIAlertAction.Create("Not Yet", UIAlertActionStyle.Default, null));

			alert.AddAction(UIAlertAction.Create("Register Now", UIAlertActionStyle.Default,
												 (action) =>
												 {
													 //var returnedViewController = viewController.NavigationController.PopViewController(true);
													 //RegistrationViewController registrationViewController = (RegistrationViewController)viewController.Storyboard.InstantiateViewController("RegistrationViewController");
													 //returnedViewController.NavigationController.PushViewController(registrationViewController, true);

													 //var returnedViewController = viewController.NavigationController.PopViewController(true);
													 //RegistrationViewController registrationViewController = (RegistrationViewController)viewController.Storyboard.InstantiateViewController("RegistrationViewController");
													 //viewController.ShowViewController(registrationViewController, viewController);

													 GridLayout classificationsGridLayout = new GridLayout();
													 int classificationItemWidth = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 200 : 100;
													 int classificationItemHeight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 150 : 100;

													 classificationsGridLayout.ItemSize = new CoreGraphics.CGSize(classificationItemWidth, classificationItemHeight);

													 int insetTop = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 125 : 75;
													 int insetLeftBottomRight = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 50 : 5;

													 classificationsGridLayout.SectionInset = new UIEdgeInsets(insetTop, insetLeftBottomRight, insetLeftBottomRight, insetLeftBottomRight);
													 classificationsGridLayout.HeaderReferenceSize = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 100, 0);

													 FavoriteClassificationsViewController favClassificationsVC = new FavoriteClassificationsViewController(classificationsGridLayout);

													 viewController.ShowViewController(new UINavigationController(favClassificationsVC), viewController);
												 }));



			if (alert.PopoverPresentationController != null)
			{
				alert.PopoverPresentationController.SourceView = button;
				alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}
			viewController.PresentViewController(alert, animated: true, completionHandler: null);
		}



		public static void ShowAndSendSMS(UIViewController viewController, string[] recipients, string body, Action successCallBack)
		{
			if (MFMessageComposeViewController.CanSendText)
			{
				MFMessageComposeViewController message =
					new MFMessageComposeViewController();

				message.Finished += (sender, e) =>
				{
					if (e.Result == MessageComposeResult.Sent)
					{
						successCallBack.Invoke();
					}
					message.DismissViewController(true, null);
				};

				message.Body = body;
				message.Recipients = recipients;
				viewController.PresentModalViewController(message, false);
			}
		}

		#region validation helpers
		public static bool IsValidEmail(string email, UIView view = null)
		{
			bool isValid = false;
			try
			{
				var addr = new System.Net.Mail.MailAddress(email);
				isValid = addr.Address == email;
			}
			catch
			{
				isValid = false;
			}

			if (view != null)
			{
				if (!isValid)
				{
					HelperMethods.AnimateValidationBorder(view);
				}
				else
				{
					HelperMethods.RemoveValidationBorder(view);
				}
			}

			if (isValid)
			{
				Settings.Email = email;
			}
			else
			{
				Settings.Email = string.Empty;
			}

			return isValid;

		}

		public static bool ReEnterEmail(string secondEmail, UIView view)
		{

			if (secondEmail.ToLower() == Settings.Email.ToLower())
			{
				HelperMethods.RemoveValidationBorder(view);
				return true;
			}
			else
			{

				HelperMethods.AnimateValidationBorder(view);
				return false;
			}
		}

		public static bool IsValidPassword(string password, UIView view = null)
		{
			//regex "^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{10,}$"
			bool isValid = false;
			Regex regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{6,}$");
			Match match = regex.Match(password);
			if (match.Success)
			{
				if (password.Length > 15)
					isValid = false;

				isValid = true;
			}
			else
			{
				isValid = false;
			}

			if (view != null)
			{
				if (isValid)
				{
					HelperMethods.RemoveValidationBorder(view);
				}
				else
				{
					HelperMethods.AnimateValidationBorder(view);
				}
			}

			if (isValid)
			{
				Settings.Password = password;
			}
			else
			{
				Settings.Password = string.Empty;
			}

			return isValid;

		}

		public static bool ReEnterPassword(string secondPassword, UIView view)
		{

			if (secondPassword.ToLower() == Settings.Password.ToLower())
			{
				HelperMethods.RemoveValidationBorder(view);
				return true;
			}
			else
			{

				HelperMethods.AnimateValidationBorder(view);
				return false;
			}
		}

		public static bool IsValidPhoneNumber(string phoneNumber, UIView view = null)
		{
			//Regex regex = new Regex(@"^(\(?\+?[0-9]*\)?)?[0-9_\- \(\)]*$");
			//Match match = regex.Match(phoneNumber);
			//if (match.Success)
			//{
			//	return true;
			//}
			//else {
			//	return false;
			//}
			bool isValid = false;
			if (phoneNumber == null || phoneNumber == string.Empty)
			{
				isValid = false;
			}
			else
			{
				isValid = true;
			}

			if (view != null)
			{
				if (isValid)
				{
					HelperMethods.RemoveValidationBorder(view);
				}
				else
				{
					HelperMethods.AnimateValidationBorder(view);
				}
			}

			if (isValid)
			{
				Settings.Phone = phoneNumber;
			}
			else
			{
				Settings.Phone = string.Empty;
			}


			return isValid;

		}

		public static bool ValidateHomeAirport(string homeAirport, UIView view)
		{
			//homeAirport = homeAirport.ToUpper();
			if (homeAirport.Count() < 6)
			{
				HelperMethods.RemoveValidationBorder(view);
				Settings.HomeAirport = homeAirport;
				return true;
			}
			else
			{

				HelperMethods.AnimateValidationBorder(view);
				Settings.HomeAirport = string.Empty;
				return false;
			}
		}





		public static void AnimateValidationBorder(UIView view)
		{
			view.Layer.BorderColor = UIColor.Red.CGColor;

			var orignalCenter = view.Center;
			UIView.Animate(.1, 0, UIViewAnimationOptions.CurveEaseInOut | UIViewAnimationOptions.Autoreverse,
				() =>
				{
					view.Center =
						new CGPoint(orignalCenter.X + 3f, view.Center.Y);

					view.Layer.BorderWidth = 4f;
				},
				() =>
				{
					view.Center = orignalCenter;
					view.Layer.BorderWidth = 2f;

				}
			);
		}

		public static void RemoveValidationBorder(UIView view)
		{
			view.Layer.BorderColor = UIColor.Clear.CGColor;
			view.Layer.BorderWidth = 0f;
		}

		public static bool ValidateRequiredRegistrationFieldExceptClassifications(string reEnterEmail, string reEnterPassword)
		{
			//Ensure required fields are input
			if (Settings.Email == null || Settings.Email == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input a valid email address");
				return false;
			}
			if (Settings.Password == null || Settings.Password == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input a valid password");
				return false;
			}

			if (Settings.Email != reEnterEmail)
			{
				HelperMethods.SendBasicAlert("Validation", "Email doesn't match");
				return false;
			}

			if (Settings.Password != reEnterPassword)
			{
				HelperMethods.SendBasicAlert("Validation", "Password doesn't match");
				return false;
			}


			if (Settings.FirstName == null || Settings.FirstName == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input your first name");
				return false;
			}
			if (Settings.LastName == null || Settings.LastName == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input your last name");
				return false;
			}
			if (Settings.Phone == null || Settings.Phone == string.Empty)
			{
				HelperMethods.SendBasicAlert("Validation", "Please input your phone number");
				return false;
			}

			if (Settings.LocationPickerSelectedId == 0)
			{
				HelperMethods.SendBasicAlert("Validation", "Please select your location");
				return false;
			}

			return true;

		}

		#endregion
	}
}
