using iTextSharp.text;
using NUnit.Framework;
using System;
using System.IO;
using Twenty57.Linx.Components.Pdf.PdfOperations;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.TestKit;

#pragma warning disable 0612
namespace Twenty57.Linx.Components.Pdf.Tests.PdfOperations
{
	public partial class TestPdfOperations
	{
		private const string signName = "John Smith";
		private const string signReason = "Make it safe.";
		private const string signLocation = "At the office.";

		private bool lockDocument = false;

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
			string inputFilePath = ResourceHelpers.WriteResourceToFile($"Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.{fileName}", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, fileName);
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			ConfigureSignCertificateProperties(designer, signAuth, this.lockDocument);
			designer.Properties[PropertyNames.SignPlacement].Value = SignaturePosition.Hidden;
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertPageSignature(outputFilePath, inputAuth, this.authenticationManager, signName, signLocation, signReason, this.lockDocument, 1, 0, 0, 0, 0);
		}

		[Test]
		public void SignAcroWithPageSignature()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Sign.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Sign.pdf");
			int left = 45;
			int top = 223;
			int width = 109;
			int height = 79;
			int page = 2;
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureSignCertificateProperties(designer, FileAuthentication.CertificateFile, this.lockDocument);
			designer.Properties[PropertyNames.SignPlacement].Value = SignaturePosition.OnPage;
			designer.Properties[PropertyNames.SignPositionX].Value = left;
			designer.Properties[PropertyNames.SignPositionY].Value = top;
			designer.Properties[PropertyNames.SignWidth].Value = width;
			designer.Properties[PropertyNames.SignHeight].Value = height;
			designer.Properties[PropertyNames.SignPage].Value = page;
			designer.Properties[PropertyNames.SignBackgroundImage].Value = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Sign_Image.png", this.inputDirectory);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertPageSignature(outputFilePath, FileAuthentication.None, this.authenticationManager, signName, signLocation, signReason, this.lockDocument,
				page, Utilities.MillimetersToPoints(left), Utilities.MillimetersToPoints(top), Utilities.MillimetersToPoints(width), Utilities.MillimetersToPoints(height));
		}

		[Test]
		public void SignAcroWithFieldSignature()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Sign.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Sign.pdf");
			string fieldName = "SignatureField";
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureSignCertificateProperties(designer, FileAuthentication.CertificateStore, this.lockDocument);
			designer.Properties[PropertyNames.SignPlacement].Value = SignaturePosition.FormField;
			designer.Properties[PropertyNames.SignFieldName].Value = fieldName;
			designer.Properties[PropertyNames.SignBackgroundImage].Value = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Sign_Image.png", this.inputDirectory);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertFieldSignature(outputFilePath, FileAuthentication.None, this.authenticationManager, fieldName, signName, signLocation, signReason, this.lockDocument);
		}

		[Test]
		public void SignXFAWithPageSignature()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.SignXFA.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "SignXFA.pdf");
			int left = 45;
			int top = 223;
			int width = 109;
			int height = 79;
			int page = 1;
			this.lockDocument = !this.lockDocument;

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureSignCertificateProperties(designer, FileAuthentication.CertificateFile, this.lockDocument);
			designer.Properties[PropertyNames.SignPlacement].Value = SignaturePosition.OnPage;
			designer.Properties[PropertyNames.SignPositionX].Value = left;
			designer.Properties[PropertyNames.SignPositionY].Value = top;
			designer.Properties[PropertyNames.SignWidth].Value = width;
			designer.Properties[PropertyNames.SignHeight].Value = height;
			designer.Properties[PropertyNames.SignPage].Value = page;
			designer.Properties[PropertyNames.SignBackgroundImage].Value = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Sign_Image.png", this.inputDirectory);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertPageSignature(outputFilePath, FileAuthentication.None, this.authenticationManager, signName, signLocation, signReason, this.lockDocument,
				page, Utilities.MillimetersToPoints(left), Utilities.MillimetersToPoints(top), Utilities.MillimetersToPoints(width), Utilities.MillimetersToPoints(height));
		}

		private void ConfigureSignCertificateProperties(FunctionDesigner designer, FileAuthentication signAuth, bool lockDocument)
		{
			designer.Properties[PropertyNames.Operation].Value = Operation.Sign;

			switch (signAuth)
			{
				case FileAuthentication.CertificateFile:
					designer.Properties[PropertyNames.SignCertificateSource].Value = CertificateSource.File;
					designer.Properties[PropertyNames.SignCertificateFilePath].Value = this.authenticationManager.CertificateFilePath;
					designer.Properties[PropertyNames.SignCertificateFilePassword].Value = this.authenticationManager.CertificateFilePassword;
					break;
				case FileAuthentication.CertificateStore:
					designer.Properties[PropertyNames.SignCertificateSource].Value = CertificateSource.Store;
					designer.Properties[PropertyNames.SignCertificate].Value = this.authenticationManager.StoredCertificate;
					break;
				default:
					throw new NotSupportedException();
			}

			designer.Properties[PropertyNames.SignSignedAt].Value = signLocation;
			designer.Properties[PropertyNames.SignReason].Value = signReason;
			designer.Properties[PropertyNames.SignLockAfterSigning].Value = lockDocument;
		}
	}
}
#pragma warning restore 0612