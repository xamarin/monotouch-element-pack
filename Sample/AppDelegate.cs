//
// Sample program to show new elements in the Element Pack
//

using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.Dialog;

using ElementPack;

namespace Sample
{
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		UIWindow window;
		DialogViewController main;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			var badgedSection = new Section () {
				new RowBadgeElement ("Default", () => {}),
				new RowBadgeElement ("Default with val", "10", () => {}),
				new RowBadgeElement ("Default with text", "text", () => {}),
				new RowBadgeElement ("Colored", "color", () => {}) {
					Color = UIColor.FromRGBA (0.792f, 0.197f, 0.219f, 1f)
				},
				new RowBadgeElement ("With radius", "9f", () => {}) {
					Radius = 9f,
					Color = UIColor.FromRGBA (0.197f, 0.592f, 0.219f, 1f)
				},
				new RowBadgeElement ("With radius/Shadow", "9f", () => {}) {
					Radius = 9f,
					Color = UIColor.FromRGBA (0.197f, 0.592f, 0.219f, 1f)
				},
				new RowBadgeElement ("With radius and styled", "9f", () => {}) {
					BackgroundColor = UIColor.Red,
					Radius = 9f,
					Color = UIColor.FromRGBA (0.197f, 0.592f, 0.219f, 1f)
				},
			};
			
			var root = new RootElement ("ElementPack Samples") {
				new Section ("New styles"){
					new RootElement ("RowBadgeElement") { badgedSection },
				}
				
			};
			
			main = new DialogViewController (root);
			window = new UIWindow (UIScreen.MainScreen.Bounds){
				RootViewController = new UINavigationController (main)
			};
			window.MakeKeyAndVisible ();
			
			return true;
		}

		static void Main (string[] args)
		{
			UIApplication.Main (args, null, "AppDelegate");
		}
	}
}

