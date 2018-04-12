using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
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
			InputAuthenticationType = functionData.TryGetPropertyValue(Common.PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(Common.PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(Common.PropertyNames.InputCertificate, null);
			ProtectProtection = functionData.TryGetPropertyValue(PropertyNames.Protection, AuthenticationType.None);
			ProtectCertificateSource = functionData.TryGetPropertyValue(PropertyNames.CertificateSource, CertificateSource.File);
			ProtectCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.Certificate, null);
			ProtectAddDocumentRestrictions = functionData.TryGetPropertyValue(PropertyNames.AddDocumentRestrictions, false);
			ProtectAllowPrinting = functionData.TryGetPropertyValue(PropertyNames.AllowPrinting, Printing.None);
			ProtectAllowChanges = functionData.TryGetPropertyValue(PropertyNames.AllowChanges, Changes.None);
			ProtectAllowCopying = functionData.TryGetPropertyValue(PropertyNames.AllowCopying, false);
			ProtectAllowScreenReaders = functionData.TryGetPropertyValue(PropertyNames.AllowScreenReaders, false);
			ProtectEncryption = functionData.TryGetPropertyValue(PropertyNames.Encryption, Encryption.AES128);
			ProtectDontEncryptMetadata = functionData.TryGetPropertyValue(PropertyNames.DontEncryptMetadata, false);

			InputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePassword);

			ProtectDocumentOpenPasswordParameterName = functionBuilder.GetParamName(PropertyNames.DocumentOpenPassword);
			ProtectPermissionsPasswordParameterName = functionBuilder.GetParamName(PropertyNames.PermissionsPassword);
			ProtectCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePath);
			ProtectCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePassword);

			OutputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}