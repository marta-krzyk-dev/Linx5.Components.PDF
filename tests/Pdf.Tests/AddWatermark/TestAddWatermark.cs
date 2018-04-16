using System;
using System.IO;
using NUnit.Framework;
using Twenty57.Linx.Components.Pdf.AddWatermark;
using Twenty57.Linx.Components.Pdf.Tests.Common;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.AddWatermark
{
	public class TestAddWatermark: TestPdfBase
	{
		private string outputDirectory;

		[SetUp]
		public void Setup()
		{
			this.outputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(this.outputDirectory);
		}

		[TearDown]
		public void Teardown()
		{
			Directory.Delete(this.outputDirectory, true);
		}

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
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.AddWatermark.Resources.Watermark.pdf", this.inputDirectory);
			string watermarkFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.AddWatermark.Resources.Overlay.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Watermark.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<AddWatermarkProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			var watermarkPages = "4;1-2,2,2";
			ConfigureWatermarkFunctionValues(designer, FileAuthentication.None, watermarkFilePath, position, watermarkPages);
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<AddWatermarkProvider>();
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
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.AddWatermark.Resources.Watermark.pdf", this.inputDirectory);
			string watermarkFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.AddWatermark.Resources.Overlay.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Watermark.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<AddWatermarkProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			var watermarkPages = string.Empty;
			ConfigureWatermarkFunctionValues(designer, watermarkAuth, watermarkFilePath, WatermarkPosition.Below, watermarkPages);
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<AddWatermarkProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertText(outputFilePath, FileAuthentication.None, this.authenticationManager, "1\nWatermark\r\n2\nWatermark\r\n3\nWatermark\r\n4\nWatermark", null);
		}

		private void ConfigureInputFileFunctionValues(FunctionDesigner designer, FileAuthentication inputAuth, string inputFilePath)
		{
			ConfigureInputFileFunctionValues(
				designer,
				inputAuth,
				inputFilePath,
				Pdf.Common.PropertyNames.InputFilePath,
				Pdf.Common.PropertyNames.InputAuthenticationType,
				Pdf.Common.PropertyNames.InputPassword,
				Pdf.Common.PropertyNames.InputCertificateSource,
				Pdf.Common.PropertyNames.InputCertificateFilePath,
				Pdf.Common.PropertyNames.InputCertificateFilePassword,
				Pdf.Common.PropertyNames.InputCertificate);
		}

		private void ConfigureWatermarkFunctionValues(FunctionDesigner designer, FileAuthentication watermarkAuth, string watermarkFilePath, WatermarkPosition position,
			string watermarkPages)
		{
			designer.Properties[PropertyNames.Position].Value = position;
			designer.Properties[PropertyNames.Pages].Value = watermarkPages;

			ConfigureInputFileFunctionValues(
				designer,
				watermarkAuth,
				watermarkFilePath,
				PropertyNames.FilePath,
				PropertyNames.AuthenticationType,
				PropertyNames.Password,
				PropertyNames.CertificateSource,
				PropertyNames.CertificateFilePath,
				PropertyNames.CertificateFilePassword,
				PropertyNames.Certificate);
		}
	}
}