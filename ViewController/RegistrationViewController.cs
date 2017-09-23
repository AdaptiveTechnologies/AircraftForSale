using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using CoreGraphics;
using Google.Analytics;

namespace AircraftForSale
{
	public partial class RegistrationViewController : UIViewController
	{
		public RegistrationViewController(IntPtr handle) : base(handle)
		{
		}

		private void HandleStepActivated(object sender, MultiStepProcessStepEventArgs args)
		{
			_pageControl.CurrentPage = args.Index;

			//RegistrationViewController registrationVC = (RegistrationViewController)((MultiStepProcessHorizontalViewController)this.ParentViewController).ContainerViewController;
			//if (Steps[args.Index] is RegistrationProfileViewController)
			//{
			//	UIViewController firstVC = Steps[0] as UIViewController;
			//	_pageViewController.SetViewControllers(new[] { firstVC }, UIPageViewControllerNavigationDirection.Reverse, false, (finished) =>
			//	{
			//		if (finished)
			//		{
			//		//finalStep.StepActivated(this, new MultiStepProcessStepEventArgs());

			//		}
			//	});
			//}
		}

		private void HandleStepDeactivated(object sender, MultiStepProcessStepEventArgs args)
		{
			//update the UI as required while transitioning between steps
		}





		public List<IMultiStepProcessStep> GetSteps()
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
			favClassificationsVC.CollectionView.BackgroundView = new UIImageView(UIImage.FromBundle("new_home_bg1"));
			favClassificationsVC.CollectionView.AllowsMultipleSelection = true;

			//RegistrationProfileViewController myInterestsVC = this.Storyboard.InstantiateViewController("MyProfileViewController") as RegistrationProfileViewController;

MainRegistrationViewController mainRegistrationVC = UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?"Registration_ipad":"Registration", null).InstantiateViewController("MainRegistrationViewController") as MainRegistrationViewController;

Page2RegistrationViewController page2RegistrationVC = UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?"Registration_ipad":"Registration", null).InstantiateViewController("Page2RegistrationViewController") as Page2RegistrationViewController;

Page3RegistrationViewController page3RegistrationVC = UIStoryboard.FromName(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ?"Registration_ipad":"Registration", null).InstantiateViewController("Page3RegistrationViewController") as Page3RegistrationViewController;

			var steps = new List<IMultiStepProcessStep>()
			{
				//favClassificationsVC,
				//myInterestsVC
					mainRegistrationVC,
				page2RegistrationVC,
				page3RegistrationVC
			};



			steps.ForEach(s =>
			{
				s.StepActivated += HandleStepActivated;
				s.StepDeactivated += HandleStepDeactivated;
			});

			return steps;
		}

		public MultiStepProcessHorizontalViewController _pageViewController;
		public UIPageControl _pageControl;

		private List<IMultiStepProcessStep> _steps;
		public List<IMultiStepProcessStep> Steps => _steps ?? (_steps = GetSteps());

		public override void LoadView()
		{
			base.LoadView();

			_pageViewController = new MultiStepProcessHorizontalViewController(new MultiStepProcessDataSource(Steps), this);
			_pageControl = new UIPageControl
			{
				CurrentPage = 0,
				Pages = Steps.Count,
				BackgroundColor = UIColor.Gray,
				PageIndicatorTintColor = UIColor.Green,//FromHex(0x4F8C13),//UIColor.Green,
				Frame = new CGRect(0, UIScreen.MainScreen.Bounds.Height - 50, UIScreen.MainScreen.Bounds.Width, 50)

			};


			View.Add(_pageViewController.View);
			View.Add(_pageControl);
		}



		public override void ViewDidLoad()
		{
			base.ViewDidLoad();


			if (this.ParentViewController is RegistrationNavigationController)
			{
				this.NavigationItem.SetRightBarButtonItem(
					new UIBarButtonItem(UIBarButtonSystemItem.Cancel, (sender, args) =>
					{
						this.DismissViewController(true, null);
					})
				, true);
			}
		}







	}
}