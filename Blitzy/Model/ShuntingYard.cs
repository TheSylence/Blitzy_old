// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Utility;

namespace Blitzy.Model
{
	internal class ShuntingYard
	{
		#region Constructor

		public ShuntingYard()
		{
			OpMap.Add( "+", new AddOperator() );
			OpMap.Add( "-", new SubOperator() );
			OpMap.Add( "*", new MulOperator() );
			OpMap.Add( "/", new DivOperator() );
			OpMap.Add( "sqrt", new SqrtOperator() );
			OpMap.Add( "log", new LogOperator() );
			OpMap.Add( "ln", new LnOperator() );
			OpMap.Add( "sin", new SinOperator() );
			OpMap.Add( "asin", new AsinOperator() );
			OpMap.Add( "acos", new AcosOperator() );
			OpMap.Add( "atan", new AtanOperator() );
			OpMap.Add( "cos", new CosOperator() );
			OpMap.Add( "tan", new TanOperator() );
			OpMap.Add( "!", new FacOperator() );
			OpMap.Add( "rnd", new RoundOperator() );
			OpMap.Add( "pow", new PowOperator() );
			OpMap.Add( "^", new PowOperator() );
		}

		#endregion Constructor

		#region Methods

		#region Helper

		private static int GetOperatorPrecedence( string str )
		{
			switch( str )
			{
				case "!":
					return 4;

				case "*":
				case "/":
				case "%":
					return 3;

				case "+":
				case "-":
					return 2;
				//case "=":
				//    return 1;
			}
			return 0;
		}

		private static bool IsFunctionSeparator( string str )
		{
			return str.Trim().Equals( "," );
		}

		private static bool IsLeftAssoc( string str )
		{
			switch( str )
			{
				// left to right
				case "*":
				case "/":
				case "%":
				case "+":
				case "-":
				case "^":
					return true;
				// right to left
				case "!":
					return false;
			}
			return false;
		}

		private static bool IsLeftParanthesis( string str )
		{
			return str.Trim().Equals( "(" );
		}

		private static bool IsNumber( string str )
		{
			double result;
			return double.TryParse( str, out result );
		}

		private static bool IsRightParanthesis( string str )
		{
			return str.Trim().Equals( ")" );
		}

		private int GetOperatorArguments( string str )
		{
			if( OpMap.ContainsKey( str ) )
				return OpMap[str].GetArgumentCount();

			return 0;
		}

		private bool IsFunction( string str )
		{
			return Functions.Contains( str.ToLowerInvariant() );
		}

		private bool IsOperator( string str )
		{
			return Operators.Contains( str.Trim() );
		}

		#endregion Helper

		public string Calculate( string input )
		{
			Valid = true;

			List<string> tokens = Parse( input );
			Queue<string> tokenQueue = ConvertToRPN( tokens );

			if( !Valid )
			{
				return "SyntaxError".Localize();
			}

			if( tokenQueue.Count == 0 )
			{
				return "EmptyExpression".Localize();
			}

			string res = Compute( tokenQueue );
			double num;
			if( double.TryParse( res, NumberStyles.Any, CultureInfo.InvariantCulture, out num ) )
			{
				return num.ToString( "G", CultureInfo.InvariantCulture );
			}

			if( res.Equals( "IncompleteExpression".Localize() ) )
			{
				return "IncompleteExpression".Localize();
			}

			return "SyntaxError".Localize();
		}

		private string Compute( Queue<string> tokens )
		{
			Stack<string> result = new Stack<string>();

			while( tokens.Count > 0 )
			{
				string token = tokens.Dequeue();

				if( IsNumber( token ) )
				{
					result.Push( token );
				}
				else
				{
					int argCount = GetOperatorArguments( token );

					if( argCount > result.Count )
					{
						return "IncompleteExpression".Localize();
					}

					List<string> args = new List<string>();
					for( int i = 0; i < argCount; ++i )
					{
						args.Add( result.Pop() );
					}

					result.Push( Compute( token, args ) );
				}
			}

			return result.Pop();
		}

		private string Compute( string operation, List<string> args )
		{
			if( OpMap.ContainsKey( operation ) )
				return OpMap[operation].Compute( args );

			return string.Empty;
		}

