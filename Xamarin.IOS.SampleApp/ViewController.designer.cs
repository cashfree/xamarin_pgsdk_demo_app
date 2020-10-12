// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace ios_sample_app
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		UIKit.UIActivityIndicatorView ActivityIndicator { get; set; }

		[Outlet]
		UIKit.UIButton Button { get; set; }

		[Outlet]
		UIKit.UITextView TextView { get; set; }

		[Action ("Button_TouchUpInside:")]
		partial void Button_TouchUpInside (UIKit.UIButton sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ActivityIndicator != null) {
				ActivityIndicator.Dispose ();
				ActivityIndicator = null;
			}

			if (Button != null) {
				Button.Dispose ();
				Button = null;
			}

			if (TextView != null) {
				TextView.Dispose ();
				TextView = null;
			}
		}
	}
}
