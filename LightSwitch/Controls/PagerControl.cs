using System;
using Xamarin.Forms;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace LightSwitch
{

	public class PagerControl: ContentView
	{
		public const int PagerHeight = 44;

		StackLayout _content;
		public PagerControl()
		{
			_content = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
			};

			Content = _content;
		}

		/// <summary>
		/// The Page property.
		/// </summary>
		public static BindableProperty PageProperty =
			BindableProperty.Create(nameof(Page), typeof(int), typeof(PagerControl), 0,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (PagerControl)bindable;
					ctrl.Page = (int)newValue;
				});

		/// <summary>
		/// Gets or sets the Page of the PagerControl instance.
		/// </summary>
		public int Page
		{
			get { return (int)GetValue(PageProperty); }
			set
			{
				SetValue(PageProperty, value);
				UpdatePager();
			}
		}	

		/// <summary>
		/// The PageCount property.
		/// </summary>
		public static BindableProperty PageCountProperty =
			BindableProperty.Create(nameof(PageCount), typeof(int), typeof(PagerControl), 0,
				BindingMode.TwoWay, null, propertyChanged: (bindable, oldValue, newValue) =>
				{
					var ctrl = (PagerControl)bindable;
					ctrl.PageCount = (int)newValue;
				});

		/// <summary>
		/// Gets or sets the PageCount of the PagerControl instance.
		/// </summary>
		public int PageCount
		{
			get { return (int)GetValue(PageCountProperty); }
			set
			{
				SetValue(PageCountProperty, value);
				UpdatePager();
			}
		}

		void UpdatePager()
		{
			_content.Children.Clear();
			var borderWidth = 2.0;

			for (var i = 0; i < PageCount; i++)
			{
				var label = new Label
				{
					Text = (i+1).ToString(),
					HorizontalOptions = LayoutOptions.Center,
					VerticalOptions = LayoutOptions.Center,
					FontSize = 24,
					TextColor = i==Page ? Color.Black : Color.FromHex("CECECE"),
				};

				Device.OnPlatform(() =>
				{
					// iOS
					var frame = new Button();
					frame.BorderWidth = borderWidth;
					frame.IsEnabled = false;
					frame.BorderColor = i == Page ? Color.FromHex("A0A0A0") : Color.FromHex("CECECE");
					frame.BorderRadius = PagerHeight / 2;

					_content.Children.Add(new Grid
					{
						HeightRequest = PagerHeight,
						WidthRequest = PagerHeight,
						Children = { frame, label }
					});
				}, () => 
				{
					// Android
					label.HeightRequest = PagerHeight;
					_content.Children.Add(label);
				});

				if (i < PageCount-1)
				{
					var hyphen = new BoxView { 
						HeightRequest = borderWidth,
						WidthRequest = 14,
						BackgroundColor = Color.FromHex("CECECE"),
						HorizontalOptions = LayoutOptions.Center,
						VerticalOptions = LayoutOptions.Center,
					};

					_content.Children.Add(hyphen);
				}
			}
		}
	}
}
