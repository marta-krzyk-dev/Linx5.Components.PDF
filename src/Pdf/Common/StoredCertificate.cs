using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

namespace Twenty57.Linx.Components.Pdf
{
	public class StoredCertificate : ISerializable
	{
		private X509Certificate2 certificate;

		public StoredCertificate()
		{ }

		public StoredCertificate(StoreLocation location, StoreName name, X509Certificate2 certificate)
		{
			StoreLocation = location;
			StoreName = name;
			ThumbPrint = certificate.Thumbprint;

			this.certificate = certificate;
		}

		public StoredCertificate(StoreLocation location, StoreName name, string thumbprint)
		{
			StoreLocation = location;
			StoreName = name;
			ThumbPrint = thumbprint;

			GetCertificate();
		}

		protected StoredCertificate(SerializationInfo info, StreamingContext context)
		{
			StoreLocation = (StoreLocation)info.GetValue("storeLocation", typeof(StoreLocation));
			StoreName = (StoreName)info.GetValue("storeName", typeof(StoreName));
			ThumbPrint = info.GetString("thumbPrint");
		}

		public StoreLocation StoreLocation { get; }
		public StoreName StoreName { get; }
		public string ThumbPrint { get; }

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("storeLocation", StoreLocation);
			info.AddValue("storeName", StoreName);
			info.AddValue("thumbPrint", ThumbPrint);
		}

		public X509Certificate2 GetCertificate()
		{
			if (this.certificate != null)
				return this.certificate;

			var store = new X509Store(StoreName, StoreLocation);
			store.Open(OpenFlags.ReadOnly);
			try
			{
				return this.certificate = store.Certificates.Find(X509FindType.FindByThumbprint, ThumbPrint, false).OfType<X509Certificate2>().Single();
			}
			catch
			{
				throw new Exception($"Certificate with thumbprint [{ThumbPrint}] not found.");
			}
			finally
			{
				store.Close();
			}
		}

		public override string ToString()
		{
			if (string.IsNullOrWhiteSpace(ThumbPrint))
				return "Select a certificate";

			if (this.certificate == null)
				return "Certificate not found";

			return new string[] { this.certificate.FriendlyName, this.certificate.Subject, this.certificate.Issuer, this.certificate.Thumbprint }
			.First(v => !string.IsNullOrEmpty(v));
		}
	}
}
