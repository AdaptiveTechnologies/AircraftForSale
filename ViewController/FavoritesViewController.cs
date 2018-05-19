using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using AircraftForSale.PCL;
using System.Threading.Tasks;
using System.Linq;
using AircraftForSale.PCL.Helpers;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class FavoritesViewController : UIViewController
	{
		void EditTableButton_TouchUpInside(object sender, EventArgs e)
		{
			if (FavoritesTableView.Editing)
			{
				FavoritesTableView.SetEditing(false, true);
				EditTableButton.SetTitle("Edit", UIControlState.Normal);
			}
			else
			{
				FavoritesTableView.SetEditing(true, true);
				EditTableButton.SetTitle("Done", UIControlState.Normal);
			}
		}

		public UIButton EditTableButton
		{
			get;
			set;
		}

		public List<Ad> FavoritesAdList
		{
			get;
			set;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);

			// This screen name value will remain set on the tracker and sent with
			// hits until it is set to a new value or to null.
			Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "Favourite View");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateScreenView().Build());
		}

		public UITableView FavoritesTableViewProperty
		{
			get
			{
				return FavoritesTableView;
			}
		}

		//~FavoritesViewController()
		//{
		//	Console.WriteLine("FavoritesViewController is about to be garbage collected");
		//}


		public FavoritesViewController(IntPtr handle) : base(handle)
		{
		}



		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;


			FavoritesTableView.BackgroundView = null;
			//FavoritesTableView.BackgroundColor = UIColor.Clear;

			EditTableButton = new UIButton(new CGRect(View.Bounds.Width - 110, statusBarHeight, 100, 25));
			EditTableButton.SetTitle("Edit", UIControlState.Normal);
			EditTableButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);

			//FavoritesTableView.RowHeight = UITableView.AutomaticDimension
			//FavoritesTableView.EstimatedRowHeight = 100f;

            


			View.Add(EditTableButton);


            


		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			//register for events
			EditTableButton.TouchUpInside += EditTableButton_TouchUpInside;
			LoadingOverlay overlay = new LoadingOverlay(this.View.Frame, "Loading...");
			this.View.AddSubview(overlay);
			Task.Run(async () =>
			{
				Ad ad = new Ad();
				var adList = await (ad.GetAllAdsForFavorites());

				var favoredAds = adList.Where(row => row.IsLiked).ToList();
				for (int i = 0; i < favoredAds.Count; i++)
				{
					string adID = favoredAds[i].ID;
					var adLocal = await AdLocal.GetAdLocalByAdID(adID);
					if (adLocal == null || string.IsNullOrEmpty(adLocal.AdID))
					{
						adLocal = new AdLocal();
						adLocal.AdID = adID;
						adLocal.Sequence = favoredAds.Count;
					}

					favoredAds[i].ClientFavoritesSortOrder = adLocal.Sequence;
					favoredAds[i].Notes = adLocal.Notes;
				}

				favoredAds = favoredAds.OrderBy(row => row.ClientFavoritesSortOrder).ToList();

				InvokeOnMainThread(() =>
				{
					overlay.Hide();
					FavoritesAdList = favoredAds;
					FavoritesTableView.Source = new FavoritesTableSource(this);

					FavoritesTableView.ReloadData();
				});
			});




		}

		public override void ViewDidDisappear(bool animated)
		{
			//unregister from events
			EditTableButton.TouchUpInside -= EditTableButton_TouchUpInside;
			return;

			var localModifiedAdList = FavoritesAdList.Where(row => row.IsModified).ToList();

			//Save modifications made to ads while browsing a magazine
			if (localModifiedAdList != null && localModifiedAdList.Count > 0)
			{
				var classifications = localModifiedAdList.Select(row => row.Classification).Distinct();
				//var classifications = Settings.GetFavoredClassifications();
				if (classifications.Count() > 0)
				{
					Task.Run(async () =>
					{

						foreach (var classification in classifications)
						{
							var modifiedAdsByClassification = localModifiedAdList.Where(row => row.Classification == classification);

							var allAdsByClassification = (await Ad.GetAdsByClassificationAsync(classification)).ToList();
							//remove favorite ads from all ads list

							//int sequence = 0;
							foreach (var modifiedAd in modifiedAdsByClassification)
							{
								var adWithSameAdID = allAdsByClassification.FirstOrDefault(row => row.ID == modifiedAd.ID);
								if (adWithSameAdID != null)
								{
									allAdsByClassification.Remove(adWithSameAdID);
								}

								//if ad local doesn't exist, create it and set values
								var adLocal = await AdLocal.GetAdLocalByAdID(modifiedAd.ID);
								if (adLocal == null || string.IsNullOrEmpty(adLocal.AdID))
								{
									adLocal = new AdLocal();
									adLocal.AdID = modifiedAd.ID;
								}
								adLocal.Sequence = modifiedAd.ClientFavoritesSortOrder;
								adLocal.Notes = modifiedAd.Notes;
								AdLocal.SaveAdLocalByAdID(adLocal, modifiedAd.ID);

								modifiedAd.IsModified = false;

							}


							//add favorite ads back into all ads list
							allAdsByClassification.AddRange(modifiedAdsByClassification);

							Ad.SaveAdsByClassification(allAdsByClassification, classification);
						}
					});
				}
			}


			FavoritesAdList = null;


			base.ViewDidDisappear(animated);





		}
	}

	public class FavoritesTableSource : UITableViewSource
	{

		WeakReference parent;
		public FavoritesViewController Owner
		{
			get
			{
				return parent.Target as FavoritesViewController;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}

		NSString CellIdentifier = (NSString)"favoritesPrototypeCell";

		public FavoritesTableSource(FavoritesViewController owner)
		{
			Owner = owner;
		}

		public override nint RowsInSection(UITableView tableview, nint section)
		{
			return Owner.FavoritesAdList.Count;
		}

		public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
		{
			FavoriteAircraftTableCell cell = tableView.DequeueReusableCell(CellIdentifier) as FavoriteAircraftTableCell;
			Ad item = Owner.FavoritesAdList[indexPath.Row];

			cell.UpdateCell(item, this);
			return cell;
		}

		public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
		{
			return 100;
		}

		public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
		{
			var cell = tableView.CellAt(indexPath);


			if (Reachability.IsHostReachable(Settings._baseDomain))
			{
				var ad = Owner.FavoritesAdList[indexPath.Row];
				var statusBarHeight = UIApplication.SharedApplication.StatusBarFrame.Height;
				var tabBarHeight = Owner.TabBarController.TabBar.Bounds.Height;

				var frame = new CGRect(0, statusBarHeight, Owner.View.Bounds.Width, Owner.View.Bounds.Height - (statusBarHeight + tabBarHeight));
				var webView = new UIWebView(frame);

				LoadingOverlay loadingOverlay = new LoadingOverlay(Owner.View.Frame);

				webView.LoadFinished += (sender, e) =>
				{
					loadingOverlay.Hide();
				};


				var url = ad.AircraftForSaleURL;
				webView.LoadRequest(new NSUrlRequest(new NSUrl(url)));

				UIView.BeginAnimations("fadeflag");
				UIView.Animate(1, () =>
				{
					cell.Alpha = .5f;
				}, () =>
				{

					Owner.View.AddSubview(webView);
					Owner.View.AddSubview(loadingOverlay);

					UIButton closeButton = new UIButton(new CGRect(Owner.View.Bounds.Width - 50, 0, 50, 50));
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
					//Owner.View.AddSubview(closeButton);
					webView.AddSubview(closeButton);

					cell.Alpha = 1f;
				});

				UIView.CommitAnimations();
				//}
			}
			else
			{
				HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to view this ad");
			}




			tableView.DeselectRow(indexPath, true);
		}

		public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle)
			{
				case UITableViewCellEditingStyle.Delete:

					//set isliked to false on root ad
					Ad item = Owner.FavoritesAdList[indexPath.Row];

					item.IsLiked = false;
					// remove the item from the underlying data source
					Owner.FavoritesAdList.RemoveAt(indexPath.Row);
					// delete the row from the table
					tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);


					//attempt to change like status in the database
					Task.Run(async () =>
					{
						AdLikeResponse adLikeResponse = await new AdLikeResponse().RecordAdLike(int.Parse(item.ID), false);

						if (adLikeResponse.Status != "Success")
						{
							InvokeOnMainThread(() =>
							{
								HelperMethods.SendBasicAlert("Oops", adLikeResponse.ResponseMsg);
								item.IsLiked = true;
								// remove the item from the underlying data source
								Owner.FavoritesAdList.Insert(indexPath.Row, item);
								// delete the row from the table
								tableView.InsertRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
							});

						}
						else
						{
							var allAdsByClassification = (await Ad.GetAdsByClassificationAsync(item.Classification)).ToList();
							var thisItem = allAdsByClassification.FirstOrDefault(row => row.ID == item.ID);
							thisItem.IsLiked = false;
							Ad.SaveAdsByClassification(allAdsByClassification, item.Classification);
						}

					});

					break;
				case UITableViewCellEditingStyle.None:
					Console.WriteLine("CommitEditingStyle:None called");
					break;
			}
		}
		public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you wish to disable editing for a specific indexPath or for all rows
		}

		public override bool CanMoveRow(UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you don't allow re-ordering
		}

		public override void MoveRow(UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			var item = Owner.FavoritesAdList[sourceIndexPath.Row];
			var deleteAt = sourceIndexPath.Row;
			var insertAt = destinationIndexPath.Row;

			// are we inserting 
			if (destinationIndexPath.Row < sourceIndexPath.Row)
			{
				// add one to where we delete, because we're increasing the index by inserting
				deleteAt += 1;
			}
			else
			{
				// add one to where we insert, because we haven't deleted the original yet
				insertAt += 1;
			}
			Owner.FavoritesAdList.Insert(insertAt, item);
			Owner.FavoritesAdList.RemoveAt(deleteAt);

			//Set sequencee
			Task.Run(async () =>
			{
				for (int i = 0; i < Owner.FavoritesAdList.Count; i++)
				{
					Owner.FavoritesAdList[i].ClientFavoritesSortOrder = i;

					//if ad local doesn't exist, create it and set values
					var adLocal = await AdLocal.GetAdLocalByAdID(Owner.FavoritesAdList[i].ID);
					if (adLocal == null || string.IsNullOrEmpty(adLocal.AdID))
					{
						adLocal = new AdLocal();
						adLocal.AdID = Owner.FavoritesAdList[i].ID;
					}

					adLocal.Sequence = i;
					adLocal.Notes = Owner.FavoritesAdList[i].Notes;
					AdLocal.SaveAdLocalByAdID(adLocal, Owner.FavoritesAdList[i].ID);

				}
			});
		}
	}
}