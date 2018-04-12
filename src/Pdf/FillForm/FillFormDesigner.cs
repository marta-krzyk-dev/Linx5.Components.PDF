using System.Collections.Generic;
using Twenty57.Linx.Components.Pdf.Common;
using Twenty57.Linx.Components.Pdf.Extensions;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Validation;

namespace Twenty57.Linx.Components.Pdf.FillForm
{
	internal class FillFormDesigner : PdfDesigner
	{
		public FillFormDesigner(IDesignerContext context)
			: base(context)
		{
			InitializeProperties();
		}

		public FillFormDesigner(IFunctionData data, IDesignerContext context)
			: base(data, context)
		{ }

		protected override void InitializeProperties(IReadOnlyDictionary<string, IPropertyData> properties)
		{
			base.InitializeProperties(properties);
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			Property formData = Properties.AddOrRetrieve(PropertyNames.FillFormFormData, typeof(object), ValueUseOption.RuntimeRead, null);
			formData.Order = propertyOrder++;
			formData.Description = "A custom object that contains the form data to insert.";
			formData.Validations.Add(new RequiredValidator());
		}
	}
}