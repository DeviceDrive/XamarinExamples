using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LightSwitch.Droid.Platform.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using FormsTintImageEffect = LightSwitch.Controls.TintImageEffect;

[assembly: ResolutionGroupName(LightSwitch.Controls.TintImageEffect.GroupName)]
[assembly: ExportEffect(typeof(TintImageEffect), LightSwitch.Controls.TintImageEffect.Name)]
namespace LightSwitch.Droid.Platform.Renderers
{
	public class TintImageEffect : PlatformEffect
	{
		protected override void OnAttached()
		{
			try
			{
				var effect = (FormsTintImageEffect)Element.Effects.FirstOrDefault(e => e is FormsTintImageEffect);

				if (effect == null || !(Control is ImageView image))
					return;

				if (effect.TintColor.ToAndroid() == Android.Graphics.Color.Transparent)
				{
					image.ClearColorFilter();
					return;
				}

				var filter = new LightingColorFilter(effect.TintColor.ToAndroid(), 0x000000);
				image.SetColorFilter(filter);
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine(
					$"An error occurred when setting the {typeof(TintImageEffect)} effect: {ex.Message}\n{ex.StackTrace}");
			}
		}

		protected override void OnDetached() { }

		
	}
}