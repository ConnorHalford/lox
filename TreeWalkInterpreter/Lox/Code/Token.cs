public class Token
{
	public TokenType Type = default;
	public string Lexeme = null;
	public object Literal = null;
	public int Line = -1;

	public Token(TokenType type, string lexeme, object literal, int line)
	{
		this.Type = type;
		this.Lexeme = lexeme;
		this.Literal = literal;
		this.Line = line;
	}

	public override string ToString()
	{
		return $"{Type} {Lexeme} {Literal}";
	}
}