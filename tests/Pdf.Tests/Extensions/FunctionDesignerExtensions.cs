using System.Linq;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.Extensions
{
	internal static class FunctionDesignerExtensions
	{
		public static PropertyValue[] GetProperties(this FunctionDesigner designer)
		{
			return designer.Properties.Select(prop => new PropertyValue(prop.Name, prop.Value)).ToArray();
		}

		public static ParameterValue[] GetParameters(this FunctionDesigner designer)
		{
			return designer.GetFunctionData().Properties.Values
				.Where(prop => prop.ValueUsage == ValueUseOption.RuntimeRead)
				.Select(param => new ParameterValue(param.Name, param.Value))
				.ToArray();
		}
	}
}
