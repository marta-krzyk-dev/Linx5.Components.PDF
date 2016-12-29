using System;
using System.Collections.Generic;
using System.Linq;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf
{
	public class TypeResolver : ITypeResolver
	{
		private static readonly HashSet<TypeMapping> mappings;

		static TypeResolver()
		{
			mappings = new HashSet<TypeMapping>
			{
				new TypeMapping("Twenty57.Linx.Components.Pdf.ReadPdf, Twenty57.Linx.Components.Pdf", typeof(ReadPdf.ReadPdfProvider)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.InputPdfProperty+AuthenticationType, Twenty57.Linx.Components.Pdf", typeof(AuthenticationType)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.CertificateProperty+Source, Twenty57.Linx.Components.Pdf", typeof(CertificateSource)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.ReadPdfShared+TextSplit, Twenty57.Linx.Components.Pdf", typeof(ReadPdf.TextSplit)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.ReadPdfShared+FormExtraction, Twenty57.Linx.Components.Pdf", typeof(ReadPdf.FormExtraction)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.PdfOperations, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.PdfOperationsProvider)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.AddWatermarkShared+Position, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.WatermarkPosition)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.SignPdfShared+SignaturePosition, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.SignaturePosition)),
#pragma warning disable 618
				new TypeMapping("Twenty57.Linx.Components.Pdf.Operation, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.Obsolete.Operation)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.PdfEncryption, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.Obsolete.Encryption)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.PdfPermissions, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.Obsolete.Permissions)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.PdfProtection, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.Obsolete.Protection)),
				new TypeMapping("Twenty57.Linx.Components.Pdf.SplitOutput, Twenty57.Linx.Components.Pdf", typeof(PdfOperations.Obsolete.SplitOutput))
#pragma warning restore 618
			};
		}

		public bool TryGetResolvedTypeName(string typeName, out string resolvedTypeName)
		{
			TypeMapping mapping = mappings.SingleOrDefault(m => typeName.StartsWith(m.PartialName));
			if (mapping != null)
			{
				resolvedTypeName = mapping.Type.AssemblyQualifiedName;
				return true;
			}
			else
			{
				resolvedTypeName = null;
				return false;
			}
		}

		private class TypeMapping
		{
			public TypeMapping(string partialName, Type type)
			{
				PartialName = partialName;
				Type = type;
			}

			public string PartialName { get; }
			public Type Type { get; }
		}
	}
}
