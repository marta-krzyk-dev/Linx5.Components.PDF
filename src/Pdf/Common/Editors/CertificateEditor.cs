using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Interop;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.UI.Editors;

namespace Twenty57.Linx.Components.Pdf.Editors
{
	public class CertificateEditor : IPropertyEditor
	{
		public string Name { get; } = "Certificate selector";

		public string IconResourcePath { get; } = null;

		public DataTemplate InlineTemplate { get; } = EditorResources.CertificateInlineEditorTemplate;

		public void EditValue(Property property, object designer)
		{
			StoredCertificate selectedCertificate = PickCertificate();
			if (selectedCertificate != null)
				property.Value = selectedCertificate;
		}

		private static StoredCertificate PickCertificate()
		{
			var availableCertificates =
				from location in new[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine }
				from name in new[] { StoreName.My }
				select Tuple.Create(Tuple.Create(location, name), GetCertificates(location, name));

			var appropriateCertificates =
				availableCertificates
				.SelectMany(v => v.Item2)
				.OfType<X509Certificate2>()
				.Where(v => v.HasPrivateKey)
				.GroupBy(v => v.Thumbprint)
				.Select(v => v.First());

			X509Certificate2 selectedCertificate = X509Certificate2UI.SelectFromCollection(
				new X509Certificate2Collection(appropriateCertificates.ToArray()), "Select certificate", string.Empty, X509SelectionFlag.SingleSelection, GetWindowHandle())
				.OfType<X509Certificate2>()
				.FirstOrDefault();

			if (selectedCertificate == null)
				return null;

			var store = availableCertificates.First(v => v.Item2.Contains(selectedCertificate)).Item1;
			return new StoredCertificate(store.Item1, store.Item2, selectedCertificate);
		}

		private static IntPtr GetWindowHandle()
		{
			return (Application.Current?.MainWindow != null) ? new WindowInteropHelper(Application.Current.MainWindow).Handle : IntPtr.Zero;
		}

		private static IEnumerable<X509Certificate2> GetCertificates(StoreLocation location, StoreName name)
		{
			var store = new X509Store(name, location);
			try
			{
				store.Open(OpenFlags.ReadOnly);
				return store.Certificates.OfType<X509Certificate2>();
			}
			finally
			{
				store.Close();
			}
		}
	}
}