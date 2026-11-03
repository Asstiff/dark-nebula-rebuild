using System.Collections.Generic;

namespace FLD
{
	public class NanoXMLNode : NanoXMLBase
	{
		private string value;

		private string name;

		private List<NanoXMLNode> subNodes = new List<NanoXMLNode>();

		private List<NanoXMLAttribute> attributes = new List<NanoXMLAttribute>();

		public string Value
		{
			get
			{
				return value;
			}
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public IEnumerable<NanoXMLNode> SubNodes
		{
			get
			{
				return subNodes;
			}
		}

		public IEnumerable<NanoXMLAttribute> Attributes
		{
			get
			{
				return attributes;
			}
		}

		public NanoXMLNode this[string nodeName]
		{
			get
			{
				foreach (NanoXMLNode subNode in subNodes)
				{
					if (subNode.name == nodeName)
					{
						return subNode;
					}
				}
				return null;
			}
		}

		internal NanoXMLNode(string str, ref int i)
		{
			name = NanoXMLBase.ParseAttributes(str, ref i, attributes, '>', '/');
			if (str[i] == '/')
			{
				i++;
				i++;
				return;
			}
			i++;
			int i2 = i;
			NanoXMLBase.SkipSpaces(str, ref i2);
			if (str[i2] == '<')
			{
				i = i2;
				while (str[i + 1] != '/')
				{
					i++;
					subNodes.Add(new NanoXMLNode(str, ref i));
					NanoXMLBase.SkipSpaces(str, ref i);
					if (i >= str.Length)
					{
						return;
					}
					if (str[i] != '<')
					{
						throw new XMLParsingException("Unexpected token");
					}
				}
				i++;
			}
			else
			{
				value = NanoXMLBase.GetValue(str, ref i, '<', '\0', false);
				i++;
				if (str[i] != '/')
				{
					throw new XMLParsingException("Invalid ending on tag " + name);
				}
			}
			i++;
			NanoXMLBase.SkipSpaces(str, ref i);
			string text = NanoXMLBase.GetValue(str, ref i, '>', '\0', true);
			if (text != name)
			{
				throw new XMLParsingException("Start/end tag name mismatch: " + name + " and " + text);
			}
			NanoXMLBase.SkipSpaces(str, ref i);
			if (str[i] != '>')
			{
				throw new XMLParsingException("Invalid ending on tag " + name);
			}
			i++;
		}

		public NanoXMLAttribute GetAttribute(string attributeName)
		{
			foreach (NanoXMLAttribute attribute in attributes)
			{
				if (attribute.Name == attributeName)
				{
					return attribute;
				}
			}
			return null;
		}
	}
}
