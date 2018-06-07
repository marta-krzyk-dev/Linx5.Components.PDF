using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Interfaces;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.CodeGeneration;
using Twenty57.Linx.Plugin.Common.Types;

namespace Twenty57.Linx.Components.Pdf.Read.Templates
{
	internal partial class ReadTemplate : ITemplate
	{
		public AuthenticationType AuthenticationType { get; private set; }
		public CertificateSource CertificateSource { get; private set; }
		public StoredCertificate Certificate { get; private set; }
		public bool ReadText { get; private set; }
		public TextSplit TextSplit { get; private set; }
		public bool ReadFormData { get; private set; }
		public FormExtraction FormExtraction { get; private set; }
		public bool ReadSignature { get; private set; }
		public bool HasOutput { get; private set; }

		public string PdfFilePathParameterName { get; private set; }
		public string PdfPasswordParameterName { get; private set; }
		public string CertificateFilePathParameterName { get; private set; }
		public string CertificateFilePasswordParameterName { get; private set; }
		public string ContextParameterName { get; private set; }

		public string OutputTypeName { get; private set; }
		public string OutputTextPropertyName { get; private set; }
		public string OutputFormDataPropertyName { get; private set; }
		public string OutputFormDataTypeName { get; private set; }
		public string OutputFormDataListPropertyName { get; private set; }
		public string OutputSignaturesPropertyName { get; private set; }
		public string OutputSignaturesTypeName { get; private set; }
		public string OutputIsSignedPropertyName { get; private set; }
		public string OutputLatestSignaturePropertyName { get; private set; }
		public string OutputLatestSignatureTypeName { get; private set; }
		public string OutputAllSignaturesPropertyName { get; private set; }
		public string OutputSignedByName { get; private set; }
		public string OutputSignedAtName { get; private set; }
		public string OutputReasonName { get; private set; }
		public string OutputSignedOnName { get; private set; }
		public string OutputUnmodifiedName { get; private set; }
		public string OutputSignedRevisionName { get; private set; }
		public string OutputIsLatestRevisionName { get; private set; }
		public string OutputVerifiedName { get; private set; }
		public string OutputVerificationMessageName { get; private set; }

		public void Populate(IFunctionBuilder functionBuilder, IFunctionData functionData)
		{
			AuthenticationType = functionData.TryGetPropertyValue(PropertyNames.AuthenticationType, AuthenticationType.None);
			CertificateSource = functionData.TryGetPropertyValue(PropertyNames.CertificateSource, CertificateSource.File);
			Certificate = functionData.TryGetPropertyValue<StoredCertificate>(PropertyNames.Certificate, null);
			ReadText = functionData.TryGetPropertyValue(PropertyNames.ReadText, false);
			TextSplit = functionData.TryGetPropertyValue(PropertyNames.SplitText, TextSplit.Never);
			ReadFormData = functionData.TryGetPropertyValue(PropertyNames.ReadFormData, false);
			FormExtraction = functionData.TryGetPropertyValue(PropertyNames.ReturnFormDataAs, FormExtraction.CustomType);
			ReadSignature = functionData.TryGetPropertyValue(PropertyNames.ReadSignature, false);
			HasOutput = (ReadText || ReadFormData || ReadSignature);

			PdfFilePathParameterName = functionBuilder.GetParamName(PropertyNames.PdfFilePath);
			PdfPasswordParameterName = functionBuilder.GetParamName(PropertyNames.PdfPassword);
			CertificateFilePathParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePath);
			CertificateFilePasswordParameterName = functionBuilder.GetParamName(PropertyNames.CertificateFilePassword);
			if (HasOutput)
				OutputTypeName = functionBuilder.GetTypeName(functionData.Output);
			OutputTextPropertyName = functionBuilder.GetParamName(OutputNames.Text);
			OutputFormDataPropertyName = functionBuilder.GetParamName(OutputNames.FormData);
			OutputFormDataTypeName = functionBuilder.GetTypeName(functionData?.Output?.GetProperty(OutputNames.FormData)?.TypeReference ?? TypeReference.Create(typeof(object)));
			OutputFormDataListPropertyName = functionBuilder.GetParamName(OutputNames.FormDataList);
			OutputSignaturesPropertyName = functionBuilder.GetParamName(OutputNames.Signatures);
			OutputSignaturesTypeName = functionBuilder.GetTypeName(functionData?.Output?.GetProperty(OutputNames.Signatures)?.TypeReference ?? TypeReference.Create(typeof(object)));
			OutputIsSignedPropertyName = functionBuilder.GetParamName(OutputNames.IsSigned);
			OutputLatestSignaturePropertyName = functionBuilder.GetParamName(OutputNames.LatestSignature);
			OutputLatestSignatureTypeName = functionBuilder.GetTypeName(functionData?.Output?.GetProperty(OutputNames.Signatures)?.TypeReference?.GetProperty(OutputNames.LatestSignature)?.TypeReference ?? TypeReference.Create(typeof(object)));
			OutputAllSignaturesPropertyName = functionBuilder.GetParamName(OutputNames.AllSignatures);
			OutputSignedByName = functionBuilder.GetParamName(OutputNames.SignedBy);
			OutputSignedAtName = functionBuilder.GetParamName(OutputNames.SignedAt);
			OutputReasonName = functionBuilder.GetParamName(OutputNames.Reason);
			OutputSignedOnName = functionBuilder.GetParamName(OutputNames.SignedOn);
			OutputUnmodifiedName = functionBuilder.GetParamName(OutputNames.Unmodified);
			OutputSignedRevisionName = functionBuilder.GetParamName(OutputNames.SignedRevision);
			OutputIsLatestRevisionName = functionBuilder.GetParamName(OutputNames.IsLatestRevision);
			OutputVerifiedName = functionBuilder.GetParamName(OutputNames.Verified);
			OutputVerificationMessageName = functionBuilder.GetParamName(OutputNames.VerificationMessage);

			ContextParameterName = functionBuilder.ContextParamName;
		}
	}
}
