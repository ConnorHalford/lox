using System.Collections.Generic;

public class Environment
{
	private Environment _enclosing = null;
	private Dictionary<string, object> _values = new Dictionary<string, object>();

	public Environment()
	{
		_enclosing = null;
	}

	public Environment(Environment enclosing)
	{
		_enclosing = enclosing;
	}

	public void Define(string name, object value)
	{
		_values.Add(name, value);
	}

	public object Get(Token name)
	{
		if (_values.TryGetValue(name.Lexeme, out object value))
		{
			return value;
		}
		if (_enclosing != null)
		{
			return _enclosing.Get(name);
		}
		throw new RuntimeError(name, $"Undefined variable {name.Lexeme}");
	}

	public void Assign(Token name, object value)
	{
		if (_values.ContainsKey(name.Lexeme))
		{
			_values[name.Lexeme] = value;
			return;
		}
		if (_enclosing != null)
		{
			_enclosing.Assign(name, value);
			return;
		}
		throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'");
	}
}
