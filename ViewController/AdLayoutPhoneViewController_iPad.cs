using UIKit;
using System.Collections.Generic;
using MessageUI;
using AircraftForSale.PCL;
using System;
using Foundation;
using AircraftForSale.PCL.Helpers;
using SDWebImage;
using System.Threading.Tasks;
using System.Linq;
using Google.Analytics;
using System.Drawing;


namespace AircraftForSale
{
	public partial class AdLayoutPhoneViewController_iPad : UIViewController, IAdLayoutInterface
	{

		//public Specification Spec
		//{
		//    get;
		//    set;
		//}

		async void AdRGButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();


			if (sender == Ad1RGButton)
			{
				ad = DataObject.Ads[0];
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

		void AdMessages1_TouchUpInside(object sender, EventArgs e)
		{

			var ad = DataObject.Ads[0];
			var brokerPhoneNumber = ad.BrokerCellPhone;
			var smsTo = NSUrl.FromString("sms:" + brokerPhoneNumber);
			if (UIApplication.SharedApplication.CanOpenUrl(smsTo))
			{
				//UIApplication.SharedApplication.OpenUrl(smsTo);

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
				if (sender == Ad1NameButton)
				{
					HelperMethods.MakeModelRegistrationRequiredPrompt(this, sender as UIButton);
				}
				//if (sender == Ad1BrokerButton)
				//{
				//    HelperMethods.SellerRegistrationRequiredPrompt(this, sender as UIButton);
				//}
				return;
			}

			Ad ad = new Ad();
			bool isAdNameSort = false;
			if (sender == Ad1NameButton)
			{
				ad = DataObject.Ads[0];
				isAdNameSort = true;
			}


			//if (sender == Ad1BrokerButton)
			//{
			//    ad = DataObject.Ads[0];
			//    isAdNameSort = false;
			//}

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
					similarAdList = adList.Where(row => row.Name == ad.Name).OrderBy(r => r.IsFeatured).ToList();
				}
				else
				{
					similarAdList = adList.Where(row => row.BrokerName == ad.BrokerName).OrderBy(r => r.IsFeatured).ToList();
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

		public Specification AircraftSpecification
		{
			get;
			set;
		}
		async void TechnicalSpecButton_TouchUpInside(object sender, EventArgs e)
		{
			Ad ad = new Ad();

			if (sender == Ad1TechnicalSpecButton)
			{
				ad = DataObject.Ads[0];
			}



			if (AircraftSpecification == null)
			{
				SpecResponse specResponse = new SpecResponse();
				AircraftSpecification = await specResponse.GetSpecBySpecIDDesignationIDAsync(ad.SpecId, ad.DesignationId);
			}

			if (AircraftSpecification.SpecId != 0)
			{
				var specTableViewController = this.Storyboard.InstantiateViewController("SpecViewController_") as SpecViewController_;

				specTableViewController.Spec = AircraftSpecification;

				this.PresentViewController(specTableViewController, true, null);
			}
			else
			{
				if (!Reachability.IsHostReachable(Settings._baseDomain))
				{
					var alert = UIAlertController.Create("Connect to a Network", "Please connect to a network to retrieve these aircraft specs", UIAlertControllerStyle.Alert);

					alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Cancel, null));
					//alert.AddAction(UIAlertAction.Create("Snooze", UIAlertActionStyle.Default, action => Snooze()))
					if (alert.PopoverPresentationController != null)
					{
						alert.PopoverPresentationController.SourceView = this.View;
						alert.PopoverPresentationController.PermittedArrowDirections = UIPopoverArrowDirection.Up;
					}
					PresentViewController(alert, animated: true, completionHandler: null);
				}
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);


			//SpecTableView.FlashScrollIndicators();


			//PerformSelector(new ObjCRuntime.Selector("demo:"), null, 1);
			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "AdLayout View");

			Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
		}

		//[Export("demo:")]
		//void RunDemo(NSString arg)
		//{
		//	//SpecTableView.FlashScrollIndicators();
		//	//PerformSelector(new ObjCRuntime.Selector("demo:"), null, 1);
		//}


		void TapImageAction1(UITapGestureRecognizer tap)
		{
			HelperMethods.LoadWebViewWithAd(tap, DataObject.Ads[0], this.View);
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
				message += DataObject.Ads[0].Name + "!";
				url = DataObject.Ads[0].AircraftForSaleURL;

			}

