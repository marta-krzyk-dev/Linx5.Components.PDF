using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Sign
{
	public class SignProvider : FunctionProvider
	{
		public override string Name { get; } = "Sign";

		public override string SearchKeywords { get; } = "pdf sign";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new SignCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new SignDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new SignDesigner(context);
	}
}