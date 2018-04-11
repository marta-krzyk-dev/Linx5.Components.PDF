using System;
using System.Collections.Generic;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
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
			var protectCertDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.Protection, AuthenticationType.Certificate));

			Action applyVisibility = () =>
			{
				AuthenticationType protectionValue = Properties[PropertyNames.Protection].GetValue<AuthenticationType>();
				bool addDocumentRestrictionsValue = Properties[PropertyNames.AddDocumentRestrictions].GetValue<bool>();

				Properties[PropertyNames.DocumentOpenPassword].IsVisible = protectionValue == AuthenticationType.Password;
				Properties[PropertyNames.AddDocumentRestrictions].IsVisible = (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password);
				Properties[PropertyNames.AllowPrinting].IsVisible = addDocumentRestrictionsValue;
				Properties[PropertyNames.AllowChanges].IsVisible = addDocumentRestrictionsValue;
				Properties[PropertyNames.AllowCopying].IsVisible = addDocumentRestrictionsValue;
				Properties[PropertyNames.AllowScreenReaders].IsVisible = addDocumentRestrictionsValue;
				Properties[PropertyNames.PermissionsPassword].IsVisible = addDocumentRestrictionsValue && protectionValue == AuthenticationType.Password;
				Properties[PropertyNames.Encryption].IsVisible = (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password);
				Properties[PropertyNames.DontEncryptMetadata].IsVisible = (protectionValue == AuthenticationType.Certificate || protectionValue == AuthenticationType.Password);
			};
			EventHandler updateVisibility = (sender, args) =>
			{
				applyVisibility();
				protectCertDependency.Refresh();
			};

			Property protection = Properties.AddOrRetrieve(PropertyNames.Protection, typeof(AuthenticationType), 
				ValueUseOption.DesignTime, AuthenticationType.None);
			protection.Order = propertyOrder++;
			protection.Description = "Method used to protect the PDF.";
			protection.ValueChanged += updateVisibility;

			Property documentOpenPassword = Properties.AddOrRetrieve(PropertyNames.DocumentOpenPassword, 
				typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			documentOpenPassword.Order = propertyOrder++;
			documentOpenPassword.Description = "Password required to open the PDF document.";

			Properties.InitializeCertificateProperties(
				PropertyNames.CertificateSource,
				PropertyNames.CertificateFilePath,
				PropertyNames.CertificateFilePassword,
				PropertyNames.Certificate,
				ref propertyOrder,
				protectCertDependency);

			Property addDocumentRestrictions = Properties.AddOrRetrieve(PropertyNames.AddDocumentRestrictions, typeof(bool), 
				ValueUseOption.DesignTime, false);
			addDocumentRestrictions.Order = propertyOrder++;
			addDocumentRestrictions.Description = "Specify restrictions on the PDF document.";
			addDocumentRestrictions.ValueChanged += updateVisibility;

			Property allowPrinting = Properties.AddOrRetrieve(PropertyNames.AllowPrinting, typeof(Printing), ValueUseOption.DesignTime, Printing.None);
			allowPrinting.Order = propertyOrder++;
			allowPrinting.Description = "The level of printing allowed on the PDF document.";

			Property allowChanges = Properties.AddOrRetrieve(PropertyNames.AllowChanges, typeof(Changes), ValueUseOption.DesignTime, Changes.None);
			allowChanges.Order = propertyOrder++;
			allowChanges.Description = "The editing actions allowed on the PDF document.";

			Property allowCopying = Properties.AddOrRetrieve(PropertyNames.AllowCopying, typeof(bool), ValueUseOption.DesignTime, false);
			allowCopying.Order = propertyOrder++;
			allowCopying.Description = "Enable copying of text, images and other content.";
			allowCopying.ValueChanged += (sender, args) =>
			{
				bool currentValue = Properties[PropertyNames.AllowCopying].GetValue<bool>();
				if (currentValue)
					Properties[PropertyNames.AllowScreenReaders].Value = true;
			};

			Property allowScreenReaders = Properties.AddOrRetrieve(PropertyNames.AllowScreenReaders, typeof(bool), 
				ValueUseOption.DesignTime, false);
			allowScreenReaders.Order = propertyOrder++;
			allowScreenReaders.Description = "Enable text access for screen reader devices for the visually impaired.";

			Property permissionsPassword = Properties.AddOrRetrieve(PropertyNames.PermissionsPassword, typeof(string), 
				ValueUseOption.RuntimeRead, string.Empty);
			permissionsPassword.Order = propertyOrder++;
			permissionsPassword.Description = "Password to override restrictions placed on the PDF document.";
			permissionsPassword.Validations.Add(new RequiredValidator());

			Property encryption = Properties.AddOrRetrieve(PropertyNames.Encryption, typeof(Encryption), 
				ValueUseOption.DesignTime, Encryption.AES128);
			encryption.Order = propertyOrder++;
			encryption.Description = "Encryption method used to proptect the PDF.";

			Property dontEncryptMetadata = Properties.AddOrRetrieve(PropertyNames.DontEncryptMetadata, typeof(bool), 
				ValueUseOption.DesignTime, false);
			dontEncryptMetadata.Order = propertyOrder++;
			dontEncryptMetadata.Description = "Don't encrypt the document metadata.";

			applyVisibility();
		}
	}
}