			var itemTitle = NSObject.FromObject(title);
			var itemMessage = NSObject.FromObject(message);
			var itemLink = NSObject.FromObject(url);
			var activityItems = new NSObject[] { itemLink, itemMessage, itemTitle, itemLink };
			UIActivity[] applicationActivities = null;

			var activityController = new UIActivityViewController(activityItems, applicationActivities);

            if (activityController.PopoverPresentationController != null) {
                activityController.PopoverPresentationController.SourceView = senderButton;
                activityController.PopoverPresentationController.SourceRect = senderButton.Bounds;
            }

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

		public AdLayoutPhoneViewController_iPad(IntPtr handle) : base(handle)
		{


		}

		//~AdLayoutPhoneViewController()
		//{
		//	Console.WriteLine("AdLayoutPhoneViewController is about to be collected");
		//}
		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				ReleaseDesignerOutlets();
			}
		}

		UITapGestureRecognizer tapGesture1;
		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			AdEmail1.TouchUpInside += SendEmail_TouchUpInside;
			AdShare1.TouchUpInside += AdShare_TouchUpInside;
			AdLike1.TouchUpInside += AdLike_TouchUpInside;
			Ad1TechnicalSpecButton.TouchUpInside += TechnicalSpecButton_TouchUpInside;
			Ad1NameButton.TouchUpInside += AdSortButton_TouchUpInside;


			//AdPhone1.TouchUpInside += AdPhone1_TouchUpInside;
			AdMessages1.TouchUpInside += AdMessages1_TouchUpInside;

			AdImage1.AddGestureRecognizer(tapGesture1);

			Ad1RGButton.TouchUpInside += AdRGButton_TouchUpInside;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			AdEmail1.TouchUpInside -= SendEmail_TouchUpInside;
			AdShare1.TouchUpInside -= AdShare_TouchUpInside;
			AdLike1.TouchUpInside -= AdLike_TouchUpInside;
			Ad1TechnicalSpecButton.TouchUpInside -= TechnicalSpecButton_TouchUpInside;

			Ad1NameButton.TouchUpInside -= AdSortButton_TouchUpInside;
			//Ad1BrokerButton.TouchUpInside -= AdSortButton_TouchUpInside;

			//AdPhone1.TouchUpInside -= AdPhone1_TouchUpInside;
			AdMessages1.TouchUpInside -= AdMessages1_TouchUpInside;

			AdImage1.RemoveGestureRecognizer(tapGesture1);

			Ad1RGButton.TouchUpInside -= AdRGButton_TouchUpInside;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();




			this.NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(
UIImage.FromFile("new_home.png"), UIBarButtonItemStyle.Plain, (sender, args) =>
{
	NavigationController.PopViewController(true);
}), true);

			//Hide text button if broker doesn't have a cell phone
			if (string.IsNullOrEmpty(DataObject.Ads[0].BrokerCellPhone))
			{
				AdMessages1.Alpha = 0f;
			};

			var ad1 = DataObject.Ads[0];

			try
			{

				FeaturedLabel.Hidden = !ad1.IsFeatured;

				AdImage1.SetImage(
url: new NSUrl(ad1.ImageURL),
placeholder: UIImage.FromBundle("ad_placeholder.jpg")
);

			}
			catch (Exception ex)
			{
				string debugLine = ex.Message;
			}




			var labelAttribute = new UIStringAttributes
			{
				Font = UIFont.BoldSystemFontOfSize(18)
			};

			var brokerLabelAttribute = new UIStringAttributes
			{
				Font = UIFont.BoldSystemFontOfSize(15)
			};
			var brokerValueAttribute = new UIStringAttributes
			{
				Font = UIFont.SystemFontOfSize(15),
				ForegroundColor = UIColor.Blue
			};



			Ad1NameButton.SetTitle(ad1.Name, UIControlState.Normal);
			Ad1NameButton.Layer.BorderWidth = 1;
			Ad1NameButton.Layer.BorderColor = UIColor.White.CGColor;
			Ad1NameButton.Layer.CornerRadius = 5;


			string price = "";
			if (ad1.Price.Length == 0)
			{
				price = "Call";
			}
			else
			{
				price = ad1.Price;
			}

			AircraftDetails[] aircraftDetails;


			if (ad1.IsFeatured)
			{

				var airframeDetails = ad1.FeaturedSpec.AirframeDetails.Replace("<br>", "\n");
				var engineDetails = ad1.FeaturedSpec.EngDetails.Replace("<br>", "\n");
				var interiorDetails = ad1.FeaturedSpec.IntDetails.Replace("<br>", "\n");
				var exteriorDetails = ad1.FeaturedSpec.ExtDetails.Replace("<br>", "\n");
				var maintenanceDetails = ad1.FeaturedSpec.Maintenance.Replace("<br>", "\n");
				var avionicsDetails = ad1.FeaturedSpec.AvDetails.Replace("<br>", "\n");
				var additionalDetails = ad1.FeaturedSpec.DescriptionDetails.Replace("<br>", "\n");

				aircraftDetails = new AircraftDetails[]{
				new AircraftDetails("Price:", price, false),
				new AircraftDetails("Offered by:", ad1.BrokerName, true),
				new AircraftDetails("Registration:", HelperMethods.GetRegistrationString(ad1, labelAttribute), false),
				new AircraftDetails("Serial Number:", HelperMethods.GetSerialString(ad1, labelAttribute), false),
				new AircraftDetails("Time:", HelperMethods.GetTotalTimeString(ad1, labelAttribute), false),
				new AircraftDetails("Location:", ad1.Location, false),
				new AircraftDetails("Summary:", ad1.Teaser == string.Empty ? "Inquire for Details" : ad1.Teaser, false),
				new AircraftDetails("AIRFRAME:", airframeDetails == "" ? "N/A" : airframeDetails, false),
				new AircraftDetails("ENGINE(S):", engineDetails == "" ? "N/A" : engineDetails, false),
				new AircraftDetails("INTERIOR:", interiorDetails == "" ? "N/A" : interiorDetails, false),
				new AircraftDetails("EXTERIOR:", exteriorDetails == "" ? "N/A" : exteriorDetails, false),
				new AircraftDetails("MAINTENANCE:", maintenanceDetails == "" ? "N/A" : maintenanceDetails, false),
				new AircraftDetails("AVIONICS:", avionicsDetails == "" ? "N/A" : avionicsDetails, false),
				new AircraftDetails("ADDITIONAL DETAILS:", additionalDetails == "" ? "N/A" : additionalDetails, false)
			};



			}
			else
			{
				aircraftDetails = new AircraftDetails[]{
				new AircraftDetails("Price:", price, false),
				new AircraftDetails("Offered by:", ad1.BrokerName, true),
				new AircraftDetails("Registration:", HelperMethods.GetRegistrationString(ad1, labelAttribute), false),
				new AircraftDetails("Serial Number:", HelperMethods.GetSerialString(ad1, labelAttribute), false),
				new AircraftDetails("Time:", HelperMethods.GetTotalTimeString(ad1, labelAttribute), false),
				new AircraftDetails("Location:", ad1.Location, false),
				new AircraftDetails("Summary:", ad1.Teaser == string.Empty ? "Inquire for Details" : ad1.Teaser, false),
			};
			}



			#region price changed
			if (!HelperMethods.ShowPriceChangedLabel(ad1.PriceLastUpdated))
			{
				Ad1PriceChangeLabel.Alpha = 0f;

			}

			#endregion


			//Set initial state of like buttons
			if (DataObject.Ads[0].IsLiked)
			{
				HelperMethods.SetInitialLikeButtonState(AdLike1, DataObject.Ads[0]);
			}



			//webview image tap gestures
			AdImage1.UserInteractionEnabled = true;

			tapGesture1 = new UITapGestureRecognizer(TapImageAction1);

			int currentIndex = this.DataObject.MagazinePageIndex;
			int totalPages = this.DataObject.TotalPages;

			PageIndicator.SetIndicatorState(currentIndex, totalPages);


			loadSpeciafication();

			//SpecTableView.RegisterClassForCellReuse(typeof(AircraftDetailsLabelCell), AircraftDetailsTableSource.CellLabelIdentifier);

			SpecTableView.Source = new AircraftDetailsTableSource(aircraftDetails, DataObject.Ads[0]);
		}

		async void loadSpeciafication()
		{
			Ad ad = new Ad();

			ad = DataObject.Ads[0];

			SpecTableView.RowHeight = UITableView.AutomaticDimension;
			SpecTableView.Source = new SpecTableViewSource_iPad(this, ad);




		}

	}



	class SpecTableViewSource_iPad : UITableViewSource
	{
		const string _cellIdentifier = "spec_cell1";
		public const string _rangeSection = "Range";
		public Ad ad
		{
			get;
			set;
		}


		//static int instanceCount = 0;

		// ~SpecTableViewSource()
		//{

		//	Console.WriteLine("SpecTableViewSource is being disposed");
		//}

		string[] keys = { "AIRFRAME:", "ENGINE(S):", "INTERIOR:", "EXTERIOR:", "MAINTENANCE:", "AVIONICS:", "ADDITIONAL DETAILS:" };

		nfloat[] heights = { 40f, 40f, 40f, 40f, 40f, 40f, 40f, 40f, 40f };


		WeakReference parent;
		public AdLayoutPhoneViewController_iPad Owner
		{
			get
			{
				return parent.Target as AdLayoutPhoneViewController_iPad;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public SpecTableViewSource_iPad(AdLayoutPhoneViewController_iPad owner, Ad ad1)
		{
			ad = ad1;
			//keys = SpecDictionary.Keys.ToArray();
			//keys = { "Item1", "Item2", "Item3", "Item4" };
			Owner = owner;
			//instanceCount++;
		}


		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// in a Storyboard, Dequeue will ALWAYS return a cell,
			var cell = tableView.DequeueReusableCell(_cellIdentifier, indexPath) as SpecCell1;
			//var cell = (SpecCell1)tableView.DequeueReusableCell(_cellIdentifier);
			// now set the properties as norma

			var item = "";

			if (indexPath.Section == 0)
				item = ad.FeaturedSpec.AirframeDetails;
			else if (indexPath.Section == 1)
				item = ad.FeaturedSpec.EngDetails;
			else if (indexPath.Section == 2)
				item = ad.FeaturedSpec.IntDetails;
			else if (indexPath.Section == 3)
				item = ad.FeaturedSpec.ExtDetails;
			else if (indexPath.Section == 4)
				item = ad.FeaturedSpec.Maintenance;
			else if (indexPath.Section == 5)
				item = ad.FeaturedSpec.AvDetails;
			else if (indexPath.Section == 6)
				item = ad.FeaturedSpec.DescriptionDetails;


			if (item != null && item.Length > 0)
				item = item.Replace(@"<br>", "\n");
			else
				item = "N/A";

			heights[indexPath.Section] = cell.UpdateCell(item);

			cell.Accessory = UITableViewCellAccessory.None;
			return cell;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{

			return heights[indexPath.Section];
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return ad.IsFeatured ? keys.Length : 0;
		}
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return 1;//SpecDictionary[keys[section]].Count;
		}
		//public override string[] SectionIndexTitles(UITableView tableView)
		//{
		//	return keys;
		//}


		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return keys[section];
		}


		void ResizeHeigthWithText(UILabel label, float maxHeight = 960f)
		{
			float width = (float)label.Frame.Width;

			var labelFrame = new CoreGraphics.CGRect(CoreGraphics.CGPoint.Empty, ((NSString)label.Text).GetBoundingRect(
				new CoreGraphics.CGSize(width, nfloat.MaxValue),
				NSStringDrawingOptions.UsesLineFragmentOrigin,
				new UIStringAttributes { Font = label.Font },
				null
				).Size);

		}



		public override UIView GetViewForHeader(UITableView tableView, nint section)
		{
			var headerWidth = tableView.Frame.Width;
			var headerHeight = 25;
			var labelLeftMargin = 20;

			UIView containerView = new UIView(new CoreGraphics.CGRect(0, 0, headerWidth, headerHeight));

			UIView barView = new UIView(new CoreGraphics.CGRect(0, 0, headerWidth, 1));
			barView.BackgroundColor = UIColor.Gray;
			containerView.AddSubview(barView);

			//var textColor = UIColor.FromRGB(50, 205, 50);
			var textColor = UIColor.White;
			var font = UIFont.BoldSystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 20 : 17);

			var sectionName = keys[section];
			if (sectionName != _rangeSection)
			{



				UILabel headerLabel = new UILabel(new CoreGraphics.CGRect(labelLeftMargin, 10, headerWidth - labelLeftMargin, headerHeight)); // Set the frame size you need
				headerLabel.TextColor = textColor; // Set your color
				headerLabel.Text = sectionName;
				headerLabel.Font = font;
				containerView.AddSubview(headerLabel);

			}



			return containerView;
		}

	}
}