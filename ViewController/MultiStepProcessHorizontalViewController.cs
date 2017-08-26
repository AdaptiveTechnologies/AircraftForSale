using System;
using System.Linq;
using UIKit;
using Google.Analytics;

namespace AircraftForSale
{
	public class MultiStepProcessHorizontalViewController : UIPageViewController
	{

		public UIViewController ContainerViewController
		{
			get;
			set;
		}
		public MultiStepProcessHorizontalViewController(MultiStepProcessDataSource dataSource, UIViewController containerViewController)
: base(UIPageViewControllerTransitionStyle.Scroll,
	  UIPageViewControllerNavigationOrientation.Horizontal)
		{
			ContainerViewController = containerViewController;
			DataSource = dataSource;
			SetViewControllers(new[] { dataSource.Steps.FirstOrDefault() as UIViewController },
							   UIPageViewControllerNavigationDirection.Forward,
							   false,
							   null);
		}
	}
}

