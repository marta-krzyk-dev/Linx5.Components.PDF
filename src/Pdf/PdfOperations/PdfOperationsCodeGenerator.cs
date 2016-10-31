using iTextSharp.text.pdf;
using System;
using System.Drawing.Drawing2D;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Components.Pdf.PdfOperations.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	internal class PdfOperationsCodeGenerator : FunctionCodeGenerator
	{
		public PdfOperationsCodeGenerator(IFunctionData data)
			: base(data)
		{ }

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));

			Operation operationValue = FunctionData.Properties[PropertyNames.Operation].GetValue<Operation>();
			switch (operationValue)
			{
				case Operation.FillForm:
					GenerateTemplateCode<FillFormTemplate>(functionBuilder);
					break;
				case Operation.Protect:
					GenerateTemplateCode<ProtectTemplate>(functionBuilder);
					break;
				case Operation.Split:
					GenerateTemplateCode<SplitTemplate>(functionBuilder);
					break;
				case Operation.Concatenate:
					GenerateTemplateCode<ConcatenateTemplate>(functionBuilder);
					break;
				case Operation.AddWatermark:
					functionBuilder.AddAssemblyReference(typeof(Matrix));
					GenerateTemplateCode<AddWatermarkTemplate>(functionBuilder);
					break;
				case Operation.Sign:
					GenerateTemplateCode<SignTemplate>(functionBuilder);
					break;
				default:
					throw new NotSupportedException("Invalid Operation specified.");
			}
		}

		private void GenerateTemplateCode<T>(IFunctionBuilder functionBuilder) where T : ITemplate
		{
			T template = Activator.CreateInstance<T>();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}
