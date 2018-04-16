using System;
using System.Collections.Generic;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;

namespace Twenty57.Linx.Components.Pdf.Split
{
	internal class SplitDesigner : PdfDesigner
	{
		public SplitDesigner(IDesignerContext context) : base(context)
		{
			InitializeProperties();
			BuildOutput();
		}

		public SplitDesigner(IFunctionData data, IDesignerContext context) : base(data, context) { }

		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			Property loopResults = Properties.AddOrRetrieve(PropertyNames.LoopResults, typeof(bool), ValueUseOption.DesignTime, false);
			loopResults.Order = propertyOrder++;
			loopResults.Description = "Loop through the generated file names.";
			loopResults.ValueChanged += RefreshOutput;
		}

		private void RefreshOutput(object sender, EventArgs e) => BuildOutput();

		private void BuildOutput()
		{
			Output = null;
			ExecutionPaths.Clear();

			var outputBuilder = new TypeBuilder();

			bool loopResultsValue = Properties[PropertyNames.LoopResults].GetValue<bool>();
			if (loopResultsValue)
				ExecutionPaths.Add(ExecutionPathNames.PageFiles, ExecutionPathNames.PageFiles, TypeReference.Create(typeof(string)));
			else
				outputBuilder.AddProperty(OutputNames.PageFiles, TypeReference.CreateList(typeof(string)));

			outputBuilder.AddProperty(OutputNames.NumberOfPages, typeof(int));
			Output = outputBuilder.CreateTypeReference();
		}
	}
}