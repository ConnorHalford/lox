public class RuntimeError : System.Exception
{
	public Token Token;

	public RuntimeError(Token token, string message)
		: base(message)
	{
		this.Token = token;
	}
}