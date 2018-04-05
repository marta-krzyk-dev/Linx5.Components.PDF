using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Components.Pdf.PdfOperations;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection.Templates
{
	internal partial class ChangeProtectionTemplate : ITemplate
	{
		public AuthenticationType InputAuthenticationType { get; private set; }
		public CertificateSource InputCertificateSource { get; private set; }
		public StoredCertificate InputCertificate { get; private set; }
		public AuthenticationType ProtectProtection { get; private set; }
		public CertificateSource ProtectCertificateSource { get; private set; }
		public StoredCertificate ProtectCertificate { get; private set; }
		public bool ProtectAddDocumentRestrictions { get; private set; }
		public Printing ProtectAllowPrinting { get; private set; }
		public Changes ProtectAllowChanges { get; private set; }
		public bool ProtectAllowCopying { get; private set; }
		public bool ProtectAllowScreenReaders { get; private set; }
		public Encryption ProtectEncryption { get; private set; }
		public bool ProtectDontEncryptMetadata { get; private set; }

		public string InputFilePathParameterName { get; private set; }
		public string InputPasswordParameterName { get; private set; }
		public string InputCertificateFilePathParameterName { get; private set; }
		public string InputCertificateFilePasswordParameterName { get; private set; }

		public string ProtectDocumentOpenPasswordParameterName { get; private set; }
		public string ProtectPermissionsPasswordParameterName { get; private set; }
		public string ProtectCertificateFilePathParameterName { get; private set; }
		public string ProtectCertificateFilePasswordParameterName { get; private set; }

		public string OutputFilePathParameterName { get; private set; }
		public string ContextParameterName { get; private set; }


		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			InputAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.InputCertificate, null);
			ProtectProtection = functionData.TryGetPropertyValue(PropertyNames.ProtectProtection, AuthenticationType.None);
			ProtectCertificateSource = functionData.TryGetPropertyValue(PropertyNames.ProtectCertificateSource, CertificateSource.File);
			ProtectCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.ProtectCertificate, null);
			ProtectAddDocumentRestrictions = functionData.TryGetPropertyValue(PropertyNames.ProtectAddDocumentRestrictions, false);
			ProtectAllowPrinting = functionData.TryGetPropertyValue(PropertyNames.ProtectAllowPrinting, Printing.None);
			ProtectAllowChanges = functionData.TryGetPropertyValue(PropertyNames.ProtectAllowChanges, Changes.None);
			ProtectAllowCopying = functionData.TryGetPropertyValue(PropertyNames.ProtectAllowCopying, false);
			ProtectAllowScreenReaders = functionData.TryGetPropertyValue(PropertyNames.ProtectAllowScreenReaders, false);
			ProtectEncryption = functionData.TryGetPropertyValue(PropertyNames.ProtectEncryption, Encryption.AES128);
			ProtectDontEncryptMetadata = functionData.TryGetPropertyValue(PropertyNames.ProtectDontEncryptMetadata, false);

			InputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePassword);

			ProtectDocumentOpenPasswordParameterName = functionBuilder.GetParamName(PropertyNames.ProtectDocumentOpenPassword);
			ProtectPermissionsPasswordParameterName = functionBuilder.GetParamName(PropertyNames.ProtectPermissionsPassword);
			ProtectCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.ProtectCertificateFilePath);
			ProtectCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.ProtectCertificateFilePassword);

			OutputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}