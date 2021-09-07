using System.Collections.Generic;

using static TokenType;

public class Parser
{
	private class ParseError : System.Exception
	{ }

	private List<Token> _tokens = new List<Token>();
	private int _current = 0;

	public Parser(List<Token> tokens)
	{
		_tokens = tokens;
	}

	public List<Stmt> Parse()
	{
		List<Stmt> statements = new List<Stmt>();
		while (!IsAtEnd())
		{
			statements.Add(Declaration());
		}
		return statements;
	}

	private Stmt Declaration()
	{
		try
		{
			if (Match(VAR))
			{
				return VarDeclaration();
			}
			return Statement();
		}
		catch (ParseError)
		{
			Synchronize();
			return null;
		}
	}

	private Stmt VarDeclaration()
	{
		Token name = Consume(IDENTIFIER, "Expect variable name");

		Expr initializer = null;
		if (Match(EQUAL))
		{
			initializer = Expression();
		}

		Consume(SEMICOLON, "Expect ';' after variable declaration");
		return new Stmt.Var(name, initializer);
	}

	private Stmt Statement()
	{
		if (Match(PRINT))
		{
			return PrintStatement();
		}
		if (Match(LEFT_BRACE))
		{
			return new Stmt.Block(Block());
		}
		return ExpressionStatement();
	}

	private Stmt PrintStatement()
	{
		Expr value = Expression();
		Consume(SEMICOLON, "Expect ';' after value");
		return new Stmt.Print(value);
	}

	private List<Stmt> Block()
	{
		List<Stmt> statements = new List<Stmt>();

		while (!Check(RIGHT_BRACE) && !IsAtEnd())
		{
			statements.Add(Declaration());
		}

		Consume(RIGHT_BRACE, "Expect '}' after block");
		return statements;
	}

	private Stmt ExpressionStatement()
	{
		Expr expr = Expression();
		Consume(SEMICOLON, "Expect ';' after expression");
		return new Stmt.Expression(expr);
	}

	private Expr Expression()
	{
		return Assignment();
	}

	private Expr Assignment()
	{
		Expr expr = Equality();

		if (Match(EQUAL))
		{
			Token equals = Previous();
			Expr value = Assignment();

			if (expr is Expr.Variable variable)
			{
				Token name = variable.Name;
				return new Expr.Assign(name, value);
			}

			Error(equals, "Invalid assignment target");
		}

		return expr;
	}

	private Expr Equality()
	{
		Expr expr = Comparison();
		while (Match(BANG_EQUAL, EQUAL_EQUAL))
		{
			Token operation = Previous();
			Expr right = Comparison();
			expr = new Expr.Binary(expr, operation, right);
		}
		return expr;
	}

	private Expr Comparison()
	{
		Expr expr = Term();
		while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
		{
			Token operation = Previous();
			Expr right = Term();
			expr = new Expr.Binary(expr, operation, right);
		}
		return expr;
	}

	private Expr Term()
	{
		Expr expr = Factor();
		while (Match(MINUS, PLUS))
		{
			Token operation = Previous();
			Expr right = Factor();
			expr = new Expr.Binary(expr, operation, right);
		}
		return expr;
	}

	private Expr Factor()
	{
		Expr expr = Unary();
		while (Match(SLASH, STAR))
		{
			Token operation = Previous();
			Expr right = Unary();
			expr = new Expr.Binary(expr, operation, right);
		}
		return expr;
	}

	private Expr Unary()
	{
		if (Match(BANG, MINUS))
		{
			Token operation = Previous();
			Expr right = Unary();
			return new Expr.Unary(operation, right);
		}
		return Primary();
	}

	private Expr Primary()
	{
		if (Match(FALSE))
		{
			return new Expr.Literal(false);
		}
		if (Match(TRUE))
		{
			return new Expr.Literal(true);
		}
		if (Match(NIL))
		{
			return new Expr.Literal(null);
		}
		if (Match(NUMBER, STRING))
		{
			return new Expr.Literal(Previous().Literal);
		}
		if (Match(IDENTIFIER))
		{
			return new Expr.Variable(Previous());
		}
		if (Match(LEFT_PAREN))
		{
			Expr expr = Expression();
			Consume(RIGHT_PAREN, "Expect ')' after expression");
			return new Expr.Grouping(expr);
		}
		throw Error(Peek(), "Expect expression");
	}

	private bool Match(params TokenType[] types)
	{
		int numTypes = types.Length;
		for (int i = 0; i < numTypes; ++i)
		{
			if (Check(types[i]))
			{
				Advance();
				return true;
			}
		}
		return false;
	}

	private Token Consume(TokenType type, string message)
	{
		if (Check(type))
		{
			return Advance();
		}
		throw Error(Peek(), message);
	}

	private bool Check(TokenType type)
	{
		if (IsAtEnd())
		{
			return false;
		}
		return Peek().Type == type;
	}

	private Token Advance()
	{
		if (!IsAtEnd())
		{
			++_current;
		}
		return Previous();
	}

	private bool IsAtEnd()
	{
		return Peek().Type == EOF;
	}

	private Token Peek()
	{
		return _tokens[_current];
	}

	private Token Previous()
	{
		return _tokens[_current - 1];
	}

	private ParseError Error(Token token, string message)
	{
		Lox.Error(token, message);
		return new ParseError();
	}

	private void Synchronize()
	{
		Advance();

		while (!IsAtEnd())
		{
			if (Previous().Type == SEMICOLON)
			{
				return;
			}

			switch (Peek().Type)
			{
				case CLASS:
				case FOR:
				case FUN:
				case IF:
				case PRINT:
				case RETURN:
				case VAR:
				case WHILE:
					return;
			}

			Advance();
		}
	}
}
