using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Twenty57.Linx.Components.Pdf.Read.Helpers
{
	//http://cjhaas.com/blog/2013/03/13/itextsharp-slightly-smarter-text-extraction-strategy/
	public class TopToBottomTextExtractionStrategy : ITextExtractionStrategy
	{
		private Vector lastStart;
		private Vector lastEnd;

		private SortedDictionary<int, StringBuilder> results = new SortedDictionary<int, StringBuilder>();

		public TopToBottomTextExtractionStrategy() { }
		public virtual void BeginTextBlock() { }
		public virtual void EndTextBlock() { }
		public virtual void RenderImage(ImageRenderInfo renderInfo) { }

		public virtual String GetResultantText()
		{
			StringBuilder buffer = new StringBuilder();
			foreach (var stringBuilderLine in results)
			{
				buffer.AppendLine(stringBuilderLine.Value.ToString());
			}
			return buffer.ToString();
		}
		public virtual void RenderText(TextRenderInfo renderInfo)
		{
			bool firstRender = results.Count == 0;

			LineSegment segment = renderInfo.GetBaseline();
			Vector start = segment.GetStartPoint();
			Vector end = segment.GetEndPoint();

			int currentLineKey = (int)start[1];

			if (!firstRender)
			{
				Vector x0 = start;
				Vector x1 = lastStart;
				Vector x2 = lastEnd;

				float distance = (x2.Subtract(x1)).Cross((x1.Subtract(x0))).LengthSquared / x2.Subtract(x1).LengthSquared;

				float sameLineThreshold = 1f;
				if (distance <= sameLineThreshold)
				{
					currentLineKey = (int)lastStart[1];
				}
			}
			currentLineKey = currentLineKey * -1;

			if (!results.ContainsKey(currentLineKey))
			{
				results.Add(currentLineKey, new StringBuilder());
			}

			if (!firstRender &&                                       
				results[currentLineKey].Length != 0 &&                 
				!results[currentLineKey].ToString().EndsWith(" ") &&  
				renderInfo.GetText().Length > 0 &&                    
				!renderInfo.GetText().StartsWith(" "))
			{             
						   
				float spacing = lastEnd.Subtract(start).Length;
				if (spacing > renderInfo.GetSingleSpaceWidth() / 2f)
				{
					results[currentLineKey].Append(" ");
				}
			}

			results[currentLineKey].Append(renderInfo.GetText());

			lastStart = start;
			lastEnd = end;
		}
	}
}
