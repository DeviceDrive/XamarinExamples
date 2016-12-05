using System;
using DeviceDrive.SDK.Contracts;
using Xamarin.Forms;
using DeviceDrive.Introspection;

namespace LightSwitch
{
	public class PropertyCellTemplateSelector: DataTemplateSelector
	{
		readonly DataTemplate BoolPropertyTemplate;
		readonly DataTemplate TextPropertyTemplate;

		public PropertyCellTemplateSelector()
		{
			BoolPropertyTemplate = new DataTemplate(typeof(BoolPropertyCell));
			TextPropertyTemplate = new DataTemplate(typeof(TextPropertyCell));
		}

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			var model = item as DevicePropertyModel;
			if (model == null)
				return null;

			return model.CanWrite && model.DataType == (int)AJTypeCode.Bool ? 
				        BoolPropertyTemplate : 
				        TextPropertyTemplate;
		}

	}
}
