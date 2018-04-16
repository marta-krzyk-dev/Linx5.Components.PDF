using System;
using iTextSharp.text.pdf;
using Twenty57.Linx.Components.Pdf.Concatenate.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.Concatenate
{
	internal class ConcatenateCodeGenerator : FunctionCodeGenerator
	{
		public ConcatenateCodeGenerator(IFunctionData data) : base(data) {	}

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));

			ConcatenateTemplate template = new ConcatenateTemplate();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}