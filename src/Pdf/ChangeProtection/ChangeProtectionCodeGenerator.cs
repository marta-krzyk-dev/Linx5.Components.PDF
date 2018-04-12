using System;
using iTextSharp.text.pdf;
using Twenty57.Linx.Components.Pdf.ChangeProtection.Templates;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection
{
	internal class ChangeProtectionCodeGenerator : FunctionCodeGenerator
	{
		public ChangeProtectionCodeGenerator(IFunctionData data) : base(data) {	}

		public override void GenerateCode(IFunctionBuilder functionBuilder)
		{
			functionBuilder.AddAssemblyReference(GetType());
			functionBuilder.AddAssemblyReference(typeof(Uri));
			functionBuilder.AddAssemblyReference(typeof(PdfReader));

			ChangeProtectionTemplate template = new ChangeProtectionTemplate();
			template.Populate(functionBuilder, FunctionData);
			functionBuilder.AddCode(template.TransformText());
		}
	}
}