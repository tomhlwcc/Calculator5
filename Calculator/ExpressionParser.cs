using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calculator
{
	static class ExpressionParser
	{
		// https://lucianamurimi.hashnode.dev/infix-to-postfix-conversion

		public enum NodeType
		{
			Invalid,
			Operand,
			Operator,
			Function
		}

		public enum MathOp
		{
			Invalid,
			OpenParen,
			CloseParen,
			Add,
			Subtract,
			Multiply,
			Divide,
			Mod,
			Factorial,
			Power,

			Comma,
			Abs,
			Cos,
			Degrees,
			Exp,
			Log,
			Log10,
			Radians,
			Round,
			Sin,
			Sqrt,
			Tan,
			Trunc,
		}

		public struct MathOpInfo
		{
			public int Precedence;
			public NodeType Type;
			public int OperandCount;

			public MathOpInfo(int precedence, NodeType type, int opCount)
			{
				Precedence = precedence;
				Type = type;
				OperandCount = opCount;
			}
		}

		public struct MathNode
		{
			public NodeType Type;
			public MathOp Operator;
			public double Operand;


			public MathNode(NodeType type, MathOp mathOp, double operand)
			{
				Type = type;
				Operator = mathOp;
				Operand = operand;
			}

			public override string ToString()
			{
				switch (Type)
				{
					case NodeType.Operand: return $"Operand:  {Operand}";
					case NodeType.Operator: return $"Operator: {Operator}";
				}
				return "";
			}
		}

		public static string ExpressionString;
		public static List<MathNode> InfixList;
		public static List<MathNode> PostfixList;
		private static Dictionary<MathOp, MathOpInfo> OpInfoMap;

		static ExpressionParser()
		{
			OpInfoMap = new Dictionary<MathOp, MathOpInfo>()
			{
				// MathType, MathOpInfo(precedence, nodeType, argumentCount)

				{ MathOp.Add,           new MathOpInfo(  8, NodeType.Operator, 2) },
				{ MathOp.Subtract,      new MathOpInfo(  8, NodeType.Operator, 2) },
				{ MathOp.Multiply,      new MathOpInfo(  9, NodeType.Operator, 2) },
				{ MathOp.Divide,        new MathOpInfo(  9, NodeType.Operator, 2) },
				{ MathOp.Mod,           new MathOpInfo(  9, NodeType.Operator, 2) },
				{ MathOp.Power,         new MathOpInfo( 10, NodeType.Operator, 2) },
				{ MathOp.Factorial,     new MathOpInfo( 10, NodeType.Operator, 2) },
				{ MathOp.OpenParen,     new MathOpInfo(  0, NodeType.Operator, 0) },
				{ MathOp.CloseParen,    new MathOpInfo(  0, NodeType.Operator, 0) },

				// functions
				//{MathOp.Abs,            new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Cos,            new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Degrees,        new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Exp,            new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Log,            new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Log10,          new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Radians,        new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Round,          new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Sin,            new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Sqrt,           new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Tan,            new MathOpInfo( 99,  NodeType.Function,  1) },
				//{MathOp.Trunc,          new MathOpInfo( 99,  NodeType.Function,  1) },

				// date/time
				//{MathOp.Hour,           new MathOpInfo( 99,  NodeType.Function,  0) },
				//{MathOp.Minute,         new MathOpInfo( 99,  NodeType.Function,  0) },
				//{MathOp.Second,         new MathOpInfo( 99,  NodeType.Function,  0) },
				//{MathOp.Year,           new MathOpInfo( 99,  NodeType.Function,  0) },
				//{MathOp.Month,          new MathOpInfo( 99,  NodeType.Function,  0) },
				//{MathOp.Day,            new MathOpInfo( 99,  NodeType.Function,  0) },
				//{MathOp.DayOfWeek,      new MathOpInfo( 99,  NodeType.Function,  0) },

				//// bit-wise
				//{MathOp.shiftRt,		new MathOpInfo( 7,	NodeType.Operator,	2},
				//{MathOp.shiftLf,		new MathOpInfo( 7,	NodeType.Operator,	2},
				//{MathOp.rotateRt,		new MathOpInfo( 7,	NodeType.Operator,	2},
				//{MathOp.rotateLf,		new MathOpInfo( 7,	NodeType.Operator,	2},
				//{MathOp.bitwiseNot,		new MathOpInfo(10,	NodeType.Operator,	1},
				//{MathOp.bitwiseAnd,		new MathOpInfo( 4,	NodeType.Operator,	2},
				//{MathOp.nand,			new MathOpInfo( 4,	NodeType.Operator,	2},
				//{MathOp.bitwiseOr,		new MathOpInfo( 3,	NodeType.Operator,	2},
				//{MathOp.bitwiseXor,		new MathOpInfo( 2,	NodeType.Operator,	2},

				//// comparison
				//{MathOp.greaterThan,	new MathOpInfo( 6,	NodeType.Operator,	2},
				//{MathOp.lessThan,		new MathOpInfo( 6,	NodeType.Operator,	2},
				//{MathOp.gteq,			new MathOpInfo( 6,	NodeType.Operator,	2},
				//{MathOp.lteq,			new MathOpInfo( 6,	NodeType.Operator,	2},
				//{MathOp.equality,		new MathOpInfo( 6,	NodeType.Operator,	2},
				//{MathOp.inequality,		new MathOpInfo( 6,	NodeType.Operator,	2},
 
				//// string
				//{MathOp.chr,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.str,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.asc,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.val,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.find,			new MathOpInfo(99,	NodeType.Function,	2},
				//{MathOp.findi,			new MathOpInfo(99,	NodeType.Function,	2},
				//{MathOp.len,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.left,			new MathOpInfo(99,	NodeType.Function,	2},
				//{MathOp.right,			new MathOpInfo(99,	NodeType.Function,	2},
				//{MathOp.mid,			new MathOpInfo(99,	NodeType.Function,	2},
				//{MathOp.lcase,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.ucase,			new MathOpInfo(99,	NodeType.Function,	1},

				//{MathOp.unaryPlus,		new MathOpInfo(11,	NodeType.Operator,	1},
				//{MathOp.unaryMinus,		new MathOpInfo(11,	NodeType.Operator,	1},
				//{MathOp.unaryNot,		new MathOpInfo( 5,	NodeType.Operator,	1},
				//{MathOp.divAndTrunc,	new MathOpInfo( 9,	NodeType.Operator,	2},
				//{MathOp.assignment,		new MathOpInfo( 1,	NodeType.Operator,	2},
				//{MathOp.noOp,			new MathOpInfo( 0,	NodeType.Operator,	0},
				//{MathOp.bin,			new MathOpInfo(99,	NodeType.Function,	1},
				//{MathOp.hex,			new MathOpInfo(99,	NodeType.Function,	1},

				//{ MathOp.keyPress,		new MathOpInfo(99,  NodeType.Function,  0},
			};
		}

		public static void Parse(string expressionString)
		{
			ExpressionString = expressionString;
			GetInfixList(expressionString);
			GetPostfixList();
		}

		// Splits the expression string into individual fields, each field containing
		//	either an operand or an operator.
		//
		// Returns a List<string>
		//
		public static List<string> GetInfixList(string expressionString)
		{
			var fields = new List<string>();
			while (expressionString.Length > 0)
			{
				var nextField = GetNextField(ref expressionString);
				fields.Add(nextField);
			}

			InfixList = new List<MathNode>();
			foreach (var field in fields)
			{
				MathNode node;
				switch (field.ToLower())
				{
					case "(": node = new MathNode(NodeType.Operator, MathOp.OpenParen,	0); break;
					case ")": node = new MathNode(NodeType.Operator, MathOp.CloseParen, 0); break;
					case "+": node = new MathNode(NodeType.Operator, MathOp.Add,		0); break;
					case "-": node = new MathNode(NodeType.Operator, MathOp.Subtract,	0); break;
					case "*": node = new MathNode(NodeType.Operator, MathOp.Multiply,	0); break;
					case "/": node = new MathNode(NodeType.Operator, MathOp.Divide,		0); break;
					case "%": node = new MathNode(NodeType.Operator, MathOp.Mod,		0); break;
					case "!": node = new MathNode(NodeType.Operator, MathOp.Factorial,	0); break;
					case "^": node = new MathNode(NodeType.Operator, MathOp.Power,		0); break;

					case "pi": node = new MathNode(NodeType.Operand, MathOp.Invalid, Math.PI); break;
					case "e": node  = new MathNode(NodeType.Operand, MathOp.Invalid, Math.E);  break;

					default:
						if (double.TryParse(field, out var value))
							node = new MathNode(NodeType.Operand, MathOp.Invalid, value);
						else
							node = new MathNode(NodeType.Operand, MathOp.Invalid, double.NaN);
						break;
				}
				InfixList.Add(node);
			}

			// Negate operands that are preceded by a unary minus sign
			ProcessUnaryOperators(fields);

			return fields;
		}

		// Postfix rules
		//	1.	While there are tokens to be read:
		//	2.	  Read a token
		//	3.    If it's a number add it to queue
		//	4.    If it's an operator
		//	5.      While there's an operator on the top of the stack with greater precedence:
		//	6.        Pop operators from the stack onto the output queue
		//	7.      Push the current operator onto the stack
		//	8.    If it's a left bracket push it onto the stack
		//	9.    If it's a right bracket 
		// 10.      While there's not a left bracket at the top of the stack:
		// 11.        Pop operators from the stack onto the output queue.
		// 12.      Pop the left bracket from the stack and discard it
		// 13. While there are operators on the stack, pop them to the queue	
		public static List<MathNode> GetPostfixList()
		{
			PostfixList = new List<MathNode>();
			var opStack = new Stack<MathNode>();

			foreach (var node in InfixList)
			{
				switch (node.Type)
				{
					case NodeType.Operand:
						PostfixList.Add(node);
						break;

					case NodeType.Operator:
						switch (node.Operator)
						{
							case MathOp.OpenParen:
								opStack.Push(node);
								break;

							case MathOp.CloseParen:
								while (opStack.Peek().Operator != MathOp.OpenParen)
								{
									PostfixList.Add(opStack.Pop());
								}
								opStack.Pop();
								break;

							default:
								while ((opStack.Count > 0) && OpInfoMap[opStack.Peek().Operator].Precedence >=
															  OpInfoMap[node.Operator].Precedence)
								{
									PostfixList.Add(opStack.Pop());
								}
								opStack.Push(node);
								break;
						}
						break;
				}
			}

			while (opStack.Count > 0)
			{
				PostfixList.Add(opStack.Pop());
			}

			return PostfixList;
		}

		// This routine returns the first operand or operator found in a string.
		//	The operand or operator string is returned and the string is
		//	modified by removing the found operand or operator.
		//	
		private static string GetNextField(ref string expressionString)
		{
			string field = "";
			int index = 0;

			// trim leading whitespace
			expressionString = expressionString.TrimStart();

			index = 0;

			switch (expressionString[0])
			{
				// ToDo: Add case statements to see if the 1st character is an operator.
				//	If so, set 'field' to the operator and remove the first character
				//	from expressionString
				case '+':
				case '-':
				case '*':
				case '/':
				case '%':
				case '!':
					field = expressionString.Substring(0, 1);
					expressionString = expressionString.Remove(0, 1);
					return field;

				default:
					// ToDo: Add tests to call ParseNumber if the 1st character of
					// 	expressionString is a digit or a decimal point; or call 
					// 	ParseWord if the 1st character is a letter. Set the variable 
					//	'field' to value returned by ParseNumber or ParseWord.
					if (char.IsDigit(expressionString[index]) || expressionString[index] == '.')
						return ParseNumber(ref expressionString);
					else if (char.IsLetter(expressionString[index]))
						return ParseWord(ref expressionString);

					field = expressionString[index].ToString();
					expressionString = expressionString.Remove(0, 1);

					return field;
			}
		}

		private static string ParseNumber(ref string expressionString)
		{
			// valid number formats
			//	int		12345
			//	double	12345.67
			//			1234e-34	(e or E; + or -)
			int index = 0;
			while (index < expressionString.Length)
			{
				// ToDo: This loop should increment index until it points to a character
				//	that is not part of a double number. Remember that doubles can include
				//	a decimal point and/or exponent in the form of an E followed by a
				//	+ or -, followed by another series of digits. See the 3rd example above.
				//	The double.TryParse routine	can handle either form.
				//
				// Hint:	
				//	If expressionString[index]
				//	  is a digit or a decimal point, increment index
				//	  is E or e, increment index twice to skip over the following + or -
				//	  none of the above, break from the loop
				//	
				if (char.IsDigit(expressionString[index]) || // digit
					expressionString[index] == '.')          // float decimal point
				{
					index++;
				}
				else if (expressionString[index] == 'e' || expressionString[index] == 'E')
				{
					index++;
					index++;    // next char should be the + or -
				}
				else
				{
					break;
				}
			}

			// At this point, the index should point to the first character that is not 
			//	part of the real number. The Substring method is used to set 'field' to 
			//	to the characters in the expressionString that are part of the number.
			//  The Remove method is used to remove the proper number characters from the
			//	beginning of the string.
			string field = expressionString.Substring(0, index);
			expressionString = expressionString.Remove(0, index);

			return field;
		}

		private static string ParseWord(ref string expressionString)
		{
			// ToDo: This loop should increment index until expressionString[index} 
			//	is either whitespace or a digit
			//
			int index = 0;
			while (index < expressionString.Length)
			{
				if (!char.IsLetter(expressionString[index]))
					break;

				index++;
			}

			string field = expressionString.Substring(0, index);
			expressionString = expressionString.Remove(0, index);
			return field;
		}

		// A minus sign can either be the subtract operator or an unary '-' meant
		//	to negated the operator value that follows.
		//
		// A minus sign is unary if
		//	the field after is an operand &&
		//	(the field before is an operator || the minus is first field).
		//
		// For all unary minus signs in the field list, a minus sign is prepended to
		//	the operand field and the unary field is removed.
		//
		private static void ProcessUnaryOperators(List<string> fields)
		{
			for (var i = fields.Count - 1; i > 0; i--)
			{
				// look for constant node preceded by unary node
				if (IsOperand(fields[i]) && IsUnaryOperator(fields[i - 1]) &&
					(i == 1 || IsOperator(fields[i - 2])))
				{
					// put unary operator in front of operand
					fields[i] = fields[i - 1] + fields[i];

					// delete unary node
					i--;
					fields.RemoveAt(i);
				}
			}

			for (var i = InfixList.Count - 1; i > 0; i--)
			{
				var node = InfixList[i];
				// look for operand node preceded by unary node
				if (IsOperand(node) &&
					IsUnaryOperator(InfixList[i - 1]) &&
					(i == 1 || IsOperator(InfixList[i - 2])))
				{
					// negate operand
					node.Operand = -node.Operand;

					// delete unary node
					i--;
					InfixList.RemoveAt(i);
				}
			}
		}

		private static bool IsOperand(string field)
		{
			return char.IsDigit(field[0]) ||
				   field.ToLower() == "pi" ||
				   field.ToLower() == "e";
		}

		private static bool IsOperator(string field)
		{
			return "+-*/%!".IndexOf(field) >= 0;
		}

		private static bool IsFunction(string field)
		{
			return "+-*/%!".IndexOf(field) >= 0;
		}

		private static bool IsUnaryOperator(string field)
		{
			return field == "-";
		}

		private static bool IsOperand(MathNode node)		{ return node.Type == NodeType.Operand; }
		private static bool IsOperator(MathNode node)		{ return node.Type == NodeType.Operator; }
		private static bool IsUnaryOperator(MathNode node)	{ return node.Operator == MathOp.Subtract; }

	}
}
