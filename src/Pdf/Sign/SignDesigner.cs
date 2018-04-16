using System;
using System.Collections.Generic;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Validation;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.Sign
{
	internal class SignDesigner : PdfDesigner
	{
		public SignDesigner(IDesignerContext context) : base(context)
		{
			InitializeProperties();
		}

		public SignDesigner(IFunctionData data, IDesignerContext context) : base(data, context) {	}

		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			Action applyVisibility = () =>
			{
				SignaturePosition signPlacementValue = Properties[PropertyNames.Placement].GetValue<SignaturePosition>();

				Properties[PropertyNames.FieldName].IsVisible = signPlacementValue == SignaturePosition.FormField;
				Properties[PropertyNames.PositionX].IsVisible = signPlacementValue == SignaturePosition.OnPage;
				Properties[PropertyNames.PositionY].IsVisible = signPlacementValue == SignaturePosition.OnPage;
				Properties[PropertyNames.Width].IsVisible = signPlacementValue == SignaturePosition.OnPage;
				Properties[PropertyNames.Height].IsVisible = signPlacementValue == SignaturePosition.OnPage;
				Properties[PropertyNames.BackgroundImage].IsVisible = (signPlacementValue == SignaturePosition.FormField || signPlacementValue == SignaturePosition.OnPage);
				Properties[PropertyNames.Page].IsVisible = signPlacementValue == SignaturePosition.OnPage;
			};
			EventHandler updateVisibility = (sender, args) => applyVisibility();

			Property signedAt = Properties.AddOrRetrieve(PropertyNames.SignedAt, typeof(string), ValueUseOption.RuntimeRead, 
				string.Empty);
			signedAt.Order = propertyOrder++;
			signedAt.Description = "Location where the signing took place.";
			signedAt.Validations.Add(new RequiredValidator());

			Property reason = Properties.AddOrRetrieve(PropertyNames.Reason, typeof(string), ValueUseOption.RuntimeRead, 
				string.Empty);
			reason.Order = propertyOrder++;
			reason.Description = "Reason for signing the document.";
			reason.Validations.Add(new RequiredValidator());

			Property lockAfterSigning = Properties.AddOrRetrieve(PropertyNames.LockAfterSigning, typeof(bool), 
				ValueUseOption.DesignTime, false);
			lockAfterSigning.Order = propertyOrder++;
			lockAfterSigning.Description = "Lock the document to prevent further changes.";

			Properties.InitializeCertificateProperties(
				PropertyNames.CertificateSource,
				PropertyNames.CertificateFilePath,
				PropertyNames.CertificateFilePassword,
				PropertyNames.Certificate,
				ref propertyOrder,
				new VisibleDependency(() => true));

			Property placement = Properties.AddOrRetrieve(PropertyNames.Placement, typeof(SignaturePosition), 
				ValueUseOption.DesignTime, SignaturePosition.Hidden);
			placement.Order = propertyOrder++;
			placement.Description = "Where to put the signature in the document.";
			placement.ValueChanged += updateVisibility;

			Property fieldName = Properties.AddOrRetrieve(PropertyNames.FieldName, typeof(string), ValueUseOption.RuntimeRead, 
				string.Empty);
			fieldName.Order = propertyOrder++;
			fieldName.Description = "Form field name to place the signature in.";
			fieldName.Validations.Add(new RequiredValidator());

			Property positionX = Properties.AddOrRetrieve(PropertyNames.PositionX, typeof(int), ValueUseOption.RuntimeRead, 0);
			positionX.Order = propertyOrder++;
			positionX.Description = "X coordinate of the signature.";

			Property positionY = Properties.AddOrRetrieve(PropertyNames.PositionY, typeof(int), ValueUseOption.RuntimeRead, 0);
			positionY.Order = propertyOrder++;
			positionY.Description = "Y coordinate of the signature.";

			Property width = Properties.AddOrRetrieve(PropertyNames.Width, typeof(int), ValueUseOption.RuntimeRead, 100);
			width.Order = propertyOrder++;
			width.Description = "Width of the signature box.";

			Property height = Properties.AddOrRetrieve(PropertyNames.Height, typeof(int), ValueUseOption.RuntimeRead, 50);
			height.Order = propertyOrder++;
			height.Description = "Height of the signature box.";

			Property backgroundImage = Properties.AddOrRetrieve(PropertyNames.BackgroundImage, typeof(string), 
				ValueUseOption.RuntimeRead, string.Empty);
			backgroundImage.Order = propertyOrder++;
			backgroundImage.Description = "Path to an image file to use as a background for the signature.";
			backgroundImage.Editor = typeof(FilePathEditor);

			Property page = Properties.AddOrRetrieve(PropertyNames.Page, typeof(int), ValueUseOption.RuntimeRead, 1);
			page.Order = propertyOrder++;
			page.Description = "Page on which to include the visible signature.";
			page.Validations.Add(new RangeValidator<int>(1, int.MaxValue));

			applyVisibility();
		}
	}
}