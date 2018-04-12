using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.AddWatermark.Templates
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
			InputAuthenticationType = functionData.TryGetPropertyValue(Common.PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(Common.PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(Common.PropertyNames.InputCertificate, null);
			WatermarkAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.AuthenticationType, AuthenticationType.None);
			WatermarkCertificateSource = functionData.TryGetPropertyValue(PropertyNames.CertificateSource, CertificateSource.File);
			WatermarkPosition = functionData.TryGetPropertyValue(PropertyNames.Position, WatermarkPosition.Above);
			WatermarkCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.Certificate, null);

			InputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePassword);

			WatermarkFilePathParameterName = functionBuilder.GetParamName(PropertyNames.FilePath);
			WatermarkPasswordParameterName = functionBuilder.GetParamName(PropertyNames.Password);
			WatermarkCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePath);
			WatermarkCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePassword);
			WatermarkPagesParameterName = functionBuilder.GetParamName(PropertyNames.Pages);

			OutputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}
