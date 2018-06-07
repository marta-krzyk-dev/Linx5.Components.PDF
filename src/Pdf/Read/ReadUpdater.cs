using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.Read
{
	internal class ReadUpdater
	{
		private static ReadUpdater instance;

		public string CurrentVersion { get; } = "1";

		public static ReadUpdater Instance
		{
			get
			{
				if (instance == null)
					instance = new ReadUpdater();
				return instance;
			}
		}

		public bool TryUpdate(IFunctionData data, IUpdateContext context, out IFunctionData updatedData)
		{
			updatedData = data;
			if (updatedData.Version == CurrentVersion)
				return false;

			if (string.IsNullOrEmpty(updatedData.Version))
				updatedData = UpdateToVersion1(updatedData);

			if (updatedData.Version == CurrentVersion)
				return true;

			throw new Exception($"Unknown version [{updatedData.Version}] specified.");
		}

		private IFunctionData UpdateToVersion1(IFunctionData data)
		{
			ReadDesigner designer = new ReadProvider().CreateDesigner(null) as ReadDesigner;
			UpdateInputFileProperties(designer, data);
			UpdateReadTextProperties(designer, data);
			UpdateReadFormDataProperties(designer, data);
			UpdateReadSignatureProperties(designer, data);
			return designer.GetFunctionData();
		}

		private void UpdateInputFileProperties(ReadDesigner designer, IFunctionData data)
		{
			designer.Properties[PropertyNames.PdfFilePath].Value = data.Properties["Pdf filename"].Value;

			AuthenticationType authenticationType = data.Properties["Authentication type"].GetValue<AuthenticationType>();
			designer.Properties[PropertyNames.AuthenticationType].Value = authenticationType;
			switch (authenticationType)
			{
				case AuthenticationType.Password:
					designer.Properties[PropertyNames.PdfPassword].Value = data.Properties["Pdf password"].Value;
					break;
				case AuthenticationType.Certificate:
					CertificateSource certificateSource = data.Properties["Certificate source"].GetValue<CertificateSource>();
					designer.Properties[PropertyNames.CertificateSource].Value = certificateSource;
					switch (certificateSource)
					{
						case CertificateSource.File:
							designer.Properties[PropertyNames.CertificateFilePath].Value = data.Properties["Certificate file path"].Value;
							designer.Properties[PropertyNames.CertificateFilePassword].Value = data.Properties["Certificate file password"].Value;
							break;
						case CertificateSource.Store:
							designer.Properties[PropertyNames.Certificate].Value = data.Properties["Certificate"].Value;
							break;
					}
					break;
			}
		}

		private void UpdateReadTextProperties(ReadDesigner designer, IFunctionData data)
		{
			bool readText = data.Properties["Read text"].GetValue<bool>();
			designer.Properties[PropertyNames.ReadText].Value = readText;
			if (readText)
				designer.Properties[PropertyNames.SplitText].Value = data.Properties["Split text"].Value;
		}

		private void UpdateReadFormDataProperties(ReadDesigner designer, IFunctionData data)
		{
			bool readFormData = data.Properties["Read form data"].GetValue<bool>();
			designer.Properties[PropertyNames.ReadFormData].Value = readFormData;
			if (readFormData)
			{
				FormExtraction formExtraction = data.Properties["Return form data as"].GetValue<FormExtraction>();
				designer.Properties[PropertyNames.ReturnFormDataAs].Value = formExtraction;
				switch (formExtraction)
				{
					case FormExtraction.CustomType:
						designer.Properties[PropertyNames.FormDataType].Value = data.Properties["Form data type"].Value;
						break;
					case FormExtraction.Infer:
						designer.Properties[PropertyNames.SamplePdf].Value = data.Properties["Sample pdf"].Value;
						break;
				}
			}
		}

		private void UpdateReadSignatureProperties(ReadDesigner designer, IFunctionData data)
		{
			designer.Properties[PropertyNames.ReadSignature].Value = data.Properties["Read signature"].Value;
		}
	}
}