		private Queue<string> ConvertToRPN( List<string> tokens )
		{
			Queue<string> output = new Queue<string>();
			Stack<string> operators = new Stack<string>();

			foreach( string token in tokens )
			{
				// If the token is a number, then add it to the output queue.
				if( IsNumber( token ) )
				{
					output.Enqueue( token );
				}
				// If the token is a function token, then push it onto the stack.
				else if( IsFunction( token ) )
				{
					operators.Push( token );
				}
				// If the token is a function argument separator (e.g., a comma):
				else if( IsFunctionSeparator( token ) )
				{
					// Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
					while( operators.Count > 0 && !IsLeftParanthesis( operators.Peek() ) )
						output.Enqueue( operators.Pop() );

					// If no left parentheses are encountered, either the separator was misplaced or parentheses were mismatched.
					if( operators.Count == 0 )
					{
						Valid = false;
						return null;
					}
				}
				// If the token is an operator, o1, then:
				else if( IsOperator( token ) )
				{
					// while there is an operator token, o2, at the top of the stack, and
					while( operators.Count > 0 )
					{
						string o2 = operators.Peek();

						// either o1 is left-associative and its precedence is less than or equal to that of o2,
						// or o1 is right-associative and its precedence is less than that of o2,
						if( ( IsLeftAssoc( token ) && GetOperatorPrecedence( token ) <= GetOperatorPrecedence( o2 ) ) ||
							( !IsLeftAssoc( token ) && GetOperatorPrecedence( token ) < GetOperatorPrecedence( o2 ) ) )
						{
							// pop o2 off the stack, onto the output queue;
							output.Enqueue( operators.Pop() );
						}
						else
							break;
					}

					// push o1 onto the stack.
					operators.Push( token );
				}
				// If the token is a left parenthesis, then push it onto the stack.
				else if( IsLeftParanthesis( token ) )
				{
					operators.Push( token );
				}
				// If the token is a right parenthesis:
				else if( IsRightParanthesis( token ) )
				{
					// Until the token at the top of the stack is a left parenthesis, pop operators off the stack onto the output queue.
					while( operators.Count > 0 && !IsLeftParanthesis( operators.Peek() ) )
					{
						output.Enqueue( operators.Pop() );
					}

					// If the stack runs out without finding a left parenthesis, then there are mismatched parentheses.
					if( operators.Count == 0 )
					{
						Valid = false;
						return null;
					}

					// Pop the left parenthesis from the stack, but not onto the output queue.
					if( IsLeftParanthesis( operators.Peek() ) )
					{
						operators.Pop();
					}

					// If the token at the top of the stack is a function token, pop it onto the output queue.
					if( operators.Count > 0 && IsFunction( operators.Peek() ) )
					{
						output.Enqueue( operators.Pop() );
					}
				}
			}

			// When there are no more tokens to read:

			// While there are still operator tokens in the stack:
			while( operators.Count > 0 )
			{
				// If the operator token on the top of the stack is a parenthesis, then there are mismatched parentheses.
				if( IsRightParanthesis( operators.Peek() ) || IsLeftParanthesis( operators.Peek() ) )
				{
					Valid = false;
					return null;
				}

				// Pop the operator onto the output queue.
				output.Enqueue( operators.Pop() );
			}

			return output;
		}

		private List<string> Parse( string input )
		{
			if( !string.IsNullOrWhiteSpace( input ) )
			{
				input = input.ToLowerInvariant().Trim();

				// FIXME: This is a real bad workaround for negative numbers...
				input = input.Replace( " ", "" ).Replace( "(-", "(0-" ).Replace( ",-", ",0-" );
				if( input[0] == '-' )
				{
					input = "0" + input;
				}

				// Ensure that operators are separated by spaces
				foreach( string search in Operators.Concat( new[] { "(", ")", "," } ) )
				{
					string replace = " " + search + " ";
					input = input.Replace( search, replace );
				}

				List<string> tokens = new List<string>( input.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ) );

				// Replace constants
				for( int i = 0; i < tokens.Count; ++i )
				{
					string token = tokens[i].Trim();

					if( Replacements.ContainsKey( token ) )
					{
						token = Replacements[token];
					}

					tokens[i] = token;
				}

				return tokens;
			}
			else
			{
				return new List<string>();
			}
		}

		#endregion Methods

		#region Constants

		private string[] Functions = new[] { "cos", "sin", "tan", "acos", "asin", "atan", "sqrt", "ln", "log", "rnd", "pow" };
		private string[] Operators = new[] { "+", "-", "*", "/", "%", "!", "^" };
		private Dictionary<string, IOperator> OpMap = new Dictionary<string, IOperator>();

		private Dictionary<string, string> Replacements = new Dictionary<string, string>
		{
			{ "pi", Math.PI.ToString(CultureInfo.InvariantCulture) },
			{ "e", Math.E.ToString(CultureInfo.InvariantCulture) }
		};

		#endregion Constants

		#region Attributes

		private bool Valid = true;

		#endregion Attributes
	}
}