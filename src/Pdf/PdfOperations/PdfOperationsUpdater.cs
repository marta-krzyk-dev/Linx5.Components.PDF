using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.PdfOperations
{
	internal class PdfOperationsUpdater
	{
		private static PdfOperationsUpdater instance;

		public string CurrentVersion { get; } = "1";

		public static PdfOperationsUpdater Instance
		{
			get
			{
				if (instance == null)
					instance = new PdfOperationsUpdater();
				return instance;
			}
		}

		public IFunctionData Update(IFunctionData data)
		{
			if (data.Version == CurrentVersion)
				return data;

			if (string.IsNullOrEmpty(data.Version))
				return UpdateToVersion1(data);

			throw new Exception($"Unknown version [{data.Version}] specified.");
		}

		private IFunctionData UpdateToVersion1(IFunctionData data)
		{
			PdfOperationsDesigner designer = new PdfOperationsProvider().CreateDesigner(null) as PdfOperationsDesigner;
			Operation operation = UpdateOperationProperties(designer, data);
			switch (operation)
			{
				case Operation.FillForm:
					UpdateFillFormProperties(designer, data);
					break;
				case Operation.Protect:
					UpdateProtectProperties(designer, data);
					break;
				case Operation.Split:
					UpdateSplitProperties(designer, data);
					break;
				case Operation.Concatenate:
					UpdateConcatenateProperties(designer, data);
					break;
				case Operation.AddWatermark:
					UpdateAddWatermarkProperties(designer, data);
					break;
				case Operation.Sign:
					UpdateSignProperties(designer, data);
					break;
			}

			return designer.GetFunctionData();
		}

		private void UpdateFillFormProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputPdfFileProperties(designer, data);
			designer.Properties[PropertyNames.FillFormFormData].Value = data.Properties["Form data"].Value;
			UpdateOutputFileProperties(designer, data);
		}

		private void UpdateProtectProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputPdfFileProperties(designer, data);
			UpdateOutputFileProperties(designer, data);
			AuthenticationType protection = UpdateProtectProtectionProperties(designer, data);

			if (protection == AuthenticationType.None)
				return;

			UpdateProtectDocumentRestrictionProperties(designer, data, protection);
			UpdateProtectEncryptionProperties(designer, data);
			designer.Properties[PropertyNames.ProtectDontEncryptMetadata].Value = data.Properties["Plaintext metadata"].Value;
		}

		private void UpdateSplitProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputPdfFileProperties(designer, data);
			designer.Properties[PropertyNames.SplitLoopResults].Value = data.Properties["Loop results"].Value;
			UpdateOutputFileProperties(designer, data);
		}

		private void UpdateConcatenateProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			designer.Properties[PropertyNames.InputFiles].Value = data.Properties["Pdf files"].Value;
			UpdateOutputFileProperties(designer, data);
		}

		private void UpdateAddWatermarkProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputPdfFileProperties(designer, data);
			UpdateWatermarkInputFileProperties(designer, data);
			designer.Properties[PropertyNames.WatermarkPages].Value = data.Properties["Pages"].Value;
			designer.Properties[PropertyNames.WatermarkPosition].Value = data.Properties["Position"].Value;
			UpdateOutputFileProperties(designer, data);
		}

		private void UpdateSignProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateSignInputFileProperties(designer, data);
			designer.Properties[PropertyNames.SignSignedAt].Value = data.Properties["Signed at"].Value;
			designer.Properties[PropertyNames.SignReason].Value = data.Properties["Reason"].Value;
			UpdateSignCertificateProperties(designer, data);
			UpdateSignPlacementProperties(designer, data);
			UpdateOutputFileProperties(designer, data);
		}

		private Operation UpdateOperationProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
