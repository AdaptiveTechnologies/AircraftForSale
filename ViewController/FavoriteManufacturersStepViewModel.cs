using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class FavoriteManufacturersStepViewModel : UIViewController, IMultiStepProcessStep
    {
        public FavoriteManufacturersStepViewModel (IntPtr handle) : base (handle)
        {
        }

		//public FavoriteManufacturersStepViewModel() { }

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			StepActivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			StepDeactivated?.Invoke(this, new MultiStepProcessStepEventArgs { Index = StepIndex });
		}

		public int StepIndex { get; set; }
		public event EventHandler<MultiStepProcessStepEventArgs> StepActivated;
		public event EventHandler<MultiStepProcessStepEventArgs> StepDeactivated;
    }
}