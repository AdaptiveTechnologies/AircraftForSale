// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace AircraftForSale
{
	[Register ("LoginViewController")]
	partial class LoginViewController
	{
		[Outlet]
		UIKit.UIImageView BackgroundImageView { get; set; }

		[Outlet]
		UIKit.UIButton changePassButton { get; set; }

		[Outlet]
		UIKit.UIProgressView DataUpdateProgressView { get; set; }

		[Outlet]
		UIKit.UIButton LaterButton { get; set; }

		[Outlet]
		UIKit.UIButton LoginButton { get; set; }

		[Outlet]
		UIKit.UIButton LogoutButton { get; set; }

		[Outlet]
		UIKit.UITextField PasswordTextField { get; set; }

		[Outlet]
		UIKit.UIButton RegisterNowButton { get; set; }

		[Outlet]
		UIKit.UITabBarItem RegisterTabBarButton { get; set; }

		[Outlet]
		UIKit.UIButton UpdateMyProfileButton { get; set; }

		[Outlet]
		UIKit.UITextField UsernameTextField { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (BackgroundImageView != null) {
				BackgroundImageView.Dispose ();
				BackgroundImageView = null;
			}

			if (changePassButton != null) {
				changePassButton.Dispose ();
				changePassButton = null;
			}

			if (LaterButton != null) {
				LaterButton.Dispose ();
				LaterButton = null;
			}

			if (LoginButton != null) {
				LoginButton.Dispose ();
				LoginButton = null;
			}

			if (LogoutButton != null) {
				LogoutButton.Dispose ();
				LogoutButton = null;
			}

			if (PasswordTextField != null) {
				PasswordTextField.Dispose ();
				PasswordTextField = null;
			}

			if (RegisterNowButton != null) {
				RegisterNowButton.Dispose ();
				RegisterNowButton = null;
			}

			if (RegisterTabBarButton != null) {
				RegisterTabBarButton.Dispose ();
				RegisterTabBarButton = null;
			}

			if (UpdateMyProfileButton != null) {
				UpdateMyProfileButton.Dispose ();
				UpdateMyProfileButton = null;
			}

			if (UsernameTextField != null) {
				UsernameTextField.Dispose ();
				UsernameTextField = null;
			}

			if (DataUpdateProgressView != null) {
				DataUpdateProgressView.Dispose ();
				DataUpdateProgressView = null;
			}
		}
	}
}
