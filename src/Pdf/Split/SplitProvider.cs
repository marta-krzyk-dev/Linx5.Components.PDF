using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Split
{
	public class SplitProvider : FunctionProvider
	{
		public override string Name { get; } = "Split";

		public override string SearchKeywords { get; } = "pdf split";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new SplitCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new SplitDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new SplitDesigner(context);
	}
}