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
    ///    This library class evaluates ...
    /// </summary>
    public class Evaluator
    {
        public delegate int Lookup(String variable_name);

        /// <summary>
        /// This function takes in a string representing ...
        /// and does...
        /// </summary>
        /// <param name="expression"> details of what an expression is</param>
        /// <param name="variableEvaluator"> details on what this is</param>
        /// <returns></returns>
        public static int Evaluate(String expression, Lookup variableEvaluator )
        {
            // TODO...
            string[] substrings = Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            var valueStack = new Stack<int>();
            var operatorStack = new Stack<string>();
            foreach (string token in substrings )
            {
                if(string.IsNullOrWhiteSpace(token)) continue;
                if(int.TryParse(token,out int number))
                {
                    ProcessNumber(number,valueStack,operatorStack);
                }
                else if (IsVariable(token))
                {
                    int varValue = variableEvaluator(token);
                    ProcessNumber(varValue,valueStack,operatorStack);
                }
                else if (token == "+" || token == "-")
                {
                    ProcessOperator(token,valueStack,operatorStack);
                    operatorStack.Push(token);
                }
                else if (token == "*" || token == "/")
                {
                    operatorStack.Push(token);
                }
                else if (token == "(")
                {
                    operatorStack.Push(token);
                }
                else if (token == ")")
                {
                    ProcessRightParenthesis(valueStack,operatorStack);
                }
                else
                {
                    throw new ArgumentException("Invalid token: " + token);
                }
            }
            return FinalEvaluation(valueStack,operatorStack);
            
        }
        private static bool IsVariable(string token)
        {
            return Regex.IsMatch(token, "^[a-zA-Z]+[0-9]*$");
        }
        private static void ProcessNumber(int number, Stack<int>  valueStack, Stack<string> operatorStack)
        {
            if(operatorStack.TryPeek(out string op) && (op == "*" ||  op == "/"))
            {
                operatorStack.Pop();
                int left = valueStack.Pop();
                valueStack.Push(ApplyOperator(left, number, op));
            }
            else
            {
                valueStack.Push((int)number);
            }
        }
        private static void ProcessOperator (string token, Stack<int> valueStack, Stack<string> operatorStack)
        {
            while(operatorStack.Count > 0 && (operatorStack.Peek() == "+" || operatorStack.Peek() == "-")){
                string op = operatorStack.Pop();
                int right = valueStack.Pop();
                int left = valueStack.Pop();
                valueStack.Push(ApplyOperator(left,right,op));
            }
        }
        private static void ProcessRightParenthesis(Stack<int> valueStack, Stack<string> operatorStack)
        {
            while(operatorStack.Count > 0 && operatorStack.Peek() != "(")
            {
                string op = operatorStack.Pop();
                int right = valueStack.Pop();
                int left = valueStack.Pop();
                valueStack.Push(ApplyOperator(left,right,op));
            }
            if(operatorStack.Peek() == "(")
            {
                operatorStack.Pop();
            }
            else
            {
                throw new ArgumentException("Mismatched parentheses in expression");
            }
        }
        private static int FinalEvaluation(Stack<int> valueStack, Stack<string> operatorStack)
        {
            while(operatorStack.Count > 0)
            {
                string op = operatorStack.Pop();
                int right = valueStack.Pop();
                int left = valueStack.Pop();
                valueStack.Push(ApplyOperator(left,right,op));
            }

            if(valueStack.Count != 1)
            {
                throw new ArgumentException("Invalid expression format");
            }
            return valueStack.Pop();
        }

        private static int ApplyOperator(int left, int right, string op)
        {
            switch (op)
            {
                case "+": return left + right;
                case "-": return left - right;
                case "*": return left * right;
                case "/": 
                    if(right == 0) throw new DivideByZeroException();
                    return left / right;
                default: throw new ArgumentException("Invalid operator: " + op);
            }
        }

    }
}
