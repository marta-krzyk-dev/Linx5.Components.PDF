using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twenty57.Linx.Components.Pdf.Split;
using Twenty57.Linx.Components.Pdf.Tests.Common;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;
using Twenty57.Linx.Plugin.Common.Types;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.Split
{
	[TestFixture]
	public class TestSplit : TestPdfBase
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
		public void ValidateSplitOutputStructure()
		{
			var tester = new FunctionTester<SplitProvider>();
			FunctionDesigner designer = tester.CreateDesigner();

			Property loopResults = designer.Properties[PropertyNames.LoopResults];
			loopResults.Value = false;

			Assert.AreEqual(0, designer.ExecutionPaths.Count);

			IEnumerable<ITypeProperty> properties = designer.Output.GetProperties();
			Assert.AreEqual(2, properties.Count());
			properties.ElementAt(0).AssertList(OutputNames.PageFiles, typeof(string));
			properties.ElementAt(1).AssertCompiled(OutputNames.NumberOfPages, typeof(int));

			loopResults.Value = true;

			Assert.AreEqual(1, designer.ExecutionPaths.Count);
			ExecutionPath executionPath = designer.ExecutionPaths[0];
			Assert.AreEqual(ExecutionPathNames.PageFiles, executionPath.Name);
			Assert.AreEqual(TypeReference.Create(typeof(string)), executionPath.Output);

			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertCompiled(OutputNames.NumberOfPages, typeof(int));
		}

		[Test]
		[Combinatorial]
		public void Split(
			[Values(
				FileAuthentication.None,
				FileAuthentication.Password,
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore)] FileAuthentication inputAuth,
			[Values(
				true,
				false)] bool loopResults)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Split.Resources.Split.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Split.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<SplitProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			designer.Properties[PropertyNames.LoopResults].Value = loopResults;
			designer.Properties[Pdf.Common.PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<SplitProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.AreEqual(2, result.Value.NumberOfPages);
			if (loopResults)
			{
				Assert.AreEqual(2, result.ExecutionPathResult.Count());
				NextResult nextResult = result.ExecutionPathResult.ElementAt(0);
				Assert.AreEqual(ExecutionPathNames.PageFiles, nextResult.Name);
				AssertOutputFile(nextResult.Value, inputAuth, Path.Combine(this.outputDirectory, "Split_1.pdf"), 1, "1");

				nextResult = result.ExecutionPathResult.ElementAt(1);
				Assert.AreEqual(ExecutionPathNames.PageFiles, nextResult.Name);
				AssertOutputFile(nextResult.Value, inputAuth, Path.Combine(this.outputDirectory, "Split_2.pdf"), 1, "2");
			}
			else
			{
				Assert.AreEqual(2, result.Value.PageFiles.Count);
				string pageFile = result.Value.PageFiles[0];
				AssertOutputFile(pageFile, inputAuth, Path.Combine(this.outputDirectory, "Split_1.pdf"), 1, "1");

				pageFile = result.Value.PageFiles[1];
				AssertOutputFile(pageFile, inputAuth, Path.Combine(this.outputDirectory, "Split_2.pdf"), 1, "2");
			}
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

		private void AssertOutputFile(string outputFilePath, FileAuthentication outputAuth, string expectedFilePath, int expectedNumberOfPages, string expectedText)
		{
			Assert.IsTrue(File.Exists(outputFilePath));
			Assert.AreEqual(expectedFilePath, outputFilePath);
			PdfComparer.AssertPageCount(outputFilePath, outputAuth, this.authenticationManager, expectedNumberOfPages);
			PdfComparer.AssertText(outputFilePath, outputAuth, this.authenticationManager, expectedText, null);
		}
	}
}