using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf
{
	public class TypeResolver : ITypeResolver
	{
		public bool TryGetResolvedTypeName(string typeName, out string resolvedTypeName)
		{
			if (typeName.StartsWith("Twenty57.Linx.Components.Pdf.ReadPdf, Twenty57.Linx.Components.Pdf"))
			{
				resolvedTypeName = typeof(ReadPdf.ReadPdfProvider).AssemblyQualifiedName;
				return true;
			}

			if (typeName.StartsWith("Twenty57.Linx.Components.Pdf.InputPdfProperty+AuthenticationType, Twenty57.Linx.Components.Pdf"))
			{
				resolvedTypeName = typeof(AuthenticationType).AssemblyQualifiedName;
				return true;
			}

			if (typeName.StartsWith("Twenty57.Linx.Components.Pdf.CertificateProperty+Source, Twenty57.Linx.Components.Pdf"))
			{
				resolvedTypeName = typeof(CertificateSource).AssemblyQualifiedName;
				return true;
			}

			if (typeName.StartsWith("Twenty57.Linx.Components.Pdf.ReadPdfShared+TextSplit, Twenty57.Linx.Components.Pdf"))
			{
				resolvedTypeName = typeof(ReadPdf.TextSplit).AssemblyQualifiedName;
				return true;
			}

			if (typeName.StartsWith("Twenty57.Linx.Components.Pdf.ReadPdfShared+FormExtraction, Twenty57.Linx.Components.Pdf"))
			{
				resolvedTypeName = typeof(ReadPdf.FormExtraction).AssemblyQualifiedName;
				return true;
			}

			resolvedTypeName = null;
			return false;
		}
	}
}
