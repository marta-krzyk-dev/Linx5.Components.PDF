using System;
using System.Collections.Generic;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Components.Pdf.Helpers;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.AddWatermark
{
	internal class AddWatermarkDesigner : PdfDesigner
	{
		public AddWatermarkDesigner(IDesignerContext context) : base(context)
		{
			InitializeProperties();
		}

		public AddWatermarkDesigner(IFunctionData data, IDesignerContext context) : base(data, context)	{ }
		
		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			Properties.InitializeInputFileProperties(
				PropertyNames.FilePath,
				PropertyNames.AuthenticationType,
				PropertyNames.Password,
				PropertyNames.CertificateSource,
				PropertyNames.CertificateFilePath,
				PropertyNames.CertificateFilePassword,
				PropertyNames.Certificate,
				true,
				ref propertyOrder,
				new VisibleDependency(() => true));

			Property watermarkPages = Properties.AddOrRetrieve(PropertyNames.Pages, typeof(string), 
				ValueUseOption.RuntimeRead, string.Empty);
			watermarkPages.Order = propertyOrder++;
			watermarkPages.Description = "Page range to stamp with watermark. Leave this blank to add the watermark to all pages.";

			Property watermarkPosition = Properties.AddOrRetrieve(PropertyNames.Position, typeof(WatermarkPosition), 
				ValueUseOption.DesignTime, WatermarkPosition.Above);
			watermarkPosition.Order = propertyOrder++;
			watermarkPosition.Description = "Draws the watermark above or below the original document content.";
		}
	}
}