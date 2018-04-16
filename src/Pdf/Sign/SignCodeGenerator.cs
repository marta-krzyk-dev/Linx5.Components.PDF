using System;
using iTextSharp.text.pdf;
using Twenty57.Linx.Components.Pdf.Sign.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.Sign
{
	internal class SignCodeGenerator : FunctionCodeGenerator
	{
		public SignCodeGenerator(IFunctionData data) : base(data) {	}

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));

			SignTemplate template = new SignTemplate();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}