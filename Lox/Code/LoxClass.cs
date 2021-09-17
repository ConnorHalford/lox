using System.Collections.Generic;

public class LoxClass : LoxCallable
{
	public string Name = null;
	private Dictionary<string, LoxFunction> _methods = null;

	public LoxClass(string name, Dictionary<string, LoxFunction> methods)
	{
		Name = name;
		_methods = methods;
	}

	public LoxFunction FindMethod(string name)
	{
		if (_methods.TryGetValue(name, out LoxFunction method))
		{
			return method;
		}
		return null;
	}

	public object Call(Interpreter interpreter, List<object> arguments)
	{
		LoxInstance instance = new LoxInstance(this);
		return instance;
	}

	public int Arity()
	{
		return 0;
	}

	public override string ToString()
	{
		return Name;
	}
}
