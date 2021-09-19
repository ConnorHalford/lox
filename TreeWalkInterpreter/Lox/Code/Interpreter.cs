using System.Collections.Generic;

using static TokenType;

public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
{
	private Environment _globals = new Environment();
	private Environment _environment = null;
	private Dictionary<Expr, int> _locals = new Dictionary<Expr, int>();

	public Interpreter()
	{
		_environment = _globals;
		_globals.Define("clock", new Clock());
	}

	public void Interpret(List<Stmt> statements)
	{
		try
		{
			int numStatements = statements.Count;
			for (int i = 0; i < numStatements; ++i)
			{
				Execute(statements[i]);
			}
		}
		catch (RuntimeError error)
		{
			Lox.RuntimeError(error);
		}
	}

	public object VisitCallExpr(Expr.Call expr)
	{
		object callee = Evaluate(expr.Callee);

		int numArguments = expr.Arguments.Count;
		List<object> arguments = new List<object>(numArguments);
		for (int i = 0; i < numArguments; ++i)
		{
			arguments.Add(Evaluate(expr.Arguments[i]));
		}

		LoxCallable function = (LoxCallable)callee;
		if (function == null)
		{
			throw new RuntimeError(expr.Parenthesis, "Can only call functions and classes");
		}
		if (arguments.Count != function.Arity())
		{
			throw new RuntimeError(expr.Parenthesis, $"Expected {function.Arity()} arguments but got {arguments.Count}");
		}

		return function.Call(this, arguments);
	}

	public object VisitGetExpr(Expr.Get expr)
	{
		object instance = Evaluate(expr.Instance);
		if (instance is LoxInstance loxInstance)
		{
			return loxInstance.Get(expr.Name);
		}
		throw new RuntimeError(expr.Name, "Only instances have properties");
	}

	public object VisitGroupingExpr(Expr.Grouping expr)
	{
		return Evaluate(expr.Expression);
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

	public object VisitVariableExpr(Expr.Variable expr)
	{
		return LookUpVariable(expr.Name, expr);
	}

	public object VisitLiteralExpr(Expr.Literal expr)
	{
		return expr.Value;
	}

	public object VisitAssignExpr(Expr.Assign expr)
	{
		object value = Evaluate(expr.Value);

		if (_locals.TryGetValue(expr, out int distance))
		{
			_environment.AssignAt(distance, expr.Name, value);
		}
		else
		{
			_globals.Assign(expr.Name, value);
		}

		return value;
	}

	public object VisitLogicalExpr(Expr.Logical expr)
	{
		object left = Evaluate(expr.Left);

		if (expr.Operation.Type == TokenType.OR)
		{
			if (IsTruthy(left))
			{
				return left;
			}
		}
		else
		{
			if (!IsTruthy(left))
			{
				return left;
			}
		}

		return Evaluate(expr.Right);
	}

	public object VisitSetExpr(Expr.Set expr)
	{
		object instance = Evaluate(expr.Instance);
		if (!(instance is LoxInstance loxInstance))
		{
			throw new RuntimeError(expr.Name, "Only instances have fields");
		}
		object value = Evaluate(expr.Value);
		loxInstance.Set(expr.Name, value);
		return value;
	}

	public object VisitSuperExpr(Expr.Super expr)
	{
		int distance = _locals[expr];
		LoxClass superclass = (LoxClass)_environment.GetAt(distance, "super");
		LoxInstance instance = (LoxInstance)_environment.GetAt(distance - 1, "this");

		LoxFunction method = superclass.FindMethod(expr.Method.Lexeme);
		if (method == null)
		{
			throw new RuntimeError(expr.Method, $"Undefined property '{expr.Method.Lexeme}'");
		}

		return method.Bind(instance);
	}

	public object VisitThisExpr(Expr.This expr)
	{
		return LookUpVariable(expr.Keyword, expr);
	}

	private object LookUpVariable(Token name, Expr expr)
	{
		if (_locals.TryGetValue(expr, out int distance))
		{
			return _environment.GetAt(distance, name.Lexeme);
		}
		return _globals.Get(name);
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

	public void Execute(Stmt stmt)
	{
		stmt.Accept(this);
	}

	public void ExecuteBlock(List<Stmt> statements, Environment environment)
	{
		Environment previous = _environment;
		try
		{
			_environment = environment;
			int numStatements = statements.Count;
			for (int i = 0; i < numStatements; ++i)
			{
				Execute(statements[i]);
			}
		}
		finally
		{
			_environment = previous;
		}
	}

	public void Resolve(Expr expr, int depth)
	{
		_locals.Add(expr, depth);
	}

	public object VisitExpressionStmt(Stmt.Expression stmt)
	{
		Evaluate(stmt.Expr);
		return null;
	}

	public object VisitFunctionStmt(Stmt.Function stmt)
	{
		LoxFunction function = new LoxFunction(stmt, _environment, false);
		_environment.Define(stmt.Name.Lexeme, function);
		return null;
	}

	public object VisitReturnStmt(Stmt.Return stmt)
	{
		object value = null;
		if (stmt.Value != null)
		{
			value = Evaluate(stmt.Value);
		}
		throw new Return(value);
	}

	public object VisitIfStmt(Stmt.If stmt)
	{
		if (IsTruthy(Evaluate(stmt.Condition)))
		{
			Execute(stmt.ThenBranch);
		}
		else if (stmt.ElseBranch != null)
		{
			Execute(stmt.ElseBranch);
		}
		return null;
	}

	public object VisitWhileStmt(Stmt.While stmt)
	{
		while (IsTruthy(Evaluate(stmt.Condition)))
		{
			Execute(stmt.Body);
		}
		return null;
	}

	public object VisitPrintStmt(Stmt.Print stmt)
	{
		object value = Evaluate(stmt.Expr);
		System.Console.WriteLine(Stringify(value));
		return null;
	}

	public object VisitBlockStmt(Stmt.Block stmt)
	{
		ExecuteBlock(stmt.Statements, new Environment(_environment));
		return null;
	}

	public object VisitClassStmt(Stmt.Class stmt)
	{
		object superclass = null;
		if (stmt.Superclass != null)
		{
			superclass = Evaluate(stmt.Superclass);
			if (!(superclass is LoxClass))
			{
				throw new RuntimeError(stmt.Superclass.Name, "Superclass must be a class");
			}
		}

		_environment.Define(stmt.Name.Lexeme, null);

		if (stmt.Superclass != null)
		{
			_environment = new Environment(_environment);
			_environment.Define("super", superclass);
		}

		int numMethods = stmt.Methods.Count;
		Dictionary<string, LoxFunction> methods = new Dictionary<string, LoxFunction>(numMethods);
		for (int i = 0; i < numMethods; ++i)
		{
			bool isInitializer = stmt.Methods[i].Name.Lexeme.Equals("Init", System.StringComparison.Ordinal);
			LoxFunction function = new LoxFunction(stmt.Methods[i], _environment, isInitializer);
			methods.Add(stmt.Methods[i].Name.Lexeme, function);
		}

		LoxClass loxClass = new LoxClass(stmt.Name.Lexeme, (LoxClass)superclass, methods);

		if (superclass != null)
		{
			_environment = _environment.Enclosing;
		}

		_environment.Assign(stmt.Name, loxClass);

		return null;
	}

	public object VisitVarStmt(Stmt.Var stmt)
	{
		object value = null;
		if (stmt.Initializer != null)
		{
			value = Evaluate(stmt.Initializer);
		}
		_environment.Define(stmt.Name.Lexeme, value);
		return null;
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
