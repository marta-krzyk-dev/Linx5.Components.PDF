using System;
using System.Collections.Generic;
using System.Linq;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Components.Pdf.Validators;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Validation;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.FillForm
{
	internal class FillFormDesigner : FunctionDesigner
	{
		public FillFormDesigner(IDesignerContext context)
			: base(context)
		{
			InitializeProperties();
		}

		public FillFormDesigner(IFunctionData data, IDesignerContext context)
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

			InitializeInputProperties(ref propertyOrder);

			Property formData = Properties.AddOrRetrieve(PropertyNames.FillFormFormData, typeof(object), ValueUseOption.RuntimeRead, null);
			formData.Order = propertyOrder++;
			formData.Description = "A custom object that contains the form data to insert.";
			formData.Validations.Add(new RequiredValidator());

			Property outputFilePath = Properties.AddOrRetrieve(PropertyNames.OutputFilePath, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			outputFilePath.Order = propertyOrder++;
			outputFilePath.Description = "Path of the PDF file to write to.";
			outputFilePath.Editor = typeof(FilePathEditor);
			outputFilePath.Validations.Add(new RequiredValidator());
		}

		private void InitializeInputProperties(ref int propertyOrder)
		{
			Property inputPDFFilePathProperty = Properties.AddOrRetrieve(PropertyNames.InputFilePath, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			inputPDFFilePathProperty.Order = propertyOrder++;
			inputPDFFilePathProperty.Description = "Path to the PDF file.";
			inputPDFFilePathProperty.Editor = typeof(FilePathEditor);
			inputPDFFilePathProperty.Validations.Add(new RequiredValidator());

			VisibleDependency certVisibleDependency = new VisibleDependency(() => Properties.PropertyValueEquals(PropertyNames.InputAuthenticationType,
				AuthenticationType.Certificate));
			Action applyVisibility = () =>
			{
				AuthenticationType authenticationTypeValue = Properties[PropertyNames.InputAuthenticationType].GetValue<AuthenticationType>();
				Properties[PropertyNames.InputPassword].IsVisible = authenticationTypeValue == AuthenticationType.Password;
			};
			EventHandler updateVisibility = (sender, args) =>
			{
				applyVisibility();
				certVisibleDependency.Refresh();
			};

			Property authenticationTypeProperty = Properties.AddOrRetrieve(PropertyNames.InputAuthenticationType, typeof(AuthenticationType),
				ValueUseOption.DesignTime, AuthenticationType.None);
			authenticationTypeProperty.Order = propertyOrder++;
			authenticationTypeProperty.Description = "Authentication type required to open the PDF file.";
			authenticationTypeProperty.Validations.Add(new CertificateAuthenticationValidator());
			authenticationTypeProperty.ValueChanged += updateVisibility;

			Property pdfPassword = Properties.AddOrRetrieve(PropertyNames.InputPassword, typeof(string), ValueUseOption.RuntimeRead, string.Empty);
			pdfPassword.Order = propertyOrder++;
			pdfPassword.Description = "Password required to access the PDF file.";
			pdfPassword.Validations.Add(new RequiredValidator());

			Properties.InitializeCertificateProperties(
				PropertyNames.InputCertificateSource,
				PropertyNames.InputCertificateFilePath,
				PropertyNames.InputCertificateFilePassword,
				PropertyNames.InputCertificate,
				ref propertyOrder,
				certVisibleDependency);

			applyVisibility();
		}
	}
}