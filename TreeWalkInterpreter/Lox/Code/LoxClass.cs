using System.Collections.Generic;

public class LoxClass : LoxCallable
{
	public string Name = null;
	public LoxClass Superclass = null;
	private Dictionary<string, LoxFunction> _methods = null;

	public LoxClass(string name, LoxClass superclass, Dictionary<string, LoxFunction> methods)
	{
		Name = name;
		Superclass = superclass;
		_methods = methods;
	}

	public LoxFunction FindMethod(string name)
	{
		if (_methods.TryGetValue(name, out LoxFunction method))
		{
			return method;
		}
		if (Superclass != null)
		{
			return Superclass.FindMethod(name);
		}
		return null;
	}

	public object Call(Interpreter interpreter, List<object> arguments)
	{
		LoxInstance instance = new LoxInstance(this);
		LoxFunction initializer = FindMethod("Init");
		if (initializer != null)
		{
			initializer.Bind(instance).Call(interpreter, arguments);
		}
		return instance;
	}

	public int Arity()
	{
		LoxFunction initializer = FindMethod("Init");
		if (initializer != null)
		{
			return initializer.Arity();
		}
		return 0;
	}

	public override string ToString()
	{
		return Name;
	}
}
