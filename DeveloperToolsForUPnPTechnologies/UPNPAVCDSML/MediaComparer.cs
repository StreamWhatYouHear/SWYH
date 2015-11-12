/*   
Copyright 2006 - 2010 Intel Corporation

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Net;
using System.Text;
using System.Collections;
using OpenSource.UPnP.AV;
using System.Text.RegularExpressions;

namespace OpenSource.UPnP.AV.CdsMetadata
{
	/// <summary>
	/// Interface for determining whether a media object
	/// matches a set of comparison parameters.
	/// </summary>
	public interface IMediaComparer
	{
		/// <summary>
		/// Returns true if the object matches the comparer's criteria.
		/// </summary>
		/// <param name="mediaObject"></param>
		/// <returns></returns>
		bool IsMatch(IUPnPMedia mediaObject);
	}

	/// <summary>
	/// <para>
	/// </para>
	/// 
	/// <para>The language
	/// has a formal Extended Backus-Naur Form (EBNF) description below. <b>Bold</b>
	/// faced items are literal and <i>italicized</i> items are defined terms.
	/// <list type="table">
	/// <listheader><term>Defined Terms</term><description>Term Definition</description></listheader>
	/// <item>
	/// <term><i>searchexp</i></term>
	/// <description>
	/// <para><i>relexp</i> |</para>
	/// <para><i>searchexp</i> <i>logop</i> <i>searchexp</i> |</para>
	/// <para><b>(</b> <i>searchexp</i> <b>)</b></para>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>logop</i></term>
	/// <description>
	/// <b>and</b> | <b>or</b>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>relexp</i></term>
	/// <description>
	/// <para><i>property relop delim-value</i> |</para>
	/// <para><i>property stringop delim-value</i> |</para>
	/// <para><i>property existence-op true-false</i> |</para>
	/// <para><i>property derived-op delim-value</i></para>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>relop</i></term>
	/// <description>
	/// <b>=</b> | <b>!=</b> | <b>&lt;</b> | <b>&lt;=</b> | <b>&gt;</b> | <b>&gt;=</b>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>stringop</i></term>
	/// <description>
	/// <b>contains</b> | <b>doesNotContain</b>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>existence-op</i></term>
	/// <description>
	/// <b>exists</b>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>true-false</i></term>
	/// <description>
	/// <b>true</b> | <b>false</b>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>delim-value</i></term>
	/// <description>
	/// <i>dquote escape-value dquote</i>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>dquote</i></term>
	/// <description>
	/// <b>"</b> (remark: double quote character, UTF-8 code 0x22)
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>delim-value</i></term>
	/// <description>
	/// <i>dquote escape-value dquote</i>
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>property</i></term>
	/// <description>
	/// A property name as defined in the ContentDirectory specification.
	/// Property names that are standardized can be obtained from a
	/// <see cref="Tags"/>
	/// object, using the indexer [] operator with a value from the
	/// <see cref="CommonPropertyNames"/>
	/// enumeration. Metadata properties that are attributes of
	/// "item", "container", and "res" elements in ContentDirectory
	/// XML responses can be obtained as static strings from the
	/// <see cref="Tags.PropertyAttributes"/>
	/// class.
	/// </description>
	/// </item>
	/// 
	/// <item>
	/// <term><i>escape-value</i></term>
	/// <description>
	/// A string properly escaped such that a backslash <b>\</b> character
	/// is represented as <b>\\</b> and a double-quote <b>"</b> character
	/// is represented as <b>\"</b>.
	/// </description>
	/// </item>
	/// </list>
	/// </para>
	/// </summary>
	public class MediaComparer : IMediaComparer
	{
		/// <summary>
		/// This indicates if the 
		/// <see cref="IUPnPMedia"/>
		/// object matches the Boolean expression.
		/// </summary>
		/// <param name="entry">the media object that should be compared against the expression</param>
		/// <returns>true, indicates a match against the expression</returns>
		public virtual bool IsMatch(IUPnPMedia entry)
		{
			return this.EvaluateMedia(entry);
		}

		/// <summary>
		/// State machine enumeration for the next
		/// expected token in an expression.
		/// </summary>
		private enum RelExpStates
		{
			unknown,
			expectOp,
			expectValue
		}

		/// <summary>
		/// If true, then all comparisons are case insensitive.
		/// </summary>
		public bool IgnoreCase = true;

		/// <summary>
		/// This constructor takes a Boolean expression in infix format.
		/// Some examples
		/// are shown below. Assume <b>T</b> is an instance of the 
		/// <see cref="Tags"/>
		/// class.
		/// <list type="table">
		/// <item>
		/// T[CommonPropertyNames.title] contains "foo"
		/// <term>
		/// </term>
		/// <description>
		/// <see cref="MediaComparer.IsMatch"/> will return
		/// true on media objects that have <i>foo</i> in the title.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// T[CommonPropertyNames.creator] doesNotContain "\"HappyBand\""
		/// <term>
		/// </term>
		/// <description>
		/// <see cref="MediaComparer.IsMatch"/> will return
		/// true on media objects that do not have <i>"HappyBand"</i> in the title.
		/// </description>
		/// </item>
		/// 
		/// <item>
		/// (T[CommonPropertyNames.Class] = "object.item.audioItem.musicTrack" and Tags.PropertyAttributes.res_size > "40") or (T[CommonPropertyNames.author] exists true)
		/// <term>
		/// </term>
		/// <description>
		/// <see cref="MediaComparer.IsMatch"/> will return
		/// true on media objects that are music tracks with at least one resource greater than 40 bytes
		/// OR on media objects that have a value set for the author metadata.
		/// </description>
		/// </item>
		/// </list>
		/// </summary>
		/// <param name="infix">The boolean infix expression conforming to the syntax and semantics of ContentDirectory's boolean query language.</param>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_MalformedSearchCriteria">
		/// Thrown if the infix expression has a syntax error.
		/// </exception>
		public MediaComparer (string infix)
		{

			string allstring = infix.Trim();
			if ((allstring == "") || (allstring == "*"))
			{
				this.m_AlwaysMatch = true;
				return;
			}

			//Initialize an empty stack and empty result string variable.
			//
			Stack stack = new Stack();
			m_Postfix = new Queue();

			RelExpStates state = RelExpStates.unknown;
			TokenResult token;
			while (infix.Length > 0)
			{
				infix = infix.Trim();
				token = GetToken(ref infix, state);

				switch (state)
				{
					case RelExpStates.unknown:
						if (token.TokenType == Tokens.PropertyName)
						{
							state = RelExpStates.expectOp;
						} 
						break;
					case RelExpStates.expectOp:
						if (token.TokenType != Tokens.Operator)
						{
							throw new UPnPCustomException(402, "Invalid Args: Invalid operator " + token.Data);
						}
						state = RelExpStates.expectValue;
						break;
					case RelExpStates.expectValue:
						if (token.TokenType != Tokens.PropertyValue)
						{
							throw new UPnPCustomException(402, "Invalid Args: Unexpected value " + token.Data);
						}
						state = RelExpStates.unknown;
						break;
				}

				switch (token.TokenType)
				{
					case Tokens.Operator:
						if (token.OpToken == OperatorTokens.LParen)
						{
							//left paren
							//
							stack.Push(token);
						}
						else if (token.OpToken == OperatorTokens.RParen)
						{
							//right paren
							//
							TokenResult tr = new TokenResult(false);
							do
							{
								if (stack.Count > 0)
								{
									tr = (TokenResult) stack.Pop();
									if (tr.OpToken != OperatorTokens.LParen)
									{
										m_Postfix.Enqueue(tr);
									}
								}
								else
								{
									throw new UPnPCustomException(402, "Invalid Args: Missing Left Parenthesis.");
								}
							}
							while (tr.OpToken != OperatorTokens.LParen);
						}
						else
						{
							//standard operator
							//
							if (token.OpToken == OperatorTokens.Invalid)
							{
								throw new Exception("bad code");
							}

							while
								(
								(stack.Count > 0) && 
								( ((TokenResult) stack.Peek()).Precedence >= token.Precedence) && 
								( ((TokenResult) stack.Peek()).OpToken != OperatorTokens.LParen) 
								)
							{
								// While stack is not empty &&
								// top operator has higher or equal precedence...
								// pop operator and stuff into queue
								m_Postfix.Enqueue( stack.Pop() );
							}
							stack.Push(token);
						}
						break;

					case Tokens.PropertyName:
						m_Postfix.Enqueue(token);
						TagExtractor te = new TagExtractor(token.Data);
						this.m_PE[token.Data] = te;
						break;

					case Tokens.PropertyValue:
						m_Postfix.Enqueue(token);
						break;
				}
			}

			// pop remaining items in stack and stuff into queue
			// 

			while (stack.Count > 0)
			{
				TokenResult tr = (TokenResult) stack.Pop();
				if (tr.OpToken != OperatorTokens.LParen) 
				{
					m_Postfix.Enqueue( tr );
				}
			}
		}

		/// <summary>
		/// Implements the core logic for evaluating a media object
		/// against an expression.
		/// String comparison methods are always case insensitive.
		/// </summary>
		/// <param name="media">the media to evaluate</param>
		/// <returns>true, if the media object is a match</returns>
		private bool EvaluateMedia (IUPnPMedia media)
		{
			if (this.m_AlwaysMatch == true) return true;

			// postfix expression is complete...now evaluate
			// 
			Queue postfix = new Queue(this.m_Postfix);
			Stack stack = new Stack();
			while (postfix.Count > 0)
			{
				if (((TokenResult)postfix.Peek()).TokenType == Tokens.PropertyName)
				{
					TokenResult lo = (TokenResult) postfix.Dequeue();	//left operand
					TokenResult ro = (TokenResult) postfix.Dequeue();	//right operand
					TokenResult op = (TokenResult) postfix.Dequeue();	//operator
						
					//evaluate sub-expression against media
					//
					bool exp = Evaluate(media, lo.Data, op.OpToken, ro.Data);
					stack.Push(exp);
				}
				else if (((TokenResult)postfix.Peek()).TokenType == Tokens.Operator)
				{
					TokenResult op = (TokenResult) postfix.Dequeue();		//logical operator
					bool ro = (bool) stack.Pop();
					bool lo = (bool) stack.Pop();
					bool result = false;

					if (op.OpToken == OperatorTokens.And)
					{
						result = (lo && ro);
					}
					else if (op.OpToken == OperatorTokens.Or)
					{
						result = (lo || ro);
					}
					else
					{
						throw new UPnPCustomException(402, "Invalid Args: Badly formed SearchCriteria.");
					}

					stack.Push(result);
				}
				else
				{
					throw new UPnPCustomException(402, "Invalid Args: Bad SearchCriteria.");
				}
			}


			return (bool) stack.Pop();
		}

		/// <summary>
		/// Evaluates a subexpression of a whole expression.
		/// </summary>
		/// <param name="media">the media object with the metadata</param>
		/// <param name="prop">the property/attribute metadata to examine</param>
		/// <param name="op">the operator to use for examination</param>
		/// <param name="val">the value to compare against in string form</param>
		/// <returns></returns>
		/// <exception cref="OpenSource.UPnP.AV.CdsMetadata.Error_MalformedSearchCriteria">
		/// Thrown when the expression provided at constructor time could not
		/// be used to evaluate the media because of syntax errors in 
		/// the expression.
		/// </exception>
		private bool Evaluate(IUPnPMedia media, string prop, OperatorTokens op, string val)
		{
			bool result = false;

			TagExtractor te = (TagExtractor) this.m_PE[prop];

			IList values = te.Extract(media);


			if (op == OperatorTokens.Exists)
			{
				bool testVal = (string.Compare(val, "true", true) == 0);

				result = (
					((values.Count > 0) && (testVal)) ||
					((values.Count == 0) && (testVal == false))
					);
			}
			else
			{
				foreach (object testVal in values)
				{
					int opCode = (int) op;
					if ((opCode >= (int) OperatorTokens.Equal) && (opCode <= (int) OperatorTokens.GreaterThanEqualTo))
					{
						// Compare using a relational operator
						// 
						try
						{
							int relResult = MetadataValueComparer.CompareTagValues(testVal, val, this.IgnoreCase);

							if (relResult == 0)
							{
								result = true;
								break;
							}
						}
						catch //(Exception e)
						{
							string opString = Enum.GetName(typeof(OperatorTokens), opCode);
							throw new Error_MalformedSearchCriteria("("+val+") "+opString+" "+testVal.ToString()+") could not occur.");
						}
					}
					else if (op == OperatorTokens.Contains)
					{
						string tv = testVal.ToString();
						int pos = tv.IndexOf(val);
						result = (pos >= 0);
					}
					else if (op == OperatorTokens.DoesNotContain)
					{
						string tv = testVal.ToString();
						int pos = tv.IndexOf(val);
						result = (pos < 0);
					}
					else if (op == OperatorTokens.DerivedFrom)
					{
						string tv = testVal.ToString();

						result = tv.StartsWith(val);
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Takes a portion of the expression and determines if the
		/// expression begins with an operator. If so, return
		/// the operator code. The operator token is removed
		/// from the expression.
		/// </summary>
		/// <param name="exp">infix expression</param>
		/// <param name="operators">the set of operators to search for</param>
		/// <returns></returns>
		private TokenResult SeachOperators (ref string exp, ICollection operators)
		{
			TokenResult token = new TokenResult(false);
			foreach (string op in operators)
			{
				if (exp.StartsWith(op))
				{
					token = new TokenResult(op);
					exp = exp.Substring(op.Length);

					if (token.OpToken == OperatorTokens.Invalid)
					{
						throw new Exception("Bad code in GetToken()");
					}
					break;
				}
			}
		
			return token;
		}

		/// <summary>
		/// Obtains the next token, given the current state and the expression.
		/// </summary>
		/// <param name="exp">the remaining portion of the infix expression to translate into a postfix expression</param>
		/// <param name="state">the state determines what types of tokens can be accepted.</param>
		/// <returns></returns>
		private TokenResult GetToken(ref string exp, RelExpStates state)
		{
			TokenResult token = new TokenResult(false);

			bool searchOps1 = false;
			bool searchOps2 = false;
			bool searchOps3 = false;
			bool parsePropName = false;
			bool parsePropValue = false;

			switch (state)
			{
				case RelExpStates.unknown:
					//look for everything
					searchOps1 = true;
					searchOps3 = true;
					parsePropName = true;
					break;

				case RelExpStates.expectOp:
					//look for relop, stringop, derived-op 
					searchOps2 = true;
					break;

				case RelExpStates.expectValue:
					parsePropValue = true;
					break;
			}

			//remove any preceding whitespace chars
			exp = exp.Trim();

			if (exp != "")
			{
				if ((searchOps1) && (token.TokenType == Tokens.Invalid))
				{
					token = this.SeachOperators(ref exp, Operators1);
				}

				if ((searchOps2) && (token.TokenType == Tokens.Invalid))
				{
					token = this.SeachOperators(ref exp, Operators2);
				}

				if ((searchOps3) && (token.TokenType == Tokens.Invalid))
				{
					token = this.SeachOperators(ref exp, Operators3);
				}


				if (token.TokenType == Tokens.Invalid)
				{
					if (parsePropValue)
					{
						//it must be an operand... so parse the next whitespace delimited string or 
						//extract the next string enclosed by quotes

						if ((exp.StartsWith("true")) || (exp.StartsWith("false")))
						{
							// This is a value for an existence-op operation
							token = new TokenResult(exp, false);
							exp = exp.Substring(exp.Length);
						}
						else if (exp.StartsWith("\""))
						{
							// This is a value operand that is delimited
							// by another double-quote
							int endQuote = 1;
							int escape = 0;
							bool endQuoteFound = false;

							// find the next end-quote that is not
							// an escaped end-quote.

							while (!endQuoteFound)
							{
								endQuote = exp.IndexOf("\"", endQuote);
								escape = exp.IndexOf("\\\"", escape);

								if (
									(escape < 0) ||
									(escape == endQuote - 1) ||
									(escape > endQuote)
									)
								{
									endQuoteFound = true;
								}
							}

							if (endQuote <= 0)
							{
								StringBuilder msg = new StringBuilder(exp.Length);
								msg.AppendFormat("Invalid Args: Unterminated end-quote in SearchCriteria, near \"{0}\"", exp);
								throw new UPnPCustomException(402, msg.ToString());
							}

							string unescaped = exp.Substring(1, endQuote-1);
							string escaped = unescaped.Replace("\\\\", "\\").Replace("\\\"", "\"");
							token = new TokenResult(escaped, false);
					
							exp = exp.Substring(endQuote+1);
						}
						else
						{
							// Assume the CP provided a white-space delimited value without quotes
						
							//int endPos = exp.IndexOf(" ");
							int endPos = this.FindNextIndexOf(exp, WHITESPACESANDENDPAREN);
							string str = exp;
							if (endPos > 0)
							{
								str = exp.Substring(0, endPos);
							}
							token = new TokenResult(str, false);

							exp = exp.Substring(str.Length);
						}
					}
					else
					{
						// This is a property name, that is delimited by 
						// a whitespace.

						//int endPos = exp.IndexOf(" ");
						int endPos = this.FindNextIndexOf(exp, WHITESPACES);
						if (endPos > 0)
						{
							string prop = exp.Substring(0, endPos);

							token = new TokenResult(prop, true);
							exp = exp.Substring(prop.Length);
						}
						else
						{
							throw new Error_MalformedSearchCriteria("Property name has not been properly delimited.");
						}
					}
				}
			}

			if (
				(token.TokenType == Tokens.Invalid) 
				)
			{
				throw new UPnPCustomException(402, "Invalid Args: Invalid SearchCriteria string.");
			}

			return token;
		}

		private int FindNextIndexOf(string exp, char[] chars)
		{
			int pos = -1;

			foreach (char c in chars)
			{
				int p;
				if (pos > 0)
				{
					p = exp.IndexOf(c, 0, pos);
				}
				else if (pos != 0)
				{
					p = exp.IndexOf(c, 0);
				}
				else
				{
					break;
				}

				if (p >= 0)
				{
					if (pos == -1)
					{
						pos = p;
					}
					else
					{
						if (p < pos)
						{
							pos = p;
						}
					}
				}
			}

			return pos;
		}

		private static char[] WHITESPACES = 
			{
				'\x09',
				'\x0A',
				'\x0B',
				'\x0C',
				'\x0D',
				'\x20',
				'\x22',
				'\x2A'
		};

		private static char[] WHITESPACESANDENDPAREN = 
			{
				'\x09',
				'\x0A',
				'\x0B',
				'\x0C',
				'\x0D',
				'\x20',
				'\x22',
				'\x2A',
				')'
		};

		/// <summary>
		/// An empty expression matches all items.
		/// </summary>
		private bool m_AlwaysMatch = false;

		/// <summary>
		/// Contains the postfix expression as a queue of tokens.
		/// </summary>
		private Queue m_Postfix;

		/// <summary>
		/// Hashtable with a mapping of property/attribute names as keys,
		/// to TagExtractor objects.
		/// </summary>
		private Hashtable m_PE = new Hashtable();

		/// <summary>
		/// Enumerates the different types of tokens in the infix expression.
		/// </summary>
		private enum Tokens
		{
			Invalid,
			PropertyName,
			PropertyValue,
			Operator,
		}

		/// <summary>
		/// Set of operators with highest order are parenthetical ops.
		/// </summary>
		private static string[] Operators1 = { "(", ")" };
		/// <summary>
		/// Second highest order of operators are relational, string, and existence operators
		/// </summary>
		private static string[] Operators2 = { "=", "!=", "<", "<=", ">", ">=", "contains", "doesNotContain", "exists", "derivedfrom"};
		
		/// <summary>
		/// logical operators are the lowest order of operators.
		/// </summary>
		private static string[] Operators3 = { "and", "or" };

		/// <summary>
		/// Complete set of operators. The order of these operators must match the order
		/// of each of the operator sets in OperatorsX.
		/// </summary>
		private static string[] Operators = 
			{ 
				"(", ")",
				"=", "!=", "<", "<=", ">", ">=", "contains", "doesNotContain", "exists", "derivedfrom",
				"and", "or",
			};


		/// <summary>
		/// Enumeration of the operators. The order of these operators matter, 
		/// as the indices map directly into the Operator string array.
		/// </summary>
		private enum OperatorTokens
		{
			LParen = 0,
			RParen,

			Equal,
			NotEqual,
			
			LessThan,
			LessThanEqualTo,

			GreaterThan,
			GreaterThanEqualTo,
			
			Contains,
			DoesNotContain,

			Exists,
			
			DerivedFrom,

			And,
			Or,
			
			Invalid,
		}

		/// <summary>
		/// Used for token-delimited textparsing.
		/// </summary>
		private static DText DT = new DText();
		/// <summary>
		/// Used for obtaining the attribute and tag names of standard metadata properties.
		/// </summary>
		private static Tags T = Tags.GetInstance();

		/// <summary>
		/// Structure is used when extracting a token from the infix expression
		/// while translating into a postfix expression.
		/// </summary>
		private struct TokenResult
		{
			/// <summary>
			/// The type of token.
			/// </summary>
			public readonly Tokens TokenType;
			/// <summary>
			/// Data associated with the token. Used only when
			/// the token is a property name or value.
			/// </summary>
			public readonly string Data;
			/// <summary>
			/// The operator value, if the token is an operator.
			/// </summary>
			public readonly OperatorTokens OpToken;

			/// <summary>
			/// The precedence of the operator. 10,20,30.
			/// </summary>
			public readonly int Precedence;

			/// <summary>
			/// Default constructor - considered a parameterless constructor.
			/// </summary>
			/// <param name="ignored">This parameter is ignored.</param>
			public TokenResult(bool ignored)
			{
				TokenType = Tokens.Invalid;
				Data = "";
				OpToken = OperatorTokens.Invalid;
				Precedence = -1;
			}

			/// <summary>
			/// Used when the token is a property name or property value.
			/// </summary>
			/// <param name="data">the string value of the propery name or value</param>
			/// <param name="dataIsProperty">true, if a property name</param>
			public TokenResult(string data, bool dataIsProperty)
			{
				Data = data.Trim();
				if (dataIsProperty)
				{
					TokenType = Tokens.PropertyName;
				}
				else
				{
					TokenType = Tokens.PropertyValue;
				}
				OpToken = OperatorTokens.Invalid;
				Precedence = -1;
			}

			/// <summary>
			/// Used when the token is an operator.
			/// </summary>
			/// <param name="operatorString">the string representation of the operator</param>
			public TokenResult(string operatorString)
			{
				TokenType = Tokens.Operator;
				Data = "";
				OpToken = OperatorTokens.Invalid;
				Precedence = -1;

				int i;
				
				i=0;
				foreach (string op in Operators1)
				{
					if (String.Compare(op, operatorString) == 0)
					{
						OpToken = (OperatorTokens) ((int) OperatorTokens.LParen + i) ;
						Precedence = 30;
						break;
					}
					i++;
				}

				if (OpToken == OperatorTokens.Invalid)
				{
					i=0;
					foreach (string op in Operators2)
					{
						if (String.Compare(op, operatorString) == 0)
						{
							OpToken = (OperatorTokens) ((int) OperatorTokens.Equal + i) ;
							Precedence = 20;
							break;
						}
						i++;
					}
				}

				if (OpToken == OperatorTokens.Invalid)
				{
					i=0;
					foreach (string op in Operators3)
					{
						if (String.Compare(op, operatorString) == 0)
						{
							OpToken = (OperatorTokens) ((int) OperatorTokens.And + i) ;
							Precedence = 10;
							break;
						}
						i++;
					}
				}
			}
		}

	}
}
