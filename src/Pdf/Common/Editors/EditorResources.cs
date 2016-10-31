using System.Windows;

namespace Twenty57.Linx.Components.Pdf.Editors
{
	public partial class EditorResources
	{
		private static EditorResources instance;

		private EditorResources()
		{
			InitializeComponent();
		}

		public static DataTemplate CertificateInlineEditorTemplate { get; } = Instance["CertificateInlineEditorTemplate"] as DataTemplate;

		private static EditorResources Instance
		{
			get
			{
				if (null == instance)
					instance = new EditorResources();

				return instance;
			}
		}
	}
}