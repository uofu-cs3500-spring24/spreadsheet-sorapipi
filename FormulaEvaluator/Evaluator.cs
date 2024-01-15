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
            string[] substrings =
            Regex.Split(expression, "(\\()|(\\))|(-)|(\\+)|(\\*)|(/)");
            throw new NotImplementedException();
        }

    }
}
