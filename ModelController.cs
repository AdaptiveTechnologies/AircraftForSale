using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using UIKit;

namespace AircraftForSale
{
	public class MagazinePage
	{
		public int Layout
		{
			get;
			set;
		}
		public List<Ad> Ads
		{
			get;
			set;
		}

		public int Count
		{
			get;
			set;
		}

		public string SelectedClassification
		{
			get;
			set;
		}

		public int MagazinePageIndex
		{
			get;
			set;
		}

		public int TotalPages
		{
			get;
			set;
		}

	}
	public class ModelController : UIPageViewControllerDataSource
	{

		List<MagazinePage> magPageList;

		int PageIndex;

		public List<Ad> adList;

		public ModelController(String selectedClassification)
		{

			Task.Run(async () =>
			{
				var adListEnumerable = (await Ad.GetAdsByClassificationAsync(selectedClassification));
				switch (Settings.SortOptions)
				{
					case "No Preference":
						{
							adList = adListEnumerable.ToList();
							break;
						}
					case "Recently Updated":
						{
							adList = adListEnumerable.OrderByDescending(row => DateTime.Parse(row.LastUpdated)).ToList();
							break;
						}
					case "Price":
						{
							adList = adListEnumerable.OrderBy(row => row.Price == null || row.Price == string.Empty || row.Price == "N/A" ? 999999999 : double.Parse(row.Price, NumberStyles.Currency)).ToList();
							break;
						}
					case "Total Time":
						{
							adList = adListEnumerable.OrderBy(row => double.Parse(row.TotalTime)).ToList();
							break;
						}
					default:
						{
							adList = adListEnumerable.ToList();
							break;
						}
				}
			}).Wait();
			LoadModalController(adList, selectedClassification);
		}

		MagazinePage AssociatsAdsWithMagazinePageStruct(List<Ad> adListParam, int totalAdsCount, int adsIndex, MagazinePage magPage, int adCount)
		{
			List<Ad> adsForThisLayout = new List<Ad>();

			bool adsAvailableForPage = (adsIndex + adCount) <= totalAdsCount;

			if (adsAvailableForPage)
			{
				adsForThisLayout = adListParam.GetRange(adsIndex, adCount);
			}
			else
			{
				int adsNeeded = adsIndex + adCount;

				int adsNeededFromAdListStart = adsNeeded - totalAdsCount;
				for (int i = 0; i < adsNeededFromAdListStart; i++)
				{
					adListParam.Add(adListParam[i]);
				}
				adsForThisLayout = adListParam.GetRange(adsIndex, adCount);
			}
			magPage.Ads = adsForThisLayout;
			return magPage;
		}

		public void LoadModalController(List<Ad> adListParam, string selectedClassification)
		{
			magPageList = new List<MagazinePage>();



			adList = adListParam;

			int totalAdsCount = adList.Count;

			var device = UIDevice.CurrentDevice;




			bool adsAvailable = true;
			int adsIndex = 0;
			int magazinePageIndex = 0;

			int banManProCounter = 1;


			Random rnd = new Random();
			do
			{

				int layout = 1;

				if (banManProCounter == 5)
				{
					layout = 4;
					banManProCounter = 1;
				}
				else
				{
					banManProCounter++;
					var randomDouble = rnd.NextDouble();

					if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
					{

						//condition will be met 35% of the time
						if (randomDouble > 0 && randomDouble <= .33)
						{
							layout = 1;
						}

						//condition will be met 35% of the time
						if (randomDouble > .33 && randomDouble <= .66)
						{
							layout = 2;
						}

						//condition will be met 15% of the time
						if (randomDouble > .66)
						{
							layout = 3;
						}


					}
					else
					{
						layout = 2;
					}
				}


				MagazinePage magPage = new MagazinePage();
				magPage.SelectedClassification = selectedClassification;

				var currentAd = adList[adsIndex];
				if (currentAd.IsFeatured && layout != 4)
				{
					magPage.Layout = 2;
					List<Ad> adsForThisLayout = new List<Ad>();
					adsForThisLayout.Add(currentAd);
					magPage.Ads = adsForThisLayout;
					adsIndex++;
				}
				else
				{

					switch (layout)
					{
						case 1:
							{
								//int adCount = 1;
								int adCount = 2;
								magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
								magPage.Layout = 1;

								adsIndex += adCount;
								break;
							}
						case 2:
							{
								int adCount = 1;
								magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
								magPage.Layout = 2;

								adsIndex += adCount;
								break;
							}
						case 3:
							{
								//int adCount = 1;
								int adCount = 3;
								magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
								magPage.Layout = 3;

								adsIndex += adCount;
								break;
							}
						case 4:
							{
								int adCount = 0;
								magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
								magPage.Layout = 4;

								adsIndex += adCount;
								break;
							}

						default:
							{

								int adCount = 0;
								magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
								magPage.Layout = 1;

								adsIndex += adCount;
								break;
							}
					}
				}

				magPage.MagazinePageIndex = magazinePageIndex++;
            
				magPageList.Add(magPage);

				if (adsIndex < adList.Count)
				{
					adsAvailable = true;
				}
				else
				{
					adsAvailable = false;
				}



			} while (adsAvailable);

			//Assign total pages to each MagazinePageStruct
			for (int i = 0; i < magPageList.Count; i++)
			{
				magPageList[i].TotalPages = magPageList.Count;
				var ads = magPageList[i].Ads;
				Console.WriteLine("Layout: " + magPageList[i].Layout);
				ads.ForEach(r => Console.WriteLine(r.Name + " id: " + r.ID));
			}

			Console.WriteLine("Total Ads: " + adList.Count);
		}


