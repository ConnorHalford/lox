using System.Collections.Generic;

public class Resolver : Expr.Visitor<object>, Stmt.Visitor<object>
{
	private enum FunctionType
	{
		None,
		Function,
		Method,
		Initializer
	}

	private enum ClassType
	{
		None,
		Class,
		Subclass
	}

	private Interpreter _interpreter = null;
	private List<Dictionary<string, bool>> _scopes = new List<Dictionary<string, bool>>();
	private FunctionType _currentFunction = FunctionType.None;
	private ClassType _currentClass = ClassType.None;

	public Resolver(Interpreter interpreter)
	{
		_interpreter = interpreter;
	}

	public void Resolve(List<Stmt> statements)
	{
		int numStatements = statements.Count;
		for (int i = 0; i < numStatements; ++i)
		{
			Resolve(statements[i]);
		}
	}

	private void Resolve(Stmt stmt)
	{
		stmt.Accept(this);
	}

	private void Resolve(Expr expr)
	{
		expr.Accept(this);
	}

	private void ResolveLocal(Expr expr, Token name)
	{
		int numScopes = _scopes.Count;
		for (int i = numScopes - 1; i >= 0; --i)
		{
			if (_scopes[i].ContainsKey(name.Lexeme))
			{
				_interpreter.Resolve(expr, numScopes - 1 - i);
				return;
			}
		}
	}

	private void ResolveFunction(Stmt.Function function, FunctionType type)
	{
		FunctionType enclosing = _currentFunction;
		_currentFunction = type;

		BeginScope();
		int numParameters = function.Parameters.Count;
		for (int i = 0; i < numParameters; ++i)
		{
			Declare(function.Parameters[i]);
			Define(function.Parameters[i]);
		}
		Resolve(function.Body);
		EndScope();

		_currentFunction = enclosing;
	}

	private void BeginScope()
	{
		_scopes.Add(new Dictionary<string, bool>());
	}

	private void EndScope()
	{
		_scopes.RemoveAt(_scopes.Count - 1);
	}

	private void Declare(Token name)
	{
		int numScopes = _scopes.Count;
		if (numScopes == 0)
		{
			return;
		}
		Dictionary<string, bool> scope = _scopes[numScopes - 1];
		if (scope.ContainsKey(name.Lexeme))
		{
			Lox.Error(name, "Already a variable with this name in this scope");
		}
		else
		{
			scope.Add(name.Lexeme, false);
		}
	}

	private void Define(Token name)
	{
		int numScopes = _scopes.Count;
		if (numScopes == 0)
		{
			return;
		}
		_scopes[numScopes - 1][name.Lexeme] = true;
	}

	public object VisitBlockStmt(Stmt.Block stmt)
	{
		BeginScope();
		Resolve(stmt.Statements);
		EndScope();
		return null;
	}

	public object VisitClassStmt(Stmt.Class stmt)
	{
		ClassType enclosingClass = _currentClass;
		_currentClass = ClassType.Class;

		Declare(stmt.Name);
		Define(stmt.Name);

		if (stmt.Superclass != null)
		{
			if (stmt.Name.Lexeme.Equals(stmt.Superclass.Name.Lexeme, System.StringComparison.Ordinal))
			{
				Lox.Error(stmt.Superclass.Name, "A class can't inherit from itself");
			}

			_currentClass = ClassType.Subclass;
			Resolve(stmt.Superclass);

			BeginScope();
			_scopes[_scopes.Count - 1].Add("super", true);
		}

		BeginScope();
		_scopes[_scopes.Count - 1].Add("this", true);

		int numMethods = stmt.Methods.Count;
		for (int i = 0; i < numMethods; ++i)
		{
			FunctionType declaration = FunctionType.Method;
			if (stmt.Methods[i].Name.Lexeme.Equals("Init", System.StringComparison.Ordinal))
			{
				declaration = FunctionType.Initializer;
			}
			ResolveFunction(stmt.Methods[i], declaration);
		}

		EndScope();

		if (stmt.Superclass != null)
		{
			EndScope();
		}

		_currentClass = enclosingClass;
		return null;
	}