#pragma warning disable 618
			Obsolete.Operation oldOperation = data.Properties["Operation"].GetValue<Obsolete.Operation>();
			Operation newOperation = 0;
			switch (oldOperation)
			{
				case Obsolete.Operation.FillForm:
					newOperation = Operation.FillForm;
					break;
				case Obsolete.Operation.ProtectPdf:
					newOperation = Operation.Protect;
					break;
				case Obsolete.Operation.SplitPdf:
					newOperation = Operation.Split;
					break;
				case Obsolete.Operation.ConcatenatePdf:
					newOperation = Operation.Concatenate;
					break;
				case Obsolete.Operation.AddWatermark:
					newOperation = Operation.AddWatermark;
					break;
				case Obsolete.Operation.SignPdf:
					newOperation = Operation.Sign;
					break;
			}
#pragma warning restore 618

			designer.Properties[PropertyNames.Operation].Value = newOperation;
			return newOperation;
		}

		private AuthenticationType UpdateProtectProtectionProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
#pragma warning disable 618
			Obsolete.Protection oldProtection = data.Properties["Output pdf protection"].GetValue<Obsolete.Protection>();
			AuthenticationType newAuthenticationType = 0;
			switch (oldProtection)
			{
				case Obsolete.Protection.None:
					newAuthenticationType = AuthenticationType.None;
					break;
				case Obsolete.Protection.Password:
					newAuthenticationType = AuthenticationType.Password;
					break;
				case Obsolete.Protection.Certificate:
					newAuthenticationType = AuthenticationType.Certificate;
					break;
			}
#pragma warning restore 618

			designer.Properties[PropertyNames.ProtectProtection].Value = newAuthenticationType;
			switch (newAuthenticationType)
			{
				case AuthenticationType.Password:
					bool addDocumentRestrictions = data.Properties["Allow user access"].GetValue<bool>();
					if (addDocumentRestrictions)
						designer.Properties[PropertyNames.ProtectDocumentOpenPassword].Value = data.Properties["User password"].Value;
					else
						designer.Properties[PropertyNames.ProtectDocumentOpenPassword].Value = data.Properties["Owner password"].Value;
					break;
				case AuthenticationType.Certificate:
					CertificateSource certificateSource = data.Properties["Certificate source"].GetValue<CertificateSource>();
					designer.Properties[PropertyNames.ProtectCertificateSource].Value = certificateSource;
					switch (certificateSource)
					{
						case CertificateSource.File:
							designer.Properties[PropertyNames.ProtectCertificateFilePath].Value = data.Properties["Certificate file path"].Value;
							designer.Properties[PropertyNames.ProtectCertificateFilePassword].Value = data.Properties["Certificate file password"].Value;
							break;
						case CertificateSource.Store:
							designer.Properties[PropertyNames.ProtectCertificate].Value = data.Properties["Certificate"].Value;
							break;
					}
					break;
			}

			return newAuthenticationType;
		}

		private void UpdateProtectEncryptionProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
#pragma warning disable 618
			Obsolete.Encryption oldEncryption = data.Properties["Encryption"].GetValue<Obsolete.Encryption>();
			Encryption newEncryption = 0;
			switch (oldEncryption)
			{
				case Obsolete.Encryption.Standard:
				case Obsolete.Encryption.Standard128:
					newEncryption = Encryption.Standard128;
					break;
				case Obsolete.Encryption.AES:
					newEncryption = Encryption.AES128;
					break;
				case Obsolete.Encryption.AES256:
					newEncryption = Encryption.AES256;
					break;
			}
