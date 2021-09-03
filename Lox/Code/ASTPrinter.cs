using System.Text;

public class ASTPrinter : Expr.Visitor<string>
{
	public string Print(Expr expr)
	{
		return expr.Accept(this);
	}

	public string VisitBinaryExpr(Expr.Binary expr)
	{
		return Parenthesize(expr.Operation.Lexeme, expr.Left, expr.Right);
	}

	public string VisitGroupingExpr(Expr.Grouping expr)
	{
		return Parenthesize("group", expr.Expression);
	}

	public string VisitLiteralExpr(Expr.Literal expr)
	{
		return (expr.Value == null) ? "nil" : expr.Value.ToString();
	}

	public string VisitUnaryExpr(Expr.Unary expr)
	{
		return Parenthesize(expr.Operation.Lexeme, expr.Right);
	}

	private string Parenthesize(string name, params Expr[] exprs)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append("(");
		sb.Append(name);
		int numExpr = exprs.Length;
		for (int i = 0; i < numExpr; ++i)
		{
			sb.Append(" ");
			sb.Append(exprs[i].Accept(this));
		}
		sb.Append(")");
		return sb.ToString();
	}
}