using iTextSharp.text;
using NUnit.Framework;
using System;
using System.IO;
using Twenty57.Linx.Components.Pdf.Sign;
using Twenty57.Linx.Components.Pdf.Tests.Common;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.Sign
{
	public class TestSign : TestPdfBase
	{
		private string outputDirectory;
		private bool lockDocument = false;

		private const string signName = "John Smith";
		private const string signReason = "Make it safe.";
		private const string signLocation = "At the office.";

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
		public void SignWithInvisibleSignature(
			[Values(
				FileAuthentication.None,
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication inputAuth,
			[Values(
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore)] FileAuthentication signAuth,
			[Values(
				"Sign.pdf",
				"SignXFA.pdf")] string fileName)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile($"Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.{fileName}", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, fileName);
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<SignProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			ConfigureSignCertificateProperties(designer, signAuth, this.lockDocument);
			designer.Properties[PropertyNames.Placement].Value = SignaturePosition.Hidden;
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<SignProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertPageSignature(outputFilePath, inputAuth, this.authenticationManager, signName, signLocation, signReason, this.lockDocument, 1, 0, 0, 0, 0);
		}

		[Test]
		public void SignAcroWithPageSignature()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.Sign.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Sign.pdf");
			int left = 45;
			int top = 223;
			int width = 109;
			int height = 79;
			int page = 2;
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<SignProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureSignCertificateProperties(designer, FileAuthentication.CertificateFile, this.lockDocument);
			designer.Properties[PropertyNames.Placement].Value = SignaturePosition.OnPage;
			designer.Properties[PropertyNames.PositionX].Value = left;
			designer.Properties[PropertyNames.PositionY].Value = top;
			designer.Properties[PropertyNames.Width].Value = width;
			designer.Properties[PropertyNames.Height].Value = height;
			designer.Properties[PropertyNames.Page].Value = page;
			designer.Properties[PropertyNames.BackgroundImage].Value = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.Sign_Image.png", this.inputDirectory);
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<SignProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertPageSignature(outputFilePath, FileAuthentication.None, this.authenticationManager, signName, signLocation, signReason, this.lockDocument,
				page, Utilities.MillimetersToPoints(left), Utilities.MillimetersToPoints(top), Utilities.MillimetersToPoints(width), Utilities.MillimetersToPoints(height));
		}

		[Test]
		public void SignAcroWithFieldSignature()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.Sign.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Sign.pdf");
			string fieldName = "SignatureField";
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<SignProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureSignCertificateProperties(designer, FileAuthentication.CertificateStore, this.lockDocument);
			designer.Properties[PropertyNames.Placement].Value = SignaturePosition.FormField;
			designer.Properties[PropertyNames.FieldName].Value = fieldName;
			designer.Properties[PropertyNames.BackgroundImage].Value = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.Sign_Image.png", this.inputDirectory);
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<SignProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertFieldSignature(outputFilePath, FileAuthentication.None, this.authenticationManager, fieldName, signName, signLocation, signReason, this.lockDocument);
		}

		[Test]
		public void SignXFAWithPageSignature()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.SignXFA.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "SignXFA.pdf");
			int left = 45;
			int top = 223;
			int width = 109;
			int height = 79;
			int page = 1;
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<SignProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureSignCertificateProperties(designer, FileAuthentication.CertificateFile, this.lockDocument);
			designer.Properties[PropertyNames.Placement].Value = SignaturePosition.OnPage;
			designer.Properties[PropertyNames.PositionX].Value = left;
			designer.Properties[PropertyNames.PositionY].Value = top;
			designer.Properties[PropertyNames.Width].Value = width;
			designer.Properties[PropertyNames.Height].Value = height;
			designer.Properties[PropertyNames.Page].Value = page;
			designer.Properties[PropertyNames.BackgroundImage].Value = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Sign.Resources.Sign_Image.png", this.inputDirectory);
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<SignProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertPageSignature(outputFilePath, FileAuthentication.None, this.authenticationManager, signName, signLocation, signReason, this.lockDocument,
				page, Utilities.MillimetersToPoints(left), Utilities.MillimetersToPoints(top), Utilities.MillimetersToPoints(width), Utilities.MillimetersToPoints(height));
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

		private void ConfigureSignCertificateProperties(FunctionDesigner designer, FileAuthentication signAuth, bool lockDocument)
		{
			switch (signAuth)
			{
				case FileAuthentication.CertificateFile:
					designer.Properties[PropertyNames.CertificateSource].Value = CertificateSource.File;
					designer.Properties[PropertyNames.CertificateFilePath].Value = this.authenticationManager.CertificateFilePath;
					designer.Properties[PropertyNames.CertificateFilePassword].Value = this.authenticationManager.CertificateFilePassword;
					break;
				case FileAuthentication.CertificateStore:
					designer.Properties[PropertyNames.CertificateSource].Value = CertificateSource.Store;
					designer.Properties[PropertyNames.Certificate].Value = this.authenticationManager.StoredCertificate;
					break;
				default:
					throw new NotSupportedException();
			}

			designer.Properties[PropertyNames.SignedAt].Value = signLocation;
			designer.Properties[PropertyNames.Reason].Value = signReason;
			designer.Properties[PropertyNames.LockAfterSigning].Value = lockDocument;
		}
	}
}