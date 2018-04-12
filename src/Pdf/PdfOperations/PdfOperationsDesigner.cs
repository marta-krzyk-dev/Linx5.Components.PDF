using System;
using System.Collections.Generic;
using System.Linq;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;
using Twenty57.Linx.Plugin.Common.Validation;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	internal class PdfOperationsDesigner : FunctionDesigner
	{
		public PdfOperationsDesigner(IDesignerContext context)
			: base(context)
		{
			Version = PdfOperationsUpdater.Instance.CurrentVersion;

			InitializeProperties();
			BuildOutput();
		}

		public PdfOperationsDesigner(IFunctionData data, IDesignerContext context)
			: base(data, context)
		{ }

		public override IFunctionData GetFunctionData()
		{
			var data = new FunctionData();
			if (ExecutionPaths.Any())
				data.ExecutionPaths.AddRange(ExecutionPaths);
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

			var fillFormDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Operation, Operation.FillForm));
			var protectDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Operation, Operation.Protect));
			var splitDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Operation, Operation.Split));
			var concatenateDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Operation, Operation.Concatenate));
			var notConcatenateDependency = new VisibleDependency(() => !Properties.PropertyValueEquals(PropertyNames.Operation, Operation.Concatenate));
			var addWatermarkDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Operation, Operation.AddWatermark));
			var signDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Operation, Operation.Sign));

			Property operation = Properties.AddOrRetrieve(PropertyNames.Operation, typeof(Operation), ValueUseOption.DesignTime, Operation.FillForm);
			operation.Order = propertyOrder++;
			operation.Description = "The operation to perform on the document.";
			operation.ValueChanged += RefreshOutput;
			operation.ValueChanged += (sender, args) =>
			{
				fillFormDependency.Refresh();
				protectDependency.Refresh();
				splitDependency.Refresh();
				concatenateDependency.Refresh();
				notConcatenateDependency.Refresh();
				addWatermarkDependency.Refresh();
				signDependency.Refresh();
			};

			InitializeInputProperties(notConcatenateDependency, ref propertyOrder);
			InitializeConcatenateProperties(concatenateDependency, ref propertyOrder);
			InitializeSplitProperties(splitDependency, ref propertyOrder);
			InitializeAddWatermarkProperties(addWatermarkDependency, ref propertyOrder);
			InitializeSignProperties(signDependency, ref propertyOrder);

			Property outputFilePath = Properties.AddOrRetrieve(PropertyNames.OutputFilePath, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			outputFilePath.Order = propertyOrder++;
			outputFilePath.Description = "Path of the PDF file to write to.";
			outputFilePath.Editor = typeof(FilePathEditor);
			outputFilePath.Validations.Add(new RequiredValidator());

			InitializeFillFormProperties(fillFormDependency, ref propertyOrder);
			InitializeProtectProperties(protectDependency, ref propertyOrder);
		}

		private void InitializeInputProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			Properties.InitializeInputFileProperties(
				PropertyNames.InputFilePath,
				PropertyNames.InputAuthenticationType,
				PropertyNames.InputPassword,
				PropertyNames.InputCertificateSource,
				PropertyNames.InputCertificateFilePath,
				PropertyNames.InputCertificateFilePassword,
				PropertyNames.InputCertificate,
				false,
				ref propertyOrder,
				visibleDependency);
		}

		private void InitializeConcatenateProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			Action<bool> applyVisibility = (isVisible) =>
			{
				Properties[PropertyNames.InputFiles].IsVisible = isVisible;
			};
			visibleDependency.VisibleChanged += (visible) => applyVisibility(visible);

			Property inputFiles = Properties.AddOrRetrieve(PropertyNames.InputFiles, TypeReference.CreateList(typeof(string)), ValueUseOption.RuntimeRead, null);
			inputFiles.Order = propertyOrder++;
			inputFiles.Description = "List of PDF files to concatenate.";
			inputFiles.Validations.Add(new RequiredValidator());

			applyVisibility(visibleDependency.Visible);
		}

		private void InitializeSplitProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			Action<bool> applyVisibility = (isVisible) =>
			{
				Properties[PropertyNames.SplitLoopResults].IsVisible = isVisible;
			};
			visibleDependency.VisibleChanged += (visible) => applyVisibility(visible);

			Property loopResults = Properties.AddOrRetrieve(PropertyNames.SplitLoopResults, typeof(bool), ValueUseOption.DesignTime, false);
			loopResults.Order = propertyOrder++;
			loopResults.Description = "Loop through the generated file names.";
			loopResults.ValueChanged += RefreshOutput;

			applyVisibility(visibleDependency.Visible);
		}

		private void InitializeFillFormProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			Action<bool> applyVisibility = (isVisible) =>
			{
				Properties[PropertyNames.FillFormFormData].IsVisible = isVisible;
			};
			visibleDependency.VisibleChanged += (visible) => applyVisibility(visible);

			Property formData = Properties.AddOrRetrieve(PropertyNames.FillFormFormData, typeof(object), ValueUseOption.RuntimeRead, null);
			formData.Order = propertyOrder++;
			formData.Description = "A custom object that contains the form data to insert.";
			formData.Validations.Add(new RequiredValidator());

			applyVisibility(visibleDependency.Visible);
		}

		private void InitializeAddWatermarkProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			Properties.InitializeInputFileProperties(
				PropertyNames.WatermarkFilePath,
				PropertyNames.WatermarkAuthenticationType,
				PropertyNames.WatermarkPassword,
				PropertyNames.WatermarkCertificateSource,
				PropertyNames.WatermarkCertificateFilePath,
				PropertyNames.WatermarkCertificateFilePassword,
				PropertyNames.WatermarkCertificate,
				true,
				ref propertyOrder,
				visibleDependency);

			Action<bool> applyVisibility = (isVisible) =>
			{
				Properties[PropertyNames.WatermarkPages].IsVisible = isVisible;
				Properties[PropertyNames.WatermarkPosition].IsVisible = isVisible;
			};
			visibleDependency.VisibleChanged += (visible) => applyVisibility(visible);

			Property watermarkPages = Properties.AddOrRetrieve(PropertyNames.WatermarkPages, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			watermarkPages.Order = propertyOrder++;
			watermarkPages.Description = "Page range to stamp with watermark. Leave this blank to add the watermark to all pages.";

			Property watermarkPosition = Properties.AddOrRetrieve(PropertyNames.WatermarkPosition, typeof(WatermarkPosition), ValueUseOption.DesignTime, WatermarkPosition.Above);
			watermarkPosition.Order = propertyOrder++;
			watermarkPosition.Description = "Draws the watermark above or below the original document content.";

			applyVisibility(visibleDependency.Visible);
		}

		private void InitializeSignProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			Action<bool> applyVisibility = (isVisible) =>
			{
				SignaturePosition signPlacementValue = Properties[PropertyNames.SignPlacement].GetValue<SignaturePosition>();

				Properties[PropertyNames.SignSignedAt].IsVisible = isVisible;
				Properties[PropertyNames.SignReason].IsVisible = isVisible;
				Properties[PropertyNames.SignLockAfterSigning].IsVisible = isVisible;
				Properties[PropertyNames.SignPlacement].IsVisible = isVisible;
				Properties[PropertyNames.SignFieldName].IsVisible = (isVisible && signPlacementValue == SignaturePosition.FormField);
				Properties[PropertyNames.SignPositionX].IsVisible = (isVisible && signPlacementValue == SignaturePosition.OnPage);
				Properties[PropertyNames.SignPositionY].IsVisible = (isVisible && signPlacementValue == SignaturePosition.OnPage);
				Properties[PropertyNames.SignWidth].IsVisible = (isVisible && signPlacementValue == SignaturePosition.OnPage);
				Properties[PropertyNames.SignHeight].IsVisible = (isVisible && signPlacementValue == SignaturePosition.OnPage);
				Properties[PropertyNames.SignBackgroundImage].IsVisible = (isVisible && (signPlacementValue == SignaturePosition.FormField || signPlacementValue == SignaturePosition.OnPage));
				Properties[PropertyNames.SignPage].IsVisible = (isVisible && signPlacementValue == SignaturePosition.OnPage);
			};
			EventHandler updateVisibility = (sender, args) => applyVisibility(visibleDependency.Visible);
			visibleDependency.VisibleChanged += (visible) => applyVisibility(visible);

			Property signedAt = Properties.AddOrRetrieve(PropertyNames.SignSignedAt, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			signedAt.Order = propertyOrder++;
			signedAt.Description = "Location where the signing took place.";
			signedAt.Validations.Add(new RequiredValidator());

			Property reason = Properties.AddOrRetrieve(PropertyNames.SignReason, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			reason.Order = propertyOrder++;
			reason.Description = "Reason for signing the document.";
			reason.Validations.Add(new RequiredValidator());

			Property lockAfterSigning = Properties.AddOrRetrieve(PropertyNames.SignLockAfterSigning, typeof(bool), ValueUseOption.DesignTime, false);
			lockAfterSigning.Order = propertyOrder++;
			lockAfterSigning.Description = "Lock the document to prevent further changes.";

			Properties.InitializeCertificateProperties(
				PropertyNames.SignCertificateSource,
				PropertyNames.SignCertificateFilePath,
				PropertyNames.SignCertificateFilePassword,
				PropertyNames.SignCertificate,
				ref propertyOrder,
				visibleDependency);

			Property placement = Properties.AddOrRetrieve(PropertyNames.SignPlacement, typeof(SignaturePosition), ValueUseOption.DesignTime, SignaturePosition.Hidden);
			placement.Order = propertyOrder++;
			placement.Description = "Where to put the signature in the document.";
			placement.ValueChanged += updateVisibility;

			Property fieldName = Properties.AddOrRetrieve(PropertyNames.SignFieldName, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			fieldName.Order = propertyOrder++;
			fieldName.Description = "Form field name to place the signature in.";
			fieldName.Validations.Add(new RequiredValidator());

			Property positionX = Properties.AddOrRetrieve(PropertyNames.SignPositionX, typeof(int), ValueUseOption.RuntimeRead, 0);
			positionX.Order = propertyOrder++;
			positionX.Description = "X coordinate of the signature.";

			Property positionY = Properties.AddOrRetrieve(PropertyNames.SignPositionY, typeof(int), ValueUseOption.RuntimeRead, 0);
			positionY.Order = propertyOrder++;
			positionY.Description = "Y coordinate of the signature.";

			Property width = Properties.AddOrRetrieve(PropertyNames.SignWidth, typeof(int), ValueUseOption.RuntimeRead, 100);
			width.Order = propertyOrder++;
			width.Description = "Width of the signature box.";

			Property height = Properties.AddOrRetrieve(PropertyNames.SignHeight, typeof(int), ValueUseOption.RuntimeRead, 50);
			height.Order = propertyOrder++;
			height.Description = "Height of the signature box.";

			Property backgroundImage = Properties.AddOrRetrieve(PropertyNames.SignBackgroundImage, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			backgroundImage.Order = propertyOrder++;
			backgroundImage.Description = "Path to an image file to use as a background for the signature.";
			backgroundImage.Editor = typeof(FilePathEditor);

			Property page = Properties.AddOrRetrieve(PropertyNames.SignPage, typeof(int), ValueUseOption.RuntimeRead, 1);
			page.Order = propertyOrder++;
			page.Description = "Page on which to include the visible signature.";
			page.Validations.Add(new RangeValidator<int>(1, int.MaxValue));

			applyVisibility(visibleDependency.Visible);
		}

		private void InitializeProtectProperties(VisibleDependency visibleDependency, ref int propertyOrder)
		{
			var protectCertDependency = new VisibleDependency(() => visibleDependency.Visible && Properties.PropertyValueEquals(PropertyNames.ProtectProtection, AuthenticationType.Certificate));

			Action<bool> applyVisibility = (isVisible) =>
			{
				AuthenticationType protectionValue = Properties[PropertyNames.ProtectProtection].GetValue<AuthenticationType>();
				bool addDocumentRestrictionsValue = Properties[PropertyNames.ProtectAddDocumentRestrictions].GetValue<bool>();

				Properties[PropertyNames.ProtectProtection].IsVisible = isVisible;
				Properties[PropertyNames.ProtectDocumentOpenPassword].IsVisible = (isVisible && protectionValue == AuthenticationType.Password);
				Properties[PropertyNames.ProtectAddDocumentRestrictions].IsVisible = (isVisible && (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password));
				Properties[PropertyNames.ProtectAllowPrinting].IsVisible = (isVisible && addDocumentRestrictionsValue);
				Properties[PropertyNames.ProtectAllowChanges].IsVisible = (isVisible && addDocumentRestrictionsValue);
				Properties[PropertyNames.ProtectAllowCopying].IsVisible = (isVisible && addDocumentRestrictionsValue);
				Properties[PropertyNames.ProtectAllowScreenReaders].IsVisible = (isVisible && addDocumentRestrictionsValue);
				Properties[PropertyNames.ProtectPermissionsPassword].IsVisible = (isVisible && addDocumentRestrictionsValue && protectionValue == AuthenticationType.Password);
				Properties[PropertyNames.ProtectEncryption].IsVisible = (isVisible && (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password));
				Properties[PropertyNames.ProtectDontEncryptMetadata].IsVisible = (isVisible && (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password));
			};
			EventHandler updateVisibility = (sender, args) =>
			{
				applyVisibility(visibleDependency.Visible);
				protectCertDependency.Refresh();
			};
			visibleDependency.VisibleChanged += (visible) =>
			{
				applyVisibility(visible);
				protectCertDependency.Refresh();
			};

			Property protection = Properties.AddOrRetrieve(PropertyNames.ProtectProtection, typeof(AuthenticationType), ValueUseOption.DesignTime, AuthenticationType.None);
			protection.Order = propertyOrder++;
			protection.Description = "Method used to protect the PDF.";
			protection.ValueChanged += updateVisibility;

			Property documentOpenPassword = Properties.AddOrRetrieve(PropertyNames.ProtectDocumentOpenPassword, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			documentOpenPassword.Order = propertyOrder++;
			documentOpenPassword.Description = "Password required to open the PDF document.";

			Properties.InitializeCertificateProperties(
				PropertyNames.ProtectCertificateSource,
				PropertyNames.ProtectCertificateFilePath,
				PropertyNames.ProtectCertificateFilePassword,
				PropertyNames.ProtectCertificate,
				ref propertyOrder,
				protectCertDependency);

			Property addDocumentRestrictions = Properties.AddOrRetrieve(PropertyNames.ProtectAddDocumentRestrictions, typeof(bool), ValueUseOption.DesignTime, false);
			addDocumentRestrictions.Order = propertyOrder++;
			addDocumentRestrictions.Description = "Specify restrictions on the PDF document.";
			addDocumentRestrictions.ValueChanged += updateVisibility;

			Property allowPrinting = Properties.AddOrRetrieve(PropertyNames.ProtectAllowPrinting, typeof(Printing), ValueUseOption.DesignTime, Printing.None);
			allowPrinting.Order = propertyOrder++;
			allowPrinting.Description = "The level of printing allowed on the PDF document.";

			Property allowChanges = Properties.AddOrRetrieve(PropertyNames.ProtectAllowChanges, typeof(Changes), ValueUseOption.DesignTime, Changes.None);
			allowChanges.Order = propertyOrder++;
			allowChanges.Description = "The editing actions allowed on the PDF document.";

			Property allowCopying = Properties.AddOrRetrieve(PropertyNames.ProtectAllowCopying, typeof(bool), ValueUseOption.DesignTime, false);
			allowCopying.Order = propertyOrder++;
			allowCopying.Description = "Enable copying of text, images and other content.";
			allowCopying.ValueChanged += (sender, args) =>
			 {
				 bool currentValue = Properties[PropertyNames.ProtectAllowCopying].GetValue<bool>();
				 if (currentValue)
					 Properties[PropertyNames.ProtectAllowScreenReaders].Value = true;
			 };

			Property allowScreenReaders = Properties.AddOrRetrieve(PropertyNames.ProtectAllowScreenReaders, typeof(bool), ValueUseOption.DesignTime, false);
			allowScreenReaders.Order = propertyOrder++;
			allowScreenReaders.Description = "Enable text access for screen reader devices for the visually impaired.";

			Property permissionsPassword = Properties.AddOrRetrieve(PropertyNames.ProtectPermissionsPassword, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			permissionsPassword.Order = propertyOrder++;
			permissionsPassword.Description = "Password to override restrictions placed on the PDF document.";
			permissionsPassword.Validations.Add(new RequiredValidator());

			Property encryption = Properties.AddOrRetrieve(PropertyNames.ProtectEncryption, typeof(Encryption), ValueUseOption.DesignTime, Encryption.AES128);
			encryption.Order = propertyOrder++;
			encryption.Description = "Encryption method used to proptect the PDF.";

			Property dontEncryptMetadata = Properties.AddOrRetrieve(PropertyNames.ProtectDontEncryptMetadata, typeof(bool), ValueUseOption.DesignTime, false);
			dontEncryptMetadata.Order = propertyOrder++;
			dontEncryptMetadata.Description = "Don't encrypt the document metadata.";

			applyVisibility(visibleDependency.Visible);
		}

		private void RefreshOutput(object sender, EventArgs e) => BuildOutput();

		private void BuildOutput()
		{
			Output = null;
			ExecutionPaths.Clear();

			Operation operationValue = Properties[PropertyNames.Operation].GetValue<Operation>();
			switch (operationValue)
			{
				case Operation.FillForm:
				case Operation.Protect:
				case Operation.Concatenate:
				case Operation.AddWatermark:
				case Operation.Sign:
					break;
				case Operation.Split:
					BuildSplitOutput();
					break;
				default:
					throw new Exception("Invalid Operation specified.");
			}
		}

		private void BuildSplitOutput()
		{
			var outputBuilder = new TypeBuilder();

			bool loopResultsValue = Properties[PropertyNames.SplitLoopResults].GetValue<bool>();
			if (loopResultsValue)
				ExecutionPaths.Add(ExecutionPathNames.PageFiles, ExecutionPathNames.PageFiles, TypeReference.Create(typeof(string)));
			else
				outputBuilder.AddProperty(OutputNames.PageFiles, TypeReference.CreateList(typeof(string)));

			outputBuilder.AddProperty(OutputNames.NumberOfPages, typeof(int));
			Output = outputBuilder.CreateTypeReference();
		}
	}
}
