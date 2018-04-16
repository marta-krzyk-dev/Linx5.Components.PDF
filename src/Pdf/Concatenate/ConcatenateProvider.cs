using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Concatenate
{
	public class ConcatenateProvider : FunctionProvider
	{
		public override string Name { get; } = "Concatenate";

		public override string SearchKeywords { get; } = "pdf concatenate";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new ConcatenateCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new ConcatenateDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new ConcatenateDesigner(context);
	}
}