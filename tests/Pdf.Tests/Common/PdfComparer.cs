using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using iTextSharp.text.pdf.security;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Twenty57.Linx.Components.Pdf.Tests
{
	internal class PdfComparer : IDisposable
	{
		private PdfReader pdfReader;

		private PdfComparer(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager)
		{
			this.pdfReader = authenticationManager.GetReader(filePath, fileAuth);
		}

		public void Dispose()
		{
			this.pdfReader.Dispose();
		}

		public static void AssertPageCount(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			int expectedNumberOfPages)
		{
			using (var comparer = new PdfComparer(filePath, fileAuth, authenticationManager))
			{
				Assert.AreEqual(expectedNumberOfPages, comparer.pdfReader.NumberOfPages);
			}
		}

		public static void AssertText(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			string expectedText, string expectedJavaScript)
		{
			using (var comparer = new PdfComparer(filePath, fileAuth, authenticationManager))
			{
				var pageText = new List<string>();
				for (int pageIndex = 1; pageIndex <= comparer.pdfReader.NumberOfPages; pageIndex++)
				{
					pageText.Add(PdfTextExtractor.GetTextFromPage(comparer.pdfReader, pageIndex));
				}
				var pagesText = string.Join(Environment.NewLine, pageText);

				Assert.AreEqual(expectedText, pagesText);
				Assert.AreEqual(expectedJavaScript, comparer.pdfReader.JavaScript);
			}
		}

		public static void AssertFields(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			IReadOnlyDictionary<string, string> expectedFieldValues)
		{
			using (var comparer = new PdfComparer(filePath, fileAuth, authenticationManager))
			{
				if (comparer.pdfReader.AcroFields.Xfa.XfaPresent)
				{
					Dictionary<string, string> fieldValues = comparer.pdfReader.AcroFields.Xfa.DatasetsSom.Name2Node.ToDictionary(field => field.Key, field => field.Value.InnerText);
					Assert.AreEqual(expectedFieldValues, fieldValues);
				}
				else
				{
					Dictionary<string, string> fieldValues = comparer.pdfReader.AcroFields.Fields.Keys.ToDictionary(key => key, key => comparer.pdfReader.AcroFields.GetField(key));
					Assert.AreEqual(expectedFieldValues, fieldValues);
				}
			}
		}

		public static void AssertProtectionAllRights(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			bool expectedIsEncrypted,
			bool expectedDontEncryptMetadata)
		{
			AssertProtection(filePath, fileAuth, authenticationManager, expectedIsEncrypted, expectedDontEncryptMetadata, true, true, true, true, true, true, true, true);
		}

		public static void AssertProtection(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			bool expectedIsEncrypted,
			bool expectedDontEncryptMetadata,
			bool expectedAllowAssembly = false,
			bool expectedAllowCopy = false,
			bool expectedAllowDegradedPrinting = false,
			bool expectedAllowFillIn = false,
			bool expectedAllowModifyAnnotations = false,
			bool expectedAllowModifyContents = false,
			bool expectedAllowPrinting = false,
			bool expectedAllowScreenReaders = false)
		{
			using (var comparer = new PdfComparer(filePath, fileAuth, authenticationManager))
			{
				Assert.AreEqual(expectedIsEncrypted, comparer.pdfReader.IsEncrypted());
				Assert.AreEqual(expectedDontEncryptMetadata, !comparer.pdfReader.IsMetadataEncrypted());

				if (expectedAllowAssembly && expectedAllowCopy && expectedAllowDegradedPrinting && expectedAllowFillIn &&
					expectedAllowModifyAnnotations && expectedAllowModifyContents && expectedAllowPrinting && expectedAllowScreenReaders)
					Assert.IsTrue(comparer.pdfReader.IsOpenedWithFullPermissions);
				else
				{
					Assert.AreEqual(expectedAllowAssembly, PdfEncryptor.IsAssemblyAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowCopy, PdfEncryptor.IsCopyAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowDegradedPrinting, PdfEncryptor.IsDegradedPrintingAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowFillIn, PdfEncryptor.IsFillInAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowModifyAnnotations, PdfEncryptor.IsModifyAnnotationsAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowModifyContents, PdfEncryptor.IsModifyContentsAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowPrinting, PdfEncryptor.IsPrintingAllowed((int)comparer.pdfReader.Permissions));
					Assert.AreEqual(expectedAllowScreenReaders, PdfEncryptor.IsScreenReadersAllowed((int)comparer.pdfReader.Permissions));
				}
			}
		}

		public static void AssertPageSignature(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			string expectedSignName, string expectedLocation, string expectedReason, bool expectedLockDocument,
			int expectedPage, float expectedLeft, float expectedTop, float expectedWidth, float expectedHeight)
		{
			using (var comparer = new PdfComparer(filePath, fileAuth, authenticationManager))
			{
				string signatureName = comparer.GetSignatureName();
				comparer.AssertCertificationLevel(expectedLockDocument);
				comparer.AssertSignatureDetails(signatureName, expectedSignName, expectedLocation, expectedReason);
				comparer.AssertSignaturePosition(signatureName, expectedPage, expectedLeft, expectedTop, expectedWidth, expectedHeight);
			}
		}

		public static void AssertFieldSignature(string filePath, FileAuthentication fileAuth, AuthenticationManager authenticationManager,
			string expectedFieldName, string expectedSignName, string expectedLocation, string expectedReason, bool expectedLockDocument)
		{
			using (var comparer = new PdfComparer(filePath, fileAuth, authenticationManager))
			{
				string signatureName = comparer.GetSignatureName();
				Assert.AreEqual(expectedFieldName, signatureName);
				comparer.AssertCertificationLevel(expectedLockDocument);
				comparer.AssertSignatureDetails(signatureName, expectedSignName, expectedLocation, expectedReason);
			}
		}

		private string GetSignatureName()
		{
			AcroFields fields = this.pdfReader.AcroFields;
			List<string> signatureNames = fields.GetSignatureNames();
			if (!signatureNames.Any())
				throw new Exception("No signatures found.");
			if (signatureNames.Count > 1)
				throw new NotSupportedException("Multiple signatures not supported.");

			return signatureNames.First();
		}

		private void AssertCertificationLevel(bool lockDocument)
		{
			int expectedCertificationLevel = (lockDocument) ? PdfSignatureAppearance.CERTIFIED_NO_CHANGES_ALLOWED : PdfSignatureAppearance.NOT_CERTIFIED;
			Assert.AreEqual(expectedCertificationLevel, this.pdfReader.GetCertificationLevel());
		}

		private void AssertSignatureDetails(string signatureName,
			string expectedSignName, string expectedLocation, string expectedReason)
		{
			AcroFields fields = this.pdfReader.AcroFields;
			PdfPKCS7 pk = fields.VerifySignature(signatureName);
			Assert.AreEqual(expectedSignName, CertificateInfo.GetSubjectFields(pk.SigningCertificate).GetField("CN"));
			Assert.AreEqual(expectedLocation, pk.Location);
			Assert.AreEqual(expectedReason, pk.Reason);
			Assert.That(pk.SignDate, Is.EqualTo(DateTime.Now).Within(1).Minutes);
		}

		private void AssertSignaturePosition(string signatureName,
			int expectedPage, float expectedLeft, float expectedTop, float expectedWidth, float expectedHeight)
		{
			AcroFields fields = this.pdfReader.AcroFields;
			IList<AcroFields.FieldPosition> positions = fields.GetFieldPositions(signatureName);
			Assert.AreEqual(1, positions.Count());
			Assert.AreEqual(expectedPage, positions[0].page);
			Rectangle rectangle = positions[0].position;
			Assert.AreEqual(Math.Round(expectedLeft), Math.Round(rectangle.Left));
			Assert.AreEqual(Math.Round(expectedTop), Math.Round(rectangle.Top));
			Assert.AreEqual(Math.Round(expectedWidth), Math.Round(rectangle.Width));
			Assert.AreEqual(Math.Round(expectedHeight), Math.Round(rectangle.Height));
		}
	}
}
