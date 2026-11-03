using System.Collections.Generic;

namespace FLD
{
	public class NanoXMLDocument : NanoXMLBase
	{
		private NanoXMLNode rootNode;

		private List<NanoXMLAttribute> declarations = new List<NanoXMLAttribute>();

		public NanoXMLNode RootNode
		{
			get
			{
				return rootNode;
			}
		}

		public IEnumerable<NanoXMLAttribute> Declarations
		{
			get
			{
				return declarations;
			}
		}

		public NanoXMLDocument(string xmlString)
		{
			int i = 0;
			while (true)
			{
				NanoXMLBase.SkipSpaces(xmlString, ref i);
				if (xmlString[i] != '<')
				{
					throw new XMLParsingException("Unexpected token");
				}
				i++;
				if (xmlString[i] == '?')
				{
					i++;
					NanoXMLBase.ParseAttributes(xmlString, ref i, declarations, '?', '>');
					i++;
					i++;
					continue;
				}
				if (xmlString[i] != '!')
				{
					break;
				}
				for (; xmlString[i] != '>'; i++)
				{
				}
				i++;
			}
			rootNode = new NanoXMLNode(xmlString, ref i);
		}
	}
}
