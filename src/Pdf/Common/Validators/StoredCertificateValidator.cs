using System;
using Twenty57.Linx.Plugin.Common.Validation;

namespace Twenty57.Linx.Components.Pdf.Validators
{
	internal class StoredCertificateValidator : Validator
	{
		protected override bool IsValid(object value, string name)
		{
			var certificate = value as StoredCertificate;
			if (certificate == null || string.IsNullOrEmpty(certificate.ThumbPrint))
			{
				ErrorMessage = "Certificate is required.";
				return false;
			}

			try
			{
				certificate.GetCertificate();
				return true;
			}
			catch (Exception exception)
			{
				ErrorMessage = exception.Message;
				return false;
			}
		}
	}
}