using iTextSharp.text.pdf;
using Org.BouncyCastle.Security;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Twenty57.Linx.Components.Pdf.Tests.Helpers;

namespace Twenty57.Linx.Components.Pdf.Tests
{
	public class AuthenticationManager : IDisposable
	{
		private StoredCertificate storedCertificate;

		public AuthenticationManager(string password = "secret")
		{
			Password = password;
			CertificateFilePath = ResourceHelpers.WriteResourceToFile("Twenty57.Linx.Components.Pdf.Tests.Common.Resources.John Smith.pfx", Path.GetTempPath());
			CertificateFilePassword = "secret";
			Certificate = new X509Certificate2(CertificateFilePath, CertificateFilePassword, X509KeyStorageFlags.Exportable);
		}

		public void Dispose()
		{
			if (File.Exists(CertificateFilePath))
				File.Delete(CertificateFilePath);

			if (this.storedCertificate != null)
			{
				var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
				store.Open(OpenFlags.ReadWrite);
				store.Remove(this.storedCertificate.GetCertificate());
				store.Close();
			}
		}

		public string Password { get; }
		public string CertificateFilePath { get; }
		public string CertificateFilePassword { get; }
		public X509Certificate2 Certificate { get; }

		public StoredCertificate StoredCertificate
		{
			get
			{
				if (this.storedCertificate == null)
				{
					var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
					store.Open(OpenFlags.ReadWrite);
					store.Add(Certificate);
					store.Close();

					this.storedCertificate = new StoredCertificate(StoreLocation.CurrentUser, StoreName.My, Certificate);
				}

				return this.storedCertificate;
			}
		}

		public void Protect(string filePath, FileAuthentication fileAuth)
		{
			CheckFileExists(filePath);

			if (fileAuth == FileAuthentication.None)
				return;

			string outputFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			using (var reader = new PdfReader(filePath))
			using (var outputStream = new FileStream(outputFilePath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
			using (var stamper = new PdfStamper(reader, outputStream))
			{
				switch (fileAuth)
				{
					case FileAuthentication.Password:
						stamper.SetEncryption(PdfWriter.ENCRYPTION_AES_256, Password, Password, 0);
						break;
					case FileAuthentication.CertificateFile:
					case FileAuthentication.CertificateStore:
						var certificates = new Org.BouncyCastle.X509.X509Certificate[] { DotNetUtilities.FromX509Certificate(new X509Certificate2(CertificateFilePath, CertificateFilePassword)) };
						stamper.SetEncryption(certificates, new int[] { 0 }, PdfWriter.ENCRYPTION_AES_256);
						break;
					default:
						throw new NotSupportedException("Invalid FileAuthentication specified.");
				}
			}

			File.Delete(filePath);
			File.Move(outputFilePath, filePath);
		}

		public PdfReader GetReader(string filePath, FileAuthentication fileAuth)
		{
			CheckFileExists(filePath);

			switch (fileAuth)
			{
				case FileAuthentication.None:
					return new PdfReader(filePath);
				case FileAuthentication.Password:
					return new PdfReader(filePath, Encoding.UTF8.GetBytes(Password));
				case FileAuthentication.CertificateFile:
				case FileAuthentication.CertificateStore:
					if (!Certificate.HasPrivateKey)
						throw new NotSupportedException("Certificate must have a private key.");

					return new PdfReader(filePath, DotNetUtilities.FromX509Certificate(Certificate), DotNetUtilities.GetKeyPair(Certificate.PrivateKey).Private);
				default:
					throw new NotSupportedException("Invalid FileAuthentication specified.");
			}
		}

		private static void CheckFileExists(string filePath)
		{
			if (!File.Exists(filePath))
				throw new FileNotFoundException($"File [{filePath}] does not exist.");
		}
	}
}
