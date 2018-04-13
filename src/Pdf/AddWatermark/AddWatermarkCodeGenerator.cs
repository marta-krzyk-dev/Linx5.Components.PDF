using System;
using System.Drawing.Drawing2D;
using iTextSharp.text.pdf;
using Twenty57.Linx.Components.Pdf.AddWatermark.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.AddWatermark
{
	internal class AddWatermarkCodeGenerator : FunctionCodeGenerator
	{
		public AddWatermarkCodeGenerator(IFunctionData data) : base(data) {	}

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));
			functionBuilder.AddAssemblyReference(typeof(Matrix));

			AddWatermarkTemplate template = new AddWatermarkTemplate();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}