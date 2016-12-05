using System;
using DeviceDrive.SDK.Contracts;
using Xamarin.Forms;

namespace LightSwitch
{
	public class BoolPropertyCell : BasePropertyCell
	{
		public const string BoolPropertyChangedMessage = "BoolPropertyChangedMessage";

		readonly Label _textLabel;
		readonly Switch _switch;

		public BoolPropertyCell()
		{
			_textLabel = new Label { 
				FontAttributes = FontAttributes.Bold,
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				VerticalTextAlignment = TextAlignment.Center,
			};

			_switch = new Switch
			{
				HorizontalOptions = LayoutOptions.End,
				VerticalOptions = LayoutOptions.Center,
			};

			_switch.Toggled += (sender, e) =>
			{
				if (BindingContext != null)
				{
					var converter = new StringToBoolConverter();

					var currentValue = (string)converter.ConvertBack(_switch.IsToggled, typeof(bool), null, null);
					if (!currentValue.Equals((BindingContext as DevicePropertyModel).Value))
					{
						(BindingContext as DevicePropertyModel).Value = (string)converter.ConvertBack(_switch.IsToggled, typeof(bool), null, null);
						MessagingCenter.Send(
							this, BoolPropertyChangedMessage, BindingContext as DevicePropertyModel);
					}
				}
			};

			View = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				Padding = new Thickness(8, 2),
				Spacing = 8,
				Children = {
					_textLabel, _switch
				}
			};
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			if (BindingContext == null)
			{
				_textLabel.Text = "";
				_switch.IsToggled = false;
				return;
			}

			_textLabel.Text = (BindingContext as DevicePropertyModel).Name;
			_switch.IsToggled = (bool)new StringToBoolConverter().Convert(
				(BindingContext as DevicePropertyModel).Value, typeof(bool), null, null);
		}

		protected override void UpdateFromProperty(DevicePropertyModel model)
		{
			base.UpdateFromProperty(model);
			OnBindingContextChanged();
		}
	}
}

