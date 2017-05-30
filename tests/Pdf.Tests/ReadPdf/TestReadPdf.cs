using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Twenty57.Linx.Components.Pdf.ReadPdf;
using Twenty57.Linx.Components.Pdf.Tests.Common;
using Twenty57.Linx.Components.Pdf.Tests.Extensions;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;
using Twenty57.Linx.Plugin.TestKit;

namespace Twenty57.Linx.Components.Pdf.Tests.ReadPdf
{
	[TestFixture]
	public class TestReadPdf : TestPdfBase
	{
		[Test]
		public void ValidateReadTextOutputStructure()
		{
			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionDesigner designer = tester.CreateDesigner();
			Assert.IsNull(designer.Output);

			Property readText = designer.Properties[PropertyNames.ReadText];
			Assert.IsFalse(readText.GetValue<bool>());
			readText.Value = true;

			Property splitText = designer.Properties[PropertyNames.SplitText];
			splitText.Value = TextSplit.Never;
			IEnumerable<ITypeProperty> properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertCompiled(OutputNames.Text, typeof(string));

			splitText.Value = TextSplit.Page;
			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertList(OutputNames.Text, typeof(string));
		}

		[Test]
		public void ValidateReadFormDataOutputStructure()
		{
			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionDesigner designer = tester.CreateDesigner();
			Assert.IsNull(designer.Output);

			Property readFormData = designer.Properties[PropertyNames.ReadFormData];
			Assert.IsFalse(readFormData.GetValue<bool>());
			readFormData.Value = true;

			Property returnDataAs = designer.Properties[PropertyNames.ReturnFormDataAs];
			returnDataAs.Value = FormExtraction.CustomType;
			IEnumerable<ITypeProperty> properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertCompiled(OutputNames.FormData, typeof(object));

			Property formDataType = designer.Properties[PropertyNames.FormDataType];
			formDataType.Value = TypeReference.Create(typeof(string));
			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertCompiled(OutputNames.FormData, typeof(string));

			returnDataAs.Value = FormExtraction.Infer;
			Property samplePdf = designer.Properties[PropertyNames.SamplePdf];
			Assert.AreEqual(string.Empty, samplePdf.GetValue<string>());
			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertGenerated(OutputNames.FormData);
			Assert.AreEqual(0, properties.ElementAt(0).TypeReference.GetProperties().Count());

			string blankPdfFile = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.Blank.pdf", this.inputDirectory);
			samplePdf.Value = blankPdfFile;
			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertGenerated(OutputNames.FormData);
			Assert.AreEqual(0, properties.ElementAt(0).TypeReference.GetProperties().Count());

			string inferPdfFile = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.InferFields.pdf", this.inputDirectory);
			samplePdf.Value = inferPdfFile;
			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertGenerated(OutputNames.FormData);
			IEnumerable<ITypeProperty> outputProperties = properties.ElementAt(0).TypeReference.GetProperties();
			Assert.AreEqual(2, outputProperties.Count());
			outputProperties.ElementAt(0).AssertCompiled("First_32Name", typeof(string));
			outputProperties.ElementAt(1).AssertCompiled("Surname", typeof(string));

			returnDataAs.Value = FormExtraction.List;
			properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertList(OutputNames.FormDataList, typeof(KeyValuePair<string, string>));
		}

		[Test]
		public void ValidateReadSignatureOutputStructure()
		{
			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionDesigner designer = tester.CreateDesigner();
			Assert.IsNull(designer.Output);

			Property readSignature = designer.Properties[PropertyNames.ReadSignature];
			Assert.IsFalse(readSignature.GetValue<bool>());
			readSignature.Value = true;

			IEnumerable<ITypeProperty> properties = designer.Output.GetProperties();
			Assert.AreEqual(1, properties.Count());
			properties.ElementAt(0).AssertGenerated(OutputNames.Signatures);

			properties = properties.ElementAt(0).TypeReference.GetProperties();
			Assert.AreEqual(3, properties.Count());
			properties.ElementAt(0).AssertCompiled(OutputNames.IsSigned, typeof(bool));
			properties.ElementAt(1).AssertGenerated(OutputNames.LatestSignature);
			ITypeReference signatureType = properties.ElementAt(1).TypeReference;
			properties.ElementAt(2).AssertList(OutputNames.AllSignatures, signatureType);

			properties = signatureType.GetProperties();
			Assert.AreEqual(9, properties.Count());
			properties.ElementAt(0).AssertCompiled(OutputNames.SignedBy, typeof(string));
			properties.ElementAt(1).AssertCompiled(OutputNames.SignedAt, typeof(string));
			properties.ElementAt(2).AssertCompiled(OutputNames.Reason, typeof(string));
			properties.ElementAt(3).AssertCompiled(OutputNames.SignedOn, typeof(DateTime));
			properties.ElementAt(4).AssertCompiled(OutputNames.Unmodified, typeof(bool));
			properties.ElementAt(5).AssertCompiled(OutputNames.SignedRevision, typeof(int));
			properties.ElementAt(6).AssertCompiled(OutputNames.IsLatestRevision, typeof(bool));
			properties.ElementAt(7).AssertCompiled(OutputNames.Verified, typeof(bool));
			properties.ElementAt(8).AssertCompiled(OutputNames.VerificationMessage, typeof(string));
		}

