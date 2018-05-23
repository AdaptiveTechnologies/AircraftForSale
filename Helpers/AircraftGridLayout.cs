using System;
using UIKit;

namespace AircraftForSale
{
	public class AircraftGridLayout : GridLayout
    {
		public AircraftGridLayout(UIViewController viewController)
        {
			int width, height;

			//if(UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad){
			//	width = 200;
			//	height = 150;
			//}else{
			//	var horizontalClass = viewController.TraitCollection.HorizontalSizeClass;
   //             switch (horizontalClass)
   //             {
   //                 case UIUserInterfaceSizeClass.Compact:
   //                     {
			//				width = 90;
			//				height = 90;

   //                         break;
   //                     }

   //                 case UIUserInterfaceSizeClass.Regular:
   //                     {
			//				width = 100;
   //                         height = 100;
   //                         break;
   //                     }
                        
   //                 default:
   //                     {
			//				width = 90;
   //                         height = 90;
   //                         break;
   //                     }
   //             }
			//}
           
			width = (int)(viewController.View.Bounds.Width / 3.8);
			//height = (int)(viewController.View.Bounds.Height / 8);
            
            ItemSize = new CoreGraphics.CGSize(width, width);

			int insetTop = (int)(viewController.View.Bounds.Height / 10);
			int insetLeftBottomRight = (int)(viewController.View.Bounds.Width/ 14);

            SectionInset = new UIEdgeInsets(insetTop, insetLeftBottomRight, insetLeftBottomRight, insetLeftBottomRight);
           // HeaderReferenceSize = new CoreGraphics.CGSize(UIScreen.MainScreen.Bounds.Width - 100, 0);

        }
    }
}
