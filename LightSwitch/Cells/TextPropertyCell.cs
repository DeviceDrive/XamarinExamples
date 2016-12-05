using System;
using DeviceDrive.SDK.Contracts;
using Xamarin.Forms;

namespace LightSwitch
{
	public class TextPropertyCell: BasePropertyCell
	{
		readonly Label _textLabel;
		readonly Label _valueLabel;

		public TextPropertyCell()
		{
			_textLabel = new Label
			{
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				VerticalTextAlignment = TextAlignment.Center,
			};

			_valueLabel = new Label
			{
				HorizontalOptions = LayoutOptions.EndAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalTextAlignment = TextAlignment.End,
			};

			View = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(8, 2),
				Spacing = 8,
				Children = {
					_textLabel, _valueLabel
				}
			};
		}
		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext == null)
			{
				_textLabel.Text = "";
				_valueLabel.Text = "";
				return;
			}

			_textLabel.Text = (BindingContext as DevicePropertyModel).Name;
			_valueLabel.Text = (BindingContext as DevicePropertyModel).Value;
		}

		protected override void UpdateFromProperty(DeviceDrive.SDK.Contracts.DevicePropertyModel model)
		{
			base.UpdateFromProperty(model);

			OnBindingContextChanged();
		}
	}
}
