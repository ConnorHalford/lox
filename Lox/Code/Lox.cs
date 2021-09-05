using System;
using System.Collections.Generic;
using System.IO;

public class Lox
{
	private static bool _hadError = false;

	public static int Main(string[] args)
	{
		int numArgs = args == null ? 0 : args.Length;
		if (numArgs > 1)
		{
			Console.WriteLine("Usage: lox [script]");
			return -1;
		}
		else if (numArgs == 1)
		{
			RunFile(args[0]);
		}
		else
		{
			RunPrompt();
		}
		return 0;
	}

	private static void RunFile(string path)
	{
		string source = File.ReadAllText(path, System.Text.Encoding.ASCII);
		Run(source);
		if (_hadError)
		{
			Environment.Exit(-1);
		}
	}

	private static void RunPrompt()
	{
		string line = null;
		while (true)
		{
			Console.Write("> ");
			line = Console.ReadLine();
			if (string.IsNullOrEmpty(line))
			{
				break;
			}
			Run(line);
			_hadError = false;
		}
	}

	private static void Run(string source)
	{
		Scanner scanner = new Scanner(source);
		List<Token> tokens = scanner.ScanTokens();
		Parser parser = new Parser(tokens);
		Expr expression = parser.Parse();

		// Stop if there was a syntax error
		if (_hadError)
		{
			return;
		}

		Console.WriteLine(new ASTPrinter().Print(expression));
	}

	public static void Error(int line, string message)
	{
		Report(line, string.Empty, message);
	}

	public static void Error(Token token, string message)
	{
		if (token.Type == TokenType.EOF)
		{
			Report(token.Line, " at end", message);
		}
		else
		{
			Report(token.Line, $" at '{token.Lexeme}'", message);
		}
	}

	public static void Report(int line, string where, string message)
	{
		Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
		_hadError = true;
	}
}
