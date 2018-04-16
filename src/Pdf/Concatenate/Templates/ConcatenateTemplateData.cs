using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.Concatenate.Templates
{
	internal partial class ConcatenateTemplate : ITemplate
	{
		public string InputFilesParameterName { get; private set; }
		public string OutputFilePathParameterName { get; private set; }
		public string ContextParameterName { get; private set; }

		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			InputFilesParameterName = functionBuilder.GetParamName(PropertyNames.InputFiles);
			OutputFilePathParameterName = functionBuilder.GetParamName(Common.PropertyNames.OutputFilePath);
			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}