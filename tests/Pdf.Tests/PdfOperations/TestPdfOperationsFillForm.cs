﻿using NUnit.Framework;
using System.Collections.Generic;
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
		[Test]
		public void FillFormAcro(
		[Values(
				FileAuthentication.None,
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication inputAuth)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.FillForm.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "Fill.pdf");

			var formData = new
			{
				First_32Name = "Jane",
				Surname = "Woods",
				Gender = "Female",
				AcceptTCs = true
			};

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			designer.Properties[PropertyNames.Operation].Value = Operation.FillForm;
			designer.Properties[PropertyNames.FillFormFormData].Value = formData;
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			var formValues = new Dictionary<string, string>
			{
				{ "First Name", formData.First_32Name },
				{ "Surname", formData.Surname },
				{ "Gender", formData.Gender },
				{ "AcceptTCs", (formData.AcceptTCs)? "Yes" : "No" }
			};
			PdfComparer.AssertFields(outputFilePath, inputAuth, this.authenticationManager, formValues);
		}

		[Test]
		public void FillFormXfa(
		[Values(
				FileAuthentication.None,
				FileAuthentication.Password/*,
				Ignore: http://stackoverflow.com/questions/40045745/itextsharp-object-reference-error-on-pdfstamper-for-certificate-protected-file
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore*/)] FileAuthentication inputAuth)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.PdfOperations.Resources.FillFormXFA.pdf", this.inputDirectory);
			string outputFilePath = Path.Combine(this.outputDirectory, "FillXfa.pdf");

			var formData = new
			{
				form1_910_93_46FullName_910_93 = "John",
				form1_910_93_46Surname_910_93 = "Doe",
				form1_910_93_46Email_910_93 = "JohnDoe@digiata.com",
				form1_910_93_46EmailMe_910_93 = true
			};

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<PdfOperationsProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			designer.Properties[PropertyNames.Operation].Value = Operation.FillForm;
			designer.Properties[PropertyNames.FillFormFormData].Value = formData;
			designer.Properties[PropertyNames.OutputFilePath].Value = outputFilePath;

			var tester = new FunctionTester<PdfOperationsProvider>();
			tester.Execute(designer.GetProperties(), designer.GetParameters());

			var formValues = new Dictionary<string, string>
			{
				{ "form1[0].FullName[0]", formData.form1_910_93_46FullName_910_93 },
				{ "form1[0].Surname[0]", formData.form1_910_93_46Surname_910_93 },
				{ "form1[0].Email[0]", formData.form1_910_93_46Email_910_93 },
				{ "form1[0].EmailMe[0]", (formData.form1_910_93_46EmailMe_910_93) ? "Yes" : "No" },
				{ "form1[0]", "JohnDoeYesJohnDoe@digiata.com" }
			};
			PdfComparer.AssertFields(outputFilePath, inputAuth, this.authenticationManager, formValues);
		}
	}
}
#pragma warning restore 0612
