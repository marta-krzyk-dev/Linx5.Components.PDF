using iTextSharp.text.pdf;
using System.ComponentModel;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	public enum Changes
	{
		None = 0,
		[Description("Inserting, deleting and rotating pages")]
		Assembly = PdfWriter.ALLOW_ASSEMBLY,
		[Description("Filling in form fields and signing existing signature fields")]
		FillIn = PdfWriter.ALLOW_FILL_IN,
		[Description("Commenting, filling in form fields and signing existing signature fields")]
		AnnotateAndFillIn = PdfWriter.ALLOW_MODIFY_ANNOTATIONS | PdfWriter.ALLOW_FILL_IN,
		[Description("Any except extracting pages")]
		AnyExpectExtract = PdfWriter.ALLOW_MODIFY_CONTENTS | PdfWriter.ALLOW_MODIFY_ANNOTATIONS | PdfWriter.ALLOW_FILL_IN
	}
}
