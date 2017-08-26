using Foundation;
using System;
using UIKit;

namespace AircraftForSale
{
    public partial class TextCollectionViewCell : UICollectionViewCell
    {
		#region Computed Properties
		public string AircraftName
		{
			get { return AircraftNameLabel.Text; }
			set { AircraftNameLabel.Text = value; }
		}

		public UIImage AircraftImageProperty
		{
			get { return AircraftImage.Image; }
			set { AircraftImage.Image = value; }
		}

		public string Manufacturer
		{
			get { return ManufacturerLabel.Text; }
			set { ManufacturerLabel.Text = value; }
		}

		public string BrokerName
		{
			get { return BrokerNameLabel.Text; }
			set { BrokerNameLabel.Text = value; }
		}

		public string AdDescriptionProperty
		{
			get { return AdDescription.Text; }
			set { AdDescription.Text = value; }
		}

		//public string Classification
		//{
		//	get { return ClassificationLabel.Text; }
		//	set { ClassificationLabel.Text = value; }
		//}

		public string Price
		{
			get { return PriceLabel.Text; }
			set { PriceLabel.Text = value; }
		}
		#endregion

		public TextCollectionViewCell (IntPtr handle) : base (handle)
        {

        }
    }
}