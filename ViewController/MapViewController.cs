using Foundation;
using System;
using UIKit;
using AircraftForSale.PCL;
using System.Collections.Generic;
using CoreLocation;
using MapKit;
using System.Linq;

namespace AircraftForSale
{
	public partial class MapViewController : UIViewController
	{
		//~MapViewController()
		//{
		//	Console.WriteLine("MapViewController is about to be collected");
		//}
		void Lm_LocationsUpdated(object sender, CLLocationsUpdatedEventArgs e)
		{
			CLLocation newLocation = e.Locations[e.Locations.Length - 1];
			lastKnowLocation = newLocation;

			if (normalRangeMeters != 0 || maxRangeMeters != 0)
			{
				if (RangeMapView.Overlays != null && RangeMapView.Overlays.Count() > 0)
				{
					return;
					//RangeMapView.RemoveOverlays(RangeMapView.Overlays);
				}

				if (normalRangeMeters != 0)
				{
					var normalRangeCircle = MKCircle.Circle(lastKnowLocation.Coordinate, normalRangeMeters);
					RangeMapView.AddOverlay(normalRangeCircle);
				}

				if (maxRangeMeters != 0)
				{
					var maxRangeCircle = MKCircle.Circle(lastKnowLocation.Coordinate, maxRangeMeters);
					RangeMapView.AddOverlay(maxRangeCircle);
				}


				MKMapCamera camera = new MKMapCamera
				{
					CenterCoordinate = lastKnowLocation.Coordinate,
					//Altitude = maxRangeMeters != 0 ? maxRangeMeters * 20 : normalRangeMeters *20
					Altitude = 100000000,
					Pitch = 0.0f,


				};

				RangeMapView.SetCamera(camera, true);

				View.BringSubviewToFront(CloseButton);


			}
		}

		void CloseButton_TouchUpInside(object sender, EventArgs e)
		{
			this.DismissViewController(true, null);
		}

		public List<SpecField> SpecFieldList
		{
			get;
			set;
		}

		CLLocationManager lm;

		CLLocation lastKnowLocation;

		int normalRangeMeters = 0;
		int maxRangeMeters = 0;

		public MapViewController(IntPtr handle) : base(handle)
		{
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			RangeMapView.MapType = MapKit.MKMapType.Hybrid;
			//RangeMapView.PitchEnabled = true;

			lm = new CLLocationManager { DesiredAccuracy = 500 };

			if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
			{
				lm.RequestWhenInUseAuthorization();
			}

			lastKnowLocation = lm.Location;

			if (CLLocationManager.LocationServicesEnabled)
			{
				lm.LocationsUpdated += Lm_LocationsUpdated;
				lm.StartUpdatingLocation();
			}

			RangeMapView.OverlayRenderer += (mapView, overlay) =>
			{
				if (overlay is MKCircle)
				{
					var renderer = new MKCircleRenderer((MKCircle)overlay)
					{
						FillColor = UIColor.FromRGBA(0f, .5f, 1f, .3f)
						//FillColor = UIColor.Purple
					};
					return renderer;
				}
				return null;
			};

			var normalRangeRadius = SpecFieldList.FirstOrDefault(row => row.FieldName == "Normal Range");
			var maxRangeRadius = SpecFieldList.FirstOrDefault(row => row.FieldName == "Max Range");


			normalRangeMeters = 0;
			if (normalRangeRadius != null)
			{
				int normRangeMiles;
				int.TryParse(normalRangeRadius.FieldValue.Split(' ')[0], out normRangeMiles);

				normalRangeMeters = (int)((double)normRangeMiles * 1852d);
			}

			maxRangeMeters = 0;
			if (maxRangeRadius != null)
			{
				int maxRangeMiles;
				int.TryParse(maxRangeRadius.FieldValue.Split(' ')[0], out maxRangeMiles);
				maxRangeMeters = (int)((double)maxRangeMiles * 1852d); ;

			}




		}

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);

			CloseButton.TouchUpInside += CloseButton_TouchUpInside;
		}

		public override void ViewDidAppear(bool animated)
		{
			base.ViewDidAppear(animated);
			if (CLLocationManager.Status == CLAuthorizationStatus.Denied)
			{
				if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
				{
					var alert = UIAlertController.Create("Unable to determine location", "Would you like to change your location permissions?", UIAlertControllerStyle.Alert);
					alert.AddAction(UIAlertAction.Create("Yes", UIAlertActionStyle.Default, (UIAlertAction obj) =>
					{
						var settingsString = UIApplication.OpenSettingsUrlString;
						var url = new NSUrl(settingsString);
						UIApplication.SharedApplication.OpenUrl(url);
					}));
					alert.AddAction(UIAlertAction.Create("No Thanks", UIAlertActionStyle.Cancel, null));
					this.PresentViewController(alert, true, null);
				}
				else {
					HelperMethods.SendBasicAlert("Unable to determine location", "To change this, go to Settings and change location permissions for this app.");
				}

			}



		}

		public override void ViewDidDisappear(bool animated)
		{
			base.ViewDidDisappear(animated);

			CloseButton.TouchUpInside -= CloseButton_TouchUpInside;
		}
	}
}