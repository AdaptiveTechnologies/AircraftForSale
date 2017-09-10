using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class TestViewController : UIViewController
    {
        public TestViewController (IntPtr handle) : base (handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //this.View.TranslatesAutoresizingMaskIntoConstraints = false;
            //btnClickHere.TranslatesAutoresizingMaskIntoConstraints = false;

            var newConstraint = NSLayoutConstraint.Create(
                btnClickHere,
                NSLayoutAttribute.Top,
                NSLayoutRelation.Equal,
                this.View,
                NSLayoutAttribute.Top,
                1,
                30
            );

           

            btnClickHere.TouchUpInside += (sender, e) => {

                UIView.Animate(2d, () => {
					this.View.AddConstraint(newConstraint);
					this.View.RemoveConstraint(constraintBottom);
                    this.View.LayoutIfNeeded();
                });
            };

        }
    }
}