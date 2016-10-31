using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.PdfOperations.Templates
{
	internal partial class AddWatermarkTemplate : ITemplate
	{
		public AuthenticationType InputAuthenticationType { get; private set; }
		public CertificateSource InputCertificateSource { get; private set; }
		public StoredCertificate InputCertificate { get; private set; }
		public AuthenticationType WatermarkAuthenticationType { get; private set; }
		public CertificateSource WatermarkCertificateSource { get; private set; }
		public StoredCertificate WatermarkCertificate { get; private set; }
		public WatermarkPosition WatermarkPosition { get; private set; }

		public string InputFilePathParameterName { get; private set; }
		public string InputPasswordParameterName { get; private set; }
		public string InputCertificateFilePathParameterName { get; private set; }
		public string InputCertificateFilePasswordParameterName { get; private set; }

		public string WatermarkFilePathParameterName { get; private set; }
		public string WatermarkPasswordParameterName { get; private set; }
		public string WatermarkCertificateFilePathParameterName { get; private set; }
		public string WatermarkCertificateFilePasswordParameterName { get; private set; }
		public string WatermarkPagesParameterName { get; private set; }

		public string OutputFilePathParameterName { get; private set; }
		public string ContextParameterName { get; private set; }

		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			InputAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.InputCertificate, null);
			WatermarkAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.WatermarkAuthenticationType, AuthenticationType.None);
			WatermarkCertificateSource = functionData.TryGetPropertyValue(PropertyNames.WatermarkCertificateSource, CertificateSource.File);
			WatermarkPosition = functionData.TryGetPropertyValue(PropertyNames.WatermarkPosition, WatermarkPosition.Above);
			WatermarkCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.WatermarkCertificate, null);

			InputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePassword);

			WatermarkFilePathParameterName = functionBuilder.GetParamName(PropertyNames.WatermarkFilePath);
			WatermarkPasswordParameterName = functionBuilder.GetParamName(PropertyNames.WatermarkPassword);
			WatermarkCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.WatermarkCertificateFilePath);
			WatermarkCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.WatermarkCertificateFilePassword);
			WatermarkPagesParameterName = functionBuilder.GetParamName(PropertyNames.WatermarkPages);

			OutputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}
