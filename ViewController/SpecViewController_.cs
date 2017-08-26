using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;
using System.Collections.Generic;
using AircraftForSale.PCL.Helpers;
using SDWebImage;
using System.Linq;
using Google.Analytics;
using System.Drawing;

namespace AircraftForSale
{
	public partial class SpecViewController_ : UIViewController
	{
		void CloseButton_TouchUpInside(object sender, EventArgs e)
		{
			DismissViewController(true, null);
		}

		public Specification Spec
		{
			get;
			set;
		}

		public SpecViewController_(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "Specification View");

			Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			string imageURL = string.Empty;

			if (string.IsNullOrEmpty(Spec.ImagePath))
			{
				SpecImage.Image = UIImage.FromBundle("ad_placeholder.jpg");
			}
			else
			{
				//imageURL = System.Net.WebUtility.UrlEncode("http://images.globalair.com" + Spec.ImagePath);
				string imagePath =  Spec.ImagePath;
				imageURL = imagePath.Replace(" ", "%20");
				try
				{
					SpecImage.SetImage(
					url: new NSUrl(imageURL),
					placeholder: UIImage.FromBundle("ad_placeholder.jpg")
					);
				}
				catch (Exception)
				{
					SpecImage.Image = UIImage.FromBundle("ad_placeholder.jpg");

				}
			}

			var device = UIDevice.CurrentDevice;

			if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
			{
				var font = UIFont.SystemFontOfSize(17);
				AircraftLabel.Font = UIFont.BoldSystemFontOfSize(17);
				ClassificationLabel.Font = font;
				CategoryLabel.Font = font;

			}
			AircraftLabel.Text = Spec.Aircraft;

			var labelAttribute = new UIStringAttributes
			{
				Font = UIFont.BoldSystemFontOfSize(device.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone ? 17 : 22)
			};

