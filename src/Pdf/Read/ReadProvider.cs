using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Read
{
	public class ReadProvider : FunctionProvider
	{
		public override string Name { get; } = "Read";

		public override string SearchKeywords { get; } = "pdf read text form signature";

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new ReadDesigner(context);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new ReadDesigner(data, context);

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new ReadCodeGenerator(data);

		public override bool TryUpdateToLatestVersion(IFunctionData data, IUpdateContext context, out IFunctionData updatedData) => ReadUpdater.Instance.TryUpdate(data, context, out updatedData);
	}
}