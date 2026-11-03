using System;
using System.IO;
using System.Text;

namespace FLD
{
	public class TokenStream : IDisposable
	{
		public enum TokenType
		{
			Identifier = 0,
			Digits = 1,
			Punctuation = 2,
			Whitespace = 3
		}

		private TextReader _reader;

		private StringBuilder _parsingToken = new StringBuilder(1024);

		private string _putbackToken;

		private bool _isEof;

		public string NearContext
		{
			get
			{
				return Peek();
			}
		}

		public TokenStream(string pText)
		{
			_reader = new StringReader(pText);
		}

		public TokenStream(Stream pStream)
		{
			_reader = new StreamReader(pStream);
		}

		public void Dispose()
		{
			if (_reader != null)
			{
				_reader.Dispose();
			}
		}

		public string Read()
		{
			string pToken;
			TokenType pType;
			if (!GetNextToken(out pToken, out pType))
			{
				return null;
			}
			return pToken;
		}

		public bool TryRead(out string pToken)
		{
			TokenType pType;
			return GetNextToken(out pToken, out pType);
		}

		public string Peek()
		{
			return _putbackToken = Read();
		}

		public char ReadPunctuationToken()
		{
			SkipWhitespace();
			string text = ReadToken(TokenType.Punctuation);
			if (text != null)
			{
				return text[0];
			}
			return '\0';
		}

		public string ReadToken(TokenType pExpectedType)
		{
			string pToken;
			TokenType pType;
			GetNextToken(out pToken, out pType);
			if (pType != pExpectedType)
			{
				return null;
			}
			return pToken;
		}

		public bool ExpectToken(string pToken)
		{
			string text = Read();
			return text == pToken;
		}

		protected bool GetNextToken(out string pToken, out TokenType pType)
		{
			if (!string.IsNullOrEmpty(_putbackToken))
			{
				pToken = _putbackToken;
				pType = GetTokenTypeOfCharacter(_putbackToken[0]);
				_putbackToken = null;
				return true;
			}
			SkipWhitespace();
			if (_isEof)
			{
				pToken = null;
				pType = TokenType.Digits;
				return false;
			}
			char c = (char)_reader.Read();
			pType = GetTokenTypeOfCharacter(c);
			if (pType == TokenType.Punctuation)
			{
				pToken = c.ToString();
				return true;
			}
			_parsingToken.Length = 0;
			_parsingToken.Append(c);
			while (!_isEof)
			{
				int num = _reader.Peek();
				if (num < 0)
				{
					_isEof = true;
					break;
				}
				c = (char)num;
				if (GetTokenTypeOfCharacter(c) != pType)
				{
					break;
				}
				_parsingToken.Append(c);
				_reader.Read();
			}
			pToken = _parsingToken.ToString();
			return true;
		}

		public char ReadSingleCharacter()
		{
			return (char)_reader.Read();
		}

		public string ReadCharacters(int pCount)
		{
			char[] array = new char[pCount];
			_reader.Read(array, 0, pCount);
			return new string(array);
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
			while (true)
			{
				int num = _reader.Peek();
				if (num < 0)
				{
					_isEof = true;
					break;
				}
				if (!char.IsWhiteSpace((char)num))
				{
					break;
				}
				_reader.Read();
			}
		}
	}
}
