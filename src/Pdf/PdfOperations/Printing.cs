using iTextSharp.text.pdf;
using System.ComponentModel;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	public enum Printing
	{
		None = 0,
		[Description("Low resolution (150 dpi)")]
		LowResolution = PdfWriter.ALLOW_DEGRADED_PRINTING,
		[Description("High resolution")]
		HighResolution = PdfWriter.ALLOW_PRINTING
	}
}
