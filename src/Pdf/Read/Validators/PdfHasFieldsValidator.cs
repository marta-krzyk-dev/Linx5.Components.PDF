﻿using iTextSharp.text.pdf;
using System.IO;
using System.Linq;
using Twenty57.Linx.Plugin.Common.Validation;

namespace Twenty57.Linx.Components.Pdf.Read.Validators
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
				if (reader.AcroFields.Xfa.XfaPresent)
					return reader.AcroFields.Xfa.DatasetsSom.Name2Node.Keys.Any();
				else
					return reader.AcroFields.Fields.Keys.Any();
			}
		}
	}
}
