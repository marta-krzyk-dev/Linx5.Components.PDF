using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.PdfOperations.Templates
{
	internal partial class SignTemplate : ITemplate
	{
		public AuthenticationType InputAuthenticationType { get; private set; }
		public CertificateSource InputCertificateSource { get; private set; }
		public StoredCertificate InputCertificate { get; private set; }
		public CertificateSource SignCertificateSource { get; private set; }
		public StoredCertificate SignCertificate { get; private set; }
		public SignaturePosition SignPlacement { get; private set; }
		public bool SignLockAfterSigning { get; private set; }

		public string InputFilePathParameterName { get; private set; }
		public string InputPasswordParameterName { get; private set; }
		public string InputCertificateFilePathParameterName { get; private set; }
		public string InputCertificateFilePasswordParameterName { get; private set; }

		public string SignSignedAtParameterName { get; private set; }
		public string SignReasonParameterName { get; private set; }
		public string SignCertificateFilePathParameterName { get; private set; }
		public string SignCertificateFilePasswordParameterName { get; private set; }
		public string SignFieldNameParameterName { get; private set; }
		public string SignPositionXParameterName { get; private set; }
		public string SignPositionYParameterName { get; private set; }
		public string SignWidthParameterName { get; private set; }
		public string SignHeightParameterName { get; private set; }
		public string SignBackgroundImageParameterName { get; private set; }
		public string SignPageParameterName { get; private set; }

		public string OutputFilePathParameterName { get; private set; }
		public string ContextParameterName { get; private set; }

		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			InputAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.InputCertificate, null);
			SignCertificateSource = functionData.TryGetPropertyValue(PropertyNames.SignCertificateSource, CertificateSource.File);
			SignCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.SignCertificate, null);
			SignPlacement = functionData.TryGetPropertyValue(PropertyNames.SignPlacement, SignaturePosition.Hidden);
			SignLockAfterSigning = functionData.TryGetPropertyValue(PropertyNames.SignLockAfterSigning, false);

			InputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePassword);

			SignSignedAtParameterName = functionBuilder.GetParamName(PropertyNames.SignSignedAt);
			SignReasonParameterName = functionBuilder.GetParamName(PropertyNames.SignReason);
			SignCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.SignCertificateFilePath);
			SignCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.SignCertificateFilePassword);
			SignFieldNameParameterName = functionBuilder.GetParamName(PropertyNames.SignFieldName);
			SignBackgroundImageParameterName = functionBuilder.GetParamName(PropertyNames.SignBackgroundImage);
			SignPositionXParameterName = functionBuilder.GetParamName(PropertyNames.SignPositionX);
			SignPositionYParameterName = functionBuilder.GetParamName(PropertyNames.SignPositionY);
			SignWidthParameterName = functionBuilder.GetParamName(PropertyNames.SignWidth);
			SignHeightParameterName = functionBuilder.GetParamName(PropertyNames.SignHeight);
			SignPageParameterName = functionBuilder.GetParamName(PropertyNames.SignPage);

			OutputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}
