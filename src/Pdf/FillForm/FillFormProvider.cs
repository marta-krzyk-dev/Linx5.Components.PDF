using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.FillForm
{
	public class FillFormProvider : FunctionProvider
	{
		public override string Name { get; } = "FillForm";

		public override string SearchKeywords { get; } = "pdf fill form";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new FillFormCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new FillFormDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new FillFormDesigner(context);
	}
}