using System;
using System.Text;

namespace FLD
{
	public class TextTokenizer
	{
		public enum TokenType
		{
			Identifier = 0,
			Digits = 1,
			Punctuation = 2,
			Whitespace = 3
		}

		private string _text;

		private int _cursor;

		private StringBuilder _parsingToken = new StringBuilder(1024);

		private string _putbackToken;

		public bool IsEndOfStream
		{
			get
			{
				return _cursor >= _text.Length;
			}
		}

		public string NearContext
		{
			get
			{
				return _text.Substring(_cursor - 1, Math.Min(16, _text.Length - _cursor));
			}
		}

		public TextTokenizer(string pText)
		{
			_text = pText;
		}

		public string TakeToken()
		{
			string pToken;
			TokenType pType;
			GetNextToken(out pToken, out pType);
			return pToken;
		}

		public string PeekToken()
		{
			return _putbackToken = TakeToken();
		}

		public char TakePunctuationToken()
		{
			SkipWhitespace();
			return TakeToken(TokenType.Punctuation)[0];
		}

		public string TakeToken(TokenType pExpectedType)
		{
			string pToken;
			TokenType pType;
			GetNextToken(out pToken, out pType);
			if (pType != pExpectedType)
			{
				throw new FormatException(string.Concat("Unexpected token; expected ", pExpectedType, ", got ", pType));
			}
			return pToken;
		}

		public void ExpectToken(string pToken)
		{
			string text = TakeToken();
			if (text != pToken)
			{
				throw new FormatException("Expected \"" + pToken + "\", got \"" + text + "\"");
			}
		}

		protected void GetNextToken(out string pToken, out TokenType pType)
		{
			if (!string.IsNullOrEmpty(_putbackToken))
			{
				pToken = _putbackToken;
				pType = GetTokenTypeOfCharacter(_putbackToken[0]);
				_putbackToken = null;
				return;
			}
			SkipWhitespace();
			if (IsEndOfStream)
			{
				throw new FormatException("End of stream");
			}
			char c = _text[_cursor++];
			pType = GetTokenTypeOfCharacter(c);
			if (pType == TokenType.Punctuation)
			{
				pToken = c.ToString();
				return;
			}
			_parsingToken.Length = 0;
			_parsingToken.Append(c);
			while (!IsEndOfStream)
			{
				c = _text[_cursor];
				if (GetTokenTypeOfCharacter(c) != pType)
				{
					break;
				}
				_parsingToken.Append(c);
				_cursor++;
			}
			pToken = _parsingToken.ToString();
		}

		public char TakeCharacter()
		{
			if (IsEndOfStream)
			{
				throw new FormatException("End of stream");
			}
			return _text[_cursor++];
		}

		public string TakeCharacters(int pCount)
		{
			if (_cursor > _text.Length - pCount)
			{
				throw new FormatException("End of stream");
			}
			string result = _text.Substring(_cursor, pCount);
			_cursor += pCount;
			return result;
		}

		protected TokenType GetTokenTypeOfCharacter(char pChar)
		{
			if (char.IsWhiteSpace(pChar))
			{
				return TokenType.Whitespace;
			}
			if (char.IsDigit(pChar))
			{
				return TokenType.Digits;
			}
			if (char.IsLetter(pChar))
			{
				return TokenType.Identifier;
			}
			return TokenType.Punctuation;
		}

		protected void SkipWhitespace()
		{
			while (!IsEndOfStream && char.IsWhiteSpace(_text[_cursor]))
			{
				_cursor++;
			}
		}
	}
}
