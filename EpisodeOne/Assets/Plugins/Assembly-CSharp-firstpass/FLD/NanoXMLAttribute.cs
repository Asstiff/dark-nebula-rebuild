namespace FLD
{
	public class NanoXMLAttribute
	{
		private string name;

		private string value;

		public string Name
		{
			get
			{
				return name;
			}
		}

		public string Value
		{
			get
			{
				return value;
			}
		}

		internal NanoXMLAttribute(string name, string value)
		{
			this.name = name;
			this.value = value;
		}
	}
}