#pragma warning restore 618

			designer.Properties[PropertyNames.ProtectEncryption].Value = newEncryption;
		}

		private void UpdateSignCertificateProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			CertificateSource certificateSource = data.Properties["Signing certificate source"].GetValue<CertificateSource>();
			designer.Properties[PropertyNames.SignCertificateSource].Value = certificateSource;
			switch (certificateSource)
			{
				case CertificateSource.File:
					designer.Properties[PropertyNames.SignCertificateFilePath].Value = data.Properties["Signing certificate file path"].Value;
					designer.Properties[PropertyNames.SignCertificateFilePassword].Value = data.Properties["Signing certificate file password"].Value;
					break;
				case CertificateSource.Store:
					designer.Properties[PropertyNames.SignCertificate].Value = data.Properties["Signing certificate"].Value;
					break;
			}
		}

		private void UpdateSignPlacementProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			SignaturePosition signaturePosition = data.Properties["Signature placement"].GetValue<SignaturePosition>();
			designer.Properties[PropertyNames.SignPlacement].Value = signaturePosition;
			switch (signaturePosition)
			{
				case SignaturePosition.FormField:
					designer.Properties[PropertyNames.SignFieldName].Value = data.Properties["Field name"].Value;
					designer.Properties[PropertyNames.SignBackgroundImage].Value = data.Properties["Background image"].Value;
					break;
				case SignaturePosition.OnPage:
					designer.Properties[PropertyNames.SignPositionX].Value = data.Properties["Position X (mm)"].Value;
					designer.Properties[PropertyNames.SignPositionY].Value = data.Properties["Position Y (mm)"].Value;
					designer.Properties[PropertyNames.SignWidth].Value = data.Properties["Width (mm)"].Value;
					designer.Properties[PropertyNames.SignHeight].Value = data.Properties["Height (mm)"].Value;
					designer.Properties[PropertyNames.SignBackgroundImage].Value = data.Properties["Background image"].Value;
					designer.Properties[PropertyNames.SignPage].Value = data.Properties["Page"].Value;
					break;
			}
		}

		private void UpdateWatermarkInputFileProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputFileProperties(designer, data,
				"Watermark",
				PropertyNames.WatermarkFilePath,
				PropertyNames.WatermarkAuthenticationType,
				PropertyNames.WatermarkPassword,
				PropertyNames.WatermarkCertificateSource,
				PropertyNames.WatermarkCertificateFilePath,
				PropertyNames.WatermarkCertificateFilePassword,
				PropertyNames.WatermarkCertificate);
		}

		private void UpdateInputPdfFileProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputFileProperties(designer, data,
				"Input",
				PropertyNames.InputFilePath,
				PropertyNames.InputAuthenticationType,
				PropertyNames.InputPassword,
				PropertyNames.InputCertificateSource,
				PropertyNames.InputCertificateFilePath,
				PropertyNames.InputCertificateFilePassword,
				PropertyNames.InputCertificate);
		}

		private void UpdateSignInputFileProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			UpdateInputFileProperties(designer, data,
				string.Empty,
				PropertyNames.InputFilePath,
				PropertyNames.InputAuthenticationType,
				PropertyNames.InputPassword,
				PropertyNames.InputCertificateSource,
				PropertyNames.InputCertificateFilePath,
				PropertyNames.InputCertificateFilePassword,
				PropertyNames.InputCertificate);
		}

		private void UpdateOutputFileProperties(PdfOperationsDesigner designer, IFunctionData data)
		{
			designer.Properties[PropertyNames.OutputFilePath].Value = data.Properties["Output pdf filename"].Value;
		}

		private void UpdateProtectDocumentRestrictionProperties(PdfOperationsDesigner designer, IFunctionData data, AuthenticationType protection)
		{
			bool addDocumentRestrictions = data.Properties["Allow user access"].GetValue<bool>();
			designer.Properties[PropertyNames.ProtectAddDocumentRestrictions].Value = addDocumentRestrictions;
			if (!addDocumentRestrictions)
				return;

#pragma warning disable 618
			Obsolete.Permissions oldPermissions = data.Properties["User permissions"].GetValue<Obsolete.Permissions>();
			UpdateProtectAllowPrintingProperties(designer, oldPermissions);
			UpdateProtectAllowChangesProperties(designer, oldPermissions);
			designer.Properties[PropertyNames.ProtectAllowCopying].Value = oldPermissions.HasFlag(Obsolete.Permissions.AllowCopy);
			designer.Properties[PropertyNames.ProtectAllowScreenReaders].Value = oldPermissions.HasFlag(Obsolete.Permissions.AllowScreenreaders);
#pragma warning restore 618

			if (protection == AuthenticationType.Password)
				designer.Properties[PropertyNames.ProtectPermissionsPassword].Value = data.Properties["Owner password"].Value;
		}

