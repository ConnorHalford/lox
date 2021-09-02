using System.Collections.Generic;

using static TokenType;

public class Scanner
{
	private string _source = null;
	private List<Token> _tokens = new List<Token>();
	private int _start = 0;
	private int _current = 0;
	private int _line = 1;

	private static Dictionary<string, TokenType> _keywords = new Dictionary<string, TokenType>()
	{
		{ "and",	AND },
		{ "class",	CLASS },
		{ "else",	ELSE },
		{ "false",	FALSE },
		{ "fun",	FUN },
		{ "for",	FOR },
		{ "if",		IF },
		{ "nil",	NIL },
		{ "or",		OR },
		{ "print",	PRINT },
		{ "return",	RETURN },
		{ "super",	SUPER },
		{ "this",	THIS },
		{ "true",	TRUE },
		{ "var",	VAR },
		{ "while",	WHILE }
	};

	public Scanner(string source)
	{
		_source = source;
	}

	public List<Token> ScanTokens()
	{
		while (!IsAtEnd())
		{
			// We are at the beginning of the next lexeme
			_start = _current;
			ScanToken();
		}

		_tokens.Add(new Token(EOF, null, null, _line));
		return _tokens;
	}

	private void ScanToken()
	{
		char c = Advance();
		switch (c)
		{
			case '(':	AddToken(LEFT_PAREN);	break;
			case ')':	AddToken(RIGHT_PAREN);	break;
			case '{':	AddToken(LEFT_BRACE);	break;
			case '}':	AddToken(RIGHT_BRACE);	break;
			case ',':	AddToken(COMMA);		break;
			case '.':	AddToken(DOT);			break;
			case '-':	AddToken(MINUS);		break;
			case '+':	AddToken(PLUS);			break;
			case ';':	AddToken(SEMICOLON);	break;
			case '*':	AddToken(STAR);			break;

			case '!':
				AddToken(Match('=') ? BANG_EQUAL : BANG);
				break;
			case '=':
				AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
				break;
			case '<':
				AddToken(Match('=') ? LESS_EQUAL : LESS);
				break;
			case '>':
				AddToken(Match('=') ? GREATER_EQUAL : GREATER);
				break;

			case '/':
				if (Match('/'))
				{
					// A comment goes until the end of the line
					while (Peek() != '\n' && !IsAtEnd())
					{
						Advance();
					}
				}
				else
				{
					AddToken(SLASH);
				}
				break;

			case ' ':
			case '\r':
			case '\t':
				// Ignore whitespace
				break;

			case '\n':
				++_line;
				break;

			case '"':
				String();
				break;

			default:
				if (IsDigit(c))
				{
					Number();
				}
				else if (IsAlpha(c))
				{
					Identifier();
				}
				else
				{
					Lox.Error(_line, "Unexpected character");
				}
				break;
		}
	}

	private void Identifier()
	{
		while (IsAlphaNumeric(Peek()))
		{
			Advance();
		}

		string text = _source.Substring(_start, _current - _start);
		if (!_keywords.TryGetValue(text, out TokenType type))
		{
			type = IDENTIFIER;
		}

		AddToken(type);
	}

	private void Number()
	{
		while (IsDigit(Peek()))
		{
			Advance();
		}

		// Look for a fractional part
		if (Peek() == '.' && IsDigit(PeekNext()))
		{
			// Consume the .
			Advance();
		}

		while (IsDigit(Peek()))
		{
			Advance();
		}

		AddToken(NUMBER, double.Parse(_source.Substring(_start, _current - _start)));
	}

	private void String()
	{
		while (Peek() != '"' && !IsAtEnd())
		{
			if (Peek() == '\n')
			{
				++_line;
			}
			Advance();
		}

		if (IsAtEnd())
		{
			Lox.Error(_line, "Unterminated string");
			return;
		}

		Advance();	// The closing "

		// Trim the surrounding quotes
		string value = _source.Substring(_start + 1, _current - _start - 2);
		AddToken(STRING, value);
	}

	private bool Match(char expected)
	{
		if (IsAtEnd())
		{
			return false;
		}
		if (_source[_current] != expected)
		{
			return false;
		}
		++_current;
		return true;
	}

	private char Peek()
	{
		if (IsAtEnd())
		{
			return '\0';
		}
		return _source[_current];
	}

	private char PeekNext()
	{
		if (_current + 1 >= _source.Length)
		{
			return '\0';
		}
		return _source[_current + 1];
	}

	private bool IsAlpha(char c)
	{
		return (c >= 'a' && c <= 'z')
			|| (c >= 'A' && c <= 'Z')
			|| c == '_';
	}

	private bool IsAlphaNumeric(char c)
	{
		return IsAlpha(c) || IsDigit(c);
	}

	private bool IsDigit(char c)
	{
		return c >= '0' && c <= '9';
	}

	private bool IsAtEnd()
	{
		return _current >= _source.Length;
	}

	private char Advance()
	{
		return _source[_current++];
	}

	private void AddToken(TokenType type)
	{
		AddToken(type, null);
	}

	private void AddToken(TokenType type, object literal)
	{
		string text = _source.Substring(_start, _current - _start);
		_tokens.Add(new Token(type, text, literal, _line));
	}
}