			string simpleCategoryText = "Category: " + Spec.Category;
			var categoryString = new NSMutableAttributedString(simpleCategoryText);
			categoryString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 10));

			CategoryLabel.AttributedText = categoryString;

			string simpleClassificationText = "Classification: " + Spec.Classification;
			var classificationString = new NSMutableAttributedString(simpleClassificationText);
			classificationString.SetAttributes(labelAttribute.Dictionary, new NSRange(0, 15));

			ClassificationLabel.AttributedText = classificationString;
			if (!(Spec.IsConklin == "True"))
			{
				ConklinImageView.Alpha = 0f;
			}

			//SpecDescriptionLabel.Text = Spec.SpecDescription;

			//ResizeHeigthWithText(SpecDescriptionLabel);
		
			//if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			//{
			//	SpecDescriptionLabel.Text = Spec.SpecDescription;
			//}
			//else
			//{
			//	SpecDescriptionLabel.Text = "";
			//}



			//SpecTableView.RowHeight = UITableView.AutomaticDimension;
			//SpecTableView.EstimatedRowHeight = 70f;
			SpecTableView.Source = new SpecTableViewSource1(Spec, this);
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

			labelFrame.Y = View.Frame.Height - labelFrame.Height;
			var tableFrame = SpecTableView.Frame;
			tableFrame.Height = tableFrame.Height - labelFrame.Height - (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?50:10);

			label.Frame = labelFrame;
			SpecTableView.Frame = tableFrame;

		}

		//~SpecViewController_()
		//{
		//	Console.WriteLine("SpecViewController_ is being disposed");
		//}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
			CloseButton.TouchUpInside += CloseButton_TouchUpInside;
		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);
			CloseButton.TouchUpInside -= CloseButton_TouchUpInside;
		}
	}

	class SpecTableViewSource1 : UITableViewSource
	{
		const string _cellIdentifier = "spec_cell";
		public const string _rangeSection = "Range";
		public Specification spec
		{
			get;
			set;
		}
		public Dictionary<string, List<SpecField>> SpecDictionary
		{
			get;
			set;
		}

		//static int instanceCount = 0;

		// ~SpecTableViewSource()
		//{

		//	Console.WriteLine("SpecTableViewSource is being disposed");
		//}

		string[] keys;

		WeakReference parent;
		public SpecViewController_ Owner
		{
			get
			{
				return parent.Target as SpecViewController_;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		public SpecTableViewSource1(Specification specification, SpecViewController_ owner)
		{
			spec = specification;
			SpecDictionary = specification.SpecFieldDictionary;
			keys = SpecDictionary.Keys.ToArray();
			Owner = owner;
			//instanceCount++;
		}


		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			// in a Storyboard, Dequeue will ALWAYS return a cell,
			var cell = (SpecCell)tableView.DequeueReusableCell(_cellIdentifier);
			// now set the properties as norma
			var item = SpecDictionary[keys[indexPath.Section]][indexPath.Row];
			cell.UpdateCell(item);

			var section = keys[indexPath.Section];



			if (section == _rangeSection && item.FieldName.Contains(_rangeSection))
			{
				//cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
				UIImageView mapIndicator = new UIImageView(UIImage.FromBundle("map.png"));
				//mapIndicator.SizeToFit();
				cell.AccessoryView = null;
			}
			else
				cell.AccessoryView = null;
			cell.Accessory = UITableViewCellAccessory.None;
			return cell;
		}

		public override nint NumberOfSections(UITableView tableView)
		{
			return keys.Length;
		}
		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return SpecDictionary[keys[section]].Count;
		}
		//public override string[] SectionIndexTitles(UITableView tableView)
		//{
		//	return keys;
		//}


		public override string TitleForHeader(UITableView tableView, nint section)
		{
			return keys[section];
		}



		public override UIView GetViewForFooter(UITableView tableView, nint section)
		{
			
			UITextView footerlabel = new UITextView(new CoreGraphics.CGRect(0, UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?40:30, UIScreen.MainScreen.Bounds.Size.Width-20, 700));
			UILabel footerTitle = new UILabel(new CoreGraphics.CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width-20, 50));
			var font = UIFont.SystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?24:15);

			var font1 = UIFont.SystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 20 : 16);

			footerlabel.TextColor = UIColor.White;
			footerlabel.BackgroundColor = UIColor.Clear;
			footerlabel.Editable = false;


			footerlabel.Font = font;
			if (section == SpecDictionary.Count - 1)
			{
				footerlabel.Text = spec.SpecDescription;
				footerTitle.Text = "  Description";
			}
			else
			{
				footerlabel.Text = "";
				footerTitle.Text = "";
			}
			//ResizeHeigthWithText(footerlabel);

			UIView footerView = new UIView(new CoreGraphics.CGRect(10, 0, UIScreen.MainScreen.Bounds.Size.Width-20, 800));



			footerTitle.Font = font1;
			footerTitle.TextColor = UIColor.Green;
			footerView.AddSubview(footerTitle);
			footerView.AddSubview(footerlabel);
			return footerView;
		}

		public override nfloat GetHeightForFooter(UITableView tableView, nint section)
		{
			if (section == SpecDictionary.Count - 1)
			{
				
				return 800;
			}
			else
				return 0;
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

			var textColor = UIColor.FromRGB(50, 205, 50);
var font = UIFont.BoldSystemFontOfSize(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?24:16);

			var sectionName = keys[section];
			if (sectionName != _rangeSection)
			{



				UILabel headerLabel = new UILabel(new CoreGraphics.CGRect(labelLeftMargin, 0, headerWidth - labelLeftMargin, headerHeight)); // Set the frame size you need
				headerLabel.TextColor = textColor; // Set your color
				headerLabel.Text = sectionName;
				headerLabel.Font = font;
				containerView.AddSubview(headerLabel);

			}
			else
			{
				UIButton button = new UIButton(new CoreGraphics.CGRect(labelLeftMargin, 0, headerWidth - labelLeftMargin, headerHeight));
				button.SetTitle(sectionName, UIControlState.Normal);
				button.SetTitleColor(textColor, UIControlState.Normal);
				button.Font = font;
				button.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;
				//button.SetImage(UIImage.FromBundle("map.png"), UIControlState.Normal);

				button.TouchUpInside += (sender, e) =>
				{
					if (Reachability.IsHostReachable(Settings._baseDomain))
					{
						//var specSection = SpecDictionary.Where(row => row

						MapViewController mapViewController = (MapViewController)Owner.Storyboard.InstantiateViewController("MapViewController");
						mapViewController.SpecFieldList = SpecDictionary[_rangeSection];
						Owner.ShowDetailViewController(mapViewController, this);
					}

					else
					{
						HelperMethods.SendBasicAlert("Connect to a Network", Settings._networkProblemMessage);
					}
				};

				containerView.AddSubview(button);
			}


			return containerView;
		}
		//public override UIView GetViewForHeader(UITableView tableView, int section)
		//{

		//}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			//var cell = tableView.CellAt(indexPath);

			var section = keys[indexPath.Section];
			if (section == _rangeSection)
			{

				if (Reachability.IsHostReachable(Settings._baseDomain))
				{
					var specSection = SpecDictionary[keys[indexPath.Section]];

					MapViewController mapViewController = (MapViewController)Owner.Storyboard.InstantiateViewController("MapViewController");
					mapViewController.SpecFieldList = specSection;
					Owner.ShowDetailViewController(mapViewController, this);
				}

				else
				{
					HelperMethods.SendBasicAlert("Connect to a Network", Settings._networkProblemMessage);
				}
			}
			tableView.DeselectRow(indexPath, true);
		}
	}
}