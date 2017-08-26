using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using MessageUI;
using AircraftForSale.PCL;
using SDWebImage;
using System.Threading.Tasks;
using System.Linq;
using ObjCRuntime;
using AircraftForSale.PCL.Helpers;

using Google.Analytics;

namespace AircraftForSale
{
	public partial class AdLayout2ViewController : UIViewController, IAdLayoutInterface
	{
       void Ad2DetailButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad2DetailView.Hidden = !Ad2DetailView.Hidden;
			Ad2DetailButton.SetTitle(Ad2DetailView.Hidden?"Detail":"Hide Detail", UIControlState.Normal);
		}

		 void Ad1DetailButton_TouchUpInside(object sender, EventArgs e)
		{

			 Ad1DetailView.Hidden = !Ad1DetailView.Hidden;
			Ad1DetailButton.SetTitle(Ad1DetailView.Hidden?"Detail":"Hide Detail", UIControlState.Normal);

		}

		async void AdRGButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();
			if (sender == Ad1RGButton)
			{
				ad = DataObject.Ads[0];
			}

			if (sender == Ad2RGButton)
			{
				ad = DataObject.Ads[1];
			}

			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				SpecResponse specResponse = new SpecResponse();

				var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

				if (specification.SpecId != 0)
				{
					//var specTableViewController = this.Storyboard.InstantiateViewController("SpecViewController_") as SpecViewController_;

					//specTableViewController.Spec = specification;

					//this.PresentViewController(specTableViewController, true, null);

					MapViewController mapViewController = (MapViewController)Storyboard.InstantiateViewController("MapViewController");
					mapViewController.SpecFieldList = specification.SpecFieldDictionary[SpecTableViewSource._rangeSection];
					ShowDetailViewController(mapViewController, this);
				}
		

			}

			else {
				
				HelperMethods.SendBasicAlert("Connect to a Network", Settings._networkProblemMessage);
			}
		}
		void AdMessages_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();

			if (sender == Ad1MessageButton)
			{
				ad = DataObject.Ads[0];
			}
			if (sender == Ad2MessageButton)
			{
				ad = DataObject.Ads[1];
			}


			var brokerPhoneNumber = ad.BrokerCellPhone;
			var smsTo = NSUrl.FromString("sms:" + brokerPhoneNumber);
			if (UIApplication.SharedApplication.CanOpenUrl(smsTo))
			{
				string textMessageBody = "Inquiry about " + ad.Name + " from GlobalAir.com Showcase Magazine";

				Action successCallBack = async () =>
				{
					//Send Inquiry
					var response = await AdInquiryResponse.AdInquiry(int.Parse(ad.ID), string.Empty, string.Empty, brokerPhoneNumber, string.Empty
															   , ad.BrokerId, AdInquirySource.Text, textMessageBody);
					if (response.Status != "Success")
					{
						HelperMethods.SendBasicAlert("Oops", "There was a problem sending your email to the aircraft broker. Please try again");
					}

				};

				//try to send text message in the ap
				HelperMethods.ShowAndSendSMS(this, new string[] { brokerPhoneNumber }, textMessageBody, successCallBack);

			}
			else {
				var av = new UIAlertView("Not supported",
				  "Text messaging is not supported on this device",
				  null,
				  "OK",
				  null);
				av.Show();
			}

		}
		void AdSortButton_TouchUpInside(object sender, EventArgs e)
		{
			if (!Settings.IsRegistered)
			{
				if (sender == Ad1NameButton || sender == Ad2NameButton)
				{
					HelperMethods.MakeModelRegistrationRequiredPrompt(this, sender as UIButton);
				}
				if (sender == Ad1BrokerButton || sender == Ad2BrokerButton)
				{
					HelperMethods.SellerRegistrationRequiredPrompt(this, sender as UIButton);
				}
				return;
			}

			Ad ad = new Ad();
			bool isAdNameSort = false;
			if (sender == Ad1NameButton)
			{
				ad = DataObject.Ads[0];
				isAdNameSort = true;
			}
			if (sender == Ad2NameButton)
			{
				ad = DataObject.Ads[1];
				isAdNameSort = true;
			}



			if (sender == Ad1BrokerButton)
			{
				ad = DataObject.Ads[0];
				isAdNameSort = false;
			}
			if (sender == Ad2BrokerButton)
			{
				ad = DataObject.Ads[1];
				isAdNameSort = false;
			}



            LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, isAdNameSort ? "Loading Aircraft by Selected Type" : "Loading Aircraft by Selected Broker");
			this.View.AddSubview(loadingIndicator);


			var pageViewController = this.ParentViewController as UIPageViewController;
			var magFlipBoardViewController = pageViewController.ParentViewController as MagazineFlipBoardViewController;

			var modelController = magFlipBoardViewController.ModelController;
			List<Ad> adList = new List<Ad>();

			Task.Run(async () =>
			{

				adList = (await Ad.GetAdsByClassificationAsync(DataObject.SelectedClassification)).ToList();

				//get ads with this name and move them to the from of the liste
				List<Ad> similarAdList = new List<Ad>();

				if (isAdNameSort)
				{
					similarAdList = adList.Where(row => row.Name == ad.Name).ToList();
				}
				else {
					similarAdList = adList.Where(row => row.BrokerName == ad.BrokerName).ToList();
				}


				for (int i = 0; i < similarAdList.Count(); i++)
				{
					adList.Remove(similarAdList[i]);
					adList.Insert(0, similarAdList[i]);
				}

				InvokeOnMainThread(() =>
				{
					modelController.LoadModalController(adList, DataObject.SelectedClassification);
					loadingIndicator.Hide();
					var startingViewController = modelController.GetViewController(0, false);
					var viewControllers = new UIViewController[] { startingViewController };
					pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

					HelperMethods.SendBasicAlert("","Aircraft arranged by "+ (isAdNameSort? ad.Name:ad.BrokerName));
				});
			});
		}




		void AdSlideUpButton_TouchUpInside(object sender, EventArgs e)
		{
			if (sender == Ad1DetailButton)
			{
				
			}

			if (sender == Ad2DetailButton)
			{
				
			}
		}

		bool isAd1ImageUp;

		bool isAd2ImageUp;

		NSLayoutConstraint ad1ImageOriginalConstraint;
		NSLayoutConstraint ad1ImageNewConstraint;

		NSLayoutConstraint ad2ImageOriginalConstraint;
		NSLayoutConstraint ad2ImageNewConstraint;

		async void TechnicalSpecButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();

			if (sender == Ad1TechnicalSpecButton)
			{
				ad = DataObject.Ads[0];
			}




			if (sender == Ad2TechnicalSpecButton)
			{
				ad = DataObject.Ads[1];
			}


			SpecResponse specResponse = new SpecResponse();

			var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

			if (specification.SpecId != 0)
			{
				var specTableViewController = this.Storyboard.InstantiateViewController("SpecViewController_") as SpecViewController_;

				specTableViewController.Spec = specification;

				this.PresentViewController(specTableViewController, true, null);
			}
			else
			{
				if (!Reachability.IsHostReachable(Settings._baseDomain))
				{
					var alert = UIAlertController.Create("Connect to a Network", "Please connect to a network to retrieve these aircraft specs", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
					//alert.AddAction(UIAlertAction.Create("Snooze", UIAlertActionStyle.Default, action => Snooze()));
					if (alert.PopoverPresentationController != null)
					{
						alert.PopoverPresentationController.SourceView = this.View;
						alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
					}
					PresentViewController(alert, animated: true, completionHandler: null);
				}
			}
		}


		void TapImageAction1(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[0], this.View);
		}

		void TapImageAction2(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[1], this.View);
		}
		void AdLike_TouchUpInside(object sender, EventArgs e)
		{
						if (!Settings.IsRegistered)
			{
				HelperMethods.LikeRegistrationRequiredPrompt(this, sender as UIButton);
				return;
			}

			UIButton senderButton = sender as UIButton;
			if (senderButton == AdLike1)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[0]);
			}



			if (senderButton == Ad2LikeButton)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[1]);
			}

		}

		void AdShare_TouchUpInside(object sender, EventArgs e)
		{
			UIButton senderButton = sender as UIButton;
			var title = "GlobalAir.com Showcase Magazine";
			var message = "Check out ";
			var url = string.Empty;

			if (senderButton == AdShare1)
			{
				message += DataObject.Ads[0].Name + "!";
				url = DataObject.Ads[0].AircraftForSaleURL;

			}



			if (senderButton == Ad2ShareButton)
			{
				message += DataObject.Ads[1].Name + "!";
				url = DataObject.Ads[1].AircraftForSaleURL;

			}

			var itemTitle = NSObject.FromObject(title);
			var itemMessage = NSObject.FromObject(message);
			var itemLink = NSObject.FromObject(url);
			var activityItems = new NSObject[] { itemLink, itemMessage, itemTitle ,itemLink};
			UIActivity[] applicationActivities = null;

			var activityController = new UIActivityViewController(activityItems, applicationActivities);
			activityController.PopoverPresentationController.SourceView = senderButton;
			activityController.PopoverPresentationController.SourceRect = senderButton.Bounds;

			PresentViewController(activityController, true, null);

		}

		void SendEmail_TouchUpInside(object sender, EventArgs args)
		{
			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				UIButton senderButton = sender as UIButton;

				EmailInquiryViewController emailInquiryViewController = (EmailInquiryViewController)Storyboard.InstantiateViewController("EmailInquiryViewController");
				Ad ad = new Ad();
				if (senderButton == AdEmail1)
				{
					ad = DataObject.Ads[0];
				}

				if (senderButton == Ad2EmailButton)
				{
					ad = DataObject.Ads[1];
				}

				emailInquiryViewController.AdProperty = ad;
				this.PresentViewController(emailInquiryViewController, true, null);
			}
			else {
				HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to send this email");
			}
		}
		public AdLayout2ViewController(IntPtr handle) : base(handle)
		{

		}

		//~AdLayout2ViewController()
		//{
		//	Console.WriteLine("AdLayout2ViewController is about to be collected");
		//}

		//public List<Ad> AdList
		//{
		//	get; set;
		//}

			protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ReleaseDesignerOutlets();
			}
		}

		WeakReference parent;
		public MagazinePage DataObject
		{
			get
			{
				return parent.Target as MagazinePage;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}



		//MFMailComposeViewController mailController;

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//Hide MT Buttons
			//Ad1MTButton.Alpha = 0f;
			//Ad2MTButton.Alpha = 0f;

			var ad1 = DataObject.Ads[0];
			var ad2 = DataObject.Ads[1];

			//Hide text button if broker doesn't have a cell phone
			if (string.IsNullOrEmpty(ad1.BrokerCellPhone))
			{
				Ad1MessageButton.Alpha = 0f;
			};
			if (string.IsNullOrEmpty(ad2.BrokerCellPhone))
			{
				Ad2MessageButton.Alpha = 0f;
			};



			AdImage1.SetImage(
		url: new NSUrl(ad1.ImageURL),
		placeholder: UIImage.FromBundle("ad_placeholder.jpg")
	);

			AdName1.Text = ad1.Name;
			AdPrice1.Text = ad1.Price;

			if (ad1.Price.Length == 0)
				AdPrice1.Text = "Call";


			Ad1NameButton.SetTitle(ad1.Name, UIControlState.Normal);



			Ad2Image.SetImage(
		url: new NSUrl(ad2.ImageURL),
		placeholder: UIImage.FromBundle("ad_placeholder.jpg")
	);

			Ad2NameLabel.Text = ad2.Name;
			Ad2PriceLabel.Text = ad2.Price;
			if (ad2.Price.Length == 0)
				Ad2PriceLabel.Text = "Call";

			Ad2NameButton.SetTitle(ad2.Name, UIControlState.Normal);


			var labelAttribute = new UIStringAttributes
			{
				Font = UIFont.BoldSystemFontOfSize(18)
			};

			Ad1TeaserLabel.Text = ad1.Teaser == string.Empty ? "Inquire for Details" : ad1.Teaser;
			Ad2TeaserLabel.Text = ad2.Teaser == string.Empty ? "Inquire for Details" : ad2.Teaser;

			//Ad1BrokerButton.SetTitle("" + ad1.BrokerName, UIControlState.Normal);
			//Ad2BrokerButton.SetTitle("" + ad2.BrokerName, UIControlState.Normal);

			Ad1BrokerLabel.Text = ad1.BrokerName;
			Ad2BrokerLabel.Text = ad2.BrokerName;

			#region attributed labels
			Ad1RegistrationLabel.AttributedText = HelperMethods.GetRegistrationAttributedString(ad1, labelAttribute);
			Ad2RegistrationLabel.AttributedText = HelperMethods.GetRegistrationAttributedString(ad2, labelAttribute);

			Ad1SerialLabel.AttributedText = HelperMethods.GetSerialAttributedString(ad1, labelAttribute);
			Ad2SerialLabel.AttributedText = HelperMethods.GetSerialAttributedString(ad2, labelAttribute);

			Ad1TimeLabel.AttributedText = HelperMethods.GetTotalTimeAttributedString(ad1, labelAttribute);
			Ad2TimeLabel.AttributedText = HelperMethods.GetTotalTimeAttributedString(ad2, labelAttribute);

			Ad1LocationLabel.AttributedText = HelperMethods.GetLocationAttributedString(ad1, labelAttribute);
			Ad2LocationLabel.AttributedText = HelperMethods.GetLocationAttributedString(ad2, labelAttribute);
			#endregion

			#region price changed
			if (!HelperMethods.ShowPriceChangedLabel(ad1.PriceLastUpdated))
			{
				Ad1PriceChangeLabel.Alpha = 0f;

			}
			if (!HelperMethods.ShowPriceChangedLabel(ad2.PriceLastUpdated))
			{
				Ad2PriceChangeLabel.Alpha = 0f;

			}
			#endregion




			//Set initial state of like buttons
			if (DataObject.Ads[0].IsLiked)
			{
				HelperMethods.SetInitialLikeButtonState(AdLike1, DataObject.Ads[0]);
			}



			if (DataObject.Ads[1].IsLiked)
			{
				HelperMethods.SetInitialLikeButtonState(Ad2LikeButton, DataObject.Ads[1]);
			}

			//webview image tap gestures
			AdImage1.UserInteractionEnabled = true;
			Ad2Image.UserInteractionEnabled = true;

			//UITapGestureRecognizer tapGesture1 = new UITapGestureRecognizer(TapImageAction1);
			//AdImage1.AddGestureRecognizer(tapGesture1);

			//UITapGestureRecognizer tapGesture2 = new UITapGestureRecognizer(TapImageAction2);
			//Ad2Image.AddGestureRecognizer(tapGesture2);

			tapGesture1 = new UITapGestureRecognizer(TapImageAction1);
			tapGesture2 = new UITapGestureRecognizer(TapImageAction2);

			int currentIndex = this.DataObject.MagazinePageIndex;
			int totalPages = this.DataObject.TotalPages;

			PageIndicator.SetIndicatorState(currentIndex, totalPages);


			//ad1ImageOriginalConstraint = Ad1AspectRatioConstraint;
			//ad2ImageOriginalConstraint = Ad2AspectRatioConstraint;

			ad1ImageNewConstraint = NSLayoutConstraint.Create(AdImage1, NSLayoutAttribute.Height, NSLayoutRelation.Equal, AdImage1, NSLayoutAttribute.Width, 2f / 4f, 0);
			ad2ImageNewConstraint = NSLayoutConstraint.Create(Ad2Image, NSLayoutAttribute.Height, NSLayoutRelation.Equal, Ad2Image, NSLayoutAttribute.Width, 2f / 4f, 0);

			//Randomly load banmanpro
			//HelperMethods.LoadWebBanManPro(this.View);
		}

		UITapGestureRecognizer tapGesture1;
		UITapGestureRecognizer tapGesture2;

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			//like, share, and email broker button event wiring

			AdEmail1.TouchUpInside += SendEmail_TouchUpInside;
			Ad2EmailButton.TouchUpInside += SendEmail_TouchUpInside;

			AdShare1.TouchUpInside += AdShare_TouchUpInside;
			Ad2ShareButton.TouchUpInside += AdShare_TouchUpInside;

			AdLike1.TouchUpInside += AdLike_TouchUpInside;
			Ad2LikeButton.TouchUpInside += AdLike_TouchUpInside;

			Ad1TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;
			Ad2TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;

			Ad1NameButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad2NameButton.TouchUpInside += AdSortButton_TouchUpInside;


			Ad1DetailButton.TouchUpInside += Ad1DetailButton_TouchUpInside;
			Ad2DetailButton.TouchUpInside += Ad2DetailButton_TouchUpInside;




			Ad1BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad2BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;

			AdImage1.AddGestureRecognizer(tapGesture1);
			Ad2Image.AddGestureRecognizer(tapGesture2);

			Ad1MessageButton.TouchUpInside += AdMessages_TouchUpInside;
			Ad2MessageButton.TouchUpInside += AdMessages_TouchUpInside;

			Ad1RGButton.TouchUpInside += AdRGButton_TouchUpInside;
			Ad2RGButton.TouchUpInside += AdRGButton_TouchUpInside;
	
		}




		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			//like, share, and email broker button event wiring

			AdEmail1.TouchUpInside -= SendEmail_TouchUpInside;
			Ad2EmailButton.TouchUpInside -= SendEmail_TouchUpInside;

			AdShare1.TouchUpInside -= AdShare_TouchUpInside;
			Ad2ShareButton.TouchUpInside -= AdShare_TouchUpInside;

			Ad1DetailButton.TouchUpInside -= Ad1DetailButton_TouchUpInside;
			Ad2DetailButton.TouchUpInside -= Ad2DetailButton_TouchUpInside;

			AdLike1.TouchUpInside -= AdLike_TouchUpInside;
			Ad2LikeButton.TouchUpInside -= AdLike_TouchUpInside;

			Ad1TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;
			Ad2TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;

			Ad1NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad2NameButton.TouchUpInside -= AdSortButton_TouchUpInside;



			Ad1BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad2BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;

			AdImage1.RemoveGestureRecognizer(tapGesture1);
			Ad2Image.RemoveGestureRecognizer(tapGesture2);

			Ad1MessageButton.TouchUpInside -= AdMessages_TouchUpInside;
			Ad2MessageButton.TouchUpInside -= AdMessages_TouchUpInside;

			Ad1RGButton.TouchUpInside -= AdRGButton_TouchUpInside;
			Ad2RGButton.TouchUpInside -= AdRGButton_TouchUpInside;

		}
	}
}