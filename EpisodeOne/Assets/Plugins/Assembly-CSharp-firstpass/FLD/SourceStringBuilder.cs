using System.Text;

namespace FLD
{
	public class SourceStringBuilder
	{
		private const char IndentCharacter = '\t';

		private int _indentLevel;

		private StringBuilder _builder = new StringBuilder();

		private int _pendingIndents;

		public SourceStringBuilder Indent()
		{
			_indentLevel++;
			if (_pendingIndents < _indentLevel)
			{
				_pendingIndents = _indentLevel;
			}
			return this;
		}

		public SourceStringBuilder Unindent()
		{
			_indentLevel--;
			if (_pendingIndents > _indentLevel)
			{
				_pendingIndents = _indentLevel;
			}
			return this;
		}

		public SourceStringBuilder OpenBlock()
		{
			return AppendLine(" {").Indent();
		}

		public SourceStringBuilder CloseBlock()
		{
			return Unindent().AppendLine("}");
		}

		private void AppendPendingIndents()
		{
			while (_pendingIndents-- > 0)
			{
				_builder.Append('\t');
			}
		}

		public SourceStringBuilder AppendLine()
		{
			AppendPendingIndents();
			_builder.AppendLine();
			_pendingIndents = _indentLevel;
			return this;
		}

		public SourceStringBuilder AppendLine(string line)
		{
			AppendPendingIndents();
			_builder.AppendLine(line);
			_pendingIndents = _indentLevel;
			return this;
		}

		public SourceStringBuilder Append(string txt)
		{
			AppendPendingIndents();
			if (!string.IsNullOrEmpty(txt))
			{
				_builder.Append(txt);
				if (txt.IndexOf('\n') >= 0)
				{
					_pendingIndents = _indentLevel;
				}
			}
			return this;
		}

		public SourceStringBuilder Append(char c)
		{
			AppendPendingIndents();
			_builder.Append(c);
			if (c == '\n')
			{
				_pendingIndents = _indentLevel;
			}
			return this;
		}

		public SourceStringBuilder Append(int txt)
		{
			AppendPendingIndents();
			_builder.Append(txt);
			return this;
		}

		public SourceStringBuilder Append(double txt)
		{
			AppendPendingIndents();
			_builder.Append(txt);
			return this;
		}

		public SourceStringBuilder AppendFormat(string txt, params object[] args)
		{
			AppendPendingIndents();
			_builder.AppendFormat(txt, args);
			if (txt.IndexOf('\n') >= 0)
			{
				_pendingIndents = _indentLevel;
			}
			return this;
		}

		public SourceStringBuilder AppendFormat(string txt, object arg0)
		{
			AppendPendingIndents();
			_builder.AppendFormat(txt, arg0);
			if (txt.IndexOf('\n') >= 0)
			{
				_pendingIndents = _indentLevel;
			}
			return this;
		}

		public override string ToString()
		{
			return _builder.ToString();
		}
	}
}
