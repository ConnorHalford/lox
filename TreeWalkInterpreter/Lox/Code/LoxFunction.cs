using System.Collections.Generic;

public class LoxFunction : LoxCallable
{
	private Stmt.Function _declaration = null;
	private Environment _closure = null;
	private bool _isInitializer = false;

	public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
	{
		_declaration = declaration;
		_closure = closure;
		_isInitializer = isInitializer;
	}

	public LoxFunction Bind(LoxInstance instance)
	{
		Environment environment = new Environment(_closure);
		environment.Define("this", instance);
		return new LoxFunction(_declaration, environment, _isInitializer);
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
			if (_isInitializer)
			{
				return _closure.GetAt(0, "this");
			}
			return returnValue.Value;
		}
		if (_isInitializer)
		{
			return _closure.GetAt(0, "this");
		}
		return null;
	}

	public override string ToString()
	{
		return $"<fn {_declaration.Name.Lexeme}>";
	}
}
