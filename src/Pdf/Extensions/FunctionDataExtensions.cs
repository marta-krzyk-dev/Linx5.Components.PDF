using System;
using System.Linq;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Extensions
{
	internal static class FunctionDataExtensions
	{
		public static T TryGetPropertyValue<T>(this IFunctionData data, string propertyName, T defaultValue)
		{
			IPropertyData property = data.Properties.Values.FirstOrDefault(p => p.Name == propertyName);
			if (property != null)
			{
				if (property.ValueUsage != ValueUseOption.DesignTime)
					throw new NotSupportedException("Cannot run GetValue on a dynamic value.");

				return property.GetValue<T>();
			}

			return defaultValue;
		}
	}
}
