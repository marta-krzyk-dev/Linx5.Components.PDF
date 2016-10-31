using NUnit.Framework;
using System;
using Twenty57.Linx.Plugin.Common;
using Twenty57.Linx.Plugin.Common.Types;

namespace Twenty57.Linx.Components.Pdf.Tests.Extensions
{
	internal static class TypePropertyExtensions
	{
		public static void AssertCompiled(this ITypeProperty typeProperty, string expectedName, Type expectedType)
		{
			Assert.AreEqual(expectedName, typeProperty.Name);
			Assert.AreEqual(TypeReference.Create(expectedType), typeProperty.TypeReference);
		}

		public static void AssertGenerated(this ITypeProperty typeProperty, string expectedName)
		{
			Assert.AreEqual(expectedName, typeProperty.Name);
			Assert.IsTrue(typeProperty.TypeReference.IsGenerated);
		}

		public static void AssertList(this ITypeProperty typeProperty, string expectedName, Type expectedListType)
		{
			typeProperty.AssertList(expectedName, TypeReference.Create(expectedListType));
		}

		public static void AssertList(this ITypeProperty typeProperty, string expectedName, ITypeReference expectedListTypeReference)
		{
			Assert.AreEqual(expectedName, typeProperty.Name);
			Assert.IsTrue(typeProperty.TypeReference.IsList);
			Assert.AreEqual(expectedListTypeReference, typeProperty.TypeReference.GetEnumerableContentType());
		}
	}
}