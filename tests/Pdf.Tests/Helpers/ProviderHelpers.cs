using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Tests.Helpers
{
	internal static class ProviderHelpers
	{
		public static FunctionDesigner CreateDesigner<T>() where T : FunctionProvider
		{
			return Activator.CreateInstance<T>().CreateDesigner(null);
		}
	}
}