		[Test]
		public void ReadWithNoInputFileSpecified([Values(null, "")] string inputFile)
		{
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			designer.Properties[PropertyNames.PdfFilePath].Value = inputFile;

			Assert.That(() => new FunctionTester<ReadPdfProvider>().Execute(designer.GetProperties(), designer.GetParameters()),
				Throws.Exception.TypeOf<ExecuteException>()
				.With.Property("Message").EqualTo("Value cannot be null.\r\nParameter name: PDF_32file_32path\r\nSee Code and Parameter properties for more information."));
		}

		[Test]
		public void ReadWithInvalidInputFileSpecified()
		{
			string invalidFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Assert.IsFalse(File.Exists(invalidFilePath));

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			designer.Properties[PropertyNames.PdfFilePath].Value = invalidFilePath;

			Assert.That(() => new FunctionTester<ReadPdfProvider>().Execute(designer.GetProperties(), designer.GetParameters()),
				Throws.Exception.TypeOf<ExecuteException>()
				.With.Property("Message").EqualTo($"File [{invalidFilePath}] does not exist.\r\nSee Code and Parameter properties for more information."));
		}

		[Test]
		public void ReadWithNoOutput(
			[Values(
				FileAuthentication.None,
				FileAuthentication.Password,
				FileAuthentication.CertificateFile,
				FileAuthentication.CertificateStore)] FileAuthentication inputAuth)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.Blank.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, inputAuth, inputFilePath);
			designer.Properties[PropertyNames.ReadText].Value = false;
			designer.Properties[PropertyNames.ReadFormData].Value = false;
			designer.Properties[PropertyNames.ReadSignature].Value = false;

