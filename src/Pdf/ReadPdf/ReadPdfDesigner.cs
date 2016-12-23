using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Components.Pdf.ReadPdf.Validators;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;
using Twenty57.Linx.Plugin.Common.Validation;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.ReadPdf
{
	internal class ReadPdfDesigner : FunctionDesigner
	{
		public ReadPdfDesigner(IDesignerContext context)
			: base(context)
		{
			Version = ReadPdfUpdater.Instance.CurrentVersion;

			InitializeProperties();
			BuildOutput();
		}

		public ReadPdfDesigner(IFunctionData data, IDesignerContext context)
			: base(data, context)
		{ }

		public override IFunctionData GetFunctionData()
		{
			var data = new FunctionData();
			data.Output = Output;
			data.Version = Version;
			data.Properties.AddRange(Properties.Where(prop => prop.IsVisible));
			return data;
		}

		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			int propertyOrder = 1;

			InitializeInputPdfFileProperties(ref propertyOrder);
			InitializeReadTextProperties(ref propertyOrder);
			InitializeReadFormDataProperties(ref propertyOrder);

			Property readSignature = Properties.AddOrRetrieve(PropertyNames.ReadSignature, typeof(bool), ValueUseOption.DesignTime, false);
			readSignature.Order = propertyOrder++;
			readSignature.Description = "Reads the document signature.";
			readSignature.ValueChanged += RefreshOutput;
		}

		private void InitializeInputPdfFileProperties(ref int propertyOrder)
		{
			Properties.InitializeInputFileProperties(
				PropertyNames.PdfFilePath,
				PropertyNames.AuthenticationType,
				PropertyNames.PdfPassword,
				PropertyNames.CertificateSource,
				PropertyNames.CertificateFilePath,
				PropertyNames.CertificateFilePassword,
				PropertyNames.Certificate,
				true,
				ref propertyOrder,
				new VisibleDependency(() => true));
		}

		private void InitializeReadTextProperties(ref int propertyOrder)
		{
			Action applyVisibility = () =>
			{
				Properties[PropertyNames.SplitText].IsVisible = Properties.PropertyValueEquals(PropertyNames.ReadText, true);
			};
			EventHandler updateVisibility = (sender, args) => applyVisibility();

			Property readText = Properties.AddOrRetrieve(PropertyNames.ReadText, typeof(bool), ValueUseOption.DesignTime, false);
			readText.Order = propertyOrder++;
			readText.Description = "Reads text from the document.";
			readText.ValueChanged += RefreshOutput;
			readText.ValueChanged += updateVisibility;

			Property splitText = Properties.AddOrRetrieve(PropertyNames.SplitText, typeof(TextSplit), ValueUseOption.DesignTime, TextSplit.Never);
			splitText.Order = propertyOrder++;
			splitText.Description = "Controls how the document text is split.";
			splitText.ValueChanged += RefreshOutput;

			applyVisibility();
		}

		private void InitializeReadFormDataProperties(ref int propertyOrder)
		{
			Action applyVisibility = () =>
			{
				bool readFormDataValue = Properties[PropertyNames.ReadFormData].GetValue<bool>();
				FormExtraction returnFormDataAsValue = Properties[PropertyNames.ReturnFormDataAs].GetValue<FormExtraction>();

				Properties[PropertyNames.ReturnFormDataAs].IsVisible = readFormDataValue;
				Properties[PropertyNames.FormDataType].IsVisible = (readFormDataValue && returnFormDataAsValue == FormExtraction.CustomType);
				Properties[PropertyNames.SamplePdf].IsVisible = (readFormDataValue && returnFormDataAsValue == FormExtraction.Infer);
			};
			EventHandler updateVisibility = (sender, args) => applyVisibility();

			Property readFormData = Properties.AddOrRetrieve(PropertyNames.ReadFormData, typeof(bool), ValueUseOption.DesignTime, false);
			readFormData.Order = propertyOrder++;
			readFormData.Description = "Reads form data.";
			readFormData.ValueChanged += RefreshOutput;
			readFormData.ValueChanged += updateVisibility;

			Property returnFormDataAs = Properties.AddOrRetrieve(PropertyNames.ReturnFormDataAs, typeof(FormExtraction), ValueUseOption.DesignTime, FormExtraction.CustomType);
			returnFormDataAs.Order = propertyOrder++;
			returnFormDataAs.Description = "Controls how the form data is returned.";
			returnFormDataAs.ValueChanged += RefreshOutput;
			returnFormDataAs.ValueChanged += updateVisibility;

			Property formDataType = Properties.AddOrRetrieve(PropertyNames.FormDataType, typeof(ITypeReference), ValueUseOption.DesignTime, null);
			formDataType.Order = propertyOrder++;
			formDataType.Description = "The expected type for the document's form data.";
			formDataType.Validations.Add(new RequiredValidator());
			formDataType.ValueChanged += RefreshOutput;

			Property samplePdf = Properties.AddOrRetrieve(PropertyNames.SamplePdf, typeof(string), ValueUseOption.DesignTime, string.Empty);
			samplePdf.Order = propertyOrder++;
			samplePdf.Description = "A sample PDF containing the empty form.";
			samplePdf.Validations.Add(new RequiredValidator());
			samplePdf.Validations.Add(new PdfHasFieldsValidator());
			samplePdf.Editor = typeof(FilePathEditor);
			samplePdf.ValueChanged += RefreshOutput;

			applyVisibility();
		}

		private void RefreshOutput(object sender, EventArgs e) => BuildOutput();

		private void BuildOutput()
		{
			var outputBuilder = new TypeBuilder();
			BuildTextOutput(outputBuilder);
			BuildSignatureOutput(outputBuilder);
			BuildFormDataOutput(outputBuilder);

			ITypeReference outputReference = outputBuilder.CreateTypeReference();
			Output = (outputReference.GetProperties().Any()) ? outputReference : null;
		}

		private void BuildTextOutput(TypeBuilder outputBuilder)
		{
			if (!Properties[PropertyNames.ReadText].GetValue<bool>())
				return;

			switch (Properties[PropertyNames.SplitText].GetValue<TextSplit>())
			{
				case TextSplit.Never:
					outputBuilder.AddProperty(OutputNames.Text, typeof(string));
					break;
				case TextSplit.Page:
					outputBuilder.AddProperty(OutputNames.Text, TypeReference.CreateList(typeof(string)));
					break;
				default:
					throw new NotSupportedException("Invalid TextSplit specified.");
			}
		}

		private void BuildSignatureOutput(TypeBuilder outputBuilder)
		{
			if (!Properties[PropertyNames.ReadSignature].GetValue<bool>())
				return;

			var signatureBuilder = new TypeBuilder();
			signatureBuilder.AddProperty(OutputNames.SignedBy, typeof(string));
			signatureBuilder.AddProperty(OutputNames.SignedAt, typeof(string));
			signatureBuilder.AddProperty(OutputNames.Reason, typeof(string));
			signatureBuilder.AddProperty(OutputNames.SignedOn, typeof(DateTime));
			signatureBuilder.AddProperty(OutputNames.Unmodified, typeof(bool));
			signatureBuilder.AddProperty(OutputNames.SignedRevision, typeof(int));
			signatureBuilder.AddProperty(OutputNames.IsLatestRevision, typeof(bool));
			signatureBuilder.AddProperty(OutputNames.Verified, typeof(bool));
			signatureBuilder.AddProperty(OutputNames.VerificationMessage, typeof(string));

			var signaturesBuilder = new TypeBuilder();
			signaturesBuilder.AddProperty(OutputNames.IsSigned, typeof(bool));
			signaturesBuilder.AddProperty(OutputNames.LatestSignature, signatureBuilder);
			signaturesBuilder.AddProperty(OutputNames.AllSignatures, signatureBuilder, true);

			outputBuilder.AddProperty(OutputNames.Signatures, signaturesBuilder);
		}

		private void BuildFormDataOutput(TypeBuilder outputBuilder)
		{
			if (!Properties[PropertyNames.ReadFormData].GetValue<bool>())
				return;

			switch (Properties[PropertyNames.ReturnFormDataAs].GetValue<FormExtraction>())
			{
				case FormExtraction.CustomType:
					ITypeReference dataType = Properties[PropertyNames.FormDataType].GetValue<ITypeReference>() ?? TypeReference.Create(typeof(object));
					outputBuilder.AddProperty(OutputNames.FormData, dataType);
					break;
				case FormExtraction.Infer:
					var fieldsBuilder = new TypeBuilder();
					var sampleFilePath = Properties[PropertyNames.SamplePdf].GetValue<string>();
					try
					{
						foreach (string fieldName in GetFieldNames(sampleFilePath))
							fieldsBuilder.AddProperty(Names.GetValidName(fieldName), typeof(string));
					}
					catch { }
					outputBuilder.AddProperty(OutputNames.FormData, fieldsBuilder.CreateTypeReference());
					break;
				case FormExtraction.List:
					outputBuilder.AddProperty(OutputNames.FormDataList, TypeReference.CreateList(typeof(KeyValuePair<string, string>)));
					break;
				default:
					throw new NotSupportedException("Invalid FormExtraction specified.");
			}
		}

		private static List<string> GetFieldNames(string pdfFile)
		{
			if (!File.Exists(pdfFile))
				return new List<string>();

			using (var reader = new PdfReader(pdfFile))
			{
				return reader.AcroFields.Fields.Keys.ToList();
			}
		}
	}
}
