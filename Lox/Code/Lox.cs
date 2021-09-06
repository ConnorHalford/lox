﻿using System;
using System.Collections.Generic;
using System.IO;

public class Lox
{
	private static Interpreter _interpreter = new Interpreter();
	private static bool _hadError = false;
	private static bool _hadRuntimeError = false;

	public static void Main(string[] args)
	{
		int numArgs = args == null ? 0 : args.Length;
		if (numArgs > 1)
		{
			Console.WriteLine("Usage: lox [script]");
			Environment.Exit(64);
		}
		else if (numArgs == 1)
		{
			RunFile(args[0]);
		}
		else
		{
			RunPrompt();
		}
	}

	private static void RunFile(string path)
	{
		string source = File.ReadAllText(path, System.Text.Encoding.ASCII);
		Run(source);
		if (_hadError)
		{
			Environment.Exit(65);
		}
		if (_hadRuntimeError)
		{
			Environment.Exit(70);
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
		List<Stmt> statements = parser.Parse();

		// Stop if there was a syntax error
		if (_hadError)
		{
			return;
		}

		_interpreter.Interpret(statements);
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

	public static void RuntimeError(RuntimeError error)
	{
		Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
		_hadRuntimeError = true;
	}

	public static void Report(int line, string where, string message)
	{
		Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
		_hadError = true;
	}
}