			var tester = new FunctionTester<ReadPdfProvider>();
			Assert.DoesNotThrow(() => tester.Execute(designer.GetProperties(), designer.GetParameters()));
		}

		[Test]
		public void ReadText(
			[Values(
				TextSplit.Never,
				TextSplit.Page)] TextSplit splitText)
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.Text.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadText].Value = true;
			designer.Properties[PropertyNames.SplitText].Value = splitText;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			switch (splitText)
			{
				case TextSplit.Never:
					Assert.AreEqual("Text on page 1\nFooter text on page 1\r\nText on page 2\r\nText on page 3", result.Value.Text);
					break;
				case TextSplit.Page:
					Assert.AreEqual(new List<string> { "Text on page 1\nFooter text on page 1", "Text on page 2", "Text on page 3" }, result.Value.Text);
					break;
			}
		}

		[Test]
		public void ReadAcroFormDataWithCustomTypeOutput()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormData.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadFormData].Value = true;

			ITypeReference dataType = TypeReference.CreateGeneratedType(
				new TypeProperty("First_32Name", typeof(string)),
				new TypeProperty("Surname", typeof(string)),
				new TypeProperty("Gender", typeof(string)),
				new TypeProperty("AcceptTCs", typeof(string)));

			designer.Properties[PropertyNames.ReturnFormDataAs].Value = FormExtraction.CustomType;
			designer.Properties[PropertyNames.FormDataType].Value = dataType;

			var tester = new FunctionTester<ReadPdfProvider>();
			tester.CustomTypes.Add(dataType);
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.AreEqual("Jeremy", result.Value.FormData.First_32Name);
			Assert.AreEqual("Woods", result.Value.FormData.Surname);
			Assert.AreEqual("Male", result.Value.FormData.Gender);
			Assert.AreEqual("Yes", result.Value.FormData.AcceptTCs);
		}

		[Test]
		public void ReadXfaFormDataWithCustomTypeOutput()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormDataXFA.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadFormData].Value = true;

			ITypeReference dataType = TypeReference.CreateGeneratedType(
				new TypeProperty("form1_910_93_46FullName_910_93", typeof(string)),
				new TypeProperty("form1_910_93_46Surname_910_93", typeof(string)),
				new TypeProperty("form1_910_93_46Email_910_93", typeof(string)),
				new TypeProperty("form1_910_93_46EmailMe_910_93", typeof(string)));

			designer.Properties[PropertyNames.ReturnFormDataAs].Value = FormExtraction.CustomType;
			designer.Properties[PropertyNames.FormDataType].Value = dataType;

			var tester = new FunctionTester<ReadPdfProvider>();
			tester.CustomTypes.Add(dataType);
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.AreEqual("John", result.Value.FormData.form1_910_93_46FullName_910_93);
			Assert.AreEqual("Doe", result.Value.FormData.form1_910_93_46Surname_910_93);
			Assert.AreEqual("jdoe@digiata.com", result.Value.FormData.form1_910_93_46Email_910_93);
			Assert.AreEqual("1", result.Value.FormData.form1_910_93_46EmailMe_910_93);
		}

		[Test]
		public void ReadAcroFormDataWithInferredOutput()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormData.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadFormData].Value = true;

			designer.Properties[PropertyNames.ReturnFormDataAs].Value = FormExtraction.Infer;
			string inferFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormData.pdf", this.inputDirectory);
			designer.Properties[PropertyNames.SamplePdf].Value = inferFilePath;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.AreEqual("Jeremy", result.Value.FormData.First_32Name);
			Assert.AreEqual("Woods", result.Value.FormData.Surname);
			Assert.AreEqual("Male", result.Value.FormData.Gender);
			Assert.AreEqual("Yes", result.Value.FormData.AcceptTCs);
		}

		[Test]
		public void ReadXfaFormDataWithInferredOutput()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormDataXFA.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadFormData].Value = true;

			designer.Properties[PropertyNames.ReturnFormDataAs].Value = FormExtraction.Infer;
			string inferFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.InferFieldsXFA.pdf", this.inputDirectory);
			designer.Properties[PropertyNames.SamplePdf].Value = inferFilePath;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.AreEqual("John", result.Value.FormData.form1_910_93_46FullName_910_93);
			Assert.AreEqual("Doe", result.Value.FormData.form1_910_93_46Surname_910_93);
			Assert.AreEqual("jdoe@digiata.com", result.Value.FormData.form1_910_93_46Email_910_93);
			Assert.AreEqual("1", result.Value.FormData.form1_910_93_46EmailMe_910_93);
		}

		[Test]
		public void ReadAcroFormDataWithListOutput()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormData.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadFormData].Value = true;
			designer.Properties[PropertyNames.ReturnFormDataAs].Value = FormExtraction.List;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			List<KeyValuePair<string, string>> dataList = result.Value.FormDataList;
			Assert.AreEqual(4, dataList.Count);
			KeyValuePair<string, string> item = dataList[0];
			Assert.AreEqual("First Name", item.Key);
			Assert.AreEqual("Jeremy", item.Value);
			item = dataList[1];
			Assert.AreEqual("Surname", item.Key);
			Assert.AreEqual("Woods", item.Value);
			item = dataList[2];
			Assert.AreEqual("Gender", item.Key);
			Assert.AreEqual("Male", item.Value);
			item = dataList[3];
			Assert.AreEqual("AcceptTCs", item.Key);
			Assert.AreEqual("Yes", item.Value);
		}

		[Test]
		public void ReadXfaFormDataWithListOutput()
		{
			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.FormDataXFA.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadFormData].Value = true;
			designer.Properties[PropertyNames.ReturnFormDataAs].Value = FormExtraction.List;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			List<KeyValuePair<string, string>> dataList = result.Value.FormDataList;
			Assert.AreEqual(5, dataList.Count);
			KeyValuePair<string, string> item = dataList[0];
			Assert.AreEqual("form1[0].FullName[0]", item.Key);
			Assert.AreEqual("John", item.Value);
			item = dataList[1];
			Assert.AreEqual("form1[0].Surname[0]", item.Key);
			Assert.AreEqual("Doe", item.Value);
			item = dataList[2];
			Assert.AreEqual("form1[0].EmailMe[0]", item.Key);
			Assert.AreEqual("1", item.Value);
			item = dataList[3];
			Assert.AreEqual("form1[0].Email[0]", item.Key);
			Assert.AreEqual("jdoe@digiata.com", item.Value);
			item = dataList[4];
			Assert.AreEqual("form1[0]", item.Key);
			Assert.AreEqual("JohnDoe1jdoe@digiata.com", item.Value);
		}

		[Test]
		public void ReadSignature()
		{
			var store = new X509Store(StoreName.TrustedPeople, StoreLocation.CurrentUser);
			store.Open(OpenFlags.ReadWrite);
			store.Add(this.authenticationManager.Certificate);
			store.Close();

			string inputFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.Signature.pdf", this.inputDirectory);
			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			ConfigureInputFileFunctionValues(designer, FileAuthentication.None, inputFilePath);
			designer.Properties[PropertyNames.ReadSignature].Value = true;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.IsTrue(result.Value.Signatures.IsSigned);
			AssertSignature(result.Value.Signatures.LatestSignature, false, "I moderated the doc", "Office location 2", "Jane Doe", new DateTime(2016, 8, 8, 17, 12, 57), true, 1,
				"A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider.\r\n", false);
			dynamic allSignatures = result.Value.Signatures.AllSignatures;
			Assert.AreEqual(2, allSignatures.Count);
			AssertSignature(allSignatures[0], false, "I created the doc", "Office location 1", "John Smith", new DateTime(2016, 8, 8, 17, 12, 14), true, 1,
				string.Empty, true);
			AssertSignature(allSignatures[1], false, "I moderated the doc", "Office location 2", "Jane Doe", new DateTime(2016, 8, 8, 17, 12, 57), true, 1,
				"A certificate chain processed, but terminated in a root certificate which is not trusted by the trust provider.\r\n", false);

			store.Open(OpenFlags.ReadWrite);
			store.Remove(this.authenticationManager.Certificate);
			store.Close();
		}

		[Test]
		public void ReadSignatureWithUnsignedDocument()
		{
			string blankPdfFile = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.ReadPdf.Resources.Blank.pdf", this.inputDirectory);

			FunctionDesigner designer = ProviderHelpers.CreateDesigner<ReadPdfProvider>();
			designer.Properties[PropertyNames.ReadSignature].Value = true;
			designer.Properties[PropertyNames.PdfFilePath].Value = blankPdfFile;
			designer.Properties[PropertyNames.AuthenticationType].Value = AuthenticationType.None;

			var tester = new FunctionTester<ReadPdfProvider>();
			FunctionResult result = tester.Execute(designer.GetProperties(), designer.GetParameters());

			Assert.IsFalse(result.Value.Signatures.IsSigned);
			Assert.AreEqual(0, result.Value.Signatures.AllSignatures.Count);
			AssertSignature(result.Value.Signatures.LatestSignature, true, string.Empty, string.Empty, string.Empty, DateTime.MinValue, true, 0, "No certificate found.", false);
		}

		private void ConfigureInputFileFunctionValues(FunctionDesigner designer, FileAuthentication inputAuth, string inputFilePath)
		{
			ConfigureInputFileFunctionValues(
				designer,
				inputAuth,
				inputFilePath,
				PropertyNames.PdfFilePath,
				PropertyNames.AuthenticationType,
				PropertyNames.PdfPassword,
				PropertyNames.CertificateSource,
				PropertyNames.CertificateFilePath,
				PropertyNames.CertificateFilePassword,
				PropertyNames.Certificate);
		}

		private static void AssertSignature(dynamic actualSignature,
			bool expectedIsLatestRevision, string expectedReason, string expectedSignedAt, string expectedSignedBy, DateTime expectedSignedOn,
			bool expectedUnmodified, int expectedSignedRevision, string expectedVerificationMessage, bool expectedVerified)
		{
			Assert.AreEqual(expectedIsLatestRevision, actualSignature.IsLatestRevision);
			Assert.AreEqual(expectedReason, actualSignature.Reason);
			Assert.AreEqual(expectedSignedAt, actualSignature.SignedAt);
			Assert.AreEqual(expectedSignedBy, actualSignature.SignedBy);
			Assert.AreEqual(expectedSignedOn, actualSignature.SignedOn);
			Assert.AreEqual(expectedUnmodified, actualSignature.Unmodified);
			Assert.AreEqual(expectedSignedRevision, actualSignature.SignedRevision);
			Assert.AreEqual(expectedVerificationMessage, actualSignature.VerificationMessage);
			Assert.AreEqual(expectedVerified, actualSignature.Verified);
		}
	}
}
