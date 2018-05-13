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
			else {
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


			Random rnd = new Random();
			do
			{

				int layout = 1;


                var randomDouble = rnd.NextDouble();
                //var randomDouble = .4;

				if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
				{

					//condition will be met 35% of the time
					if (randomDouble > 0 && randomDouble <= .35)
					{
						layout = 1;
					}

					//condition will be met 35% of the time
					if (randomDouble > .35 && randomDouble <= .7)
					{
						layout = 2;
					}

					//condition will be met 15% of the time
					if (randomDouble > .7 && randomDouble <= .85)
					{
						layout = 3;
					}

					//condition will be met 10% of the time
					if (randomDouble > .85)
					{
						layout = 4;
					}
				}
				else {
					if (randomDouble <= .15)
					{
						layout = 4; //banmanpro layout
					}
					else {
						layout = 5; //phone
					}
				}


				MagazinePage magPage = new MagazinePage();
				magPage.SelectedClassification = selectedClassification;

				switch (layout)
				{
					case 1:
						{
							int adCount = 1;
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
							int adCount = 1;
							magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
							magPage.Layout = 3;

							adsIndex += adCount;
							break;
						}
					case 4:
						{
							int adCount = 1;
							magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
							magPage.Layout = 4;

							adsIndex += adCount;
							break;
						}
						case 5:
						{
							int adCount = 1;
							magPage = AssociatsAdsWithMagazinePageStruct(adList, totalAdsCount, adsIndex, magPage, adCount);
							magPage.Layout = 5;

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

				magPage.MagazinePageIndex = magazinePageIndex++;
				//magPage.TotalPages = adList.Count;

				magPageList.Add(magPage);

				if (adsIndex < adList.Count)
				{
					adsAvailable = true;
				}
				else {
					adsAvailable = false;
				}
			} while (adsAvailable);

			//Assign total pages to each MagazinePageStruct
			for (int i = 0; i < magPageList.Count; i++)
			{
				magPageList[i].TotalPages = magPageList.Count;
				//magPageList[i].ModelControllerProperty = this;
			}
		}


		public UIViewController GetViewController(int index, Boolean isPrev)
		{
			if (index >= magPageList.Count)
				return null;

			if (index == 0)
				PageIndex = 0;

			if (isPrev)
				PageIndex -=1;
			else
			PageIndex +=1;





			var currentPage = magPageList[index];

			UIViewController returnViewController;

			var device = UIDevice.CurrentDevice;



			Boolean isAd = PageIndex % 5 == 0 ? true : false;

			int layout = PageIndex % 3;

			if (device.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{



				layout = isAd ? 3 : layout;


				if (layout == 1) {
					if (!magPageList[index].Ads[0].IsFeatured)
						layout = 2;
				}

				if (magPageList[index].Ads[0].IsFeatured)
					layout = 1;

				if (isPrev)
				{


					var data2 = magPageList[index - 1 >= 0 ? index - 1 : index];
					if (data2.Ads[0].IsFeatured)
					layout = 1;
					var data3 = magPageList[index - 2 >= 0 ? index - 2 : index + 1];
					if (data3.Ads[0].IsFeatured)
					layout = 1;
				}

				//if (index < 4 )
				//	layout = 1;

				//if (index > 4 && layout == 1)
				//	layout = 0;

			}
			else {
				layout = isAd ? 3: 1;
				
			}


			var ad1 = magPageList[index];

            //TODO: Remove after testing
            layout = 0;


			switch (layout)
			{

				case 0:
					{
						AdLayout2ViewController adLayout1 = (AdLayout2ViewController)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayout2ViewController");


						if (isPrev)
						{

							var data1 = magPageList[index - 1];
							var data2 = magPageList[index - 2 >= 0 ? index - 2 : index];

							adLayout1.DataObject = data2;
							adLayout1.DataObject.Ads.Add(data1.Ads[0]);
						}
						else
						{

							var data1 = magPageList[index];
							var data2 = magPageList[((index + 1) < magPageList.Count) ? index + 1 : index - 1];

							adLayout1.DataObject = data2;
							adLayout1.DataObject.Ads.Add(data1.Ads[0]);
						}
						//adLayout1.AdList = magPageList[index].Ads;

						returnViewController = adLayout1 as UIViewController;
						break;
					}




					case 1:
					{ 
                        //var phoneLayout = (IAdLayoutInterface)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "Main_ipad" : "Main", null).InstantiateViewController(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "AdLayoutPhoneViewController_iPad" : "AdLayoutPhoneViewController");
                        var phoneLayout = (IAdLayoutInterface)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayoutPhoneViewController_iPad");


						phoneLayout.DataObject = magPageList[index];
						//phoneLayout.AdList = magPageList[index].Ads;

						returnViewController = phoneLayout as UIViewController;
						break;
					}

					case 2:
					{

                   Random rnd = new Random();
                   var randomDouble = rnd.NextDouble();


						AdLayout1ViewController adLayout1 = (AdLayout1ViewController)UIStoryboard.FromName("Main_ipad", null).InstantiateViewController("AdLayout11ViewController");


						if (isPrev)
						{

							var data1 = magPageList[index - 1];
							var data2 = magPageList[index - 2>=0?index-2:index];
							var data3 = magPageList[index - 3>= 0 ? index - 3:index+1];

							adLayout1.DataObject = data3;
							adLayout1.DataObject.Ads.Add(data2.Ads[0]);
							adLayout1.DataObject.Ads.Add(data1.Ads[0]);
						}
						else { 

							var data1 = magPageList[index];
							var data2 = magPageList[((index + 1) < magPageList.Count) ? index + 1 : index - 1];
							var data3 = magPageList[((index + 2) < magPageList.Count) ? index + 2 : index - 2];

							adLayout1.DataObject = data3;
							adLayout1.DataObject.Ads.Add(data2.Ads[0]);
							adLayout1.DataObject.Ads.Add(data1.Ads[0]);
						}


						//adLayout1.AdList = magPageList[index].Ads;

						returnViewController = adLayout1 as UIViewController;
						break;
					}
					
					case 3:

					{
						BanManProViewController banManProLayout = (BanManProViewController)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "Registration_ipad" : "Registration", null).InstantiateViewController("BanManProViewController");

						banManProLayout.DataObject = magPageList[index];

						returnViewController = banManProLayout as UIViewController;
						break;
					}

				case 5: //banmanpro layout
					{
						AdLayoutPhoneViewController phoneLayout = (AdLayoutPhoneViewController)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?"Main_ipad":"Main", null).InstantiateViewController("AdLayoutPhoneViewController");
						phoneLayout.DataObject = magPageList[index];
						//phoneLayout.AdList = magPageList[index].Ads;

						returnViewController = phoneLayout as UIViewController;
						break;
					}

				case 4: //phone layout
					{
						BanManProViewController banManProLayout = (BanManProViewController)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "Registration_ipad" : "Registration", null).InstantiateViewController("BanManProViewController");

						banManProLayout.DataObject = magPageList[index];

						returnViewController = banManProLayout as UIViewController;
						break;
					}
				default:
					{
						//AdLayout1ViewController adLayout1 = (AdLayout1ViewController)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?"Main_ipad":"Main", null).InstantiateViewController("AdLayout1ViewController");
						//adLayout1.DataObject = magPageList[index];
						////adLayout1.AdList = magPageList[index].Ads;

						//returnViewController = adLayout1 as UIViewController;

						BanManProViewController banManProLayout = (BanManProViewController)UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? "Registration_ipad" : "Registration", null).InstantiateViewController("BanManProViewController");

						banManProLayout.DataObject = magPageList[index];

						returnViewController = banManProLayout as UIViewController;
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

