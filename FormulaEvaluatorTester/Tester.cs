
using FormulaEvaluator;
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
///    This console App is used to test the FormulaEvaluator library
/// </summary>
class Tester
{
    public static void Main(string[] args)
    {
        //Test + - * /
        if (Evaluator.Evaluate("X+X", (x) => 5) == 10)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("(2+X)*5", (x) => 2) == 20)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("X+2", (x) => 5) == 7)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("(5-2)*X", (x) => 5) == 15)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("X*2-4", (x) => 5) == 6)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("(X-5)/5", (x) => 10) == 1)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("(X+X)/2", (x) => 5) == 5)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("X-1*4", (x) => 5) == 1)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("1+2*X", (x) => 5) == 11)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("1+X/2", (x) => 10) == 6)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("4+A1-6", (a1) => 5) == 3)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        if (Evaluator.Evaluate("B2+0/2", (b2) => 5) == 5)
            Console.WriteLine("Passed");
        else
            Console.WriteLine("Failed");
        
        
        //Test exception
        
        
        //test invalid token
        try
        {
            Evaluator.Evaluate("2^2+5", null);
            Console.WriteLine("Failed ");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Passed " + e.Message);
        }
        // test divide by zero
        try
        {
            Evaluator.Evaluate("1/0", null);
            Console.WriteLine("Failed");
        }
        catch (DivideByZeroException d)
        {
            Console.WriteLine("Passed " + d.Message);
        }
        // test unbalanced paranthesis
        try
        {
            Evaluator.Evaluate("(3+2", null);
            Console.WriteLine("Failed");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Passed "+e.Message);
        }
        // test unknow variable
        try
        {
            Evaluator.Evaluate("X+2", (x) => throw new ArgumentException("Unknown variable"));
            Console.WriteLine("Failed");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Passed "+e.Message);
        }
        // test empty expression
        try
        {
            Evaluator.Evaluate("", null);
            Console.WriteLine("Failed");
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Passed "+e.Message);
        }
        
    }
}

