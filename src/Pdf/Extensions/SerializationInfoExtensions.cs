using System.Runtime.Serialization;

namespace Twenty57.Linx.Components.Pdf.Extensions
{
	internal static class SerializationInfoExtensions
	{
		public static bool TryGetValue<T>(this SerializationInfo serializationInfo, string name, out T value)
		{
			foreach (SerializationEntry nextEntry in serializationInfo)
			{
				if (nextEntry.Name == name)
				{
					value = (T)((nextEntry.Value is T) ? nextEntry.Value : serializationInfo.GetValue(name, typeof(T)));
					return true;
				}
			}

			value = default(T);
			return false;
		}
	}
}
