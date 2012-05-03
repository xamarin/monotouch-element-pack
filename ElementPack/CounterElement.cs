// 
// CounterElement.cs: // Author:
//   Guido Van Hoecke
//
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System;
using MonoTouch.Dialog;
using System.Drawing;
using System.Collections.Generic;

namespace ElementPack {
	public class CounterElement : StringElement {
		public UIPickerView counterPicker;
		protected CounterPickerDataModel model;

		protected class CounterPickerDataModel : UIPickerViewModel
		{
			public string Counter;
			public string Digits;
			int nrOfDecimals;
			int nrOfDigits;
			char comma;

			public event EventHandler<EventArgs> ValueChanged;
			
			public List<int> SelectedIndex {
				get { return _selectedIndex; }
			}
			List<int> _selectedIndex;
			
			public int NrOfDecimals {
				get { return nrOfDecimals; }
				set { nrOfDecimals = value; }
			}

			public List<List<string>> Items 
			{
				get { return this._items; }
				set { this._items = value; }
			}
			List<List<string>> _items = new List<List<string>>();

			public CounterPickerDataModel (string counter)
			{
				Counter = counter.Trim();
				var i = Counter.IndexOfAny(new char[] 
				                          {',', '.'});
				if (i < 0) {
					comma = ' ';
					Digits = Counter;
					nrOfDecimals = 0;
				} else {
					comma = Counter[i];
					Digits = Counter.Substring(0, i) + Counter.Substring(i + 1);
					nrOfDecimals = Digits.Length - i;
				}
				nrOfDigits = Digits.Length;

				_items = new List<List<string>>();
				_selectedIndex = new List<int>();

				// add overflow column (uses blank rather than 0) 
				SetDigit(-1);

				// add columns for all digits
				for(int d = 0; d < nrOfDigits; d++) 
				{
					SetDigit(d);
				}
			}
			
			private void SetDigit(int d, bool isFirstDecimal = false) {
				if (d < 0 || Digits[d] < '0' || Digits[d] > '9') {
					_selectedIndex.Add(0);
				} else {
					var n = Digits[d] - '0';
					_selectedIndex.Add(n);
				} 
				_items.Add(new List<string> { 
					"0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
				if (d < 0) {
					// show blank iso '0' in the overflow column
					_items[0][0] = "";
				}
			}

			public override int GetRowsInComponent (UIPickerView picker, int component)
			{
				return this._items[component].Count;
			}

			public override UIView GetView(UIPickerView picker, int row, int component, UIView view) {
				var label = new UILabel {
					Text = this._items[component][row],
					TextAlignment = UITextAlignment.Center,
				};
				if (row == SelectedIndex[component]) {
					label.TextColor = UIColor.Magenta;
				} 
				if (component >= Items.Count - NrOfDecimals) {
					label.BackgroundColor = UIColor.FromRGB(208, 208, 208);
				}
				return label;
			}


			public override int GetComponentCount (UIPickerView picker) {
				return this._items.Count;
			}

			public override void Selected (UIPickerView picker, int row, int component) {
				this._selectedIndex[component] = row;
				if (this.ValueChanged != null) {
					this.ValueChanged (this, new EventArgs ());
				}
			} 

			public string FormatValue () 
			{
				var started = false;
				var s = new List<string>();
				for (int d = 0; d < _items.Count - nrOfDecimals; d++) {
					started |= _selectedIndex[d] > 0; 
					if (started) {
						s.Add(_selectedIndex[d].ToString());
					}
				}
				if (s.Count == 0) {
					s.Add("0");
				}
				if (nrOfDecimals > 0) {
					s.Add(comma.ToString());
					for (int d = _items.Count - nrOfDecimals; d < _items.Count; d++) {
						s.Add(_selectedIndex[d].ToString());
					}
				}
				Counter = string.Join("", s.ToArray());
				return Counter;
			}
		} 
		
		public CounterElement(string caption, string counter) : base (caption) {
			model = new CounterPickerDataModel(counter);
		}

		public override UITableViewCell GetCell (UITableView tv)
		{
			Value = model.FormatValue();
			var cell = base.GetCell(tv);
			cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			return cell;
		}

		protected override void Dispose (bool disposing)
		{ 
			base.Dispose(disposing);
			if (disposing) {
				if (model != null) {
					model.Dispose();
					model = null;
				}
				if (counterPicker != null) {
					counterPicker.Dispose();
					counterPicker = null;
				}
			}
		}

		public virtual UIPickerView CreatePicker ()
		{
			var picker = new UIPickerView (RectangleF.Empty){
				AutoresizingMask = UIViewAutoresizing.FlexibleWidth,
				ShowSelectionIndicator = true,

			};
			return picker;
		}
		                                                                                                                                
		static RectangleF PickerFrameWithSize (SizeF size)
		{                                                                                                                                    
			var screenRect = UIScreen.MainScreen.ApplicationFrame;
			float fY = 0, fX = 0;
			
			switch (UIApplication.SharedApplication.StatusBarOrientation){
			case UIInterfaceOrientation.LandscapeLeft:
			case UIInterfaceOrientation.LandscapeRight:
				fX = (screenRect.Height - size.Width) /2;
				fY = (screenRect.Width - size.Height) / 2 -17;
				break;
				
			case UIInterfaceOrientation.Portrait:
			case UIInterfaceOrientation.PortraitUpsideDown:
				fX = (screenRect.Width - size.Width) / 2;
				fY = (screenRect.Height - size.Height) / 2 - 25;
				break;
			}
			
			return new RectangleF (fX, fY, size.Width, size.Height);
		}                                                                                                                                    

		class MyViewController : UIViewController {
			CounterElement container;
			
			public MyViewController (CounterElement container)
			{
				this.container = container;
			}
			
			public override void ViewWillDisappear (bool animated)
			{
				base.ViewWillDisappear (animated);
			}
			
			public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
			{
				base.DidRotate (fromInterfaceOrientation);
				container.counterPicker.Frame = PickerFrameWithSize (container.counterPicker.SizeThatFits (SizeF.Empty));
			}
			
			public bool Autorotate { get; set; }
			
			public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
			{
				return Autorotate;
			}
		}
		
		public override void Selected (DialogViewController dvc, UITableView tableView, NSIndexPath path)
		{
			model = new CounterPickerDataModel(model.Counter);
			var vc = new MyViewController (this) {
				Autorotate = dvc.Autorotate
			};
			counterPicker = CreatePicker ();
			counterPicker.Frame = PickerFrameWithSize (counterPicker.SizeThatFits (SizeF.Empty));
			counterPicker.Model = model;
			for (int d = 0; d < model.Items.Count; d++) {
				counterPicker.Select(model.SelectedIndex[d], d, true);
			}
			vc.View.BackgroundColor = UIColor.Black;
			vc.View.AddSubview (counterPicker);
			dvc.ActivateController (vc);
		}
	}
}	