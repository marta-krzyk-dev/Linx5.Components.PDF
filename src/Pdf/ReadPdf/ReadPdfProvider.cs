using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.ReadPdf
{
	public class ReadPdfProvider : FunctionProvider
	{
		public override string Name { get; } = "ReadPdf";

		public override string SearchKeywords { get; } = "pdf read text form signature";

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new ReadPdfDesigner(context);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new ReadPdfDesigner(data, context);

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new ReadPdfCodeGenerator(data);

		public override IFunctionData UpdateToLatestVersion(IFunctionData data) => ReadPdfUpdater.Instance.Update(data);
	}
}