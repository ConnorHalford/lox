using System.Collections.Generic;

public class LoxFunction : LoxCallable
{
	private Stmt.Function _declaration = null;
	private Environment _closure = null;

	public LoxFunction(Stmt.Function declaration, Environment closure)
	{
		_declaration = declaration;
		_closure = closure;
	}

	public int Arity()
	{
		return _declaration.Parameters.Count;
	}

	public object Call(Interpreter interpreter, List<object> arguments)
	{
		Environment environment = new Environment(_closure);
		int numParams = _declaration.Parameters.Count;
		for (int i = 0; i < numParams; ++i)
		{
			environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
		}
		try
		{
			interpreter.ExecuteBlock(_declaration.Body, environment);
		}
		catch (Return returnValue)
		{
			return returnValue.Value;
		}
		return null;
	}

	public override string ToString()
	{
		return $"<fn {_declaration.Name.Lexeme}>";
	}
}
