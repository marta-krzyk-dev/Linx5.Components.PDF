using System;
using Twenty57.Linx.Components.Pdf.Editors;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Components.Pdf.Validators;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;
using Twenty57.Linx.Plugin.Common.Validation;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.Extensions
{
	internal static class PropertyCollectionExtensions
	{
		public static void InitializeInputFileProperties(
			this PropertyCollection properties,
			string inputFilePathPropertyName,
			string authenticationTypePropertyName,
			string passwordPropertyName,
			string certificateSourcePropertyName,
			string certificateFilePathPropertyName,
			string certificateFilePasswordPropertyName,
			string certificatePropertyName,
			bool supportCertificateAuthentication,
			ref int propertyOrder,
			VisibleDependency visibleDependency)
		{
			var certVisibleDependency = new VisibleDependency(() => visibleDependency.Visible && properties.PropertyValueEquals(authenticationTypePropertyName, AuthenticationType.Certificate));
			Action<bool> applyVisibility = (isVisible) =>
			{
				AuthenticationType authenticationTypeValue = properties[authenticationTypePropertyName].GetValue<AuthenticationType>();

				properties[inputFilePathPropertyName].IsVisible = isVisible;
				properties[authenticationTypePropertyName].IsVisible = isVisible;
				properties[passwordPropertyName].IsVisible = (isVisible && authenticationTypeValue == AuthenticationType.Password);
			};
			EventHandler updateVisibility = (sender, args) =>
			{
				applyVisibility(visibleDependency.Visible);
				certVisibleDependency.Refresh();
			};
			visibleDependency.VisibleChanged += (visible) =>
			{
				applyVisibility(visible);
				certVisibleDependency.Refresh();
			};

			Property pdfFilePath = properties.AddOrRetrieve(inputFilePathPropertyName, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			pdfFilePath.Order = propertyOrder++;
			pdfFilePath.Description = "Path to the PDF file.";
			pdfFilePath.Editor = typeof(FilePathEditor);
			pdfFilePath.Validations.Add(new RequiredValidator());

			Property authenticationType = properties.AddOrRetrieve(authenticationTypePropertyName, typeof(AuthenticationType), ValueUseOption.DesignTime, AuthenticationType.None);
			authenticationType.Order = propertyOrder++;
			authenticationType.Description = "Authentication type required to open the PDF file.";
			if (!supportCertificateAuthentication)
				authenticationType.Validations.Add(new CertificateAuthenticationValidator());
			authenticationType.ValueChanged += updateVisibility;

			Property pdfPassword = properties.AddOrRetrieve(passwordPropertyName, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			pdfPassword.Order = propertyOrder++;
			pdfPassword.Description = "Password required to access the PDF file.";
			pdfPassword.Validations.Add(new RequiredValidator());

			properties.InitializeCertificateProperties(
				certificateSourcePropertyName,
				certificateFilePathPropertyName,
				certificateFilePasswordPropertyName,
				certificatePropertyName,
				ref propertyOrder,
				certVisibleDependency);

			applyVisibility(visibleDependency.Visible);
		}

		public static void InitializeCertificateProperties(
			this PropertyCollection properties,
			string certificateSourcePropertyName,
			string certificateFilePathPropertyName,
			string certificateFilePasswordPropertyName,
			string certificatePropertyName,
			ref int propertyOrder,
			VisibleDependency visibleDependency)
		{
			Action<bool> applyVisibility = (isVisible) =>
			{
				CertificateSource certificateSourceValue = properties[certificateSourcePropertyName].GetValue<CertificateSource>();

				properties[certificateSourcePropertyName].IsVisible = isVisible;
				properties[certificateFilePathPropertyName].IsVisible = (isVisible && certificateSourceValue == CertificateSource.File);
				properties[certificateFilePasswordPropertyName].IsVisible = (isVisible && certificateSourceValue == CertificateSource.File);
				properties[certificatePropertyName].IsVisible = (isVisible && certificateSourceValue == CertificateSource.Store);
			};
			EventHandler updateVisibility = (sender, args) => applyVisibility(visibleDependency.Visible);
			visibleDependency.VisibleChanged += (visible) => applyVisibility(visible);

			Property certificateSource = properties.AddOrRetrieve(certificateSourcePropertyName, typeof(CertificateSource), ValueUseOption.DesignTime, CertificateSource.File);
			certificateSource.Order = propertyOrder++;
			certificateSource.Description = "Source to load the certificate from.";
			certificateSource.ValueChanged += updateVisibility;

			Property certificateFilePath = properties.AddOrRetrieve(certificateFilePathPropertyName, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			certificateFilePath.Order = propertyOrder++;
			certificateFilePath.Description = "Path to the file containing a certificate.";
			certificateFilePath.Editor = typeof(FilePathEditor);
			certificateFilePath.Validations.Add(new RequiredValidator());

			Property certificateFilePassword = properties.AddOrRetrieve(certificateFilePasswordPropertyName, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			certificateFilePassword.Order = propertyOrder++;
			certificateFilePassword.Description = "Password required to open the certificate file.";

			Property certificate = properties.AddOrRetrieve(certificatePropertyName, typeof(StoredCertificate), ValueUseOption.DesignTime, new StoredCertificate());
			certificate.Order = propertyOrder++;
			certificate.Description = "Certificate in the windows keystore.";
			certificate.Validations.Add(new StoredCertificateValidator());
			certificate.Editor = typeof(CertificateEditor);

			applyVisibility(visibleDependency.Visible);
		}

		public static Property AddOrRetrieve(
			this PropertyCollection properties,
			string name,
			Type type,
			ValueUseOption valueUsage,
			object defaultValue)
		{
			return properties.AddOrRetrieve(name, TypeReference.Create(type), valueUsage, defaultValue);
		}

		public static Property AddOrRetrieve(
			this PropertyCollection properties,
			string name,
			ITypeReference typeReference,
			ValueUseOption valueUsage,
			object defaultValue)
		{
			if (!properties.ContainsName(name))
				properties.Add(new Property(name, typeReference, valueUsage, defaultValue));

			return properties.GetItemByName(name);
		}

		public static bool PropertyValueEquals<T>(this PropertyCollection properties, string name, T comparisonValue)
		{
			return properties.GetItemByName(name).GetValue<T>().Equals(comparisonValue);
		}
	}
}
