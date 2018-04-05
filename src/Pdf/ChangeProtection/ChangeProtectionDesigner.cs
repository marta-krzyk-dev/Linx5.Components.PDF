using System;
using System.Collections.Generic;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Components.Pdf.PdfOperations;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Validation;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection
{
	internal class ChangeProtectionDesigner : PdfDesigner
	{
		public ChangeProtectionDesigner(IDesignerContext context) : base(context)
		{
			InitializeProperties();
		}

		public ChangeProtectionDesigner(IFunctionData data, IDesignerContext context) : base(data, context) { }

		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			var protectCertDependency = new VisibleDependency(() => Properties.PropertyValueEquals(ChangeProtectionProvider.Protection, AuthenticationType.Certificate));

			Action applyVisibility = () =>
			{
				AuthenticationType protectionValue = Properties[ChangeProtectionProvider.Protection].GetValue<AuthenticationType>();
				bool addDocumentRestrictionsValue = Properties[ChangeProtectionProvider.AddDocumentRestrictions].GetValue<bool>();

				Properties[ChangeProtectionProvider.DocumentOpenPassword].IsVisible = protectionValue == AuthenticationType.Password;
				Properties[ChangeProtectionProvider.AddDocumentRestrictions].IsVisible = (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password);
				Properties[ChangeProtectionProvider.AllowPrinting].IsVisible = addDocumentRestrictionsValue;
				Properties[ChangeProtectionProvider.AllowChanges].IsVisible = addDocumentRestrictionsValue;
				Properties[ChangeProtectionProvider.AllowCopying].IsVisible = addDocumentRestrictionsValue;
				Properties[ChangeProtectionProvider.AllowScreenReaders].IsVisible = addDocumentRestrictionsValue;
				Properties[ChangeProtectionProvider.PermissionsPassword].IsVisible = addDocumentRestrictionsValue && protectionValue == AuthenticationType.Password;
				Properties[ChangeProtectionProvider.Encryption].IsVisible = (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password);
				Properties[ChangeProtectionProvider.DontEncryptMetadata].IsVisible = (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password);
			};
			EventHandler updateVisibility = (sender, args) =>
			{
				applyVisibility();
				protectCertDependency.Refresh();
			};

			Property protection = Properties.AddOrRetrieve(ChangeProtectionProvider.Protection, typeof(AuthenticationType), 
				ValueUseOption.DesignTime, AuthenticationType.None);
			protection.Order = propertyOrder++;
			protection.Description = "Method used to protect the PDF.";
			protection.ValueChanged += updateVisibility;

			Property documentOpenPassword = Properties.AddOrRetrieve(ChangeProtectionProvider.DocumentOpenPassword, 
				typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			documentOpenPassword.Order = propertyOrder++;
			documentOpenPassword.Description = "Password required to open the PDF document.";

			Properties.InitializeCertificateProperties(
				ChangeProtectionProvider.CertificateSource,
				ChangeProtectionProvider.CertificateFilePath,
				ChangeProtectionProvider.CertificateFilePassword,
				ChangeProtectionProvider.Certificate,
				ref propertyOrder,
				protectCertDependency);

			Property addDocumentRestrictions = Properties.AddOrRetrieve(ChangeProtectionProvider.AddDocumentRestrictions, typeof(bool), 
				ValueUseOption.DesignTime, false);
			addDocumentRestrictions.Order = propertyOrder++;
			addDocumentRestrictions.Description = "Specify restrictions on the PDF document.";
			addDocumentRestrictions.ValueChanged += updateVisibility;

			Property allowPrinting = Properties.AddOrRetrieve(ChangeProtectionProvider.AllowPrinting, typeof(Printing), ValueUseOption.DesignTime, Printing.None);
			allowPrinting.Order = propertyOrder++;
			allowPrinting.Description = "The level of printing allowed on the PDF document.";

			Property allowChanges = Properties.AddOrRetrieve(ChangeProtectionProvider.AllowChanges, typeof(Changes), ValueUseOption.DesignTime, Changes.None);
			allowChanges.Order = propertyOrder++;
			allowChanges.Description = "The editing actions allowed on the PDF document.";

			Property allowCopying = Properties.AddOrRetrieve(ChangeProtectionProvider.AllowCopying, typeof(bool), ValueUseOption.DesignTime, false);
			allowCopying.Order = propertyOrder++;
			allowCopying.Description = "Enable copying of text, images and other content.";
			allowCopying.ValueChanged += (sender, args) =>
			{
				bool currentValue = Properties[ChangeProtectionProvider.AllowCopying].GetValue<bool>();
				if (currentValue)
					Properties[ChangeProtectionProvider.AllowScreenReaders].Value = true;
			};

			Property allowScreenReaders = Properties.AddOrRetrieve(ChangeProtectionProvider.AllowScreenReaders, typeof(bool), 
				ValueUseOption.DesignTime, false);
			allowScreenReaders.Order = propertyOrder++;
			allowScreenReaders.Description = "Enable text access for screen reader devices for the visually impaired.";

			Property permissionsPassword = Properties.AddOrRetrieve(ChangeProtectionProvider.PermissionsPassword, typeof(string), 
				ValueUseOption.RuntimeRead, string.Empty);
			permissionsPassword.Order = propertyOrder++;
			permissionsPassword.Description = "Password to override restrictions placed on the PDF document.";
			permissionsPassword.Validations.Add(new RequiredValidator());

			Property encryption = Properties.AddOrRetrieve(ChangeProtectionProvider.Encryption, typeof(Encryption), 
				ValueUseOption.DesignTime, Encryption.AES128);
			encryption.Order = propertyOrder++;
			encryption.Description = "Encryption method used to proptect the PDF.";

			Property dontEncryptMetadata = Properties.AddOrRetrieve(ChangeProtectionProvider.DontEncryptMetadata, typeof(bool), 
				ValueUseOption.DesignTime, false);
			dontEncryptMetadata.Order = propertyOrder++;
			dontEncryptMetadata.Description = "Don't encrypt the document metadata.";

			applyVisibility();
		}
	}
}