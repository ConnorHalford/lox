using System.Collections.Generic;

public class LoxInstance
{
	private LoxClass _loxClass = null;
	private Dictionary<string, object> _fields = new Dictionary<string, object>();

	public LoxInstance(LoxClass loxClass)
	{
		_loxClass = loxClass;
	}

	public object Get(Token name)
	{
		if (_fields.TryGetValue(name.Lexeme, out object value))
		{
			return value;
		}
		LoxFunction method = _loxClass.FindMethod(name.Lexeme);
		if (method != null)
		{
			return method.Bind(this);
		}
		throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'");
	}

	public void Set(Token name, object value)
	{
		_fields.Add(name.Lexeme, value);
	}

	public override string ToString()
	{
		return $"{_loxClass.Name} instance";
	}
}