	public object VisitFunctionStmt(Stmt.Function stmt)
	{
		Declare(stmt.Name);
		Define(stmt.Name);
		ResolveFunction(stmt, FunctionType.Function);
		return null;
	}

	public object VisitVarStmt(Stmt.Var stmt)
	{
		Declare(stmt.Name);
		if (stmt.Initializer != null)
		{
			Resolve(stmt.Initializer);
		}
		Define(stmt.Name);
		return null;
	}

	public object VisitExpressionStmt(Stmt.Expression stmt)
	{
		Resolve(stmt.Expr);
		return null;
	}

	public object VisitIfStmt(Stmt.If stmt)
	{
		Resolve(stmt.Condition);
		Resolve(stmt.ThenBranch);
		if (stmt.ElseBranch != null)
		{
			Resolve(stmt.ElseBranch);
		}
		return null;
	}

	public object VisitPrintStmt(Stmt.Print stmt)
	{
		Resolve(stmt.Expr);
		return null;
	}

	public object VisitReturnStmt(Stmt.Return stmt)
	{
		if (_currentFunction == FunctionType.None)
		{
			Lox.Error(stmt.Keyword, "Can't return from top-level code");
		}
		if (stmt.Value != null)
		{
			if (_currentFunction == FunctionType.Initializer)
			{
				Lox.Error(stmt.Keyword, "Can't return a value from an initializer");
			}
			Resolve(stmt.Value);
		}
		return null;
	}

	public object VisitWhileStmt(Stmt.While stmt)
	{
		Resolve(stmt.Condition);
		Resolve(stmt.Body);
		return null;
	}

	public object VisitAssignExpr(Expr.Assign expr)
	{
		Resolve(expr.Value);
		ResolveLocal(expr, expr.Name);
		return null;
	}

	public object VisitVariableExpr(Expr.Variable expr)
	{
		int numScopes = _scopes.Count;
		if (numScopes > 0
			&& _scopes[numScopes - 1].TryGetValue(expr.Name.Lexeme, out bool initialized)
			&& !initialized)
		{
			Lox.Error(expr.Name, "Can't read local variable in its own initializer");
		}
		ResolveLocal(expr, expr.Name);
		return null;
	}

	public object VisitBinaryExpr(Expr.Binary expr)
	{
		Resolve(expr.Left);
		Resolve(expr.Right);
		return null;
	}

	public object VisitCallExpr(Expr.Call expr)
	{
		Resolve(expr.Callee);
		int numArguments = expr.Arguments.Count;
		for (int i = 0; i < numArguments; ++i)
		{
			Resolve(expr.Arguments[i]);
		}
		return null;
	}

	public object VisitGetExpr(Expr.Get expr)
	{
		Resolve(expr.Instance);
		return null;
	}

	public object VisitGroupingExpr(Expr.Grouping expr)
	{
		Resolve(expr.Expression);
		return null;
	}

	public object VisitLiteralExpr(Expr.Literal expr)
	{
		return null;
	}

	public object VisitLogicalExpr(Expr.Logical expr)
	{
		Resolve(expr.Left);
		Resolve(expr.Right);
		return null;
	}

	public object VisitSetExpr(Expr.Set expr)
	{
		Resolve(expr.Value);
		Resolve(expr.Instance);
		return null;
	}

	public object VisitSuperExpr(Expr.Super expr)
	{
		if (_currentClass == ClassType.None)
		{
			Lox.Error(expr.Keyword, "Can't use 'super' outside of a class");
		}
		else if (_currentClass != ClassType.Subclass)
		{
			Lox.Error(expr.Keyword, "Can't use 'super' in a class with no superclass");
		}

		ResolveLocal(expr, expr.Keyword);
		return null;
	}

	public object VisitThisExpr(Expr.This expr)
	{
		if (_currentClass == ClassType.None)
		{
			Lox.Error(expr.Keyword, "Can't use 'this' outside of a class");
			return null;
		}
		ResolveLocal(expr, expr.Keyword);
		return null;
	}

	public object VisitUnaryExpr(Expr.Unary expr)
	{
		Resolve(expr.Right);
		return null;
	}
}
