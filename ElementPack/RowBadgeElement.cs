//
// RowBadgeElement.cs: StyledStringElements with badges
//
// Authors:
//   Aleksander Heintz (http://alxandr.me)
//   Miguel de Icaza (miguel@gnome.org)
//
// Licensed under the terms of the MIT X11 license
//
using System;
using System.Drawing;
using System.Reflection;

using MonoTouch.Foundation;
using MonoTouch.Dialog;
using MonoTouch.UIKit;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;

namespace ElementPack
{
	/// <summary>
	/// Row badge element. Ported from https://github.com/tmdvs/TDBadgedCell
	/// </summary>
	public class RowBadgeElement : StyledStringElement
	{
		static NSString [] skey = { new NSString ("rbe.1"), new NSString ("rbe.2"), new NSString ("rbe.3"), new NSString ("rbe.4") };

		public static NSString GetKey (int style)
		{
			return skey [style];
		}
		
		string badgeValue;
		
		public UIColor Color { get; set; }
		public UIColor HighlightColor { get; set; }
		public float Radius { get; set; }
		public bool Shadow { get; set; }
		
		MethodInfo prepare_cell_method_info;
		
		Delegate d;
		public RowBadgeElement (string caption) : base(caption)
		{
			HighlightColor = UIColor.FromRGBA (1f, 1f, 1f, 1f);
			Color = UIColor.FromRGBA (0.530f, 0.600f, 0.738f, 1f);
			Radius = 4f;
			
			// Slow code path to workaround the fact that PrepareCell was not public in the past
			prepare_cell_method_info = typeof (StyledStringElement).GetMethod ("PrepareCell", BindingFlags.Instance | BindingFlags.NonPublic);
		}
		
		public RowBadgeElement (string caption, string badgeValue) : this(caption)
		{
			this.badgeValue = badgeValue;
		}
		
		public RowBadgeElement (string caption, NSAction tapped) : this(caption)
		{
			Tapped += tapped;
		}
		
		public RowBadgeElement (string caption, string badgeValue, NSAction tapped) : this(caption, badgeValue)
		{
			Tapped += tapped;
		}
		
		public event NSAction Tapped;
		
		public override UITableViewCell GetCell (UITableView tv)
		{
			var key = GetKey ((int) style);
			BadgeCell cell = (BadgeCell)tv.DequeueReusableCell (key);
			if (cell == null) {
				cell = new BadgeCell (key);
				cell.SelectionStyle = (Tapped != null) ? UITableViewCellSelectionStyle.Blue : UITableViewCellSelectionStyle.None;
			}
			prepare_cell_method_info.Invoke (this, new object [] { cell });
			//PrepareCell (cell);

			cell.BadgeText = badgeValue;
			cell.Color = Color;
			cell.Radius = Radius;
			cell.HighlightColor = HighlightColor;
			cell.ShowShadow = Shadow;
			cell.SetNeedsLayout ();
			
			return cell;
		}
		
		public override string Summary ()
		{
			return Caption;
		}
		
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath indexPath)
		{
			if (Tapped != null)
				Tapped ();
			tableView.DeselectRow (indexPath, true);
		}
		
		public override bool Matches (string text)
		{
			return (badgeValue != null ? badgeValue.IndexOf (text, StringComparison.CurrentCultureIgnoreCase) != -1 : false) || base.Matches (text);
		}
		
		private class BadgeView : UIView
		{
			static float fontSize = 11;
			
			public string Text;
			public float Radius = 4f;
			public BadgeCell Parent;
			public UIColor HighlightColor = UIColor.FromRGBA (1f, 1f, 1f, 1f);
			public UIColor Color = UIColor.FromRGBA (0.530f, 0.600f, 0.738f, 1f);
			public bool Shadow;
			
			public BadgeView (RectangleF rect) : base(rect)
			{
				BackgroundColor = UIColor.Clear;
			}
			
