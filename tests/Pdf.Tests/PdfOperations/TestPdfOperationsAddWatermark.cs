using NUnit.Framework;
using System.IO;
using Twenty57.Linx.Components.Pdf.PdfOperations;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.PdfOperations
{
	public partial class TestPdfOperations
	{
		[Test]
		[Combinatorial]
		public void AddWatermark(
			[Values(
				FileAuthentication.None,
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication inputAuth,
			[Values(
				WatermarkPosition.Above,
				WatermarkPosition.Below)] WatermarkPosition position)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Watermark.pdf", this.inputDirectory);
			string watermarkFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Overlay.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Watermark.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			var watermarkPages = "4;1-2,2,2";
			ConfigureWatermarkFunctionValues(designer, FileAuthentication.None, watermarkFilePath, position, watermarkPages);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertText(outputFilePath, inputAuth, this.authenticationManager, "1\nWatermark\r\n2\nWatermark\r\n3\r\n4\nWatermark", null);
		}

		[Test]
		public void AddWatermarkWithAuthentication(
			[Values(
				FileAuthentication.None,
				FileAuthentication.Password,
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore)] FileAuthentication watermarkAuth)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Watermark.pdf", this.inputDirectory);
			string watermarkFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Overlay.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Watermark.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			var watermarkPages = string.Empty;
			ConfigureWatermarkFunctionValues(designer, watermarkAuth, watermarkFilePath, WatermarkPosition.Below, watermarkPages);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertText(outputFilePath, FileAuthentication.None, this.authenticationManager, "1\nWatermark\r\n2\nWatermark\r\n3\nWatermark\r\n4\nWatermark", null);
		}

		private void ConfigureWatermarkFunctionValues(FunctionDesigner designer, FileAuthentication watermarkAuth, string watermarkFilePath, WatermarkPosition position,
			string watermarkPages)
		{
			designer.Properties[PropertyNames.Operation].Value = Operation.AddWatermark;
			designer.Properties[PropertyNames.WatermarkPosition].Value = position;
			designer.Properties[PropertyNames.WatermarkPages].Value = watermarkPages;

			ConfigureInputFileFunctionValues(
				designer,
				watermarkAuth,
				watermarkFilePath,
				PropertyNames.WatermarkFilePath,
				PropertyNames.WatermarkAuthenticationType,
				PropertyNames.WatermarkPassword,
				PropertyNames.WatermarkCertificateSource,
				PropertyNames.WatermarkCertificateFilePath,
				PropertyNames.WatermarkCertificateFilePassword,
				PropertyNames.WatermarkCertificate);
		}
	}
}
