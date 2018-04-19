﻿using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	[Obsolete]
	public class PdfOperationsProvider : FunctionProvider
	{
		private static readonly string searchKeywords;

		static PdfOperationsProvider()
		{
			searchKeywords = "pdf " + string.Join(" ", Enum.GetNames(typeof(Operation)));
		}

		public override string Name { get; } = "PdfOperations";

		public override string SearchKeywords { get; } = searchKeywords;

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new PdfOperationsDesigner(context);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new PdfOperationsDesigner(data, context);

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new PdfOperationsCodeGenerator(data);

		public override bool TryUpdateToLatestVersion(IFunctionData data, IUpdateContext context, out IFunctionData updatedData) => PdfOperationsUpdater.Instance.TryUpdate(data, context, out updatedData);
	}
}