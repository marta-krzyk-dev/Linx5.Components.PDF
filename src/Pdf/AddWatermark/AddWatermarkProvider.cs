using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.AddWatermark
{
	public class AddWatermarkProvider : FunctionProvider
	{
		public override string Name { get; } = "AddWatermark";

		public override string SearchKeywords { get; } = "pdf add watermark";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new AddWatermarkCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new AddWatermarkDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new AddWatermarkDesigner(context);
	}
}