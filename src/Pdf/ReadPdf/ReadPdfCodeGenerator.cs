using iTextSharp.text.pdf;
using System;
using Twenty57.Linx.Components.Pdf.ReadPdf.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.ReadPdf
{
	internal class ReadPdfCodeGenerator : FunctionCodeGenerator
	{
		public ReadPdfCodeGenerator(IFunctionData data)
			: base(data)
		{ }

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));

			var template = new ReadTemplate();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}