		public UIViewController GetViewController(int index, Boolean isPrev)
		{
			if (index >= magPageList.Count)
				return null;

			if (index == 0)
			{
				PageIndex = 0;
			}
			else
			{

				if (isPrev)
					PageIndex -= 1;
				else
					PageIndex += 1;
			}





			var currentPage = magPageList[index];

			UIViewController returnViewController;

			var device = UIDevice.CurrentDevice;
         
			var ad = magPageList[PageIndex];

            //featured ads always have to have layout 2
            //if(ad)

			var layout = ad.Layout;
                     
			switch (layout)
			{

				case 1://2 ads stacked: iPad only
					{
						AdLayout2ViewController adLayout1 = (AdLayout2ViewController)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayout2ViewController");
      
						adLayout1.DataObject = ad;

						returnViewController = adLayout1 as UIViewController;
						break;
					}




				case 2://1 ad: iPad (featured) and all iPhone
					{
						var phoneLayout = (IAdLayoutInterface)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayoutPhoneViewController_iPad");

						phoneLayout.DataObject = ad;

						returnViewController = phoneLayout as UIViewController;
						break;
					}

				case 3://3 ads: iPad only
					{

						AdLayout1ViewController adLayout1 = (AdLayout1ViewController)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayout11ViewController");
                  
						adLayout1.DataObject = ad;


						returnViewController = adLayout1 as UIViewController;
						break;
					}

				case 4://BanManPro

					{
						BanManProViewController banManProLayout = (BanManProViewController)UIStoryboard.FromName("Registration_New", null).InstantiateViewController("BanManProViewController");

						banManProLayout.DataObject = ad;

						returnViewController = banManProLayout as UIViewController;
						break;
					}


				default: {//Should never be called
						var phoneLayout = (IAdLayoutInterface)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayoutPhoneViewController_iPad");


						phoneLayout.DataObject = ad;

                        returnViewController = phoneLayout as UIViewController;
                        break;
					}

			}
			return returnViewController;
		}

		public int IndexOf(IAdLayoutInterface viewController)
		{
			return magPageList.IndexOf(viewController.DataObject);
		}

		#region Page View Controller Data Source

		public override UIViewController GetNextViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			int index = IndexOf((IAdLayoutInterface)referenceViewController);

			//Handle first page flip after clicking the ad name button in a MagazinePage
			if (index == -1)
			{
				index = 0;
			}

			if (index == magPageList.Count - 1)
			{
				HelperMethods.SendBasicAlert("Magazine", "We are sorry but currently there are no more aircraft to view in this magazine");
				return null;
			}

			//referenceViewController.Dispose();

			return GetViewController(index + 1, false);
		}

		public override UIViewController GetPreviousViewController(UIPageViewController pageViewController, UIViewController referenceViewController)
		{
			int index = IndexOf((IAdLayoutInterface)referenceViewController);

			if (index == -1 || index == 0)
			{
				//HelperMethods.SendBasicAlert("Magazine", "We are sorry but currently there are no more aircraft to view in this magazine");
				return null;
			}

			//referenceViewController.Dispose();

			return GetViewController(index - 1, true);
		}

		#endregion
	}
}

