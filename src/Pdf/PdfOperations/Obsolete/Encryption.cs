using iTextSharp.text.pdf;
using System;

namespace Twenty57.Linx.Components.Pdf.PdfOperations.Obsolete
{
	[Obsolete("Required to update from old versions")]
	public enum Encryption
	{
		Standard = PdfWriter.STANDARD_ENCRYPTION_40,
		Standard128 = PdfWriter.STANDARD_ENCRYPTION_128,
		AES = PdfWriter.ENCRYPTION_AES_128,
		AES256 = PdfWriter.ENCRYPTION_AES_256
	}
}
