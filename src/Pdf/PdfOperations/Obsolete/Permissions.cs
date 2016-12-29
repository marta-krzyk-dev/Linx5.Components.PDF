using System;

namespace Twenty57.Linx.Components.Pdf.PdfOperations.Obsolete
{
	[Obsolete("Required to update from old versions")]
	[Flags]
	public enum Permissions
	{
		ViewOnly = 0,
		AllowDegradedPrinting = 4,
		AllowModifyContents = 8,
		AllowCopy = 16,
		AllowModifyAnnotations = 32,
		AllowScreenreaders = 512,
		AllowFillIn = 256,
		AllowAssembly = 1024,
		AllowPrinting = 2052,
		FullAccess = 4095
	}
}