			public override void Draw (RectangleF rect)
			{
				var fontSize = BadgeView.fontSize;
				var text = new NSString (Text);
				var numberSize = text.StringSize (UIFont.BoldSystemFontOfSize (fontSize));
				var radius = Radius;
				
				var bounds = new RectangleF (PointF.Empty, new SizeF (numberSize.Width + 12f, 18f));
				UIColor color;
				if (Parent.SelectionStyle != UITableViewCellSelectionStyle.None && (Parent.Highlighted || Parent.Selected)) {
					color = HighlightColor;
				} else {
					color = Color;
				}
				
				bounds.X = (bounds.Width - numberSize.Width) / 2f + .5f;
				bounds.Y += 2f;
				
				CALayer badge = new CALayer ();
				badge.Frame = rect;
				
				var imageSize = badge.Frame.Size;
				
				// Render the image @x2 for retina people
				if (UIScreen.MainScreen.Scale == 2) {
					imageSize = new SizeF (imageSize.Width * 2, imageSize.Height * 2);
					badge.Frame = new RectangleF (badge.Frame.Location, new SizeF (
						badge.Frame.Width * 2, badge.Frame.Height * 2));
					
					fontSize *= 2;
					bounds.X = ((bounds.Width * 2) - (numberSize.Width * 2)) / 2f + 1;
					bounds.Y += 3;
					bounds.Width *= 2;
					radius *= 2;
				}
				
				badge.BackgroundColor = color.CGColor;
				badge.CornerRadius = radius;
				
				UIGraphics.BeginImageContext (imageSize);
				var context = UIGraphics.GetCurrentContext ();
				
				context.SaveState ();
				badge.RenderInContext (context);
				context.RestoreState ();
				
				context.SetBlendMode (CGBlendMode.Clear);
				text.DrawString (bounds, UIFont.BoldSystemFontOfSize (fontSize), UILineBreakMode.Clip);
				context.SetBlendMode (CGBlendMode.Normal);
				
				var outputImage = UIGraphics.GetImageFromCurrentImageContext ();
				UIGraphics.EndImageContext ();
				
				outputImage.Draw (rect);
				if (Parent.SelectionStyle == UITableViewCellSelectionStyle.None && (Parent.Highlighted || Parent.Selected) && Shadow) {
					Layer.CornerRadius = radius;
					Layer.ShadowOffset = new SizeF (0f, 1f);
					Layer.ShadowRadius = 1f;
					Layer.ShadowOpacity = .8f;
				} else {
					Layer.CornerRadius = radius;
					Layer.ShadowOffset = SizeF.Empty;
					Layer.ShadowRadius = 0f;
					Layer.ShadowOpacity = 0f;
				}
			}
		}
		
		private class BadgeCell : UITableViewCell
		{
			private BadgeView badge;
			
			public string BadgeText;
			public bool ShowShadow;
			public UIColor HighlightColor = UIColor.FromRGBA (1f, 1f, 1f, 1f);
			public UIColor Color = UIColor.FromRGBA (0.530f, 0.600f, 0.738f, 1f);
			public float Radius = 4f;
			
			public BadgeCell (NSString cellIdentifier) : base(UITableViewCellStyle.Default, cellIdentifier)
			{
				badge = new BadgeView (RectangleF.Empty);
				badge.Parent = this;
				
				ContentView.AddSubview (badge);
				badge.SetNeedsDisplay ();
			}
			
			public override void LayoutSubviews ()
			{
				base.LayoutSubviews ();
				
				if (!string.IsNullOrEmpty (BadgeText)) {
					if (Editing)
						badge.Hidden = true;
					else
						badge.Hidden = false;
					
					var badgeSize = new NSString (BadgeText).StringSize (UIFont.BoldSystemFontOfSize (11));
					var badgeFrame = new RectangleF (ContentView.Frame.Width - (badgeSize.Width + 25f),
					                                (float)Math.Round ((ContentView.Frame.Height - 18f) / 2f),
					                                badgeSize.Width + 13f, 18f);
					badge.Shadow = ShowShadow;
					badge.Frame = badgeFrame;
					badge.Text = BadgeText;
					badge.Radius = Radius;
					
					float badgeWidth;
					if (TextLabel.Frame.Right >= badgeFrame.X) {
						badgeWidth = TextLabel.Frame.Width - badgeFrame.Width - 10;
						TextLabel.Frame = new RectangleF (TextLabel.Frame.Location, new SizeF (badgeWidth, TextLabel.Frame.Height));
					}
					
					badge.HighlightColor = HighlightColor;
					badge.Color = Color;
				} else {
					badge.Hidden = true;
				}
			}
			
			public override void SetHighlighted (bool highlighted, bool animated)
			{
				base.SetHighlighted (highlighted, animated);
				badge.SetNeedsDisplay ();
			}
		}
	}
}

