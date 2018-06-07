using System.ComponentModel;

namespace Twenty57.Linx.Components.Pdf.Read
{
	public enum FormExtraction
	{
		[Description("Custom type")]
		CustomType,
		[Description("Infer type from a sample PDF")]
		Infer,
		List
	}
}
