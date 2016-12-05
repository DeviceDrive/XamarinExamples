using System;
using DeviceDrive.SDK.Contracts;
using Xamarin.Forms;

namespace LightSwitch
{
	public class BasePropertyCell: ViewCell
	{
		public BasePropertyCell ()
		{
			MessagingCenter.Subscribe<MainPage, DevicePropertyModel>(
				this, MainPage.PropertyUpdatedMessage, (sender, model) => UpdateFromProperty(model));
		}

		protected virtual void UpdateFromProperty(DevicePropertyModel model)
		{ 			
		}
	}
}
