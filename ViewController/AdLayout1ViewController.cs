using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using MessageUI;
using AircraftForSale.PCL;
using System.Drawing;
using AircraftForSale.PCL.Helpers;
using System.Linq;
using SDWebImage;
using System.Threading.Tasks;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class AdLayout1ViewController : UIViewController, IAdLayoutInterface
	{
		async void AdRGButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();

			if (sender == Ad1RGButton)
			{
				ad = DataObject.Ads[2];
			}
         
			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				SpecResponse specResponse = new SpecResponse();

				var specification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);

				if (specification.SpecId != 0)
				{
               
					MapViewController mapViewController = (MapViewController)Storyboard.InstantiateViewController("MapViewController");
					mapViewController.SpecFieldList = specification.SpecFieldDictionary[SpecTableViewSource._rangeSection];
					ShowDetailViewController(mapViewController, this);
				}


			}

			else
			{
				HelperMethods.SendBasicAlert("Connect to a Network", Settings._networkProblemMessage);
			}
		}
      
		void Ad1DetailButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad1DetailView.Hidden = !Ad1DetailView.Hidden;

			Ad1DetailButton.SetTitle(Ad1DetailView.Hidden ? "Detail" : "Hide Detail", UIControlState.Normal);
		}
      
		void AdMessages_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();

			if (sender == Ad1MessageButton)
			{
				ad = DataObject.Ads[2];
			}
         
			var brokerPhoneNumber = ad.BrokerCellPhone;
			//var brokerPhoneNumber = "5024171595";

			var smsTo = NSUrl.FromString("sms:" + brokerPhoneNumber);
			if (UIApplication.SharedApplication.CanOpenUrl(smsTo))
			{
                //Clay Martin 1/1/18: Change app name to BuyPlane
				string textMessageBody = "Inquiry about " + ad.Name + " from GlobalAir.com BuyPlane Magazine";

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
			else
			{
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
				if (sender == Ad1NameButton || sender == Ad2NameButton || sender == Ad3NameButton)
				{
					HelperMethods.MakeModelRegistrationRequiredPrompt(this, sender as UIButton);
				}
				if (sender == Ad1BrokerButton || sender == Ad3BrokerButton)
				{
					HelperMethods.SellerRegistrationRequiredPrompt(this, sender as UIButton);
				}
				return;
			}

			var pageViewController = this.ParentViewController as UIPageViewController;
			var magFlipBoardViewController = pageViewController.ParentViewController as MagazineFlipBoardViewController;



			Ad ad = new Ad();
			bool isAdNameSort = false;
			if (sender == Ad1NameButton)
			{
				ad = DataObject.Ads[2];
				isAdNameSort = true;
			}
			if (sender == Ad2NameButton)
			{
				ad = DataObject.Ads[1];
				isAdNameSort = true;
			}
			if (sender == Ad3NameButton)
			{
				ad = DataObject.Ads[0];
				isAdNameSort = true;
			}


			if (sender == Ad1BrokerButton)
			{
				ad = DataObject.Ads[2];
				isAdNameSort = false;
			}
         
			LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, isAdNameSort ? "Loading Aircraft by Selected Type" : "Loading Aircraft by Selected Broker");
			this.View.AddSubview(loadingIndicator);

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
				else
				{
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

					HelperMethods.SendBasicAlert("", "Aircraft arranged by " + (isAdNameSort ? ad.Name : ad.BrokerName));
				});
			});
		}

		async void TechnicalSpecButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();

			if (sender == Ad1TechnicalSpecButton)
			{

				ad = DataObject.Ads[DataObject.Ads.Count - 1];
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
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[2], this.View);
		}

		void TapImageAction2(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[1], this.View);
		}

		void TapImageAction3(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[0], this.View);
		}

		void AdLike_TouchUpInside(object sender, EventArgs e)
		{
			UIButton senderButton = sender as UIButton;

			if (!Settings.IsRegistered)
			{
				HelperMethods.LikeRegistrationRequiredPrompt(this, sender as UIButton);
				return;
			}

			if (senderButton == AdLike1)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[2]);
			}

			if (senderButton == AdLike2)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[1]);
			}

			if (senderButton == AdLike3)
			{
				HelperMethods.SwapLikeButtonState(senderButton, DataObject.Ads[0]);
			}

		}



		void AdShare_TouchUpInside(object sender, EventArgs e)
		{
			UIButton senderButton = sender as UIButton;
            //Clay Martin 1/1/18: Change app name to BuyPlane
			var title = "GlobalAir.com BuyPlane Magazine";
			var message = "Check out ";
			var url = string.Empty;

			if (senderButton == AdShare1)
			{
				message += DataObject.Ads[2].Name + "!";
				url = DataObject.Ads[2].AircraftForSaleURL;

			}

			if (senderButton == AdShare2)
			{
				message += DataObject.Ads[1].Name + "!";
				url = DataObject.Ads[1].AircraftForSaleURL;

			}

			if (senderButton == AdShare3)
			{
				message += DataObject.Ads[0].Name + "!";
				url = DataObject.Ads[0].AircraftForSaleURL;

			}

			var itemTitle = NSObject.FromObject(title);
			var itemMessage = NSObject.FromObject(message);
			var itemLink = NSObject.FromObject(url);
			var activityItems = new NSObject[] { itemLink, itemMessage, itemTitle, itemLink };
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
					ad = DataObject.Ads[2];
				}

				if (senderButton == AdEmail2)
				{
					ad = DataObject.Ads[1];
				}

				if (senderButton == AdEmail3)
				{
					ad = DataObject.Ads[0];
				}
				emailInquiryViewController.AdProperty = ad;
				this.PresentViewController(emailInquiryViewController, true, null);
			}
			else
			{
				HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to send this email");
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

		public AdLayout1ViewController(IntPtr handle) : base(handle)
		{


		}

		//~AdLayout1ViewController()
		//{
		//  Console.WriteLine("AdLayout1ViewController is about to be collected");
		//}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ReleaseDesignerOutlets();
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
         
			var ad3 = DataObject.Ads[0];
			var ad2 = DataObject.Ads[1];
			var ad1 = DataObject.Ads[2];

			//Hide text button if broker doesn't have a cell phone
			if (string.IsNullOrEmpty(ad1.BrokerCellPhone))
			{
				Ad1MessageButton.Alpha = 0f;
			};
			if (string.IsNullOrEmpty(ad2.BrokerCellPhone))
			{
				Ad2MessageButton.Alpha = 0f;
			};
			if (string.IsNullOrEmpty(ad3.BrokerCellPhone))
			{
				Ad3MessageButton.Alpha = 0f;
			};
            

			AdImage1.SetImage(
				url: new NSUrl(ad1.ImageURL),
				placeholder: UIImage.FromBundle("ad_placeholder.jpg")
			);

			Ad1BrokerButton.SetTitle("", UIControlState.Normal);
			Ad1TeaserLabel.Text = ad1.Teaser == string.Empty ? "Inquire for Details" : ad1.Teaser;
			AdPrice1.Text = (ad1.Price.Length == 0 ? "Call" : ad1.Price);
         
			var ad2AircraftDetails = new AircraftDetails[]{
				new AircraftDetails("", "Technical Specifications", false, true, false),
				new AircraftDetails("", "Aircraft Range", false, false, true),
				new AircraftDetails("Price:", (ad2.Price.Length == 0 ? "Call" : ad2.Price), false),
				new AircraftDetails("Offered by:", ad2.BrokerName, true),
				new AircraftDetails("Registration:", ad2.RegistrationNumber, false),
				new AircraftDetails("Serial Number:", ad2.SerialNumber, false),
				new AircraftDetails("Time:", ad2.TotalTime, false),
				new AircraftDetails("Location:", ad2.Location, false),
                new AircraftDetails("Summary:", ad2.Teaser == string.Empty ? "Inquire for Details" : ad2.Teaser, false),
            };

			var ad3AircraftDetails = new AircraftDetails[]{
                new AircraftDetails("", "Technical Specifications", false, true, false),
                new AircraftDetails("", "Aircraft Range", false, false, true),
                new AircraftDetails("Price:", (ad3.Price.Length == 0 ? "Call" : ad3.Price), false),
                new AircraftDetails("Offered by:", ad3.BrokerName, true),
                new AircraftDetails("Registration:", ad3.RegistrationNumber, false),
                new AircraftDetails("Serial Number:", ad3.SerialNumber, false),
                new AircraftDetails("Time:", ad3.TotalTime, false),
                new AircraftDetails("Location:", ad3.Location, false),
                new AircraftDetails("Summary:", ad3.Teaser == string.Empty ? "Inquire for Details" : ad3.Teaser, false),
            };

			Ad2TableView.Source = new AircraftDetailsTableSource(ad2AircraftDetails, ad2);
			Ad3TableView.Source = new AircraftDetailsTableSource(ad3AircraftDetails, ad3);

			Ad1BrokerLabel.Text = ad1.BrokerName;
		

			Ad1NameButton.SetTitle(ad1.Name, UIControlState.Normal);
			Ad1NameButton.Layer.BorderColor = UIColor.White.CGColor;
			Ad1NameButton.Layer.BorderWidth = 1f;
         
			AdImage2.SetImage(
				url: new NSUrl(ad2.ImageURL),
				placeholder: UIImage.FromBundle("ad_placeholder.jpg")
			);



         
			Ad2NameButton.SetTitle(ad2.Name, UIControlState.Normal);
			Ad2NameButton.Layer.BorderColor = UIColor.White.CGColor;
			Ad2NameButton.Layer.BorderWidth = 1f;

			AdImage3.SetImage(
				url: new NSUrl(ad3.ImageURL),
				placeholder: UIImage.FromBundle("ad_placeholder.jpg")
			);
         
			Ad3NameButton.SetTitle(ad3.Name, UIControlState.Normal);
			Ad3NameButton.Layer.BorderColor = UIColor.White.CGColor;
			Ad3NameButton.Layer.BorderWidth = 1f;

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
         
			Ad1RegistrationLabel.Text = HelperMethods.GetRegistrationString(ad1, labelAttribute);
            Ad1SerialLabel.Text = HelperMethods.GetSerialString(ad1, labelAttribute);
            Ad1TimeLabel.Text = HelperMethods.GetTotalTimeString(ad1, labelAttribute);
            Ad1LocationLabel.Text = ad1.Location;
         
			#region price changed
			if (!HelperMethods.ShowPriceChangedLabel(ad1.PriceLastUpdated))
			{
				Ad1PriceChangedLabel.Alpha = 0f;

			}
			if (!HelperMethods.ShowPriceChangedLabel(ad2.PriceLastUpdated))
			{
				Ad2PriceChangeLabel.Alpha = 0f;

			}
			if (!HelperMethods.ShowPriceChangedLabel(ad3.PriceLastUpdated))
			{
				Ad3PriceChangeLabel.Alpha = 0f;
			}
			#endregion
         
			////Set initial state of like buttons
			if (DataObject.Ads[0].IsLiked)
			{
				HelperMethods.SetInitialLikeButtonState(AdLike1, DataObject.Ads[0]);
			}
          
			//webview image tap gestures
			AdImage1.UserInteractionEnabled = true;
			AdImage2.UserInteractionEnabled = true;
			AdImage3.UserInteractionEnabled = true;

			tapGesture1 = new UITapGestureRecognizer(TapImageAction1);
			tapGesture2 = new UITapGestureRecognizer(TapImageAction2);
			tapGesture3 = new UITapGestureRecognizer(TapImageAction3);
         
			int currentIndex = this.DataObject.MagazinePageIndex;
			int totalPages = this.DataObject.TotalPages;

			PageIndicator.SetIndicatorState(currentIndex, totalPages);
         
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "AdLayout1 View");

			Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			//like, share, and email broker button event wiring
			AdEmail1.TouchUpInside += SendEmail_TouchUpInside;
			AdEmail2.TouchUpInside += SendEmail_TouchUpInside;
			AdEmail3.TouchUpInside += SendEmail_TouchUpInside;

			AdShare1.TouchUpInside += AdShare_TouchUpInside;
			AdShare2.TouchUpInside += AdShare_TouchUpInside;
			AdShare3.TouchUpInside += AdShare_TouchUpInside;

			AdLike1.TouchUpInside += AdLike_TouchUpInside;
			AdLike2.TouchUpInside += AdLike_TouchUpInside;
			AdLike3.TouchUpInside += AdLike_TouchUpInside;

			Ad1DetailButton.TouchUpInside += Ad1DetailButton_TouchUpInside;
                     
			Ad1TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;
                     
			Ad1NameButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad2NameButton.TouchUpInside += AdSortButton_TouchUpInside;
			Ad3NameButton.TouchUpInside += AdSortButton_TouchUpInside;

			Ad1BrokerButton.TouchUpInside += AdSortButton_TouchUpInside;
		


			AdImage1.AddGestureRecognizer(tapGesture1);
			AdImage2.AddGestureRecognizer(tapGesture2);
			AdImage3.AddGestureRecognizer(tapGesture3);

			Ad1MessageButton.TouchUpInside += AdMessages_TouchUpInside;
			Ad2MessageButton.TouchUpInside += AdMessages_TouchUpInside;
			Ad3MessageButton.TouchUpInside += AdMessages_TouchUpInside;



			Ad1RGButton.TouchUpInside += AdRGButton_TouchUpInside;

		}

		UITapGestureRecognizer tapGesture1;
		UITapGestureRecognizer tapGesture2;
		UITapGestureRecognizer tapGesture3;

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);

			//like, share, and email broker button event wiring
			AdEmail1.TouchUpInside -= SendEmail_TouchUpInside;
			AdEmail2.TouchUpInside -= SendEmail_TouchUpInside;
			AdEmail3.TouchUpInside -= SendEmail_TouchUpInside;

			AdShare1.TouchUpInside -= AdShare_TouchUpInside;
			AdShare2.TouchUpInside -= AdShare_TouchUpInside;
			AdShare3.TouchUpInside -= AdShare_TouchUpInside;

			AdLike1.TouchUpInside -= AdLike_TouchUpInside;
			AdLike2.TouchUpInside -= AdLike_TouchUpInside;
			AdLike3.TouchUpInside -= AdLike_TouchUpInside;

			Ad1DetailButton.TouchUpInside -= Ad1DetailButton_TouchUpInside;


			Ad1TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;
                     
			Ad1NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad2NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			Ad3NameButton.TouchUpInside -= AdSortButton_TouchUpInside;

			Ad1BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;


			AdImage1.RemoveGestureRecognizer(tapGesture1);
			AdImage2.RemoveGestureRecognizer(tapGesture2);
			AdImage3.RemoveGestureRecognizer(tapGesture3);

			Ad1MessageButton.TouchUpInside -= AdMessages_TouchUpInside;
			Ad2MessageButton.TouchUpInside -= AdMessages_TouchUpInside;
			Ad3MessageButton.TouchUpInside -= AdMessages_TouchUpInside;



			Ad1RGButton.TouchUpInside -= AdRGButton_TouchUpInside;

		}
	}
}