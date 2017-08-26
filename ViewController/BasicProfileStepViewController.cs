using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class BasicProfileStepViewController : UIViewController, IMultiStepProcessStep
    {
        public BasicProfileStepViewController (IntPtr handle) : base (handle)
        {
        }

		//public BasicProfileStepViewController() { }

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