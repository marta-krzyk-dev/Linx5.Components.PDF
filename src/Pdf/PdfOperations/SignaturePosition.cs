using System.ComponentModel;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	public enum SignaturePosition
	{
		[Description("No visible signature")]
		Hidden,
		[Description("Form signature field")]
		FormField,
		[Description("On page")]
		OnPage
	}
}
