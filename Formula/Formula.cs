// Skeleton written by Joe Zachary for CS 3500, September 2013
// Read the entire skeleton carefully and completely before you
// do anything else!

// Version 1.1 (9/22/13 11:45 a.m.)

// Change log:
//  (Version 1.1) Repaired mistake in GetTokens
//  (Version 1.1) Changed specification of second constructor to
//                clarify description of how validation works

// (Daniel Kopta) 
// Version 1.2 (9/10/17) 

// Change log:
//  (Version 1.2) Changed the definition of equality with regards
//                to numeric tokens


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace SpreadsheetUtilities
{
    /// <summary>
    /// Author:    YINGHAO CHEN
    /// Partner:   -NONE-
    /// Date:      15/01/2024
    /// Course:    CS 3500, University of Utah, School of Computing
    /// Copyright: CS 3500 and YINGHAO CHEN - This work may not 
    ///            be copied for use in Academic Coursework.
    ///
    /// I, YINGHAO CHEN, certify that I wrote this code from scratch and
    /// did not copy it in part or whole from another source.  All 
    /// references used in the completion of the assignments are cited 
    /// in my README file.
    ///
    /// File Contents:
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  The allowed symbols are non-negative numbers written using double-precision 
    /// floating-point syntax (without unary preceeding '-' or '+'); 
    /// variables that consist of a letter or underscore followed by 
    /// zero or more letters, underscores, or digits; parentheses; and the four operator 
    /// symbols +, -, *, and /.  
    /// 
    /// Spaces are significant only insofar that they delimit tokens.  For example, "xy" is
    /// a single variable, "x y" consists of two variables "x" and y; "x23" is a single variable; 
    /// and "x 23" consists of a variable "x" and a number "23".
    /// 
    /// Associated with every formula are two delegates:  a normalizer and a validator.  The
    /// normalizer is used to convert variables into a canonical form, and the validator is used
    /// to add extra restrictions on the validity of a variable (beyond the standard requirement 
    /// that it consist of a letter or underscore followed by zero or more letters, underscores,
    /// or digits.)  Their use is described in detail in the constructor and method comments.
    /// </summary>
    public class Formula
    {

        private Func<string, string> normalize;
        private List<string> tokens = new List<string>();
        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>
        public Formula(String formula) :
            this(formula, s => s, s => true)
        {
        }

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically incorrect,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// The associated normalizer and validator are the second and third parameters,
        /// respectively.  
        /// 
        /// If the formula contains a variable v such that normalize(v) is not a legal variable, 
        /// throws a FormulaFormatException with an explanatory message. 
        /// 
        /// If the formula contains a variable v such that isValid(normalize(v)) is false,
        /// throws a FormulaFormatException with an explanatory message.
        /// 
        /// Suppose that N is a method that converts all the letters in a string to upper case, and
        /// that V is a method that returns true only if a string consists of one letter followed
        /// by one digit.  Then:
        /// 
        /// new Formula("x2+y3", N, V) should succeed
        /// new Formula("x+y3", N, V) should throw an exception, since V(N("x")) is false
        /// new Formula("2x+y3", N, V) should throw an exception, since "2x+y3" is syntactically incorrect.
        /// </summary>
        public Formula(String formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            if (normalize == null)
                normalize = s => s;
            if (isValid == null)
                isValid = s => true;
            ValidateTokens(formula, normalize, isValid);
        }
        /// <summary>
        /// this function validates the token from the formula
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="normalize"></param>
        /// <param name="isValid"></param>
        /// <exception cref="FormulaFormatException"></exception>
        private void ValidateTokens(string formula, Func<string, string> normalize, Func<string, bool> isValid)
        {
            var tokenList = GetTokens(formula).ToList();
            if (tokenList.Count == 0)
                throw new FormulaFormatException("Empty formula: check if you input the formula");
            for (int i = 0; i < tokenList.Count; i++)
            {
                string token = tokenList[i];

                if (!IsValidToken(token))
                    throw new FormulaFormatException($"Invalid token: {token}");

            }

            int openParentheses = 0;
            string previousToken = null;

            for (int i = 0; i < tokenList.Count; i++)
            {
                string token = tokenList[i];
                // check balanced parentheses
                if (token == "(")
                    openParentheses++;
                if (token == ")")
                    openParentheses--;
                if (openParentheses < 0)
                    throw new FormulaFormatException("Unbalanced parentheses: check if every left parenthesis has a right parenthesis");

                //check starting and ending tokens
                if (i == 0 && !(IsVariable(token) || token == "(" || double.TryParse(token, out _)))
                    throw new FormulaFormatException($"{token} is not a valid starting token. Check if the input starting token is a variable, left parenthesis or a double");
                if (i == tokenList.Count - 1 && !(IsVariable(token) || token == ")" || double.TryParse(token, out _)))
                    throw new FormulaFormatException($"{token} is not a valid ending token. Check if the input ending token is a variable, left parenthesis or a double");

                //check sequence of tokens
                if (previousToken != null && !IsSequence(previousToken, token))
                    throw new FormulaFormatException("Invalid sequence. Check if the sequence is valid. For example: maybe you input something like '(*' ");

                //Normalize and validate
                if (IsVariable(token))
                {
                    string normalizedToken = normalize(token);
                    if (!isValid(normalizedToken))
                        throw new FormulaFormatException($"{normalizedToken} is not valid");
                    tokenList[i] = normalizedToken;
                }

                previousToken = token;


            }

            if (openParentheses != 0)
            {
                throw new FormulaFormatException("Unbalanced parentheses. Check if every left parenthesis has a right parenthesis");
            }


            tokens = tokenList;
        }
        /// <summary>
        /// check if the token is a valid token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsValidToken(string token)
        {
            return IsOperator(token) || IsParenthesis(token) || IsVariable(token) || double.TryParse(token, out _);
        }
        /// <summary>
        /// check if the token is a valid operator
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsOperator(string token)
        {
            return new List<string> { "+", "-", "*", "/" }.Contains(token);
        }

        /// <summary>
        /// check if the token is a parenthesis
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool IsParenthesis(string token)
        {
            return token == "(" || token == ")";
        }

        /// <summary>
        /// check if current sequence is valid
        /// </summary>
        /// <param name="previousToken"></param>
        /// <param name="currentToken"></param>
        /// <returns></returns>
        private bool IsSequence(string previousToken, string currentToken)
        {
            bool current = double.TryParse(currentToken, out _) || IsVariable(currentToken);

            bool previous = double.TryParse(previousToken, out _) || IsVariable(previousToken);

            List<String> list = new List<String> { "+", "-", "*", "/", };

            if ((list.Contains(previousToken) || previousToken == "(") && !(current || currentToken == "("))
                return false;
            if ((previous || previousToken == ")") && !(list.Contains(currentToken) || currentToken == ")"))
                return false;

            return true;
        }

        /// <summary>
        /// Evaluates this Formula, using the lookup delegate to determine the values of
        /// variables.  When a variable symbol v needs to be determined, it should be looked up
        /// via lookup(normalize(v)). (Here, normalize is the normalizer that was passed to 
        /// the constructor.)
        /// 
        /// For example, if L("x") is 2, L("X") is 4, and N is a method that converts all the letters 
        /// in a string to upper case:
        /// 
        /// new Formula("x+7", N, s => true).Evaluate(L) is 11
        /// new Formula("x+7").Evaluate(L) is 9
        /// 
        /// Given a variable symbol as its parameter, lookup returns the variable's value 
        /// (if it has one) or throws an ArgumentException (otherwise).
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, the value is returned.  Otherwise, a FormulaError is returned.  
        /// The Reason property of the FormulaError should have a meaningful explanation.
        ///
        /// This method should never throw an exception.
        /// </summary>
        public object Evaluate(Func<string, double> lookup)
        {
            var values = new Stack<double>();
            var operators = new Stack<string>();

            foreach (string token in tokens)
            {
                double num;

                if (double.TryParse(token, out num)) // If the token is a number
                {
                    if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                    {
                        try
                        {
                            double left = values.Pop();
                            double result = Calculate(left, num, operators.Pop());
                            values.Push(result);
                        }
                        catch (ArgumentException ex)
                        {
                            return new FormulaError(ex.Message);
                        }
                    }
                    else
                    {
                        values.Push(num);
                    }
                }
                else if (token == "+" || token == "-")
                {
                    while (operators.Count > 0 && values.Count > 1 && (operators.Peek() == "+" || operators.Peek() == "-"))
                    {
                        try
                        {
                            double right = values.Pop();
                            double left = values.Pop();
                            double result = Calculate(left, right, operators.Pop());
                            values.Push(result);
                        }
                        catch (ArgumentException ex)
                        {
                            return new FormulaError(ex.Message);
                        }
                    }
                    operators.Push(token);
                }
                else if (token == "*" || token == "/")
                {
                    operators.Push(token);
                }
                else if (token == "(")
                {
                    operators.Push(token);
                }
                else if (token == ")")
                {
                    while (operators.Count > 0 && operators.Peek() != "(")
                    {
                        try
                        {
                            double right = values.Pop();
                            double left = values.Pop();
                            double result = Calculate(left, right, operators.Pop());
                            values.Push(result);
                        }
                        catch (ArgumentException ex)
                        {
                            return new FormulaError(ex.Message);
                        }
                    }
                    operators.Pop(); // Pop the "("
                }
                else if (Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z_0-9]*$")) // If the token is a variable
                {
                    try
                    {
                        num = lookup(token);
                        if (operators.Count > 0 && (operators.Peek() == "*" || operators.Peek() == "/"))
                        {
                            try
                            {
                                double left = values.Pop();
                                double result = Calculate(left, num, operators.Pop());
                                values.Push(result);
                            }
                            catch (ArgumentException ex)
                            {
                                return new FormulaError(ex.Message);
                            }
                        }
                        else
                        {
                            values.Push(num);
                        }
                    }
                    catch (ArgumentException ex)
                    {
                        return new FormulaError(ex.Message);
                    }
                }
            }

            while (operators.Count > 0 && values.Count > 1)
            {
                try
                {
                    double right = values.Pop();
                    double left = values.Pop();
                    double result = Calculate(left, right, operators.Pop());
                    values.Push(result);
                }
                catch (ArgumentException ex)
                {
                    return new FormulaError(ex.Message);
                }
            }

            return values.Pop();
        }
        /// <summary>
        /// this function does the evaluate after getting the value of the variable if the operatorStack's first element is * or /,
        /// otherwise just push it to valueStack
        /// </summary>
        /// <param name="value"> the value of the variable</param>
        /// <param name="valueStack"> the stack of value</param>
        /// <param name="operatorStack"> the stack of the operator</param>
        private static void Variable(double value, Stack<double> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.TryPeek(out string op) && (op == "*" || op == "/"))// check if the first element in operatorStack is * or /
            {
                operatorStack.Pop();// get the first element in operatorStack
                double left = valueStack.Pop();// get the first element in valueStack
                double result = Calculate(left, value, op);
                valueStack.Push(result);
            }
            else
            {
                valueStack.Push(value);// push the value onto valueStack
            }
        }

        /// <summary>
        /// this function handles the + and - operators
        /// </summary>
        /// <param name="token"></param>
        /// <param name="valueStack"></param>
        /// <param name="operatorStack"></param>
        private static void Operator(string token, Stack<double> valueStack, Stack<string> operatorStack)
        {
            while (operatorStack.Count > 0 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
            {
                string op = operatorStack.Pop();// get the first element in operatorStack
                double right = valueStack.Pop();// get the first element in valueStack
                double left = valueStack.Pop();// get the first element for now in valueStack
                double result = Calculate(left, right, op);
                valueStack.Push(result);


            }
            operatorStack.Push(token);
        }

        /// <summary>
        /// real calculation according to + - * /
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private static double Calculate(double left, double right, string oper)
        {
            switch (oper)
            {
                case "+": return left + right;
                case "-": return left - right;
                case "*": return left * right;
                case "/":
                    if (right == 0)
                    {
                        throw new ArgumentException("Division by zero."); // Throw an exception for division by zero
                    }
                    return left / right;
                default: throw new FormulaFormatException("Invalid operator: " + oper);
            }
        }

        /// <summary>
        /// Enumerates the normalized versions of all of the variables that occur in this 
        /// formula.  No normalization may appear more than once in the enumeration, even 
        /// if it appears more than once in this Formula.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x+y*z", N, s => true).GetVariables() should enumerate "X", "Y", and "Z"
        /// new Formula("x+X*z", N, s => true).GetVariables() should enumerate "X" and "Z".
        /// new Formula("x+X*z").GetVariables() should enumerate "x", "X", and "z".
        /// </summary>
        public IEnumerable<String> GetVariables()
        {
            return tokens.Where(t => IsVariable(t)).Distinct();
        }

        private bool IsVariable(string token)
        {
            return Regex.IsMatch(token, @"^[a-zA-Z_][a-zA-Z_0-9]*$");
        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            return string.Join("", tokens);
        }

        /// <summary>
        ///  <change> make object nullable </change>
        ///
        /// If obj is null or obj is not a Formula, returns false.  Otherwise, reports
        /// whether or not this Formula and obj are equal.
        /// 
        /// Two Formulae are considered equal if they consist of the same tokens in the
        /// same order.  To determine token equality, all tokens are compared as strings 
        /// except for numeric tokens and variable tokens.
        /// Numeric tokens are considered equal if they are equal after being "normalized" 
        /// by C#'s standard conversion from string to double, then back to string. This 
        /// eliminates any inconsistencies due to limited floating point precision.
        /// Variable tokens are considered equal if their normalized forms are equal, as 
        /// defined by the provided normalizer.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        ///  
        /// new Formula("x1+y2", N, s => true).Equals(new Formula("X1  +  Y2")) is true
        /// new Formula("x1+y2").Equals(new Formula("X1+Y2")) is false
        /// new Formula("x1+y2").Equals(new Formula("y2+x1")) is false
        /// new Formula("2.0 + x7").Equals(new Formula("2.000 + x7")) is true
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null || !(obj is Formula other))
                return false;

            if (this.tokens.Count != other.tokens.Count)
                return false;

            for (int i = 0; i < this.tokens.Count; i++)
            {
                if (double.TryParse(this.tokens[i], out double thisNum) && double.TryParse(other.tokens[i], out double otherNum))
                {
                    if (thisNum != otherNum)
                        return false;
                }
                else if (this.tokens[i] != other.tokens[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        /// Reports whether f1 == f2, using the notion of equality from the Equals method.
        /// 
        /// </summary>
        public static bool operator ==(Formula f1, Formula f2)
        {
            if (ReferenceEquals(f1, null))
                return ReferenceEquals(f2, null);
            return f1.Equals(f2);
        }

        /// <summary>
        ///   <change> We are now using Non-Nullable objects.  Thus neither f1 nor f2 can be null!</change>
        ///   <change> Note: != should almost always be not ==, if you get my meaning </change>
        ///   Reports whether f1 != f2, using the notion of equality from the Equals method.
        /// </summary>
        public static bool operator !=(Formula f1, Formula f2)
        {
            return !(f1 == f2);
        }

        /// <summary>
        /// Returns a hash code for this Formula.  If f1.Equals(f2), then it must be the
        /// case that f1.GetHashCode() == f2.GetHashCode().  Ideally, the probability that two 
        /// randomly-generated unequal Formulae have the same hash code should be extremely small.
        /// </summary>
        public override int GetHashCode()
        {
            // Check if 'normalize' is null and if so, use an identity function as the default.
            Func<string, string> normalizeFunc = this.normalize ?? (s => s);

            // Normalize and concatenate all tokens to form a consistent string representation.
            var normalizedRepresentation = string.Join("", tokens.Select(token =>
                double.TryParse(token, out double num) ? num.ToString("G17") : normalizeFunc(token)));

            return normalizedRepresentation.GetHashCode();
        }

        /// <summary>
        /// Given an expression, enumerates the tokens that compose it.  Tokens are left paren;
        /// right paren; one of the four operator symbols; a string consisting of a letter or underscore
        /// followed by zero or more letters, digits, or underscores; a double literal; and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z_](?: [a-zA-Z_]|\d)*";
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: [eE][\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }

        }
    }

    /// <summary>
    /// Used to report syntactic errors in the argument to the Formula constructor.
    /// </summary>
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message)
            : base(message)
        {
        }

    }

    /// <summary>
    /// Used as a possible return value of the Formula.Evaluate method.
    /// </summary>
    public struct FormulaError
    {
        /// <summary>
        /// Constructs a FormulaError containing the explanatory reason.
        /// </summary>
        /// <param name="reason"></param>
        public FormulaError(String reason)
            : this()
        {
            Reason = reason;
        }

        public override string ToString()
        {
            return $"SpreadsheetUtilities.FormulaError: {Reason}";
        }

        /// <summary>
        ///  The reason why this FormulaError was created.
        /// </summary>
        public string Reason { get; private set; }
    }
}


// <change>
//   If you are using Extension methods to deal with common stack operations (e.g., checking for
//   an empty stack before peeking) you will find that the Non-Nullable checking is "biting" you.
//
//   To fix this, you have to use a little special syntax like the following:
//
//       public static bool OnTop<T>(this Stack<T> stack, T element1, T element2) where T : notnull
//
//   Notice that the "where T : notnull" tells the compiler that the Stack can contain any object
//   as long as it doesn't allow nulls!
// </change>
