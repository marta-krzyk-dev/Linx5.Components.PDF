using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.FillForm.Templates
{
	internal partial class FillFormTemplate : ITemplate
	{
		public AuthenticationType InputAuthenticationType { get; private set; }
		public CertificateSource InputCertificateSource { get; private set; }
		public StoredCertificate InputCertificate { get; private set; }

		public string InputFilePathParameterName { get; private set; }
		public string InputPasswordParameterName { get; private set; }
		public string InputCertificateFilePathParameterName { get; private set; }
		public string InputCertificateFilePasswordParameterName { get; private set; }

		public string FillFormFormDataParameterName { get; private set; }

		public string OutputFilePathParameterName { get; private set; }
		public string ContextParameterName { get; private set; }

		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			InputAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.InputCertificate, null);

			InputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePassword);

			FillFormFormDataParameterName = functionBuilder.GetParamName(FillFormProvider.FormData);

			OutputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}