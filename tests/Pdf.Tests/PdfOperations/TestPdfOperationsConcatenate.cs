using NUnit.Framework;
using System;
using System.Collections.Generic;
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
		public void Concatenate()
		{
			string inputFilePath1 = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Concatenate1.pdf", this.inputDirectory);
			PdfComparer.AssertPageCount(inputFilePath1, FileAuthentication.None, this.authenticationManager, 1);
			PdfComparer.AssertText(inputFilePath1, FileAuthentication.None, this.authenticationManager, "1", "function Script1()\r\n{}\n");
			string inputFilePath2 = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Concatenate2.pdf", this.inputDirectory);
			PdfComparer.AssertPageCount(inputFilePath2, FileAuthentication.None, this.authenticationManager, 1);
			PdfComparer.AssertText(inputFilePath2, FileAuthentication.None, this.authenticationManager, "2", "function Script2()\r\n{}\n");
			string outputFilePath = Path.Combine(this.outputDirectory, "Concat.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			designer.Properties[PropertyNames.Operation].Value = Operation.Concatenate;
			designer.Properties[PropertyNames.InputFiles].Value = new List<string> { inputFilePath1, inputFilePath2 };
			designer.Properties[PropertyNames.InputAuthenticationType].Value = AuthenticationType.None;
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.IsTrue(File.Exists(outputFilePath));
			PdfComparer.AssertPageCount(outputFilePath, FileAuthentication.None, this.authenticationManager, 2);
			PdfComparer.AssertText(outputFilePath, FileAuthentication.None, this.authenticationManager, $"1{Environment.NewLine}2", "function Script1()\r\n{}\n\nfunction Script2()\r\n{}\n\n");
		}

		[Test]
		public void ConcatenameWithInvalidInputFileSpecified()
		{
			string inputFilePath1 = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.Concatenate1.pdf", this.inputDirectory);
			string invalidFilePath = Path.Combine(this.inputDirectory, Path.GetRandomFileName());
			string outputFilePath = Path.Combine(this.outputDirectory, "Concat.pdf");

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			designer.Properties[PropertyNames.Operation].Value = Operation.Concatenate;
			designer.Properties[PropertyNames.InputFiles].Value = new List<string> { inputFilePath1, invalidFilePath };
			designer.Properties[PropertyNames.InputAuthenticationType].Value = AuthenticationType.None;
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			Assert.That(() => new FunctionTester<PdfOperationsProvider>().Execute(designer.GetProperties(), designer.GetParameters()),
				Throws.Exception.TypeOf<ExecuteException>()
				.With.Property("Message").EqualTo($"File [{invalidFilePath}] does not exist.\r\nSee Code and Parameter properties for more information."));
		}
	}
}
