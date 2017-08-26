using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace AircraftForSale
{
	public class NotesInputOverlay : UIView
	{
		// control declarations
		UILabel notesLabel;
		public UITextView notesTextView;
		UIButton cancelNotesButton;
		UIButton updateNotesButton;
		Action<string> updateCallbackAction;

		//variables to manage moving input elements on keyboard show
		#region managing keyboard variables
		private UIView activeview;             // Controller that activated the keyboar
		private float scroll_amount = 0.0f;    // amount to scroll
		private float bottom = 0.0f;           // bottom poin
		private float offset = 10.0f;          // extra offse
		private bool moveViewUp = false;           // which direction are we moving

		private UITapGestureRecognizer HideKeyboardGesture;


		#endregion

		~NotesInputOverlay()
		{
			Console.WriteLine("NotesInputOverlay is about to be collected");
		}

		public NotesInputOverlay(CGRect frame, Action<string> callbackAction) : base(frame)
		{

			updateCallbackAction = callbackAction;
			// configurable bits
			BackgroundColor = UIColor.Black.ColorWithAlpha(.7f);
			AutoresizingMask = UIViewAutoresizing.All;

			nfloat labelHeight = 22;
			nfloat labelWidth = Frame.Width - 20;

			nfloat notesTextViewWidth = Frame.Width * .7f;
			nfloat notesTextViewHeight = 60f;

			// derive the center x and y
			nfloat centerX = Frame.Width / 2;
			nfloat centerY = Frame.Height / 2;

			notesLabel = new UILabel(new CGRect(
				centerX - (labelWidth / 2),
				centerY - labelHeight,
				labelWidth,
				labelHeight
				));
			notesLabel.BackgroundColor = UIColor.Clear;
			notesLabel.TextColor = UIColor.White;
			notesLabel.Text = "Aircraft Notes";
			notesLabel.TextAlignment = UITextAlignment.Center;
			notesLabel.AutoresizingMask = UIViewAutoresizing.All;
			AddSubview(notesLabel);

			notesTextView = new UITextView(new CGRect((this.Frame.Width - notesTextViewWidth) / 2, centerY + labelHeight, notesTextViewWidth, notesTextViewHeight));
			notesTextView.AutoresizingMask = UIViewAutoresizing.All;
			AddSubview(notesTextView);

			cancelNotesButton = new UIButton(UIButtonType.RoundedRect);
			cancelNotesButton.Frame = new CGRect(notesTextView.Frame.Left, notesTextView.Frame.Bottom + labelHeight, notesTextViewWidth / 2f, labelHeight);
			cancelNotesButton.SetTitle("CANCEL", UIControlState.Normal);
			cancelNotesButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			cancelNotesButton.AutoresizingMask = UIViewAutoresizing.All;
			cancelNotesButton.TouchUpInside += CancelNotesButton_TouchUpInside;

			AddSubview(cancelNotesButton);

			updateNotesButton = new UIButton(UIButtonType.RoundedRect);
			updateNotesButton.Frame = new CGRect(notesTextView.Frame.Left + (notesTextViewWidth * .5f), notesTextView.Frame.Bottom + labelHeight, notesTextViewWidth / 2f, labelHeight);
			updateNotesButton.SetTitle("UPDATE", UIControlState.Normal);
			updateNotesButton.SetTitleColor(UIColor.White, UIControlState.Normal);
			updateNotesButton.AutoresizingMask = UIViewAutoresizing.All;
			updateNotesButton.TouchUpInside += UpdateNotesButton_TouchUpInside;

			AddSubview(updateNotesButton);

			//hide keyboard when touch anywhere
			HideKeyboardGesture = new UITapGestureRecognizer(() => this.EndEditing(true));

			this.AddGestureRecognizer(HideKeyboardGesture);

			//keyboard observers
			// Keyboard popup
			NSNotificationCenter.DefaultCenter.AddObserver
			(UIKeyboard.DidShowNotification, KeyBoardUpNotification);

			// Keyboard Down
			NSNotificationCenter.DefaultCenter.AddObserver
						(UIKeyboard.WillHideNotification, KeyBoardDownNotification);

		}

		/// <summary>
		/// Fades out the control and then removes it from the super view
		/// </summary>
		public void Hide()
		{
			UIView.Animate(
				0.5, // duration
				() => { Alpha = 0; },
				() =>
				{
					notesTextView.ResignFirstResponder();
					RemoveFromSuperview();
					cancelNotesButton.TouchUpInside -= CancelNotesButton_TouchUpInside;
					updateNotesButton.TouchUpInside -= UpdateNotesButton_TouchUpInside;

					this.RemoveGestureRecognizer(HideKeyboardGesture);

					//keyboard observers
					// Keyboard popup
					NSNotificationCenter.DefaultCenter.RemoveObserver
					(UIKeyboard.DidShowNotification);

					//Keyboard Down
					NSNotificationCenter.DefaultCenter.RemoveObserver
								(UIKeyboard.WillHideNotification);

					updateCallbackAction = null;

					//GC.Collect();
				}	
			);
		}

		void CancelNotesButton_TouchUpInside(object sender, EventArgs e)
		{
			Hide();
		}
		void UpdateNotesButton_TouchUpInside(object sender, EventArgs e)
		{
			if (updateCallbackAction != null)
			{
				updateCallbackAction.Invoke(notesTextView.Text);
			}
			Hide();
		}

		#region keyboard management methods
		private void KeyBoardUpNotification(NSNotification notification)
		{
			// get the keyboard size
			var val = (NSValue)notification.UserInfo.ValueForKey(UIKeyboard.FrameEndUserInfoKey);
			CGRect r = val.CGRectValue;

			// Find what opened the keyboar
			foreach (UIView view in this.Subviews)
			{
				if (view.IsFirstResponder)
				{
					activeview = view;
					break;
				}
			}

			// Bottom of the controller = initial position + height + offset     
			bottom = (float)(activeview.Frame.Y + activeview.Frame.Height + offset);

			// Calculate how far we need to scrol
			scroll_amount = (float)(r.Height - (this.Frame.Size.Height - bottom));

			// Perform the scrollin
			if (scroll_amount > 0)
			{
				moveViewUp = true;
				ScrollTheView(moveViewUp);
			}
			else {
				moveViewUp = false;
			}

		}

		private void KeyBoardDownNotification(NSNotification notification)
		{
			if (moveViewUp) { ScrollTheView(false); }
		}

		private void ScrollTheView(bool move)
		{
			// scroll the view up or dow
			UIView.BeginAnimations(string.Empty, System.IntPtr.Zero);
			UIView.SetAnimationDuration(0.3);

			var frame = this.Frame;

			if (move)
			{
				frame.Y -= scroll_amount;
			}
			else {
				frame.Y += scroll_amount;
				scroll_amount = 0;
			}

			this.Frame = frame;
			UIView.CommitAnimations();
		}

		#endregion
	}
}
