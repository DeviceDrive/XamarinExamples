using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreGraphics;
using Foundation;
using LightSwitch.iOS.Platform.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using FormsTintImageEffect = LightSwitch.Controls.TintImageEffect;

[assembly: ResolutionGroupName(LightSwitch.Controls.TintImageEffect.GroupName)]
[assembly: ExportEffect(typeof(TintImageEffect), LightSwitch.Controls.TintImageEffect.Name)]
namespace LightSwitch.iOS.Platform.Renderers
{
	public class TintImageEffect : PlatformEffect
	{
		protected override void OnAttached()
		{
			try
			{
				var effect = (FormsTintImageEffect)Element.Effects.FirstOrDefault(e => e is FormsTintImageEffect);

				if (effect == null || !(Control is UIImageView image))
					return;

				if (effect.TintColor.B == 1.0)
				{
					return;
				}

				if (image.Image != null) image.Image = image.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
				image.TintColor = effect.TintColor.ToUIColor();
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"An error occurred when setting the {typeof(TintImageEffect)} effect: {ex.Message}\n{ex.StackTrace}");
			}
		}

		protected override void OnDetached() { }

		
	}
}