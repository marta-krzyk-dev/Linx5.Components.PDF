using iTextSharp.text.pdf;
using System.Reflection;
using System.util.collections;

namespace Twenty57.Linx.Components.Pdf.PdfOperations.Runtime
{
	// A PdfReader that prefers the parent's value for an annotation for each AcroField.
	public class ParentsAnnotationPdfReader : PdfReader
	{
		public ParentsAnnotationPdfReader(string filePath)
			: base(filePath)
		{ }

		public override AcroFields AcroFields
		{
			get
			{
				var acroFields = (AcroFields)typeof(AcroFields).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0].Invoke(new object[] { this, null });
				FillFields(acroFields);
				return acroFields;
			}
		}

		private void FillFields(AcroFields acroFields)
		{
			var fields = new LinkedDictionary<string, AcroFields.Item>();
			typeof(AcroFields).GetField("fields", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(acroFields, fields);

			PdfDictionary top = (PdfDictionary)GetPdfObjectRelease(Catalog.Get(PdfName.ACROFORM));
			if (top == null)
				return;
			PdfArray arrfds = (PdfArray)GetPdfObjectRelease(top.Get(PdfName.FIELDS));
			if (arrfds == null || arrfds.Size == 0)
				return;
			for (int k = 1; k <= NumberOfPages; ++k)
			{
				PdfDictionary page = GetPageNRelease(k);
				PdfArray annots = (PdfArray)GetPdfObjectRelease(page.Get(PdfName.ANNOTS), page);
				if (annots == null)
					continue;
				for (int j = 0; j < annots.Size; ++j)
				{
					PdfDictionary annot = annots.GetAsDict(j);
					if (annot == null)
					{
						ReleaseLastXrefPartial(annots.GetAsIndirectObject(j));
						continue;
					}
					if (!PdfName.WIDGET.Equals(annot.GetAsName(PdfName.SUBTYPE)))
					{
						ReleaseLastXrefPartial(annots.GetAsIndirectObject(j));
						continue;
					}
					PdfDictionary widget = annot;
					PdfDictionary dic = new PdfDictionary();
					dic.Merge(annot);
					string name = string.Empty;
					PdfDictionary value = null;
					PdfObject lastV = null;
					while (annot != null)
					{
						dic.Merge(annot);
						PdfString t = annot.GetAsString(PdfName.T);
						if (t != null)
							name = t.ToUnicodeString() + "." + name;
						if (lastV == null && annot.Get(PdfName.V) != null)
							lastV = GetPdfObjectRelease(annot.Get(PdfName.V));
						if (value == null && t != null)
						{
							value = annot;
							if (annot.Get(PdfName.V) == null && lastV != null)
								value.Put(PdfName.V, lastV);
						}
						annot = annot.GetAsDict(PdfName.PARENT);
					}
					if (name.Length > 0)
						name = name.Substring(0, name.Length - 1);
					AcroFields.Item item;
					if (!fields.TryGetValue(name, out item))
					{
						item = new AcroFields.Item();
						fields[name] = item;
					}
					var addValueMethod = item.GetType().GetMethod("AddValue", BindingFlags.Instance | BindingFlags.NonPublic);
					if (value == null)
						addValueMethod.Invoke(item, new object[] { widget });
					else
						addValueMethod.Invoke(item, new object[] { value });
					item.GetType().GetMethod("AddWidget", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { widget });
					item.GetType().GetMethod("AddWidgetRef", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { annots.GetAsIndirectObject(j) }); // must be a reference
					if (top != null)
						dic.MergeDifferent(top);
					item.GetType().GetMethod("AddMerged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { dic });
					item.GetType().GetMethod("AddPage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { k });
					item.GetType().GetMethod("AddTabOrder", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { j });
				}
			}
			// some tools produce invisible signatures without an entry in the page annotation array
			// look for a single level annotation
			PdfNumber sigFlags = top.GetAsNumber(PdfName.SIGFLAGS);
			if (sigFlags == null || (sigFlags.IntValue & 1) != 1)
				return;
			for (int j = 0; j < arrfds.Size; ++j)
			{
				PdfDictionary annot = arrfds.GetAsDict(j);
				if (annot == null)
				{
					ReleaseLastXrefPartial(arrfds.GetAsIndirectObject(j));
					continue;
				}
				if (!PdfName.WIDGET.Equals(annot.GetAsName(PdfName.SUBTYPE)))
				{
					ReleaseLastXrefPartial(arrfds.GetAsIndirectObject(j));
					continue;
				}
				PdfArray kids = (PdfArray)GetPdfObjectRelease(annot.Get(PdfName.KIDS));
				if (kids != null)
					continue;
				PdfDictionary dic = new PdfDictionary();
				dic.Merge(annot);
				PdfString t = annot.GetAsString(PdfName.T);
				if (t == null)
					continue;
				string name = t.ToUnicodeString();
				if (fields.ContainsKey(name))
					continue;
				var item = new AcroFields.Item();
				fields[name] = item;
				item.GetType().GetMethod("AddValue", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { dic });
				item.GetType().GetMethod("AddWidget", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { dic });
				item.GetType().GetMethod("AddWidgetRef", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { arrfds.GetAsIndirectObject(j) }); // must be a reference
				item.GetType().GetMethod("AddMerged", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { dic });
				item.GetType().GetMethod("AddPage", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { -1 });
				item.GetType().GetMethod("AddTabOrder", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(item, new object[] { -1 });
			}
		}
	}
}
