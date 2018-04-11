﻿using System;
using System.IO;
using NUnit.Framework;
using Twenty57.Linx.Components.Pdf.ChangeProtection;
using Twenty57.Linx.Components.Pdf.Tests.Common;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.ChangeProtection
{
	[TestFixture]
	public class TestChangeProtection : TestPdfBase
	{
		private string outputDirectory;

		private static readonly string permissionsPassword = "permissions";

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
		public void ProtectWithRemoveProtection(
			[Values(
				FileAuthentication.None,
				FileAuthentication.Password,
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore)] FileAuthentication inputAuth)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ChangeProtection.Resources.Protect.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Protect.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ChangeProtectionProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			ConfigureProtectFunctionValues(designer, FileAuthentication.None, Encryption.AES128, false);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<ChangeProtectionProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertText(outputFilePath, FileAuthentication.None, this.authenticationManager, "Text on page 1", null);
		}

		[Test]
		public void ProtectWithNoRestrictions(
			[Values(
				FileAuthentication.Password,
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore)] FileAuthentication protectAuth)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ChangeProtection.Resources.Protect.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Protect.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ChangeProtectionProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureProtectFunctionValues(designer, protectAuth, Encryption.AES256, true);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<ChangeProtectionProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertProtectionAllRights(outputFilePath, protectAuth, this.authenticationManager, true, true);
		}

		[Test]
		[Combinatorial]
		public void ProtectWithPrintRestrictions(
			[Values(
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication protectAuth,
			[Values(
				Printing.None,
				Printing.LowResolution,
				Printing.HighResolution)] Printing printing)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ChangeProtection.Resources.Protect.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Protect.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ChangeProtectionProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureProtectFunctionValues(designer, protectAuth, Encryption.AES256, true, true, printing: printing);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<ChangeProtectionProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			bool allowDegradedPrinting = false;
			bool allowPrinting = false;
			switch (printing)
			{
				case Printing.None:
					break;
				case Printing.LowResolution:
					allowDegradedPrinting = true;
					break;
				case Printing.HighResolution:
					allowDegradedPrinting = true;
					allowPrinting = true;
					break;
				default:
					throw new NotSupportedException("Invalid Printing specified.");
			}

			PdfComparer.AssertProtection(outputFilePath, protectAuth, this.authenticationManager, true, true,
				expectedAllowDegradedPrinting: allowDegradedPrinting,
				expectedAllowPrinting: allowPrinting);

			if (protectAuth == FileAuthentication.Password)
			{
				using (var permissionsAuthHelper = new AuthenticationManager(permissionsPassword))
				{
					PdfComparer.AssertProtectionAllRights(outputFilePath, FileAuthentication.Password, permissionsAuthHelper, true, true);
				}
			}
		}

		[Test]
		[Combinatorial]
		public void ProtectWithChangeRestrictions(
			[Values(
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication protectAuth,
			[Values(
				Changes.None,
				Changes.Assembly,
				Changes.FillIn,
				Changes.AnnotateAndFillIn,
				Changes.AnyExpectExtract)] Changes changes)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ChangeProtection.Resources.Protect.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Protect.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ChangeProtectionProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureProtectFunctionValues(designer, protectAuth, Encryption.AES256, true, true, changes: changes);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<ChangeProtectionProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			bool allowAssembly = false;
			bool allowFillIn = false;
			bool allowModifyAnnotations = false;
			bool allowModifyContents = false;
			switch (changes)
			{
				case Changes.None:
					break;
				case Changes.Assembly:
					allowAssembly = true;
					break;
				case Changes.FillIn:
					allowFillIn = true;
					break;
				case Changes.AnnotateAndFillIn:
					allowModifyAnnotations = true;
					allowFillIn = true;
					break;
				case Changes.AnyExpectExtract:
					allowModifyContents = true;
					allowModifyAnnotations = true;
					allowFillIn = true;
					break;
				default:
					throw new NotSupportedException("Invalid Changes specified.");
			}

			PdfComparer.AssertProtection(outputFilePath, protectAuth, this.authenticationManager, true, true,
				expectedAllowAssembly: allowAssembly,
				expectedAllowFillIn: allowFillIn,
				expectedAllowModifyAnnotations: allowModifyAnnotations,
				expectedAllowModifyContents: allowModifyContents);

			if (protectAuth == FileAuthentication.Password)
			{
				using (var permissionsAuthHelper = new AuthenticationManager(permissionsPassword))
				{
					PdfComparer.AssertProtectionAllRights(outputFilePath, FileAuthentication.Password, permissionsAuthHelper, true, true);
				}
			}
		}

		[Test]
		[Combinatorial]
		public void ProtectWithCopyRestrictions(
			[Values(
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication protectAuth,
			[Values(
				true,
				false)] bool allowCopy)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ChangeProtection.Resources.Protect.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Protect.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ChangeProtectionProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureProtectFunctionValues(designer, protectAuth, Encryption.AES256, true, true, allowCopy: allowCopy);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<ChangeProtectionProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertProtection(outputFilePath, protectAuth, this.authenticationManager, true, true,
				expectedAllowCopy: allowCopy);

			if (protectAuth == FileAuthentication.Password)
			{
				using (var permissionsAuthHelper = new AuthenticationManager(permissionsPassword))
				{
					PdfComparer.AssertProtectionAllRights(outputFilePath, FileAuthentication.Password, permissionsAuthHelper, true, true);
				}
			}
		}

		[Test]
		[Combinatorial]
		public void ProtectWithScreenReaderRestrictions(
			[Values(
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication protectAuth,
			[Values(
				true,
				false)] bool allowScreenReaders)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ChangeProtection.Resources.Protect.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Protect.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ChangeProtectionProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			ConfigureProtectFunctionValues(designer, protectAuth, Encryption.AES256, true, true, allowScreenReaders: allowScreenReaders);
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<ChangeProtectionProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			PdfComparer.AssertProtection(outputFilePath, protectAuth, this.authenticationManager, true, true,
				expectedAllowScreenReaders: allowScreenReaders);

			if (protectAuth == FileAuthentication.Password)
			{
				using (var permissionsAuthHelper = new AuthenticationManager(permissionsPassword))
				{
					PdfComparer.AssertProtectionAllRights(outputFilePath, FileAuthentication.Password, permissionsAuthHelper, true, true);
				}
			}
		}

		private void ConfigureInputFileFunctionValues(FunctionDesigner designer, FileAuthentication inputAuth, string inputFilePath)
		{
			ConfigureInputFileFunctionValues(
				designer,
				inputAuth,
				inputFilePath,
				PropertyNames.InputFilePath,
				PropertyNames.InputAuthenticationType,
				PropertyNames.InputPassword,
				PropertyNames.InputCertificateSource,
				PropertyNames.InputCertificateFilePath,
				PropertyNames.InputCertificateFilePassword,
				PropertyNames.InputCertificate);
		}

		private void ConfigureProtectFunctionValues(FunctionDesigner designer, FileAuthentication protectAuth, Encryption encryption, bool dontEncryptMetadata,
			bool addDocumentRestrictions = false, Printing printing = Printing.None, Changes changes = Changes.None, bool allowCopy = false, bool allowScreenReaders = false)
		{
			switch (protectAuth)
			{
				case FileAuthentication.None:
					designer.Properties[PropertyNames.Protection].Value = AuthenticationType.None;
					return;
				case FileAuthentication.Password:
					designer.Properties[PropertyNames.Protection].Value = AuthenticationType.Password;
					designer.Properties[PropertyNames.DocumentOpenPassword].Value = this.authenticationManager.Password;
					designer.Properties[PropertyNames.PermissionsPassword].Value = permissionsPassword;
					break;
				case FileAuthentication.CertificateFile:
					designer.Properties[PropertyNames.Protection].Value = AuthenticationType.Certificate;
					designer.Properties[PropertyNames.CertificateSource].Value = CertificateSource.File;
					designer.Properties[PropertyNames.CertificateFilePath].Value = this.authenticationManager.CertificateFilePath;
					designer.Properties[PropertyNames.CertificateFilePassword].Value = this.authenticationManager.CertificateFilePassword;
					break;
				case FileAuthentication.CertificateStore:
					designer.Properties[PropertyNames.Protection].Value = AuthenticationType.Certificate;
					designer.Properties[PropertyNames.CertificateSource].Value = CertificateSource.Store;
					designer.Properties[PropertyNames.Certificate].Value = this.authenticationManager.StoredCertificate;
					break;
			}

			designer.Properties[PropertyNames.Encryption].Value = encryption;
			designer.Properties[PropertyNames.DontEncryptMetadata].Value = dontEncryptMetadata;

			designer.Properties[PropertyNames.AddDocumentRestrictions].Value = addDocumentRestrictions;
			designer.Properties[PropertyNames.AllowPrinting].Value = printing;
			designer.Properties[PropertyNames.AllowChanges].Value = changes;
			designer.Properties[PropertyNames.AllowCopying].Value = allowCopy;
			designer.Properties[PropertyNames.AllowScreenReaders].Value = allowScreenReaders;
		}
	}
}