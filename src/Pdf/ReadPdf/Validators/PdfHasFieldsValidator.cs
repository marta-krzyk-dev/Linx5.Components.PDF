using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using Twenty57.Linx.Plugin.Common.Validation;

namespace Twenty57.Linx.Components.Pdf.ReadPdf.Validators
{
	internal class PdfHasFieldsValidator : Validator
	{
		protected override bool IsValid(object value, string name)
		{
			try
			{
				ErrorMessage = "No form fields found in PDF file.";
				return HasFields(value as string);
			}
			catch
			{
				ErrorMessage = "Could not open PDF file";
				return false;
			}
		}

		private static bool HasFields(string pdfFile)
		{
			if (!File.Exists(pdfFile))
				return false;

			using (var reader = new PdfReader(pdfFile))
			{
				return reader.AcroFields.Fields.Any();
			}
		}
	}
}
