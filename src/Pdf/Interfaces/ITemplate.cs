using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;

namespace Twenty57.Linx.Components.Pdf.Interfaces
{
	internal interface ITemplate
	{
		void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData);
		string TransformText();
	}
}
