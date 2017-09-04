using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AircraftForSale.PCL;
using AircraftForSale.PCL.Helpers;
using CoreGraphics;
using Foundation;
using UIKit;
using Google.Analytics;

namespace AircraftForSale
{
    public partial class MagazineFlipBoardViewController : UIViewController
    {
        public ModelController ModelController
        {
            get; private set;
        }

        public UIPageViewController PageViewController
        {
            get; private set;
        }

        public string SelectedClassification
        {
            get;
            set;
        }

        //~MagazineFlipBoardViewController()
        //{
        //	Console.WriteLine("MagazineFlipBoardViewController is about to be collected");
        //}

        protected MagazineFlipBoardViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);

            // This screen name value will remain set on the tracker and sent with
            // hits until it is set to a new value or to null.
            Gai.SharedInstance.DefaultTracker.Set(GaiConstants.ScreenName, "MagazineFlip View");

            Gai.SharedInstance.DefaultTracker.Send(DictionaryBuilder.CreateAppView().Build());
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            if (this.TabBarController != null)
            {
                this.TabBarController.TabBar.Hidden = true;
            }


            //Ensure database refresh
            //this.NavigationItem.SetRightBarButtonItem(
            var refreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, async (sender, args) =>
            {
                if (Reachability.IsHostReachable(Settings._baseDomain))
                {
                    LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame);
                    this.View.AddSubview(loadingIndicator);
                    // button was clicked
                    List<Ad> adList = new List<Ad>();


                    Ad.DeleteAdsByClassification(SelectedClassification);
                    adList = (await Ad.GetAdsByClassificationAsync(SelectedClassification)).ToList();


                    this.ModelController.LoadModalController(adList, SelectedClassification);

                    var firstViewController = ModelController.GetViewController(0, false);
                    var viewControllerArray = new UIViewController[] { firstViewController };
                    PageViewController.SetViewControllers(viewControllerArray, UIPageViewControllerNavigationDirection.Forward, true, null);

                    loadingIndicator.Hide();


                }
                else
                {
                    HelperMethods.SendBasicAlert("Connect to a Network", "Please connect to a network to refresh these ads");
                }
            });
            //, true);

            //start implementing search feature
            var searchButton = new UIBarButtonItem(UIBarButtonSystemItem.Search, (sender, e) =>
            {
                //Create Alert
                var searchAlertController = UIAlertController.Create("Search " + SelectedClassification + " Aircraft", "Search by manufacturer and/or model", UIAlertControllerStyle.Alert);

                //Add Text Input
                searchAlertController.AddTextField(textField =>
                {
                });

                //Add Actions
                var cancelAction = UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, alertAction => Console.WriteLine("Cancel was Pressed"));
                var okayAction = UIAlertAction.Create("Search", UIAlertActionStyle.Default, alertAction =>
                {

                    var errorMessage = "";
                    if (ModelController != null && ModelController.adList != null && ModelController.adList.Count > 0)
                    {
                        var adList = ModelController.adList;
                        var searchText = searchAlertController.TextFields[0].Text;
                        if (string.IsNullOrEmpty(searchText))
                        {
                            errorMessage = "Please enter a manufacturer and/or model.";
                            HelperMethods.SendBasicAlert("Oops", errorMessage);
                            return;
                        }
                        else
                        {
                            //search ad list

                            //first check to see if the whole search is contained in the ad name
                            var initialSearchResults = adList.Where(row => row.Name.ToLower().Contains(searchText.ToLower()));

                            List<Ad> finalSearchResults = new List<Ad>();
                            if (initialSearchResults != null && initialSearchResults.Count() > 0)
                            {

                                finalSearchResults = initialSearchResults.ToList();
                            }
                            else
                            {

                                //if no direct hits, use searchTermList to try and get more hits
                                //get search terms
                                var searchTermList = searchText.ToLower().Split(' ').ToList();
                                finalSearchResults = adList.Where(row => searchTermList.Contains(row.Name.ToLower())).ToList();

								if (finalSearchResults == null || finalSearchResults.Count() == 0)
								{
									HelperMethods.SendBasicAlert("Oops", "No aircraft found.");
									return;
                                    
                                }
                            }
                            SearchResultsViewController searchResultsViewController = new SearchResultsViewController();
                            searchResultsViewController.SearchResultsAdList = finalSearchResults;
                            searchResultsViewController.AdSelectedAction = delegate
                            {
                                var pageViewController = this.PageViewController;
                                var magFlipBoardViewController = this;



                                Ad ad = new Ad();

                                ad = searchResultsViewController.SelectedAd;

                                LoadingOverlay loadingIndicator = new LoadingOverlay(this.View.Frame, "Loading Aircraft");
                                this.View.AddSubview(loadingIndicator);

                                var modelController = this.ModelController;
                                List<Ad> searchAddList = new List<Ad>();

                                Task.Run(async () =>
                                {




                                    searchAddList = (await Ad.GetAdsByClassificationAsync(ad.Classification)).ToList();

                                        //get ads with this name and move them to the from of the liste
                                        List<Ad> similarAdList = new List<Ad>();

                                    similarAdList = searchAddList.Where(row => row.Name == ad.Name).ToList();

                                    similarAdList.Remove(ad);


                                    for (int i = 0; i < similarAdList.Count(); i++)
                                    {
                                        searchAddList.Remove(similarAdList[i]);
                                        searchAddList.Insert(0, similarAdList[i]);
                                    }

                                    searchAddList.Insert(0, ad);

                                    InvokeOnMainThread(() =>
                                    {
                                        modelController.LoadModalController(searchAddList, ad.Classification);
                                        loadingIndicator.Hide();
                                        var initialViewController = modelController.GetViewController(0, false);
                                        var searchViewControllers = new UIViewController[] { initialViewController };
                                        pageViewController.SetViewControllers(searchViewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

                                        HelperMethods.SendBasicAlert("", "Aircraft arranged based on search selection");
                                    });
                                });
                            };
                            this.PresentViewController(searchResultsViewController, true, null);


                            //var adListSearchResults = adList.Any(row => row.Name == searchText || searchTermList.Contains(row.Manufacturer) 
                            //|| searchTermList.Contains(row.                );

                            // var bPoint = "";
                        }
                    }
                    else
                    {
                        errorMessage = "Oops... There was a problem. Aircraft ads are not yet loaded.";
                    }
                    if (errorMessage != string.Empty)
                    {
                        HelperMethods.SendBasicAlert("Alert", errorMessage);
                    }
                });

                searchAlertController.AddAction(cancelAction);
                searchAlertController.AddAction(okayAction);

                //Present Alert
                PresentViewController(searchAlertController, true, null);
            });


            List<UIBarButtonItem> navButtonList = new List<UIBarButtonItem>();
            navButtonList.Add(searchButton);
            navButtonList.Add(refreshButton); ;


            this.NavigationItem.SetRightBarButtonItems(navButtonList.ToArray(), true);

            ModelController = new ModelController(SelectedClassification);

            // Configure the page view controller and add it as a child view controller.
            PageViewController = new UIPageViewController(UIPageViewControllerTransitionStyle.PageCurl, UIPageViewControllerNavigationOrientation.Horizontal, UIPageViewControllerSpineLocation.Min);
            PageViewController.WeakDelegate = this;

            var startingViewController = ModelController.GetViewController(0, false);
            var viewControllers = new UIViewController[] { startingViewController };
            PageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, false, null);

            PageViewController.WeakDataSource = ModelController;

            AddChildViewController(PageViewController);
            View.AddSubview(PageViewController.View);

            UINavigationController magazineNavigationController = (UINavigationController)Storyboard.InstantiateViewController("MagazineNavigationViewController");
            float heightTopNavBar = (float)magazineNavigationController.NavigationBar.Frame.Size.Height;
            float statusBarHeight = (float)UIApplication.SharedApplication.StatusBarFrame.Height;
            float totalStatusBarHeight = heightTopNavBar + statusBarHeight;
            var pageViewRect = new CGRect(0, totalStatusBarHeight, View.Bounds.Width, View.Bounds.Height - totalStatusBarHeight);

            PageViewController.View.Frame = pageViewRect;

            PageViewController.DidMoveToParentViewController(this);

            // Add the page view controller's gesture recognizers to the book view controller's view so that the gestures are started more easily.
            View.GestureRecognizers = PageViewController.GestureRecognizers;

        }

        [Export("pageViewController:spineLocationForInterfaceOrientation:")]
        public UIPageViewControllerSpineLocation GetSpineLocation(UIPageViewController pageViewController, UIInterfaceOrientation orientation)
        {
            UIViewController currentViewController;
            UIViewController[] viewControllers;

            if (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown || UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone)
            {
                // In portrait orientation or on iPhone: Set the spine position to "min" and the page view controller's view controllers array to contain just one view controller.
                // Setting the spine position to 'UIPageViewControllerSpineLocation.Mid' in landscape orientation sets the doubleSided property to true, so set it to false here.
                currentViewController = pageViewController.ViewControllers[0];
                viewControllers = new[] { currentViewController };
                pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

                pageViewController.DoubleSided = false;

                return UIPageViewControllerSpineLocation.Min;
            }

            // In landscape orientation: Set set the spine location to "mid" and the page view controller's view controllers array to contain two view controllers.
            // If the current page is even, set it to contain the current and next view controllers; if it is odd, set the array to contain the previous and current view controllers.
            currentViewController = pageViewController.ViewControllers[0];

            int index = ModelController.IndexOf((IAdLayoutInterface)currentViewController);
            //if (index == 0 || index % 2 == 0)
            {
                var nextViewController = ModelController.GetNextViewController(pageViewController, currentViewController);
                viewControllers = new[] { currentViewController, nextViewController };
            }
            //else {
            //	var previousViewController = ModelController.GetPreviousViewController(pageViewController, currentViewController);
            //	viewControllers = new[] { previousViewController, currentViewController };
            //}

            pageViewController.SetViewControllers(viewControllers, UIPageViewControllerNavigationDirection.Forward, true, null);

            return UIPageViewControllerSpineLocation.Mid;
        }

        public override void ViewDidDisappear(bool animated)
        {

            //Save modifications made to ads while browsing a magazine
            if (ModelController != null && ModelController.adList != null && ModelController.adList.Count > 0)
            {
                var adList = ModelController.adList.GroupBy(p => p.ID).Select(g => g.First()).ToList();
                var classifications = adList.Select(row => row.Classification).Distinct();
                if (classifications.Count() > 0)
                {
                    foreach (var classification in classifications)
                    {
                        Ad.SaveAdsByClassification(adList.Where(row => row.Classification == classification), classification);
                    }
                }
            }
            base.ViewDidDisappear(animated);
        }
    }
}

