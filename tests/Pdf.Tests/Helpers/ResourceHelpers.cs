using System.IO;
using System.Reflection;

namespace Twenty57.Linx.Components.Pdf.Tests.Helpers
{
	internal static class ResourceHelpers
	{
		public static string WriteResourceToFile(string resourceName, string path)
		{
			if (!Directory.Exists(path))
				throw new DirectoryNotFoundException($"[{path}] does not exist.");

			string filePath = Path.Combine(path, Path.GetRandomFileName());

			using (Stream input = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			using (Stream output = File.Create(filePath))
			{
				input.CopyTo(output);
			}

			return filePath;
		}
	}
}
