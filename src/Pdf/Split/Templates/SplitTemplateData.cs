using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.Split.Templates
{
	internal partial class SplitTemplate : ITemplate
	{
		public AuthenticationType InputAuthenticationType { get; private set; }
		public CertificateSource InputCertificateSource { get; private set; }
		public StoredCertificate InputCertificate { get; private set; }
		public bool SplitLoopResults { get; private set; }

		public string InputFilePathParameterName { get; private set; }
		public string InputPasswordParameterName { get; private set; }
		public string InputCertificateFilePathParameterName { get; private set; }
		public string InputCertificateFilePasswordParameterName { get; private set; }

		public string OutputFilePathParameterName { get; private set; }
		public string ContextParameterName { get; private set; }

		public string ExecutionPathOutputParameterName { get; private set; }
		public string ExecutionPathPageFilesPropertyName { get; private set; }

		public string OutputTypeName { get; private set; }
		public string OutputNumberOfPagesPropertyName { get; private set; }
		public string OutputPageFilesPropertyName { get; private set; }

		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			InputAuthenticationType = functionData.TryGetPropertyValue(Common.PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(Common.PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(Common.PropertyNames.InputCertificate, null);
			SplitLoopResults = functionData.TryGetPropertyValue(PropertyNames.LoopResults, false);

			InputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(Common.PropertyNames.InputCertificateFilePassword);

			OutputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;

			ExecutionPathOutputParameterName = functionBuilder.ExecutionPathOutParamName;
			ExecutionPathPageFilesPropertyName = functionBuilder.GetParamName(PropertyNames.ExecutionPathName);

			OutputTypeName = functionBuilder.GetTypeName(functionData.Output);
			OutputNumberOfPagesPropertyName = functionBuilder.GetParamName(PropertyNames.OutputNumberOfPages);
			OutputPageFilesPropertyName = functionBuilder.GetParamName(PropertyNames.OutputPageFiles);
		}
	}
}
