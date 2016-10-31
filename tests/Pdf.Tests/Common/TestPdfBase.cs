using NUnit.Framework;
using System;
using System.IO;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Tests.Common
{
	public abstract class TestPdfBase
	{
		protected AuthenticationManager authenticationManager;

		protected string inputDirectory;

		[OneTimeSetUp]
		public void OneTimeSetupBase()
		{
			this.authenticationManager = new AuthenticationManager();
		}

		[OneTimeTearDown]
		public void OneTimeTeardownBase()
		{
			this.authenticationManager.Dispose();
		}

		[SetUp]
		public void SetupBase()
		{
			this.inputDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(this.inputDirectory);
		}

		[TearDown]
		public void TeardownBase()
		{
			Directory.Delete(this.inputDirectory, true);
		}

		protected void ConfigureInputFileFunctionValues(
			FunctionDesigner designer,
			FileAuthentication inputAuth,
			string inputFilePath,
			string inputFilePathPropertyName,
			string authenticationTypePropertyName,
			string inputPasswordPropertyName,
			string certificateSourcePropertyName,
			string certificateFilePathPropertyName,
			string certificateFilePasswordPropertyName,
			string certificatePropertyName)
		{
			this.authenticationManager.Protect(inputFilePath, inputAuth);
			designer.Properties[inputFilePathPropertyName].Value = inputFilePath;

			switch (inputAuth)
			{
				case FileAuthentication.None:
					designer.Properties[authenticationTypePropertyName].Value = AuthenticationType.None;
					break;
				case FileAuthentication.Password:
					designer.Properties[authenticationTypePropertyName].Value = AuthenticationType.Password;
					designer.Properties[inputPasswordPropertyName].Value = this.authenticationManager.Password;
					break;
				case FileAuthentication.CertificateFile:
					designer.Properties[authenticationTypePropertyName].Value = AuthenticationType.Certificate;
					designer.Properties[certificateSourcePropertyName].Value = CertificateSource.File;
					designer.Properties[certificateFilePathPropertyName].Value = this.authenticationManager.CertificateFilePath;
					designer.Properties[certificateFilePasswordPropertyName].Value = this.authenticationManager.Password;
					break;
				case FileAuthentication.CertificateStore:
					designer.Properties[authenticationTypePropertyName].Value = AuthenticationType.Certificate;
					designer.Properties[certificateSourcePropertyName].Value = CertificateSource.Store;
					designer.Properties[certificatePropertyName].Value = this.authenticationManager.StoredCertificate;
					break;
				default:
					throw new NotSupportedException("Invalid FileAuthentication specified.");
			}
		}
	}
}
