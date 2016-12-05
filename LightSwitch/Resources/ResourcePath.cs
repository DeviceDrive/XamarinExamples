using System;

namespace LightSwitch.Resources
{
	public static class ResourcePath
	{
		public static string Path 
		{ 
			get 
			{
				return typeof(ResourcePath).Namespace;	
			}
		}
	}
}
