using Twenty57.Linx.Plugin.Common.Validation;

namespace Twenty57.Linx.Components.Pdf.Validators
{
	internal class CertificateAuthenticationValidator : Validator
	{
		protected override bool IsValid(object value, string name)
		{
			if ((AuthenticationType)value == AuthenticationType.Certificate)
			{
				ErrorMessage = "Certificate authentication is not currently supported.";
				return false;
			}
			else
				return true;
		}
	}
}