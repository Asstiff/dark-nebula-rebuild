using System;

namespace FLD
{
	public class XMLParsingException : Exception
	{
		public XMLParsingException(string message)
			: base(message)
		{
		}
	}
}
