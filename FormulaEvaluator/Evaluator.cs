using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace FormulaEvaluator
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
    /// File Contents
    ///
    ///    This library class evaluates the result of the input expression
    /// </summary>
    public class Evaluator
    {
        public delegate int Lookup(String variable_name);

        /// <summary>
        /// This function takes in a string representing an expression
        /// and does evaluate the expression to get the calculated result
        /// </summary>
        /// <param name="expression"> details of what an expression is</param>
        /// <param name="variableEvaluator"> details on what this is</param>
        /// <returns> The evaluated result of the expression</returns>
        /// <exception cref="ArgumentException"> Throw when the expression is invalid or contains unknow variables</exception>
        public static int Evaluate(String expression, Lookup variableEvaluator)
        {
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)"); // tokenlize the input expression
            var valueStack = new Stack<int>(); // create a stack to store the integer values
            var operatorStack = new Stack<string>(); // create a stack to store the operators as strings
            foreach (string token in substrings)// loop every token in the substrings
            {
                if (string.IsNullOrWhiteSpace(token)) continue; //check if the token is null or white space

                switch (token)
                {
                    case var t when int.TryParse(t, out int number): // Case for numbers
                        Variable(number, valueStack, operatorStack);
                        break;

                    case var t when IsVariable(t): // Case for variables
                        int value = variableEvaluator(t);
                        if (value == 0 && token != "0")
                            throw new ArgumentException("Undefined variable");
                        Variable(value, valueStack, operatorStack);
                        break;

                    case "+":
                    case "-": // Cases for + and - operators
                        Operator(token, valueStack, operatorStack);
                        operatorStack.Push(token);
                        break;

                    case "*":
                    case "/": // Cases for * and / operators
                        operatorStack.Push(token);
                        break;

                    case "(": // Case for left parenthesis
                        operatorStack.Push(token);
                        break;

                    case ")": // Case for right parenthesis
                        while (operatorStack.Count > 0 && operatorStack.Peek() != "(")
                        {
                            if (valueStack.Count < 2)
                                throw new ArgumentException("Invalid expression: insufficient values for operation");
                            string oper = operatorStack.Pop();// get the first element in operatorStack
                            int right = valueStack.Pop();// get the first element in valueStack
                            int left = valueStack.Pop();// get the first element for now in valueStack
                            valueStack.Push((int)Calculate(left, right, oper));// calculate the two values by the operator then push onto valueStack
                        }
                        if (operatorStack.Count == 0)
                            throw new ArgumentException("Unbalanced parenthesis");
                        operatorStack.Pop();
                        break;

                    default: // Default case for invalid tokens
                        throw new ArgumentException("Invalid token: " + token);
                }
            }
            while (operatorStack.Count > 0)
            {
                if (valueStack.Count < 2)
                    throw new ArgumentException("Invalid expression: insufficient values for operation ");
                string oper = operatorStack.Pop();// get the first element in operatorStack
                int right = valueStack.Pop();// get the first element in valueStack
                int left = valueStack.Pop();// get the first element for now in valueStack
                valueStack.Push((int)Calculate(left, right, oper));// calculate the two values by the operator then push onto valueStack
            }

            if (valueStack.Count != 1)// after all operator processed check if there is still unprocessed value in valueStack
            {
                throw new ArgumentException("Invalid expression");
            }
            return valueStack.Pop();//get the evaluated result of the expression

        }

        /// <summary>
        /// this function just check if the token is a variable
        /// </summary>
        /// <param name="token"> token represents one part of the expression</param>
        /// <returns></returns>
        private static bool IsVariable(string token)
        {
            return Regex.IsMatch(token, "^[a-zA-Z_][a-zA-Z0-9_]*$");
        }

        /// <summary>
        /// this function does the evaluate after getting the value of the variable if the operatorStack's first element is * or /,
        /// otherwise just push it to valueStack
        /// </summary>
        /// <param name="value"> the value of the variable</param>
        /// <param name="valueStack"> the stack of value</param>
        /// <param name="operatorStack"> the stack of the operator</param>
        private static void Variable(int value, Stack<int> valueStack, Stack<string> operatorStack)
        {
            if (operatorStack.TryPeek(out string op) && (op == "*" || op == "/"))// check if the first element in operatorStack is * or /
            {
                operatorStack.Pop();// get the first element in operatorStack
                int left = valueStack.Pop();// get the first element in valueStack
                valueStack.Push((int)Calculate(left, value, op));// calculate 
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
        private static void Operator(string token, Stack<int> valueStack, Stack<string> operatorStack)
        {
            while (operatorStack.Count > 0 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-"))
            {
                string op = operatorStack.Pop();// get the first element in operatorStack
                int right = valueStack.Pop();// get the first element in valueStack
                int left = valueStack.Pop();// get the first element for now in valueStack
                valueStack.Push((int)Calculate(left, right, op));// calculate the two values by the operator then push onto valueStack
            }
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
        private static int Calculate(int left, int right, string oper)
        {
            switch (oper)
            {
                case "+": return left + right; // case for +
                case "-": return left - right;// case for -
                case "*": return left * right;// case for *
                case "/":
                    if (right == 0) 
                        throw new ArgumentException("divided by zero");
                    Console.WriteLine(left/right);
                    return left/right;// case for  /
                default: throw new ArgumentException("Invalid operator: " + oper); //default case for invalid operator
            }
        }

    }
}
