using NUnit.Framework;
using System;
using System.IO;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Tests.Common;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Tests.PdfOperations
{
	[TestFixture]
	public partial class TestPdfOperations : TestPdfBase
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
	}
}
