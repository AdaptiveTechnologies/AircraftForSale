// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace AircraftForSale
{
    [Register ("SearchResultsViewController")]
    partial class SearchResultsViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView SearchResultsTableView { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (SearchResultsTableView != null) {
                SearchResultsTableView.Dispose ();
                SearchResultsTableView = null;
            }
        }
    }
}