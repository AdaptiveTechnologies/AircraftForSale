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
using AircraftForSale.PCL.Helpers;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class AdLayout3ViewController : UIViewController, IAdLayoutInterface
	{
		//async void AdRGButton_TouchUpInside(object sender, EventArgs e)
		//{
		//	Ad ad = new Ad();


		//	if (sender == Ad1RGButton)
		//	{
		//		ad = DataObject.Ads[0];
		//	}

		//	if (sender == Ad2RGButton)
		//	{
		//		ad = DataObject.Ads[1];
		//	}

		//	if (sender == Ad3RGButton)
		//	{
		//		ad = DataObject.Ads[2];
		//	}

		//	if (sender == Ad4RGButton)
		//	{
		//		ad = DataObject.Ads[3];
		//	}


		//	if (Reachability.IsHostReachable(Settings._baseDomain))
		//	{
		//		SpecResponse specResponse = new SpecResponse();

		//		var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

		//		if (specification.SpecId != 0)
		//		{
		//			//var specTableViewController = this.Storyboard.InstantiateViewController("SpecViewController_") as SpecViewController_;

		//			//specTableViewController.Spec = specification;

		//			//this.PresentViewController(specTableViewController, true, null);

		//			MapViewController mapViewController = (MapViewController)Storyboard.InstantiateViewController("MapViewController");
		//			mapViewController.SpecFieldList = specification.SpecFieldDictionary[SpecTableViewSource._rangeSection];
		//			ShowDetailViewController(mapViewController, this);
		//		}

			


		//	}

		//	else {

		//		HelperMethods.SendBasicAlert("Connect to a Network", Settings._networkProblemMessage);
		//	}
		//}

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
			if (sender == Ad3MessageButton)
			{
				ad = DataObject.Ads[2];
			}
			if (sender == Ad4MessageButton)
			{
				ad = DataObject.Ads[3];
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
				if (sender == Ad1NameButton || sender == Ad2NameButton || sender == Ad3NameButton || sender == Ad4NameButton)
				{
					HelperMethods.MakeModelRegistrationRequiredPrompt(this, sender as UIButton);
				}
				if (sender == Ad1BrokerButton || sender == Ad2BrokerButton || sender == Ad3BrokerButton || sender == Ad4BrokerButton)
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
			if (sender == Ad3NameButton)
			{
				ad = DataObject.Ads[2];
				isAdNameSort = true;
			}
			if (sender == Ad4NameButton)
			{
				ad = DataObject.Ads[3];
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
			if (sender == Ad3BrokerButton)
			{
				ad = DataObject.Ads[2];
				isAdNameSort = false;
			}
			if (sender == Ad4BrokerButton)
			{
				ad = DataObject.Ads[3];
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

				//get ads with this name and move them to the from of the list
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
		//async void TechnicalSpecButton_TouchUpInside(object sender, EventArgs e)
		//{
		//	Ad ad = new Ad();

		//	if (sender == Ad1TechnicalSpecButton)
		//	{
		//		ad = DataObject.Ads[0];
		//	}


		//	if (sender == Ad2TechnicalSpecButton)
		//	{
		//		ad = DataObject.Ads[1];
		//	}

		//	if (sender == Ad3TechnicalSpecButton)
		//	{
		//		ad = DataObject.Ads[2];
		//	}

		//	if (sender == Ad4TechnicalSpecButton)
		//	{
		//		ad = DataObject.Ads[3];
		//	}

		//	SpecResponse specResponse = new SpecResponse();

		//	var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

		//	if (specification.SpecId != 0)
		//	{
		//		var specTableViewController = this.Storyboard.InstantiateViewController("SpecViewController_") as SpecViewController_;

		//		specTableViewController.Spec = specification;

		//		this.PresentViewController(specTableViewController, true, null);
		//	}
		//	else
		//	{
		//		if (!Reachability.IsHostReachable(Settings._baseDomain))
		//		{
		//			var alert = UIAlertController.Create("Connect to a Network", "Please connect to a network to retrieve these aircraft specs", UIAlertControllerStyle.Alert);

		//			alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
		//			if (alert.PopoverPresentationController != null)
		//			{
		//				alert.PopoverPresentationController.SourceView = this.View;
		//				alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
		//			}
		//			PresentViewController(alert, animated: true, completionHandler: null);
		//		}
		//	}
		//}


		void TapImageAction1(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[0], this.View);
		}
		void TapImageAction2(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[1], this.View);
		}
		void TapImageAction3(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[2], this.View);
		}
		void TapImageAction4(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[3], this.View);
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

			if (senderButton == AdLike2)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[1]);
			}

			if (senderButton == AdLike3)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[2]);
			}
			if (senderButton == AdLike4)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[3]);
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

			if (senderButton == AdShare2)
			{
				message += DataObject.Ads[1].Name + "!";
				url = DataObject.Ads[1].AircraftForSaleURL;

			}

			if (senderButton == AdShare3)
			{
				message += DataObject.Ads[2].Name + "!";
				url = DataObject.Ads[2].AircraftForSaleURL;

			}

			if (senderButton == AdShare4)
			{
				message += DataObject.Ads[3].Name + "!";
				url = DataObject.Ads[3].AircraftForSaleURL;

			}

			var itemTitle = NSObject.FromObject(title);
			var itemMessage = NSObject.FromObject(message);
			var itemLink = NSObject.FromObject(url);
			var activityItems = new NSObject[] { itemLink, itemMessage, itemTitle,itemLink};
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

				if (senderButton == AdEmail2)
				{
					ad = DataObject.Ads[1];
				}

				if (senderButton == AdEmail3)
				{
					ad = DataObject.Ads[2];
				}
				if (senderButton == AdEmail4)
				{
					ad = DataObject.Ads[3];
				}
				emailInquiryViewController.AdProperty = ad;
				this.PresentViewController(emailInquiryViewController, true, null);
			}
			else {
				HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to send this email");
			}
		}
		public AdLayout3ViewController(IntPtr handle) : base(handle)
		{
		}

		//~AdLayout3ViewController()
		//{
		//	Console.WriteLine("AdLayout3ViewController is about to be collected");
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

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			//Hide MT Buttons
			//Ad1MTButton.Alpha = 0f;
			//Ad2MTButton.Alpha = 0f;
			//Ad3MTButton.Alpha = 0f;
			//Ad4MTButton.Alpha = 0f;

			var ad1 = DataObject.Ads[0];
			//var ad2 = DataObject.Ads[1];
			//var ad3 = DataObject.Ads[2];
			//var ad4 = DataObject.Ads[3];

			//Hide text button if broker doesn't have a cell phone
			if (string.IsNullOrEmpty(ad1.BrokerCellPhone))
			{
				Ad1MessageButton.Alpha = 0f;
			};
			//if (string.IsNullOrEmpty(ad2.BrokerCellPhone))
			//{
			//	Ad2MessageButton.Alpha = 0f;
			//};
			//if (string.IsNullOrEmpty(ad3.BrokerCellPhone))
			//{
			//	Ad3MessageButton.Alpha = 0f;
			//};
			//if (string.IsNullOrEmpty(ad4.BrokerCellPhone))
			//{
			//	Ad4MessageButton.Alpha = 0f;
			//};



			AdImage1.SetImage(
		url: new NSUrl(ad1.ImageURL),
		placeholder: UIImage.FromBundle("ad_placeholder.jpg")
	);

			AdName1.Text = ad1.Name;
			AdPrice1.Text = ad1.Price;



			Ad1NameButton.SetTitle(ad1.Name, UIControlState.Normal);


	//		AdImage2.SetImage(
	//	url: new NSUrl(ad2.ImageURL),
	//	placeholder: UIImage.FromBundle("ad_placeholder.jpg")
	//);

	//		AdName2.Text = ad2.Name;
	//		AdPrice2.Text = ad2.Price;


	//		Ad2NameButton.SetTitle(ad2.Name, UIControlState.Normal);


	//		AdImage3.SetImage(
	//	url: new NSUrl(ad3.ImageURL),
	//	placeholder: UIImage.FromBundle("ad_placeholder.jpg")
	//);

	//		AdName3.Text = ad3.Name;
	//		AdPrice3.Text = ad3.Price;


	//		Ad3NameButton.SetTitle(ad3.Name, UIControlState.Normal);


	//		AdImage4.SetImage(
	//	url: new NSUrl(ad4.ImageURL),
	//	placeholder: UIImage.FromBundle("ad_placeholder.jpg")
	//);

	//		AdName4.Text = ad4.Name;
	//		AdPrice4.Text = ad4.Price;


	//		Ad4NameButton.SetTitle(ad4.Name, UIControlState.Normal);

			var labelAttribute = new UIStringAttributes
			{
				Font = UIFont.BoldSystemFontOfSize(18)
			};
			var brokerLabelAttribute = new UIStringAttributes
			{
				Font = UIFont.BoldSystemFontOfSize(15)
			};

			var valueAttribute = new UIStringAttributes
			{
				Font = UIFont.SystemFontOfSize(15),
				ForegroundColor = UIColor.Blue
			};

			Ad1TeaserLabel.Text = ad1.Teaser == string.Empty ? "Inquire for Details" : ad1.Teaser;
			//Ad2TeaserLabel.Text = ad2.Teaser == string.Empty ? "Inquire for Details" : ad2.Teaser;
			//Ad3TeaserLabel.Text = ad3.Teaser == string.Empty ? "Inquire for Details" : ad3.Teaser;
			//Ad4TeaserLabel.Text = ad4.Teaser == string.Empty ? "Inquire for Details" : ad4.Teaser;

			#region attributed labels
			Ad1BrokerButton.SetAttributedTitle(HelperMethods.GetBrokerAttributedString(ad1, brokerLabelAttribute, valueAttribute), UIControlState.Normal);
			//Ad2BrokerButton.SetAttributedTitle(HelperMethods.GetBrokerAttributedString(ad2, brokerLabelAttribute, valueAttribute), UIControlState.Normal);
			//Ad3BrokerButton.SetAttributedTitle(HelperMethods.GetBrokerAttributedString(ad3, brokerLabelAttribute, valueAttribute), UIControlState.Normal);
			//Ad4BrokerButton.SetAttributedTitle(HelperMethods.GetBrokerAttributedString(ad4, brokerLabelAttribute, valueAttribute), UIControlState.Normal);

			Ad1RegistrationLabel.AttributedText = HelperMethods.GetRegistrationAttributedString(ad1, labelAttribute);
			//Ad2RegistrationLabel.AttributedText = HelperMethods.GetRegistrationAttributedString(ad2, labelAttribute);
			//Ad3RegistrationLabel.AttributedText = HelperMethods.GetRegistrationAttributedString(ad3, labelAttribute);
			//Ad4RegistrationLabel.AttributedText = HelperMethods.GetRegistrationAttributedString(ad4, labelAttribute);

			Ad1SerialLabel.AttributedText = HelperMethods.GetSerialAttributedString(ad1, labelAttribute);
			//Ad2SerialLabel.AttributedText = HelperMethods.GetSerialAttributedString(ad2, labelAttribute);
			//Ad3SerialLabel.AttributedText = HelperMethods.GetSerialAttributedString(ad3, labelAttribute);
			//Ad4SerialLabel.AttributedText = HelperMethods.GetSerialAttributedString(ad4, labelAttribute);

			Ad1TimeLabel.AttributedText = HelperMethods.GetTotalTimeAttributedString(ad1, labelAttribute);
			//Ad2TimeLabel.AttributedText = HelperMethods.GetTotalTimeAttributedString(ad2, labelAttribute);
			//Ad3TimeLabel.AttributedText = HelperMethods.GetTotalTimeAttributedString(ad3, labelAttribute);
			//Ad4TimeLabel.AttributedText = HelperMethods.GetTotalTimeAttributedString(ad4, labelAttribute);

			Ad1LocationLabel.AttributedText = HelperMethods.GetLocationAttributedString(ad1, labelAttribute);
			//Ad2LocationLabel.AttributedText = HelperMethods.GetLocationAttributedString(ad2, labelAttribute);
			//Ad3LocationLabel.AttributedText = HelperMethods.GetLocationAttributedString(ad3, labelAttribute);
			//Ad4LocationLabel.AttributedText = HelperMethods.GetLocationAttributedString(ad4, labelAttribute);
			#endregion

			#region price changed
			if (!HelperMethods.ShowPriceChangedLabel(ad1.PriceLastUpdated))
			{
				Ad1PriceChangeLabel.Alpha = 0f;

			}
			//if (!HelperMethods.ShowPriceChangedLabel(ad2.PriceLastUpdated))
			//{
			//	Ad2PriceChangeLabel.Alpha = 0f;

			//}
			//if (!HelperMethods.ShowPriceChangedLabel(ad3.PriceLastUpdated))
			//{
			//	Ad3PriceChangeLabel.Alpha = 0f;

			//}
			//if (!HelperMethods.ShowPriceChangedLabel(ad4.PriceLastUpdated))
			//{
			//	Ad4PriceChangeLabel.Alpha = 0f;
			//}
			#endregion




			//Set initial state of like buttons
			if (DataObject.Ads[0].IsLiked)
			{
				HelperMethods.SetInitialLikeButtonState(AdLike1, DataObject.Ads[0]);
			}

			//if (DataObject.Ads[1].IsLiked)
			//{
			//	HelperMethods.SetInitialLikeButtonState(AdLike2, DataObject.Ads[1]);
			//}

			//if (DataObject.Ads[2].IsLiked)
			//{
			//	HelperMethods.SetInitialLikeButtonState(AdLike3, DataObject.Ads[2]);
			//}

			//if (DataObject.Ads[3].IsLiked)
			//{
			//	HelperMethods.SetInitialLikeButtonState(AdLike4, DataObject.Ads[3]);
			//}

			//webview image tap gestures
			AdImage1.UserInteractionEnabled = true;
			AdImage2.UserInteractionEnabled = true;
			AdImage3.UserInteractionEnabled = true;
			AdImage4.UserInteractionEnabled = true;

			//UITapGestureRecognizer tapGesture1 = new UITapGestureRecognizer(TapImageAction1);
			//AdImage1.AddGestureRecognizer(tapGesture1);
			//UITapGestureRecognizer tapGesture2 = new UITapGestureRecognizer(TapImageAction2);
			//AdImage2.AddGestureRecognizer(tapGesture2);
			//UITapGestureRecognizer tapGesture3 = new UITapGestureRecognizer(TapImageAction3);
			//AdImage3.AddGestureRecognizer(tapGesture3);
			//UITapGestureRecognizer tapGesture4 = new UITapGestureRecognizer(TapImageAction4);
			//AdImage4.AddGestureRecognizer(tapGesture4);

			tapGesture1 = new UITapGestureRecognizer(TapImageAction1);
			tapGesture2 = new UITapGestureRecognizer(TapImageAction2);
			tapGesture3 = new UITapGestureRecognizer(TapImageAction3);
			tapGesture4 = new UITapGestureRecognizer(TapImageAction4);

			int currentIndex = this.DataObject.MagazinePageIndex;
			int totalPages = this.DataObject.TotalPages;

			PageIndicator.SetIndicatorState(currentIndex, totalPages);

			//Randomly load banmanpro
			//HelperMethods.LoadWebBanManPro(this.View);
		}

		UITapGestureRecognizer tapGesture1;
		UITapGestureRecognizer tapGesture2;
		UITapGestureRecognizer tapGesture3;
		UITapGestureRecognizer tapGesture4;

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			//like, share, and email broker button event wiring

			AdEmail1.TouchUpInside += SendEmail_TouchUpInside;
			AdEmail2.TouchUpInside += SendEmail_TouchUpInside;
			AdEmail3.TouchUpInside += SendEmail_TouchUpInside;
			AdEmail4.TouchUpInside += SendEmail_TouchUpInside;

			AdShare1.TouchUpInside += AdShare_TouchUpInside;
			AdShare2.TouchUpInside += AdShare_TouchUpInside;
			AdShare3.TouchUpInside += AdShare_TouchUpInside;
			AdShare4.TouchUpInside += AdShare_TouchUpInside;

			AdLike1.TouchUpInside += AdLike_TouchUpInside;
			AdLike2.TouchUpInside += AdLike_TouchUpInside;
			AdLike3.TouchUpInside += AdLike_TouchUpInside;
			AdLike4.TouchUpInside += AdLike_TouchUpInside;

			//Ad1TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;
			//Ad2TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;
			//Ad3TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;
			//Ad4TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;

			Ad1NameButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad2NameButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad3NameButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad4NameButton.TouchUpInside += AdSortButton_TouchUpInside;

			Ad1BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad2BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad3BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad4BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;

			AdImage1.AddGestureRecognizer(tapGesture1);
			AdImage2.AddGestureRecognizer(tapGesture2);
			AdImage3.AddGestureRecognizer(tapGesture3);
			AdImage4.AddGestureRecognizer(tapGesture4);

			Ad1MessageButton.TouchUpInside += AdMessages_TouchUpInside;
			Ad2MessageButton.TouchUpInside += AdMessages_TouchUpInside;
			Ad3MessageButton.TouchUpInside += AdMessages_TouchUpInside;
			Ad4MessageButton.TouchUpInside += AdMessages_TouchUpInside;

			//Ad1RGButton.TouchUpInside += AdRGButton_TouchUpInside;
			//Ad2RGButton.TouchUpInside += AdRGButton_TouchUpInside;
			//Ad3RGButton.TouchUpInside += AdRGButton_TouchUpInside;
			//Ad4RGButton.TouchUpInside += AdRGButton_TouchUpInside;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			//like, share, and email broker button event wiring

			AdEmail1.TouchUpInside -= SendEmail_TouchUpInside;
			AdEmail2.TouchUpInside -= SendEmail_TouchUpInside;
			AdEmail3.TouchUpInside -= SendEmail_TouchUpInside;
			AdEmail4.TouchUpInside -= SendEmail_TouchUpInside;

			AdShare1.TouchUpInside -= AdShare_TouchUpInside;
			AdShare2.TouchUpInside -= AdShare_TouchUpInside;
			AdShare3.TouchUpInside -= AdShare_TouchUpInside;
			AdShare4.TouchUpInside -= AdShare_TouchUpInside;

			AdLike1.TouchUpInside -= AdLike_TouchUpInside;
			AdLike2.TouchUpInside -= AdLike_TouchUpInside;
			AdLike3.TouchUpInside -= AdLike_TouchUpInside;
			AdLike4.TouchUpInside -= AdLike_TouchUpInside;

			//Ad1TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;
			//Ad2TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;
			//Ad3TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;
			//Ad4TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;

			Ad1NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad2NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad3NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad4NameButton.TouchUpInside -= AdSortButton_TouchUpInside;

			Ad1BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad2BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad3BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad4BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;

			AdImage1.RemoveGestureRecognizer(tapGesture1);
			AdImage2.RemoveGestureRecognizer(tapGesture2);
			AdImage3.RemoveGestureRecognizer(tapGesture3);
			AdImage4.RemoveGestureRecognizer(tapGesture4);

			Ad1MessageButton.TouchUpInside -= AdMessages_TouchUpInside;
			Ad2MessageButton.TouchUpInside -= AdMessages_TouchUpInside;
			Ad3MessageButton.TouchUpInside -= AdMessages_TouchUpInside;
			Ad4MessageButton.TouchUpInside -= AdMessages_TouchUpInside;

			//Ad1RGButton.TouchUpInside -= AdRGButton_TouchUpInside;
			//Ad2RGButton.TouchUpInside -= AdRGButton_TouchUpInside;
			//Ad3RGButton.TouchUpInside -= AdRGButton_TouchUpInside;
			//Ad4RGButton.TouchUpInside -= AdRGButton_TouchUpInside;
		}
	}
}