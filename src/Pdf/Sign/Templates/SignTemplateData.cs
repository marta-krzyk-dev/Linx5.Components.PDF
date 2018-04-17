using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.Sign.Templates
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
			InputAuthenticationType = functionData.TryGetPropertyValue(Common.PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(Common.PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(Common.PropertyNames.InputCertificate, null);
			SignCertificateSource = functionData.TryGetPropertyValue(PropertyNames.CertificateSource, CertificateSource.File);
			SignCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.Certificate, null);
			SignPlacement = functionData.TryGetPropertyValue(PropertyNames.Placement, SignaturePosition.Hidden);
			SignLockAfterSigning = functionData.TryGetPropertyValue(PropertyNames.LockAfterSigning, false);

			InputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePassword);

			SignSignedAtParameterName = functionBuilder.GetParamName(PropertyNames.SignedAt);
			SignReasonParameterName = functionBuilder.GetParamName(PropertyNames.Reason);
			SignCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePath);
			SignCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePassword);
			SignFieldNameParameterName = functionBuilder.GetParamName(PropertyNames.FieldName);
			SignBackgroundImageParameterName = functionBuilder.GetParamName(PropertyNames.BackgroundImage);
			SignPositionXParameterName = functionBuilder.GetParamName(PropertyNames.PositionX);
			SignPositionYParameterName = functionBuilder.GetParamName(PropertyNames.PositionY);
			SignWidthParameterName = functionBuilder.GetParamName(PropertyNames.Width);
			SignHeightParameterName = functionBuilder.GetParamName(PropertyNames.Height);
			SignPageParameterName = functionBuilder.GetParamName(PropertyNames.Page);

			OutputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}