﻿using System;
using System.Collections.Generic;
using System.IO;

public class GenerateAST
{
	public static int Main(string[] args)
	{
		int numArgs = args == null ? 0 : args.Length;
		if (numArgs != 1)
		{
			Console.WriteLine("Usage: generate_ast <output directory>");
			return -1;
		}

		string outputDir = args[0];
		DefineAST(outputDir, "Expr", new List<string>() {
				"Binary		: Expr Left, Token Operation, Expr Right",
				"Grouping	: Expr Expression",
				"Literal	: object Value",
				"Unary		: Token Operation, Expr Right"
			});

		return 0;
	}

	private static void DefineAST(string outputDir, string baseName, List<string> types)
	{
		string path = $"{outputDir}/{baseName}.cs";
		using (StreamWriter sw = new StreamWriter(path))
		{
			sw.WriteLine("using System.Collections.Generic;");
			sw.WriteLine();
			sw.WriteLine("// NOTE: This file is automatically generated by the GenerateAST tool");
			sw.WriteLine($"public abstract class {baseName}");
			sw.WriteLine("{");

			DefineVisitor(sw, baseName, types);

			// The AST classes
			int numTypes = types.Count;
			for (int i = 0; i < numTypes; ++i)
			{
				string type = types[i];
				string[] parts = type.Split(':', StringSplitOptions.TrimEntries);
				string className = parts[0];
				string fields = parts[1];
				sw.WriteLine();
				DefineType(sw, baseName, className, fields);
			}

			// The base Accept() method
			sw.WriteLine();
			sw.WriteLine("	public abstract T Accept<T>(Visitor<T> visitor);");

			sw.WriteLine("}");
		}
	}

	private static void DefineVisitor(StreamWriter sw, string baseName, List<string> types)
	{
		sw.WriteLine("	public interface Visitor<T>");
		sw.WriteLine("	{");

		string lowerBaseName = baseName.ToLower();
		int numTypes = types.Count;
		for (int i = 0; i < numTypes; ++i)
		{
			string typeName = types[i].Split(':', StringSplitOptions.TrimEntries)[0];
			sw.WriteLine($"		T Visit{typeName}{baseName}({typeName} {lowerBaseName});");
		}

		sw.WriteLine("	}");
	}

	private static void DefineType(StreamWriter sw, string baseName, string className, string fieldList)
	{
		sw.WriteLine($"	public class {className} : {baseName}");
		sw.WriteLine("	{");

		string[] fields = fieldList.Split(',', StringSplitOptions.TrimEntries);
		int numFields = fields.Length;
		string[] fieldTypes = new string[numFields];
		string[] fieldNames = new string[numFields];
		string[] fieldNamesLower = new string[numFields];
		for (int i = 0; i < numFields; ++i)
		{
			string[] field = fields[i].Split(' ');
			fieldTypes[i] = field[0];
			fieldNames[i] = field[1];

			// Convert first letter to lowercase if necessary
			string lower = field[1];
			if (!string.IsNullOrEmpty(lower) && lower[0] >= 'A' && lower[0] <= 'Z')
			{
				lower = (lower.Length == 1) ? lower.ToLower() : $"{lower.Substring(0, 1).ToLower()}{lower.Substring(1)}";
			}
			fieldNamesLower[i] = lower;
		}

		// Constructor
		sw.Write($"		public {className}(");
		for (int i = 0; i < numFields; ++i)
		{
			if (i > 0)
			{
				sw.Write(", ");
			}
			sw.Write(fieldTypes[i]);
			sw.Write(" ");
			sw.Write(fieldNamesLower[i]);
		}
		sw.WriteLine(")");
		sw.WriteLine("		{");

		// Assign parameters to fields
		for (int i = 0; i < numFields; ++i)
		{
			sw.WriteLine($"			this.{fieldNames[i]} = {fieldNamesLower[i]};");
		}

		sw.WriteLine("		}");

		// Visitor pattern
		sw.WriteLine();
		sw.WriteLine("		public override T Accept<T>(Visitor<T> visitor)");
		sw.WriteLine("		{");
		sw.WriteLine($"			return visitor.Visit{className}{baseName}(this);");
		sw.WriteLine("		}");

		// Define fields
		sw.WriteLine();
		for (int i = 0; i < numFields; ++i)
		{
			sw.WriteLine($"		public {fields[i]};");
		}

		sw.WriteLine("	}");
	}
}
