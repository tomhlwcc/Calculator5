//// Week 10 Assignment, Calculator5
//
//	Name:

// Ask user if they want to evaluate expressions that are stored in
//	a file or allow the user to enter expressions. A valid expression
//	contains two operands separated by an operator. The operands 
//	can either be a floating point constant, in standard or exponential 
//	form, or they can be the words "PI" or "E". Any operand can be 
//	negated by preceding it with a minus sign.
//
// If an expression string does not parse correctly, the string
//	$"{expression} is invalid!" should be returned as the result.
//
// The expression strings may or may not have spaces separating
//	its components. Leading, trailing, and multiple internal
//	spaces are allowed.
//
// The solution includes a Expression.txt file which contains a
//	set of expressions used to test your program. Your program 
//	should write the results to the Results.txt file.,

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Expression;

namespace Calculator5
{
	class Calculator5
	{
		static Dictionary<ExpressionParser.MathOp, string> MathOp2String = new Dictionary<Expression.ExpressionParser.MathOp, string>
		{
			{ ExpressionParser.MathOp.Add,		" +" },
			{ ExpressionParser.MathOp.Subtract,	" -" },
			{ ExpressionParser.MathOp.Multiply,	" *" },
			{ ExpressionParser.MathOp.Divide,	" /" },
			{ ExpressionParser.MathOp.Mod,		" %" },
			{ ExpressionParser.MathOp.Factorial,"!"  },
			{ ExpressionParser.MathOp.Power,	" ^" },
		};


		static void Main(string[] args)
		{
			Console.Write("Enter Y if you wish to evaluate expression in a file! ");
			var key = Console.ReadKey().KeyChar;
			Console.WriteLine("");

			if (key == 'y' || key== 'Y')
				EvaluateExpressionsInFile();

			EvaluateUserEnteredExpressions();
		}


		static void EvaluateUserEnteredExpressions()
		{
			while (true)
			{
				Console.Write("Enter expression or 'Q' to quit: ");

				var expressionString = Console.ReadLine();
				if (expressionString.ToLower() == "q")
					Environment.Exit(0);

				EvaluateExpression(expressionString, out string result);
				Console.WriteLine($"{expressionString} = {result}");
			}
		}

		static void EvaluateExpressionsInFile()
		{
			GetFilePaths(out var expressionFile, out var resultFile);
			var expressionArray = File.ReadAllLines(expressionFile);
			var resultList = new List<string>();

			foreach (var expression in expressionArray)
			{
				if (expression.StartsWith(";"))
					continue;

				if (expression == "")
				{
					Console.WriteLine("Expression string is empty!");
					resultList.Add($"Expression string is empty!");
					continue;
				}

				EvaluateExpression(expression, out string result);
				resultList.Add($"{result}");
				Console.WriteLine($"{expression} = {result}");
			}

			File.WriteAllLines(resultFile, resultList);

			Console.WriteLine($"The results of {resultList.Count} expressions have been written to file.");
			Console.WriteLine("Press any key to continue.");
			Console.ReadKey();
		}


		static bool EvaluateExpression(string expressionString, out string result)
		{
			result = "";

			// 1. Call the ExpressionParser.TryParse method to parse the expression string.
			//		If the parse fails, print out the ErrorMessage and the index where the
			//		error occurred.
			// Make sure to handle an empty expression string!!


			// 2. Create a stack for the operands and use the ExpressionParser.PostfixList to 
			//		evaluate the expression. Use the DoBinaryOperator or ComputeFactorial depending
			//		on the operators found in the PostfixList



			// 3. Pop the final result off the stack and put the value in result. The stack has doubles
			//		on it but result is a string. You can use an Interpolated string to convert the 
			//		double to a string, or use the ToString() method.

			
			return true;
		}

		static double DoBinaryOperator(double value1, double value2, Expression.ExpressionParser.MathOp mathOp)
		{
			switch (mathOp)
			{
				case ExpressionParser.MathOp.Add:		return value1 + value2;
				case ExpressionParser.MathOp.Subtract:	return value1 - value2;
				case ExpressionParser.MathOp.Multiply:	return value1 * value2;
				case ExpressionParser.MathOp.Divide:	return value1 / value2;
				case ExpressionParser.MathOp.Mod:		return value1 % value2;
			}
			
			return double.NaN;
		}

		static double ComputeFactorial(double value)
		{
			value = Math.Truncate(value);
			if (value < 0)
				return double.NaN;

			double factorial = 1;
			for (var i = 2; i <= value; i++)
			{
				factorial *= i;
			}

			return factorial;
		}


		static void GetFilePaths(out string expressionFile, out string resultFile)
		{
			// Mysterious stuff happen here to get the paths to the expressionFile and resultFile
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
			path = path.Substring(0, path.IndexOf("bin"));
			expressionFile = path + "Expressions.txt";
			resultFile = path + "Results.txt";
		}

	}
}
