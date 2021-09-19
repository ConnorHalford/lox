using System.Collections.Generic;

public class Clock : LoxCallable
{
	public int Arity()
	{
		return 0;
	}

	public object Call(Interpreter interpreter, List<object> arguments)
	{
		return (double)System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
	}

	public override string ToString()
	{
		return "<native fn>";
	}
}