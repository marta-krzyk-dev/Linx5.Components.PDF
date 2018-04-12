using System.ComponentModel;
using iTextSharp.text.pdf;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection
{
	public enum Encryption
	{
		[Description("RC4 encryption (128 bit)")]
		Standard128 = PdfWriter.STANDARD_ENCRYPTION_128,
		[Description("AES encryption (128 bit)")]
		AES128 = PdfWriter.ENCRYPTION_AES_128,
		[Description("AES encryption (256 bit)")]
		AES256 = PdfWriter.ENCRYPTION_AES_256
	}
}