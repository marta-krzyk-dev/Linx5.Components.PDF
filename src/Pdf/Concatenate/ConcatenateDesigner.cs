using System.Collections.Generic;
using System.Linq;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;
using Twenty57.Linx.Plugin.Common.Validation;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.Concatenate
{
	internal class ConcatenateDesigner : FunctionDesigner
	{
		public ConcatenateDesigner(IDesignerContext context) : base(context)
		{
			InitializeProperties();
		}

		public ConcatenateDesigner(IFunctionData data, IDesignerContext context) : base(data, context) { }

		public override IFunctionData GetFunctionData()
		{
			var data = new FunctionData();
			if (ExecutionPaths.Any())
				data.ExecutionPaths.AddRange(ExecutionPaths);
			data.Output = Output;
			data.Version = Version;
			data.Properties.AddRange(Properties.Where(prop => prop.IsVisible));
			return data;
		}

		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			Property inputFiles = Properties.AddOrRetrieve(PropertyNames.InputFiles, TypeReference.CreateList(typeof(string)), ValueUseOption.RuntimeRead, null);
			inputFiles.Order = 1;
			inputFiles.Description = "List of PDF files to concatenate.";
			inputFiles.Validations.Add(new RequiredValidator());

			Property outputFilePath = Properties.AddOrRetrieve(Common.PropertyNames.OutputFilePath, typeof(string),
				ValueUseOption.RuntimeRead, string.Empty);
			outputFilePath.Order = 2;
			outputFilePath.Description = "Path of the PDF file to write to.";
			outputFilePath.Editor = typeof(FilePathEditor);
			outputFilePath.Validations.Add(new RequiredValidator());
		}
	}
}