#pragma warning disable 618
		private void UpdateProtectAllowPrintingProperties(PdfOperationsDesigner designer, Obsolete.Permissions oldPermissions)
		{
			Printing printing = Printing.None;
			if (oldPermissions.HasFlag(Obsolete.Permissions.AllowDegradedPrinting))
				printing = Printing.LowResolution;
			if (oldPermissions.HasFlag(Obsolete.Permissions.AllowPrinting))
				printing = Printing.HighResolution;

			designer.Properties[PropertyNames.ProtectAllowPrinting].Value = printing;
		}
#pragma warning restore 618

#pragma warning disable 618
		private void UpdateProtectAllowChangesProperties(PdfOperationsDesigner designer, Obsolete.Permissions oldPermissions)
		{
			Changes changes = Changes.None;
			if (oldPermissions.HasFlag(Obsolete.Permissions.AllowAssembly))
				changes = Changes.Assembly;
			if (oldPermissions.HasFlag(Obsolete.Permissions.AllowFillIn))
				changes = Changes.FillIn;
			if (oldPermissions.HasFlag(Obsolete.Permissions.AllowModifyAnnotations | Obsolete.Permissions.AllowFillIn))
				changes = Changes.AnnotateAndFillIn;
			if (oldPermissions.HasFlag(Obsolete.Permissions.AllowModifyContents | Obsolete.Permissions.AllowModifyAnnotations | Obsolete.Permissions.AllowFillIn))
				changes = Changes.AnyExpectExtract;

			designer.Properties[PropertyNames.ProtectAllowChanges].Value = changes;
		}
#pragma warning restore 618

		private void UpdateInputFileProperties(PdfOperationsDesigner designer, IFunctionData data,
			string propertyPrefix,
			string filePathPropertyName,
			string authenticationTypePropertyName,
			string passwordPropertyName,
			string certificateSourcePropertyName,
			string certificateFilePathPropertyName,
			string certificateFilePasswordPropertyName,
			string certificatePropertyName)
		{
			Func<string, string> addPrefix = (value) => (string.IsNullOrEmpty(propertyPrefix)) ? value : propertyPrefix + " " + char.ToLower(value[0]) + value.Substring(1);

			designer.Properties[filePathPropertyName].Value = data.Properties[addPrefix("Pdf filename")].Value;

			AuthenticationType authenticationType = data.Properties[addPrefix("Authentication type")].GetValue<AuthenticationType>();
			designer.Properties[authenticationTypePropertyName].Value = authenticationType;
			switch (authenticationType)
			{
				case AuthenticationType.Password:
					designer.Properties[passwordPropertyName].Value = data.Properties[addPrefix("Pdf password")].Value;
					break;
				case AuthenticationType.Certificate:
					CertificateSource certificateSource = data.Properties[addPrefix("Certificate source")].GetValue<CertificateSource>();
					designer.Properties[certificateSourcePropertyName].Value = certificateSource;
					switch (certificateSource)
					{
						case CertificateSource.File:
							designer.Properties[certificateFilePathPropertyName].Value = data.Properties[addPrefix("Certificate file path")].Value;
							designer.Properties[certificateFilePasswordPropertyName].Value = data.Properties[addPrefix("Certificate file password")].Value;
							break;
						case CertificateSource.Store:
							designer.Properties[certificatePropertyName].Value = data.Properties[addPrefix("Certificate")].Value;
							break;
					}
					break;
			}
		}
	}
}
