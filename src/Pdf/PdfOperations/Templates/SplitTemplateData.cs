using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.PdfOperations.Templates
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
			InputAuthenticationType = functionData.TryGetPropertyValue(PropertyNames.InputAuthenticationType, AuthenticationType.None);
			InputCertificateSource = functionData.TryGetPropertyValue(PropertyNames.InputCertificateSource, CertificateSource.File);
			InputCertificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.InputCertificate, null);
			SplitLoopResults = functionData.TryGetPropertyValue(PropertyNames.SplitLoopResults, false);

			InputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputFilePath);
			InputPasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputPassword);
			InputCertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePath);
			InputCertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.InputCertificateFilePassword);

			OutputFilePathParameterName = functionBuilder.GetParamName(PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;

			ExecutionPathOutputParameterName = functionBuilder.ExecutionPathOutParamName;
			ExecutionPathPageFilesPropertyName = functionBuilder.GetParamName(ExecutionPathNames.PageFiles);

			OutputTypeName = functionBuilder.GetTypeName(functionData.Output);
			OutputNumberOfPagesPropertyName = functionBuilder.GetParamName(OutputNames.NumberOfPages);
			OutputPageFilesPropertyName = functionBuilder.GetParamName(OutputNames.PageFiles);
		}
	}
}
