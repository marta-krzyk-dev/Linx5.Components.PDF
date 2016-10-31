using System.ComponentModel;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	public enum Operation
	{
		[Description("Fill form")]
		FillForm,
		[Description("Change protection")]
		Protect,
		Split,
		Concatenate,
		[Description("Add watermark")]
		AddWatermark,
		Sign
	}
}
