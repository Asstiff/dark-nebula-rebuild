using System.Collections.Generic;
using System.Text;

public class WordWrap
{
	private const char NewLine = '\n';

	private static char[] splitChars = new char[3] { ' ', '-', '\t' };

	public static string Wrap(string str, int width)
	{
		string[] array = Explode(str, splitChars);
		int num = 0;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			string text = array[i];
			if (num + text.Length > width)
			{
				if (num > 0)
				{
					stringBuilder.Append('\n');
					num = 0;
				}
				while (text.Length > width)
				{
					stringBuilder.Append(text.Substring(0, width - 1));
					text = text.Substring(width - 1);
					stringBuilder.Append('\n');
				}
				text = text.TrimStart();
			}
			stringBuilder.Append(text);
			num += text.Length;
		}
		return stringBuilder.ToString();
	}

	private static string[] Explode(string str, char[] splitChars)
	{
		List<string> list = new List<string>();
		int num = 0;
		while (true)
		{
			int num2 = str.IndexOfAny(splitChars, num);
			if (num2 == -1)
			{
				break;
			}
			string text = str.Substring(num, num2 - num);
			char c = str.Substring(num2, 1)[0];
			if (char.IsWhiteSpace(c))
			{
				list.Add(text);
				list.Add(c.ToString());
			}
			else
			{
				list.Add(text + c);
			}
			num = num2 + 1;
		}
		list.Add(str.Substring(num));
		return list.ToArray();
	}
}
