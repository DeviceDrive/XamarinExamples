using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LightSwitch.Controls
{
	public class TintImageEffect : RoutingEffect
	{
		public const string GroupName = "MyCompany";
		public const string Name = "TintImageEffect";

		public Color TintColor { get; set; }

		public TintImageEffect() : base($"{GroupName}.{Name}") { }
	}
}
