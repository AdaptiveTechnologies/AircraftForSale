using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;

namespace AircraftForSale
{
    public partial class SearchResultsTableCell : UITableViewCell
    {
		WeakReference parent;
		public SearchResultsTableSource Owner
		{
			get
			{
				return parent.Target as SearchResultsTableSource;
			}
			set
			{
				parent = new WeakReference(value);
			}
		}
		void NotesButton_TouchUpInside (object sender, EventArgs e)
		{
			var window = UIApplication.SharedApplication.KeyWindow;
			var vc = window.RootViewController;
			while (vc.PresentedViewController != null)
			{
				vc = vc.PresentedViewController;
			}



			NotesInputOverlay notesOverlay = new NotesInputOverlay(vc.View.Frame, (note) =>
			{
				if (AdProperty != null)
				{
					AdProperty.Notes = note;
					this.NotesLabel.Text = AdProperty.Notes;
					AdProperty.IsModified = true;

					if (Owner != null && Owner.Owner != null)
					{
						Owner.Owner.SearchResultsTableViewProperty.ReloadData();
					}

					//this.SetNeedsUpdateConstraints();
					//this.SetNeedsDisplay();
				}

			});

			notesOverlay.notesTextView.Text = NotesLabel.Text;

			vc.View.AddSubview(notesOverlay);

		}

		public SearchResultsTableCell (IntPtr handle) : base (handle)
        {
			

			BackgroundView = null;
			BackgroundColor = UIColor.Clear;


        }

		public override void AwakeFromNib()
		{
			base.AwakeFromNib();
			if (UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad)
			{
				int fontsize = 15;
				this.AircraftNameLabel.Font = UIFont.BoldSystemFontOfSize(fontsize);
				this.AircraftDetailsLabel.Font = UIFont.SystemFontOfSize(fontsize);
				this.NotesLabel.Font = UIFont.SystemFontOfSize(fontsize);
			}
		}

		public Ad AdProperty
		{
			get;
			set;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			this.NotesButton.TouchUpInside -= NotesButton_TouchUpInside;
			this.NotesButton.TouchUpInside += NotesButton_TouchUpInside;

		}

		public void UpdateCell(Ad ad, SearchResultsTableSource owner)
		{
			AdProperty = ad;
			this.AircraftNameLabel.Text = AdProperty.Name;
			this.AircraftDetailsLabel.Text = AdProperty.Teaser;
			this.AircraftImage.Image = HelperMethods.FromUrl(AdProperty.ImageURL);
			this.NotesLabel.Text = AdProperty.Notes;
			this.BackgroundView = null;
			this.BackgroundColor = UIColor.Clear;
			this.Owner = owner;
		}

    }
}