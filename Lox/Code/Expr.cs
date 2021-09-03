using System.Collections.Generic;

// NOTE: This file is automatically generated by the GenerateAST tool
public abstract class Expr
{
	public interface Visitor<T>
	{
		T VisitBinaryExpr(Binary expr);
		T VisitGroupingExpr(Grouping expr);
		T VisitLiteralExpr(Literal expr);
		T VisitUnaryExpr(Unary expr);
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

	public abstract T Accept<T>(Visitor<T> visitor);
}
