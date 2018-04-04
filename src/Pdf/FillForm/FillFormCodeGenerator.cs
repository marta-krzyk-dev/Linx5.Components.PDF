using System;
using iTextSharp.text.pdf;
using Twenty57.Linx.Components.Pdf.FillForm.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.FillForm
{
	internal class FillFormCodeGenerator : FunctionCodeGenerator
	{
		public FillFormCodeGenerator(IFunctionData data) : base(data) {	}

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));

			FillFormTemplate template = new FillFormTemplate();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}