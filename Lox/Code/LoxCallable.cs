using System.Collections.Generic;

public interface LoxCallable
{
	int Arity();
	object Call(Interpreter interpreter, List<object> arguments);
}