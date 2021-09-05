using static TokenType;

public class Interpreter : Expr.Visitor<object>
{
	public void Interpret(Expr expression)
	{
		try
		{
			object value = Evaluate(expression);
			System.Console.WriteLine(Stringify(value));
		}
		catch (RuntimeError error)
		{
			Lox.RuntimeError(error);
		}
	}

	public object VisitLiteralExpr(Expr.Literal expr)
	{
		return expr.Value;
	}

	public object VisitGroupingExpr(Expr.Grouping expr)
	{
		return Evaluate(expr.Expression);
	}

	public object VisitUnaryExpr(Expr.Unary expr)
	{
		object right = Evaluate(expr.Right);
		switch (expr.Operation.Type)
		{
			case BANG:
				return !IsTruthy(right);

			case MINUS:
				CheckNumberOperand(expr.Operation, right);
				return -(double)right;
		}
		return null;	// Unreachable
	}

	public object VisitBinaryExpr(Expr.Binary expr)
	{
		object left = Evaluate(expr.Left);
		object right = Evaluate(expr.Right);

		switch (expr.Operation.Type)
		{
			case GREATER:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left > (double)right;

			case GREATER_EQUAL:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left >= (double)right;

			case LESS:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left < (double)right;

			case LESS_EQUAL:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left <= (double)right;

			case BANG_EQUAL:
				return !IsEqual(left, right);

			case EQUAL_EQUAL:
				return IsEqual(left, right);

			case MINUS:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left - (double)right;

			case SLASH:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left / (double)right;

			case STAR:
				CheckNumberOperands(expr.Operation, left, right);
				return (double)left * (double)right;

			case PLUS:
				if (left is double dLeft && right is double dRight)
				{
					return dLeft + dRight;
				}
				if (left is string sLeft && right is string sRight)
				{
					return sLeft + sRight;
				}
				throw new RuntimeError(expr.Operation, "Operands must be two numbers or two strings");
		}
		return null;	// Unreachable
	}

	private void CheckNumberOperand(Token operation, object operand)
	{
		if (operand is double)
		{
			return;
		}
		throw new RuntimeError(operation, "Operand must be a number");
	}

	private void CheckNumberOperands(Token operation, object left, object right)
	{
		if (left is double && right is double)
		{
			return;
		}
		throw new RuntimeError(operation, "Operands must be numbers");
	}

	private object Evaluate(Expr expr)
	{
		return expr.Accept(this);
	}

	private bool IsTruthy(object value)
	{
		if (value == null)
		{
			return false;
		}
		if (value is bool boolean)
		{
			return boolean;
		}
		return true;
	}

	private bool IsEqual(object a, object b)
	{
		if (a == null && b == null)
		{
			return true;
		}
		if (a == null)
		{
			return false;
		}
		return a.Equals(b);
	}

	private string Stringify(object value)
	{
		if (value == null)
		{
			return "nil";
		}
		if (value is double number)
		{
			string text = number.ToString();
			if (text.EndsWith(".0"))
			{
				text = text.Substring(0, text.Length - 2);
			}
			return text;
		}
		return value.ToString();
	}
}
