using System.ComponentModel;
using iTextSharp.text.pdf;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection
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