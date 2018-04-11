using System;
using Twenty57.Linx.Plugin.Common;

namespace Twenty57.Linx.Components.Pdf.ChangeProtection
{
	public class ChangeProtectionProvider : FunctionProvider
	{
		public const string AddDocumentRestrictions = "Add document restrictions";
		public const string AllowChanges = "Allow changes";
		public const string AllowCopying = "Allow copying";
		public const string AllowPrinting = "Allow printing";
		public const string AllowScreenReaders = "Allow screen readers";
		public const string Certificate = "Certificate";
		public const string CertificateFilePassword = "Certificate file password";
		public const string CertificateFilePath = "Certificate file path";
		public const string CertificateSource = "Certificate source";
		public const string DocumentOpenPassword = "Document open password";
		public const string DontEncryptMetadata = "Don't encrypt metadata";
		public const string Encryption = "Encryption";
		public const string PermissionsPassword = "Permissions password";
		public const string Protection = "Output PDF protection";

		public override string Name { get; } = "ChangeProtection";

		public override string SearchKeywords { get; } = "pdf change protection";

		public override FunctionCodeGenerator CreateCodeGenerator(IFunctionData data) => new ChangeProtectionCodeGenerator(data);

		public override FunctionDesigner CreateDesigner(IFunctionData data, IDesignerContext context) => new ChangeProtectionDesigner(data, context);

		public override FunctionDesigner CreateDesigner(IDesignerContext context) => new ChangeProtectionDesigner(context);
	}
}