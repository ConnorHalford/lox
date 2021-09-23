using System.Collections.Generic;

// NOTE: This file is automatically generated by the GenerateAST tool
public abstract class Expr
{
	public interface Visitor<T>
	{
		T VisitCallExpr(Call expr);
		T VisitGetExpr(Get expr);
		T VisitGroupingExpr(Grouping expr);
		T VisitBinaryExpr(Binary expr);
		T VisitUnaryExpr(Unary expr);
		T VisitLogicalExpr(Logical expr);
		T VisitSetExpr(Set expr);
		T VisitSuperExpr(Super expr);
		T VisitThisExpr(This expr);
		T VisitLiteralExpr(Literal expr);
		T VisitVariableExpr(Variable expr);
		T VisitAssignExpr(Assign expr);
	}

	public class Call : Expr
	{
		public Call(Expr callee, Token parenthesis, List<Expr> arguments)
		{
			this.Callee = callee;
			this.Parenthesis = parenthesis;
			this.Arguments = arguments;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitCallExpr(this);
		}

		public Expr Callee;
		public Token Parenthesis;
		public List<Expr> Arguments;
	}

	public class Get : Expr
	{
		public Get(Expr instance, Token name)
		{
			this.Instance = instance;
			this.Name = name;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitGetExpr(this);
		}

		public Expr Instance;
		public Token Name;
	}

	public class Grouping : Expr
	{
		public Grouping(Expr expression)
		{
			this.Expression = expression;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitGroupingExpr(this);
		}

		public Expr Expression;
	}

	public class Binary : Expr
	{
		public Binary(Expr left, Token operation, Expr right)
		{
			this.Left = left;
			this.Operation = operation;
			this.Right = right;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitBinaryExpr(this);
		}

		public Expr Left;
		public Token Operation;
		public Expr Right;
	}

	public class Unary : Expr
	{
		public Unary(Token operation, Expr right)
		{
			this.Operation = operation;
			this.Right = right;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitUnaryExpr(this);
		}

		public Token Operation;
		public Expr Right;
	}

	public class Logical : Expr
	{
		public Logical(Expr left, Token operation, Expr right)
		{
			this.Left = left;
			this.Operation = operation;
			this.Right = right;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitLogicalExpr(this);
		}

		public Expr Left;
		public Token Operation;
		public Expr Right;
	}

	public class Set : Expr
	{
		public Set(Expr instance, Token name, Expr value)
		{
			this.Instance = instance;
			this.Name = name;
			this.Value = value;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitSetExpr(this);
		}

		public Expr Instance;
		public Token Name;
		public Expr Value;
	}

	public class Super : Expr
	{
		public Super(Token keyword, Token method)
		{
			this.Keyword = keyword;
			this.Method = method;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitSuperExpr(this);
		}

		public Token Keyword;
		public Token Method;
	}

	public class This : Expr
	{
		public This(Token keyword)
		{
			this.Keyword = keyword;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitThisExpr(this);
		}

		public Token Keyword;
	}

	public class Literal : Expr
	{
		public Literal(object value)
		{
			this.Value = value;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitLiteralExpr(this);
		}

		public object Value;
	}

	public class Variable : Expr
	{
		public Variable(Token name)
		{
			this.Name = name;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitVariableExpr(this);
		}

		public Token Name;
	}

	public class Assign : Expr
	{
		public Assign(Token name, Expr value)
		{
			this.Name = name;
			this.Value = value;
		}

		public override T Accept<T>(Visitor<T> visitor)
		{
			return visitor.VisitAssignExpr(this);
		}

		public Token Name;
		public Expr Value;
	}

	public abstract T Accept<T>(Visitor<T> visitor);
}