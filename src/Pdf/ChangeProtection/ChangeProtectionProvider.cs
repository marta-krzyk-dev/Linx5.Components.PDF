using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection
{
	public class ChangeProtectionProvider : FunctionProvider
	{	
		public override string Name { get; } = "ChangeProtection";

		public override string SearchKeywords { get; } = "pdf change protection";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new ChangeProtectionCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new ChangeProtectionDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new ChangeProtectionDesigner(context);
	}
}