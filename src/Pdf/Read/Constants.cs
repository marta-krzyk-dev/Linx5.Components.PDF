namespace Twenty57.Linx.Components.Pdf.Read
{
	public static class PropertyNames
	{
		public const string AuthenticationType = "Authentication type";
		public const string Certificate = "Certificate";
		public const string CertificateFilePassword = "Certificate file password";
		public const string CertificateFilePath = "Certificate file path";
		public const string CertificateSource = "Certificate source";
		public const string FormDataType = "Form data type";
		public const string PdfFilePath = "PDF file path";
		public const string PdfPassword = "PDF password";
		public const string ReadFormData = "Read form data";
		public const string ReadSignature = "Read signature";
		public const string ReadText = "Read text";
		public const string ReturnFormDataAs = "Return form data as";
		public const string SamplePdf = "Sample PDF";
		public const string SplitText = "Split text";
	}

	public static class OutputNames
	{
		public const string FormData = "FormData";
		public const string FormDataList = "FormDataList";
		public const string Signatures = "Signatures";
		public const string Text = "Text";

		public const string IsSigned = "IsSigned";
		public const string LatestSignature = "LatestSignature";
		public const string AllSignatures = "AllSignatures";

		public const string IsLatestRevision = "IsLatestRevision";
		public const string Reason = "Reason";
		public const string SignedAt = "SignedAt";
		public const string SignedBy = "SignedBy";
		public const string SignedOn = "SignedOn";
		public const string SignedRevision = "SignedRevision";
		public const string Unmodified = "Unmodified";
		public const string Verified = "Verified";
		public const string VerificationMessage = "VerificationMessage";
	}
}