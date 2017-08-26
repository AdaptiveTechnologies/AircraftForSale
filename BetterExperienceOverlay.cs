using System;
using CoreGraphics;
using UIKit;

namespace AircraftForSale
{
	public class BetterExperienceOverlay : UIView
	{
		// control declarations
		UIButton registerButton;
		UIButton laterButton;
		UILabel solicitLabel;

		public UIViewController ParentViewController
		{
			get;
			set;
		}

		public BetterExperienceOverlay(CGRect frame) : base(frame)
		{
			int fontSizeForSolicitLabel = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 18 : 10;
			int fontSizeForRegisterButton = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 30 : 18;
			int fontSizeForLaterButton = UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? 15 : 10;

			// configurable bits
			BackgroundColor = UIColor.White.ColorWithAlpha(.7f);

			var centerPoint = new CGPoint(frame.Size.Width / 2, frame.Size.Height / 2);
	
			UIView backgroundModalView = new UIView(new CGRect(0,0, UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? frame.Size.Width * .5 : frame.Size.Width * .6, UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad ? frame.Size.Height * .25 : frame.Size.Height * .3));
			backgroundModalView.BackgroundColor = UIColor.White;
			backgroundModalView.Center = centerPoint;
			backgroundModalView.Layer.CornerRadius = (float)(frame.Size.Height * .45) / 4;
			backgroundModalView.ClipsToBounds = true;

			this.AddSubview(backgroundModalView);

			AutoresizingMask = UIViewAutoresizing.All;

			nfloat labelHeight = backgroundModalView.Frame.Height/ 2.75f;
			nfloat labelWidth = (System.nfloat)(backgroundModalView.Frame.Width * .75);

			// derive the center x and y
			nfloat centerX = backgroundModalView.Frame.Width / 2;
			nfloat centerY = backgroundModalView.Frame.Height / 2;



			solicitLabel = new UILabel(new CGRect(
				centerX - (labelWidth / 2),
				centerY - (centerY/1.5),
				labelWidth,
				labelHeight
				));
			solicitLabel.Lines = 0;
			solicitLabel.TextColor = UIColor.Black;
			solicitLabel.Text = "Registering for GlobalAir.com Showcase Magazine will help us optimize and refine your experience.";
			solicitLabel.TextAlignment = UITextAlignment.Center;
			solicitLabel.Font = UIFont.BoldSystemFontOfSize(fontSizeForSolicitLabel);
			solicitLabel.AutoresizingMask = UIViewAutoresizing.All;
			solicitLabel.LineBreakMode = UILineBreakMode.WordWrap;
		
			solicitLabel.AdjustsFontSizeToFitWidth = false;
			backgroundModalView.AddSubview(solicitLabel);

			registerButton = new UIButton(UIButtonType.RoundedRect);
			registerButton.SetTitle("REGISTER NOW!", UIControlState.Normal);
			registerButton.Frame = new CGRect(
				centerX - (labelWidth / 2),
				centerY,
				labelWidth,
				labelHeight
			);
			registerButton.Font = UIFont.BoldSystemFontOfSize(fontSizeForRegisterButton);
			registerButton.AutoresizingMask = UIViewAutoresizing.All;
			registerButton.TouchUpInside += (sender, e) =>
			{
				if (ParentViewController != null)
				{
					RegistrationViewController registrationViewController = (RegistrationViewController)ParentViewController.Storyboard.InstantiateViewController("RegistrationViewController");
					ParentViewController.NavigationController.PushViewController(registrationViewController, true);
					Hide();
				}
			};
			backgroundModalView.AddSubview(registerButton);

			laterButton = new UIButton(UIButtonType.RoundedRect);
			laterButton.SetTitle("I'LL REGISTER LATER", UIControlState.Normal);
			laterButton.Frame = new CGRect(
				centerX - (labelWidth / 2),
				centerY + (centerY / 2.5),
				labelWidth,
				labelHeight
			);
			laterButton.Font = UIFont.BoldSystemFontOfSize(fontSizeForLaterButton);
			laterButton.AutoresizingMask = UIViewAutoresizing.All;
			laterButton.TouchUpInside += (sender, e) =>
			{
				Hide();
			};
			backgroundModalView.AddSubview(laterButton);

		}

		/// <summary>
		/// Fades out the control and then removes it from the super view
		/// </summary>
		public void Hide()
		{
			UIView.Animate(
				0.5, // duration
				() => { Alpha = 0; },
				() => { RemoveFromSuperview(); }
			);
		}
	}
}
