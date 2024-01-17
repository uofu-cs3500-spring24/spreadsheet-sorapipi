// See https://aka.ms/new-console-template for more information
using FormulaEvaluator;
using System.Runtime.InteropServices;

class program
{
    public static void Main(string[] args)
    {
        try
        {
            if (Evaluator.Evaluate("X+X", (x) => 5) == 10)
                Console.WriteLine("Passed");
            else
                Console.WriteLine("Failed");
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        
    }
}

