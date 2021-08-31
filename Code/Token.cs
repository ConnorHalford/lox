public class Token
{
	private TokenType _type = default;
	private string _lexeme = null;
	private object _literal = null;
	private int _line = -1;

	public Token(TokenType type, string lexeme, object literal, int line)
	{
		_type = type;
		_lexeme = lexeme;
		_literal = literal;
		_line = line;
	}

	public override string ToString()
	{
		return $"{_type} {_lexeme} {_literal}";
	}
}